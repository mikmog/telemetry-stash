using NSubstitute;
using TelemetryStash.Database.Repositories;
using TelemetryStash.Functions.Services;

namespace TelemetryStash.Functions.Tests.Services;

public class RegisterServiceTests
{

    [Fact]
    public async Task RegisterKeyService_GetOrCreate_adds_Register_to_database()
    {
        // Arrange
        using var cacheProvider = new CacheProvider();
        var repository = Substitute.For<IRegisterRepository>();

        var sut = new RegisterService(repository, cacheProvider.HybridCache);

        // Act
        await sut.GetOrCreate(registerId: 1, "OutdoorTemp");

        // Assert
        await repository.Received(1).Upsert(Arg.Is(1), Arg.Is("OutdoorTemp"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterService_caches_Register()
    {
        // Arrange
        using var cacheProvider = new CacheProvider();
        var repository = Substitute.For<IRegisterRepository>();

        var sut = new RegisterService(repository, cacheProvider.HybridCache);

        // Act
        await sut.GetOrCreate(registerId: 1, "OutdoorTemp");
        await sut.GetOrCreate(registerId: 1, "OutdoorTemp");

        // Assert
        await repository.Received(1).Upsert(Arg.Is(1), Arg.Is("OutdoorTemp"), Arg.Any<CancellationToken>());
    }
}
