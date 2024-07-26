using NSubstitute;
using TelemetryStash.Database.Repositories;
using TelemetryStash.Functions.Services;

namespace TelemetryStash.Functions.Tests.Services;

public class RegisterSetServiceTests
{
    [Fact]
    public async Task RegisterSetService_GetOrCreate_adds_RegisterSet_to_database()
    {
        // Arrange
        using var cacheProvider = new CacheProvider();
        var repository = Substitute.For<IRegisterSetRepository>();

        var sut = new RegisterSetService(repository, cacheProvider.HybridCache);

        // Act
        await sut.GetOrCreate(deviceId: 1, "TempSensor");

        // Assert
        await repository.Received(1).Upsert(Arg.Is(1), Arg.Is("TempSensor"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterSetService_caches_RegisterSet()
    {
        // Arrange
        using var cacheProvider = new CacheProvider();
        var repository = Substitute.For<IRegisterSetRepository>();

        var sut = new RegisterSetService(repository, cacheProvider.HybridCache);

        // Act
        await sut.GetOrCreate(1, "TempSensor");
        await sut.GetOrCreate(1, "TempSensor");

        // Assert
        await repository.Received(1).Upsert(Arg.Is(1), Arg.Is("TempSensor"), Arg.Any<CancellationToken>());
    }
}
