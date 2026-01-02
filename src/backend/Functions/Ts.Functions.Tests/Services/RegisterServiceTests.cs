using NSubstitute;
using TelemetryStash.Database;
using TelemetryStash.Database.Repositories;
using TelemetryStash.Functions.TelemetryTrigger.Logic;

namespace TelemetryStash.Functions.Tests.Services;

public class RegisterServiceTests
{
    [Fact]
    public async Task RegisterKeyService_GetOrCreate_adds_Register_to_database()
    {
        // Arrange
        using var cacheProvider = new CacheProvider();
        var repository = Substitute.For<IRegisterRepository>();
        repository.GetOrCreate(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<RegisterRow> { new(1, 1, "RegisterSet1", "Register1") }));

        var registerSet1 = $"RegisterSet1{Guid.NewGuid()}";
        var registerSet2 = $"RegisterSet2{Guid.NewGuid()}";
        var registers = new[] { $"Register1{Guid.NewGuid()}" };

        var sut = new RegisterService(repository, cacheProvider.HybridCache);

        // Act
        await sut.GetOrCreate(deviceId: 1, registerSet1, registers);
        await sut.GetOrCreate(deviceId: 1, registerSet2, registers);

        // Assert
        await repository
            .Received(2)
            .GetOrCreate(
                Arg.Is(1),
                Arg.Is<string>(reg => reg == registerSet1 || reg == registerSet2),
                Arg.Is<IEnumerable<string>>(x => x.SequenceEqual(registers)), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterService_caches_Register()
    {
        // Arrange
        using var cacheProvider = new CacheProvider();
        var repository = Substitute.For<IRegisterRepository>();
        repository.GetOrCreate(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<RegisterRow> { new(1, 1, "RegisterSet1", "Register1") }));

        var registerSetIdentifier = $"RegisterSet1{Guid.NewGuid()}";
        var registers = new[] { $"Register1{Guid.NewGuid()}" };

        var sut = new RegisterService(repository, cacheProvider.HybridCache);

        // Act
        await sut.GetOrCreate(deviceId: 1, registerSetIdentifier, registers);

        // Assert
        await repository.Received(1).GetOrCreate(Arg.Is(1), Arg.Is(registerSetIdentifier), Arg.Is<IEnumerable<string>>(x => x.SequenceEqual(registers)), Arg.Any<CancellationToken>());
    }
}
