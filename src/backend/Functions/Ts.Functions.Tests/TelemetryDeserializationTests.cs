using System.Text.Json;
using TelemetryStash.Functions.TelemetryTrigger;

namespace TelemetryStash.Functions.Tests;

public class TelemetryDeserializationTests
{
    [Fact]
    public void Can_deserialize_numbers_only_telemetry()
    {
        // Arrange
        var json = """
        {
            "ts": "250101120000",
            "set": {
                "RegisterSet1": {
                    "tag": ["Tag1", "Tag2"],
                    "reg": {
                        "Register1": 0,
                        "Register2": 0.000001,
                        "Register3": 1,
                        "Register4": -3.2999999999999998,
                        "Text1": "Text1",
                        "Unicode": "Känguru 🦘",
                        "Empty": ""
                    }
                },
                "RegisterSet2": {
                    "reg": {
                        "Register1": 1.000,
                        "Register2": "1.000"
                    }
                }
            }
        }
        """;

        // Act
        var telemetry = JsonSerializer.Deserialize<TelemetryRequest>(json);

        // Assert
        Assert.NotNull(telemetry);
        Assert.Equal(new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero), telemetry.Timestamp);
        Assert.Equal(2, telemetry.RegisterSets.Count);

        var registerSet1 = telemetry.RegisterSets["RegisterSet1"];
        Assert.Equal(["Tag1", "Tag2"], registerSet1.Tags);
        Assert.Equal(new Dictionary<string, string>
        {
            ["Register1"] = "0",
            ["Register2"] = "0.000001",
            ["Register3"] = "1",
            ["Register4"] = "-3.2999999999999998",
            ["Text1"] = "Text1",
            ["Unicode"] = "Känguru \U0001f998",
            ["Empty"] = ""
        }, registerSet1.RegisterValues);

        var registerSet2 = telemetry.RegisterSets["RegisterSet2"];
        Assert.Empty(registerSet2.Tags);
        Assert.Equal(new Dictionary<string, string>
        {
            ["Register1"] = "1",
            ["Register2"] = "1.000"
        }, registerSet2.RegisterValues);
    }
}
