//using NSubstitute;
//using TelemetryStash.Functions.Services;

//namespace TelemetryStash.Functions.Tests.TelemetryTrigger;

//public class DeviceServiceTests
//{
//    [Fact]
//    public async Task DeviceService_caches_Device()
//    {
//        // Arrange
//        using var cacheProvider = new CacheProvider();
//        var repository = Substitute.For<IDeviceRepository>();
//        var sut = new DeviceService(repository, cacheProvider.HybridCache);

//        // Act
//        await sut.GetOrCreate("ESP32");
//        await sut.GetOrCreate("ESP32");

//        // Assert
//        await repository.Received(1).GetByDeviceId("ESP32", Arg.Any<Opts<Device>>(), Arg.Any<CancellationToken>());
//    }

//    [Fact]
//    public async Task DeviceService_GetOrAdd_adds_Device_to_database()
//    {
//        // Arrange
//        using var cacheProvider = new CacheProvider();
//        var repository = Substitute.For<IDeviceRepository>();
//        var sut = new DeviceService(repository, cacheProvider.HybridCache);

//        // Act
//        await sut.GetOrCreate("ESP32");

//        // Assert
//        repository.Received(1).Add(Arg.Any<Device>());
//        await repository.Received(1).SaveChangesAsync();
//    }

//    [Fact]
//    public async Task DeviceService_GetOrAdd_should_block_first_call_to_GetByDeviceId_until_finished()
//    {
//        // Arrange
//        using var cacheProvider = new CacheProvider();
//        var repository = Substitute.For<IDeviceRepository>();

//        /// Simulate slow first database call
//        repository
//            .GetByDeviceId(Arg.Any<string>(), Arg.Any<Opts<Device>>(), Arg.Any<CancellationToken>())!
//            .Returns(Task.Delay(100).ContinueWith(_ => new Device("ESP32")));

//        var sut = new DeviceService(repository, cacheProvider.HybridCache);
//        var tasks = new List<Task>();

//        // Act
//        for (var i = 0; i < 10; i++)
//        {
//            tasks.Add(sut.GetOrCreate($"ESP32"));
//        }
//        await Task.WhenAll(tasks);

//        // Assert
//        Assert.DoesNotContain(tasks, t => t.IsFaulted);
//        await repository.Received(1).GetByDeviceId(Arg.Any<string>(), Arg.Any<Opts<Device>>(), Arg.Any<CancellationToken>());
//    }
//}
