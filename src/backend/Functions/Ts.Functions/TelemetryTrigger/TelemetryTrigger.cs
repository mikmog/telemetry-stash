using Azure.Messaging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TelemetryStash.Functions.TelemetryTrigger.Logic;

namespace TelemetryStash.Functions.TelemetryTrigger;

public class TelemetryTrigger(ILogger<TelemetryTrigger> logger, ITelemetryService telemetryService, IHostEnvironment environment)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    [Function(nameof(TelemetryTrigger))]
    public async Task RunAsync([EventGridTrigger] CloudEvent cloudEvent)
    {
        try
        {
            if (cloudEvent.Data == null)
            {
                throw new ArgumentNullException(nameof(cloudEvent));
            }

            var eventData = cloudEvent.Data.ToObjectFromJson<CloudEventData>(JsonOptions)
                ?? throw new Exception("Failed to process CloudEventData");

            var body = Convert.FromBase64String(eventData.Body);
            var json = Encoding.UTF8.GetString(body);

            if (!environment.IsProduction())
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Received telemetry: {Telemetry}", json);
                }
            }

            var telemetry = JsonSerializer.Deserialize<TelemetryRequest>(json, JsonOptions)
                ?? throw new Exception($"Telemetry deserialization failed: {body}");

            await telemetryService.Process(eventData.SystemProperties.DeviceId, telemetry);
        }
        catch (Exception ex)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError(ex, "Failed to process telemetry. {EventData}", cloudEvent.Data);
            }
            throw;
        }
    }
}

public record CloudEventData
{
    [JsonPropertyName("systemProperties")]
    public required SystemProperties SystemProperties { get; set; }

    [JsonPropertyName("body")]
    public required string Body { get; set; }
}

public record SystemProperties
{
    [JsonPropertyName("iothub-connection-device-id")]
    public required string DeviceId { get; set; }
}

public record TelemetryRequest
{
    [JsonPropertyName("ts")]
    [JsonConverter(typeof(TimestampConverter))]
    public required DateTimeOffset Timestamp { get; set; }

    [JsonPropertyName("set")]
    public required Dictionary<string, RegisterSet> RegisterSets { get; set; }

    public record RegisterSet
    {
        [JsonPropertyName("tag")]
        public List<string> Tags { get; set; } = [];

        [JsonPropertyName("reg")]
        [JsonConverter(typeof(RegisterValuesConverter))]
        public Dictionary<string, string> RegisterValues { get; set; } = [];
    }
}

public class TimestampConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var date = reader.GetString() ?? throw new Exception("Error reading timestamp");
        return DateTimeOffset.ParseExact(date, "yyMMddHHmmssFFFFFFF", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class RegisterValuesConverter : JsonConverter<Dictionary<string, string>>
{
    public override Dictionary<string, string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected StartObject token");
        }

        var dictionary = new Dictionary<string, string>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return dictionary;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected PropertyName token");
            }

            var key = reader.GetString();

            reader.Read();

            string? value;
            if (reader.TokenType == JsonTokenType.String)
            {
                value = reader.GetString();
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                value = reader.GetDecimal().ToStringWithoutTrailingZeroes();
            }
            else
            {
                throw new JsonException("Expected String or Number token");
            }

            if (key != null && value != null)
            {
                dictionary.Add(key, value);
            }
        }

        throw new JsonException("Expected EndObject token");
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<string, string> value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
