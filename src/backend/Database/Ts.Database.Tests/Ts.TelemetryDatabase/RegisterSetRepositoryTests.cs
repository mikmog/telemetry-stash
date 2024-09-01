namespace TelemetryStash.Database.Tests.Ts.TelemetryDatabase;

[Collection(CollectionState.SharedTestDbServer)]
public class RegisterSetRepositoryTests(SharedTestDbFixture dbFixture) : TelemetryDbSeeder(dbFixture)
{
    [Fact]
    public async Task RegisterSetRepository_Upsert_returns_created()
    {
        // Arrange
        var deviceRepository = new DeviceRepository(GetDbProvider());
        var device = await deviceRepository.Upsert("TestDeviceId");

        var sut = new RegisterSetRepository(GetDbProvider());

        // Act
        var registerSet = await sut.Upsert(device.Id, "RegisterSetIdentifier");

        // Assert
        Assert.NotEqual(0, registerSet.Id);
        Assert.Equal(device.Id, registerSet.DeviceId);
        Assert.Equal("RegisterSetIdentifier", registerSet.Identifier);
    }

    [Fact]
    public async Task RegisterSetRepository_Get_returns_single()
    {
        // Arrange
        var device = await SeedDevice("TestDeviceId");
        await SeedRegisterSet(device, "RegisterSetIdentifier");

        var sut = new RegisterSetRepository(GetDbProvider());

        // Act
        var registerSet = await sut.GetByDeviceAndIdentifier(device.Id, "RegisterSetIdentifier");

        // Assert
        Assert.NotNull(registerSet);
        Assert.NotEqual(0, registerSet.Id);
        Assert.Equal(device.Id, registerSet.DeviceId);
        Assert.Equal("RegisterSetIdentifier", registerSet.Identifier);
    }
}
