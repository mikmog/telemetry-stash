using System.Text.Json;
using TelemetryStash.Functions.TelemetryTrigger;

namespace TelemetryStash.Functions.Tests;

public class JsonTests
{
    [Fact]
    public void DeserializeStaticKeyJson()
    {
        // Arrange
        var json = """
        {
            "ts": "2309012013361234567",
            "reg": {
                "P1": {
                    "C1": 5,
                    "C2": 6,
                    "C3": 7.01
                },
                "Am2320": {
                    "Hum": 80,
                    "Temp": 21.44
                }
            }
        }
        """;

        // Act
        var telemetry = JsonSerializer.Deserialize<TelemetryRequest>(json);

        // Assert
        Assert.NotNull(telemetry);
    }

    [Fact(Skip = "TODO add support for strings")]
    public void DeserializeDynamicKeyJson()
    {
        // Arrange
        var json = """
        {
          "ts": "230901101336494",
          "set":  "BT",
          "reg": {
            "fe:fe": {
              "name": "Huw",
              "strength": 8,
              "mac": "fe:fe"
            },
            "ff:f0": {
              "name": "Son",
              "strength": 8,
              "mac": "ff:f0"
            },
            "ff:f1": {
              "name": "Mot",
              "strength": 8,
              "mac": "ff:f1"
            }
          }
        }
        """;

        // Act
        var telemetry = JsonSerializer.Deserialize<TelemetryRequest>(json);

        // Assert
        Assert.NotNull(telemetry);
    }
}
