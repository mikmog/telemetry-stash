using TelemetryStash.Functions.TelemetryTrigger.Services;

namespace TelemetryStash.Functions.Tests.TelemetryTrigger;

public class CachedDeviceServiceTests : SqLiteTestBase
{
    [Fact]
    public async Task CachedDeviceService_caches_device()
    {
        // Arrange
        using var context = GetDbContext();
        var deviceRepo = new DeviceRepository(context);
        var sut = new CachedDeviceService(deviceRepo);

        // Act
        var fromCache1 = CachedDeviceService.Get("ESP32");
        var result = await sut.AddAndGet("ESP32");
        var fromCache2 = CachedDeviceService.Get("ESP32");

        // Assert
        Assert.Null(fromCache1);
        Assert.NotNull(result);
        Assert.NotNull(fromCache2);
    }

    [Fact]
    public async Task CachedDeviceService_AddAndGet_persist_to_database()
    {
        // Arrange
        using var context = GetDbContext();
        var deviceRepo = new DeviceRepository(context);
        var sut = new CachedDeviceService(deviceRepo);

        // Act
        await sut.AddAndGet("ESP32");
        var device = await deviceRepo.GetByDeviceId("ESP32");

        // Assert
        Assert.NotNull(device);
    }
}
