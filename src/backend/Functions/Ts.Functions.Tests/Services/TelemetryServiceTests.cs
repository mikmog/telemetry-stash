using NSubstitute;
using System.Text.Json;
using TelemetryStash.Database;
using TelemetryStash.Database.Repositories;
using TelemetryStash.Functions.Services;
using TelemetryStash.Functions.TelemetryTrigger;

namespace TelemetryStash.Functions.Tests.Services;

public class TelemetryServiceTests
{
    [Fact]
    public async Task TelemetryService_adds_Telemetry_to_database()
    {
        // Arrange
        var telemetry = Telemetry();
        using var cacheProvider = new CacheProvider();
        var telemetryRepository = Substitute.For<ITelemetryRepository>();

        var deviceService = Substitute.For<IDeviceService>();
        deviceService.GetOrCreate(Arg.Any<string>()).Returns(new DeviceRow(1, "Device1"));

        var registerService = Substitute.For<IRegisterService>();
        registerService.GetOrCreate(Arg.Any<int>(), Arg.Is("RegisterSet1"), Arg.Any<IEnumerable<string>>())
            .Returns(
                new Dictionary<string, RegisterRow>
                {
                    { "Reg1", new RegisterRow(1, 1, "RegisterSet1", "Reg1") },
                    { "Reg2", new RegisterRow(2, 1, "RegisterSet1", "Reg2") },
                    { "Reg3", new RegisterRow(3, 1, "RegisterSet1", "Reg3") }
                }
            );

        registerService.GetOrCreate(Arg.Any<int>(), Arg.Is("RegisterSet2"), Arg.Any<IEnumerable<string>>())
            .Returns(
                new Dictionary<string, RegisterRow>
                {
                    { "NumReg1", new RegisterRow(4, 1, "RegisterSet2", "NumReg1") },
                    { "NumReg2", new RegisterRow(5, 1, "RegisterSet2", "NumReg2") },
                    { "TxtReg1", new RegisterRow(6, 1, "RegisterSet2", "TxtReg1") },
                    { "TxtReg2", new RegisterRow(7, 1, "RegisterSet2", "TxtReg2") }
                }
            );

        var sut = new TelemetryService(telemetryRepository, deviceService, registerService);

        // Act
        await sut.Process("Device1", telemetry);

        // Assert
        await telemetryRepository.Received(2).Upsert(
            Arg.Is(1),
            Arg.Any<DateTimeOffset>(),
            Arg.Is<IEnumerable<(int RegisterKey, string Value)>>(a => a.Count() == 3 || a.Count() == 4));
    }

    private static TelemetryRequest Telemetry()
    {
        var json = """
        {
            "ts": "250101120000",
            "set": {
                "RegisterSet1": {
                    "reg": {
                        "Reg1": -10,
                        "Reg2": 1,
                        "Reg3": 2.4
                    }
                },
                "RegisterSet2": {
                    "reg": {
                        "NumReg1": 100,
                        "NumReg2": 0,
                        "TxtReg1": "Text1",
                        "TxtReg2": "Text2"
                    }
                }
            }
        }
        """;

        return JsonSerializer.Deserialize<TelemetryRequest>(json)!;
    }
}
