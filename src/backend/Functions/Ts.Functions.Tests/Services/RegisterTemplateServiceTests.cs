using NSubstitute;
using TelemetryStash.Database.Repositories;
using TelemetryStash.Functions.Services;

namespace TelemetryStash.Functions.Tests.TelemetryTrigger;

public class RegisterTemplateServiceTests
{
    [Fact]
    public async Task RegisterTemplateService_GetOrCreate_adds_RegisterTemplate_to_database()
    {
        // Arrange
        using var cacheProvider = new CacheProvider();
        var repository = Substitute.For<IRegisterTemplateRepository>();

        var sut = new RegisterTemplateService(repository, cacheProvider.HybridCache);

        // Act
        await sut.GetOrCreate(registerSetId: 1, "OutdoorTemp");

        // Assert
        await repository.Received(1).Upsert(Arg.Is(1), Arg.Is("OutdoorTemp"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterTemplateService_caches_RegisterTemplate()
    {
        // Arrange
        using var cacheProvider = new CacheProvider();
        var repository = Substitute.For<IRegisterTemplateRepository>();

        var sut = new RegisterTemplateService(repository, cacheProvider.HybridCache);

        // Act
        await sut.GetOrCreate(1, "OutdoorTemp");
        await sut.GetOrCreate(1, "OutdoorTemp");

        // Assert
        await repository.Received(1).Upsert(Arg.Is(1), Arg.Is("OutdoorTemp"), Arg.Any<CancellationToken>());
    }
}
