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
        using var cacheProvider = new CacheProvider();
        var telemetryRepository = Substitute.For<ITelemetryRepository>();

        var deviceService = Substitute.For<IDeviceService>();
        deviceService.GetOrCreate(Arg.Any<string>()).Returns(new Device(1, "TestDevice"));

        var registerSetService = Substitute.For<IRegisterSetService>();
        registerSetService.GetOrCreate(Arg.Any<int>(), Arg.Any<string>()).Returns(new RegisterSet(1, 1, "PowerMeter"));

        var registerTemplateService = Substitute.For<IRegisterTemplateService>();
        registerTemplateService.GetOrCreate(Arg.Any<int>(), Arg.Any<string>()).Returns(new RegisterTemplate(1, 1, "L1Current"));

        var registerService = Substitute.For<IRegisterService>();
        registerService.GetOrCreate(Arg.Any<int>(), Arg.Any<string>()).Returns(new Register(1, 1, "L1Current"));

        var sut = new TelemetryService(telemetryRepository, deviceService, registerSetService, registerTemplateService, registerService);

        // Act
        await sut.Process("TestDevice", CreateTelemetry());

        // Assert
        await telemetryRepository.Received(1).Upsert(
            Arg.Is(1),
            Arg.Any<DateTimeOffset>(),
            Arg.Is<List<(int RegisterKey, decimal Value)>>(a => a.Count == 5));
    }

    private static TelemetryRequest CreateTelemetry()
    {
        var json = """
        {
            "ts": "2309012013361234567",
            "reg": {
                "PowerMeter": {
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

        return JsonSerializer.Deserialize<TelemetryRequest>(json)!;
    }
}
