namespace TelemetryStash.Database.Tests.Ts.TelemetryDatabase;

[Collection(CollectionState.SharedTestDbServer)]
public class DeviceRepositoryTests(SharedTestDbFixture dbFixture) : TelemetryDbSeeder(dbFixture)
{
    [Fact]
    public async Task DeviceRepository_Upsert_returns_created()
    {
        // Arrange
        var sut = new DeviceRepository(GetDbProvider());

        // Act
        var device = await sut.Upsert("TestDeviceId");

        // Assert
        Assert.NotEqual(0, device.Id);
        Assert.Equal("TestDeviceId", device.Identifier);
    }

    [Fact]
    public async Task DeviceRepository_Get_returns_single()
    {
        // Arrange
        var sut = new DeviceRepository(GetDbProvider());

        // Act
        await sut.Upsert("TestDeviceId");
        var device = await sut.GetByDeviceId("TestDeviceId");

        // Assert
        Assert.NotNull(device);
        Assert.NotEqual(0, device.Id);
        Assert.Equal("TestDeviceId", device.Identifier);
    }
}
