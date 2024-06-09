using NSubstitute;
using TelemetryStash.Functions.Services;
using TelemetryStash.Functions.Tests.Shared;

namespace TelemetryStash.Functions.Tests.TelemetryTrigger;

public class RegisterKeyServiceTests
{
    [Fact]
    public async Task RegisterKeyService_caches_RegisterKey()
    {
        // Arrange
        using var cacheProvider = new CacheProvider();
        var repository = Substitute.For<IRegisterKeyRepository>();

        var sut = new RegisterKeyService(repository, cacheProvider.HybridCache);

        // Act
        await sut.GetOrAdd(1, "PowerMeter");
        await sut.GetOrAdd(1, "PowerMeter");

        // Assert
        await repository.Received(1).GetRegisterKey(1, "PowerMeter", Arg.Any<Opts<RegisterKey>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterKeyService_AddAndGet_adds_RegisterKey_to_database()
    {
        // Arrange
        using var cacheProvider = new CacheProvider();
        var repository = Substitute.For<IRegisterKeyRepository>();

        var sut = new RegisterKeyService(repository, cacheProvider.HybridCache);

        // Act
        await sut.GetOrAdd(1, "PowerMeter");

        // Assert
        repository.Received(1).Add(Arg.Any<RegisterKey>());
        await repository.Received(1).SaveChangesAsync();
    }
}
