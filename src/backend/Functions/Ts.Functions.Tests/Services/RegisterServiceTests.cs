using NSubstitute;
using TelemetryStash.Functions.Services;
using TelemetryStash.Functions.Tests.Shared;

namespace TelemetryStash.Functions.Tests.TelemetryTrigger;

public class RegisterServiceTests
{
    [Fact]
    public async Task RegisterService_caches_Register()
    {
        // Arrange
        using var cacheProvider = new CacheProvider();
        var repository = Substitute.For<IRegisterRepository>();

        var sut = new RegisterService(repository, cacheProvider.HybridCache);

        // Act
        await sut.GetOrAdd(1, "OutdoorTemp");
        await sut.GetOrAdd(1, "OutdoorTemp");

        // Assert
        await repository.Received(1).GetRegister(1, "OutdoorTemp", Arg.Any<Opts<Register>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterService_AddAndGet_adds_Register_to_database()
    {
        // Arrange
        using var cacheProvider = new CacheProvider();
        var repository = Substitute.For<IRegisterRepository>();

        var sut = new RegisterService(repository, cacheProvider.HybridCache);

        // Act
        await sut.GetOrAdd(1, "OutdoorTemp");

        // Assert
        repository.Received(1).Add(Arg.Any<Register>());
        await repository.Received(1).SaveChangesAsync();
    }
}
