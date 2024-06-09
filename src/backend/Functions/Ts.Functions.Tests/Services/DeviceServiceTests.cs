using NSubstitute;
using TelemetryStash.Functions.Services;
using TelemetryStash.Functions.Tests.Shared;

namespace TelemetryStash.Functions.Tests.TelemetryTrigger;

public class DeviceServiceTests
{
    [Fact]
    public async Task DeviceService_caches_Device()
    {
        // Arrange
        using var cacheProvider = new CacheProvider();
        var repository = Substitute.For<IDeviceRepository>();
        var sut = new DeviceService(repository, cacheProvider.HybridCache);

        // Act
        await sut.GetOrAdd("ESP32");
        await sut.GetOrAdd("ESP32");

        // Assert
        await repository.Received(1).GetByDeviceId("ESP32", Arg.Any<Opts<Device>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeviceService_GetOrAdd_adds_Device_to_database()
    {
        // Arrange
        using var cacheProvider = new CacheProvider();
        var repository = Substitute.For<IDeviceRepository>();
        var sut = new DeviceService(repository, cacheProvider.HybridCache);

        // Act
        await sut.GetOrAdd("ESP32");

        // Assert
        repository.Received(1).Add(Arg.Any<Device>());
        await repository.Received(1).SaveChangesAsync();
    }
}
