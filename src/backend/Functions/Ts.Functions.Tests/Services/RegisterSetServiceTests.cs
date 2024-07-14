//using NSubstitute;
//using TelemetryStash.Functions.Services;

//namespace TelemetryStash.Functions.Tests.TelemetryTrigger;

//public class RegisterSetServiceTests
//{
//    [Fact]
//    public async Task RegisterSetService_caches_RegisterSet()
//    {
//        // Arrange
//        using var cacheProvider = new CacheProvider();
//        var repository = Substitute.For<IRegisterSetRepository>();

//        var sut = new RegisterSetService(repository, cacheProvider.HybridCache);

//        // Act
//        await sut.GetOrCreate(1, "TempSensor");
//        await sut.GetOrCreate(1, "TempSensor");

//        // Assert
//        await repository.Received(1).GetRegisterSet(1, "TempSensor", Arg.Any<Opts<RegisterSet>>(), Arg.Any<CancellationToken>());
//    }

//    [Fact]
//    public async Task RegisterSetService_AddAndGet_adds_RegisterSet_to_database()
//    {
//        // Arrange
//        using var cacheProvider = new CacheProvider();
//        var repository = Substitute.For<IRegisterSetRepository>();

//        var sut = new RegisterSetService(repository, cacheProvider.HybridCache);

//        // Act
//        await sut.GetOrCreate(1, "TempSensor");

//        // Assert
//        repository.Received(1).Add(Arg.Any<RegisterSet>());
//        await repository.Received(1).SaveChangesAsync();
//    }
//}
