using Azure.Messaging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TelemetryStash.Functions.Services;

namespace TelemetryStash.Functions.TelemetryTrigger;

public class TelemetryTrigger(ILogger<TelemetryTrigger> logger, TelemetryService telemetryService)
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
                ?? throw new Exception($"Failed deserialize telemetry Body: {body}" );

            await telemetryService.Process(eventData.SystemProperties.DeviceId, telemetry);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process telemetry. {EventData}", cloudEvent.Data);
        }
    }
}

/** CloudEvent
  {
      "id": "12f7acd7-a8c0-7336-61a6-91bdf1556cc3",
      "source": "/SUBSCRIPTIONS/1DF7D724-263C-45A3-8008-1ED420D2A7FF/RESOURCEGROUPS/TEAMCOPY/PROVIDERS/MICROSOFT.DEVICES/IOTHUBS/TEAMCOPY-IOT-HUB-FREE",
      "type": "Microsoft.Devices.DeviceTelemetry",
      "data": {
        "properties": {},
        "systemProperties": {
          "iothub-connection-device-id": "P1MeterCaSigned",
          "iothub-connection-auth-method": "{\"scope\":\"device\",\"type\":\"CA-Signed\",\"issuer\":\"iothub\",\"acceptingIpFilterRule\":null}",
          "iothub-connection-auth-generation-id": "638458640089312750",
          "iothub-enqueuedtime": "2024-04-03T18:47:40.358Z",
          "iothub-message-source": "Telemetry"
        },
        "body": "eyJUcyI6IjIwMjQtMDQtMDNUMTg6NDc6NDBaIiwiUmVnIjoxMCwiTnVtIjp7IlRBRSI6OTMzNy41MywiVEVJIjowLCJUUkQiOjMuNTksIlRSSSI6MjI5OC4zNzgsIkVEIjoxLjIzLCJFSSI6MCwiUkVEIjowLCJSRUkiOjAuNTMxLCIxRUQiOjAuMzQ0LCIxRUkiOjAsIjJFRCI6MC42ODUsIjJFSSI6MCwiM0VEIjowLjE4MywiM0VJIjowLCIxUkQiOjAsIjFSSSI6MC4yNzUsIjJSRCI6MCwiMlJJIjowLjE4OCwiM1JEIjowLCIzUkkiOjAuMDcxLCIxViI6MjMyLjksIjJWIjoyMzEuNywiM1YiOjIzMS45LCIxQyI6MiwiMkMiOjMsIjNDIjowLjh9fQ=="
      },
      "time": "2024-04-03T18:47:40.358+00:00",
      "specversion": "1.0",
      "subject": "devices/P1MeterCaSigned"
    }
 */


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
    public DateTimeOffset Timestamp { get; set; }

    [JsonPropertyName("set")]
    public string? RegisterSet { get; set; }

    [JsonPropertyName("reg")]
    public Dictionary<string, Dictionary<string, decimal>> Registers { get; set; } = [];
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
