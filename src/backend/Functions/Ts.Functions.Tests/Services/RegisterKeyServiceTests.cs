//using NSubstitute;
//using TelemetryStash.Functions.Services;

//namespace TelemetryStash.Functions.Tests.TelemetryTrigger;

//public class RegisterKeyServiceTests
//{
//    [Fact]
//    public async Task RegisterKeyService_caches_RegisterKey()
//    {
//        // Arrange
//        using var cacheProvider = new CacheProvider();
//        var repository = Substitute.For<IRegisterTemplateRepository>();

//        var sut = new RegisterService(repository, cacheProvider.HybridCache);

//        // Act
//        await sut.GetOrCreate(1, "PowerMeter");
//        await sut.GetOrCreate(1, "PowerMeter");

//        // Assert
//        await repository.Received(1).GetRegisterKey(1, "PowerMeter", Arg.Any<Opts<Register>>(), Arg.Any<CancellationToken>());
//    }

//    [Fact]
//    public async Task RegisterKeyService_AddAndGet_adds_RegisterKey_to_database()
//    {
//        // Arrange
//        using var cacheProvider = new CacheProvider();
//        var repository = Substitute.For<IRegisterTemplateRepository>();

//        var sut = new RegisterService(repository, cacheProvider.HybridCache);

//        // Act
//        await sut.GetOrCreate(1, "PowerMeter");

//        // Assert
//        repository.Received(1).Add(Arg.Any<Register>());
//        await repository.Received(1).SaveChangesAsync();
//    }
//}
