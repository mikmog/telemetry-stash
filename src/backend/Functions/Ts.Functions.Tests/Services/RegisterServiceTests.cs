using NSubstitute;
using TelemetryStash.Functions.Services;

namespace TelemetryStash.Functions.Tests.TelemetryTrigger;

public class RegisterServiceTests
{
    [Fact]
    public async Task RegisterService_caches_Register()
    {
        // Arrange
        using var cacheProvider = new CacheProvider();
        var repository = Substitute.For<IRegisterTemplateRepository>();

        var sut = new RegisterTemplateService(repository, cacheProvider.HybridCache);

        // Act
        await sut.GetOrCreate(1, "OutdoorTemp");
        await sut.GetOrCreate(1, "OutdoorTemp");

        // Assert
        await repository.Received(1).GetRegister(1, "OutdoorTemp", Arg.Any<Opts<Register>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterService_AddAndGet_adds_Register_to_database()
    {
        // Arrange
        using var cacheProvider = new CacheProvider();
        var repository = Substitute.For<IRegisterTemplateRepository>();

        var sut = new RegisterTemplateService(repository, cacheProvider.HybridCache);

        // Act
        await sut.GetOrCreate(1, "OutdoorTemp");

        // Assert
        repository.Received(1).Add(Arg.Any<Register>());
        await repository.Received(1).SaveChangesAsync();
    }
}
