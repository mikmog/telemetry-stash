using Azure.Messaging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TelemetryStash.Functions.Services;

namespace TelemetryStash.Functions.TelemetryTrigger;

public class TelemetryTrigger(ILogger<TelemetryTrigger> logger, ITelemetryService telemetryService)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = false };

    [Function(nameof(TelemetryTrigger))]
    public async Task RunAsync([EventGridTrigger] CloudEvent cloudEvent)
    {
        if (cloudEvent.Data == null)
        {
            throw new ArgumentNullException(nameof(cloudEvent));
        }

        try
        {
            var eventData = cloudEvent.Data.ToObjectFromJson<CloudEventData>(JsonOptions);
            var body = Convert.FromBase64String(eventData.Body);

            var telemetry = JsonSerializer.Deserialize<TelemetryRequest>(Encoding.UTF8.GetString(body), JsonOptions)
                ?? throw new Exception($"Failed deserialize telemetry Body: {body}");

            await telemetryService.Process(eventData.SystemProperties.DeviceId, telemetry);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process telemetry. {EventData}", cloudEvent.Data);
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
    [JsonConverter(typeof(TimestampConverter))]
    [JsonPropertyName("ts")]
    public required DateTimeOffset Timestamp { get; set; }

    [JsonPropertyName("set")]
    public string? RegisterSet { get; set; }

    [JsonPropertyName("reg")]
    public required Dictionary<string, Dictionary<string, decimal>> Registers { get; set; }
}

public class TimestampConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var date = reader.GetString() ?? throw new Exception("Error reading timestamp");
        return DateTimeOffset.ParseExact(date, "yyMMddHHmmssFFFFFFF", CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
