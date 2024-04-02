using System.Text.Json;
using TelemetryStash.Functions.TelemetryTrigger;
using TelemetryStash.Functions.TelemetryTrigger.Services;

namespace TelemetryStash.Functions.Tests;


public class TelemetryServiceTests : SqLiteTestBase
{
    [Fact]
    public async Task CachedRegisterSetService_caches_register_set()
    {
        // Arrange
        using var context = GetDbContext();
        var telemetryRepo = new TelemetryRepository(context);

        var cachedDeviceService = new CachedDeviceService(new DeviceRepository(context));
        var cachedRegisterSetService = new CachedRegisterSetService(new RegisterSetRepository(context));
        var cachedRegisterService = new CachedRegisterService(new RegisterKeyRepository(context), new RegisterRepository(context));

        var sut = new TelemetryService(telemetryRepo, cachedDeviceService, cachedRegisterSetService, cachedRegisterService);

        // Act
        await sut.Process("ESP32", CreateTelemetry());
        var result = await telemetryRepo.All();

        // Assert
        Assert.NotEmpty(result);
    }

    private static TelemetryRequest CreateTelemetry()
    {
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

        return JsonSerializer.Deserialize<TelemetryRequest>(json)!;
    }
}
