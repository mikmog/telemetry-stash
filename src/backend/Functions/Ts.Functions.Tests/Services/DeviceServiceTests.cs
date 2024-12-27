using NSubstitute;
using TelemetryStash.Database;
using TelemetryStash.Database.Repositories;
using TelemetryStash.Functions.Services;

namespace TelemetryStash.Functions.Tests.Services;

public class DeviceServiceTests
{
    [Fact]
    public async Task DeviceService_GetOrCreate_adds_Device_to_database()
    {
        // Arrange
        using var cacheProvider = new CacheProvider();
        var repository = Substitute.For<IDeviceRepository>();

        var sut = new DeviceService(repository, cacheProvider.HybridCache);

        // Act
        await sut.GetOrCreate("ESP32");

        // Assert
        await repository.Received(1).GetOrCreate(Arg.Is("ESP32"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeviceService_GetOrCreate_caches_Device()
    {
        // Arrange
        using var cacheProvider = new CacheProvider();
        var repository = Substitute.For<IDeviceRepository>();

        var sut = new DeviceService(repository, cacheProvider.HybridCache);

        // Act
        await sut.GetOrCreate("ESP32");
        await sut.GetOrCreate("ESP32");

        // Assert
        await repository.Received(1).GetOrCreate("ESP32", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeviceService_GetOrCreate_blocks_first_call_to_database_until_finished()
    {
        // Arrange
        using var cacheProvider = new CacheProvider();
        var repository = Substitute.For<IDeviceRepository>();

        /// Simulate slow first database call
        repository
            .GetOrCreate(Arg.Is("ESP32"), Arg.Any<CancellationToken>())!
            .Returns(Task.Delay(100).ContinueWith(_ => new DeviceRow(1, "ESP32")));

        var sut = new DeviceService(repository, cacheProvider.HybridCache);
        var tasks = new List<Task>();

        // Act
        for (var i = 0; i < 10; i++)
        {
            tasks.Add(sut.GetOrCreate("ESP32"));
        }
        await Task.WhenAll(tasks);

        // Assert
        Assert.DoesNotContain(tasks, t => t.IsFaulted);
        await repository.Received(1).GetOrCreate(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
