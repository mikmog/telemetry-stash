namespace TelemetryStash.Database.Tests;

[Collection("TestSet1")]
public class DeviceRepositoryTests(TestDbFixture testDbFixture) : IAsyncLifetime
{
    private IDbProvider _dbProvider = default!;

    public async Task InitializeAsync() => _dbProvider = await testDbFixture.GetTestDbProvider(nameof(DeviceRepositoryTests));

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task DeviceRepository_Upsert_device_returns_created_device()
    {
        // Arrange
        var sut = new DeviceRepository(_dbProvider);

        // Act
        var device = await sut.Upsert("TestDeviceId");

        // Assert
        Assert.NotEqual(0, device.Id);
        Assert.Equal("TestDeviceId", device.DeviceId);
    }

    [Fact]
    public async Task DeviceRepository_Get_device_returns_device()
    {
        // Arrange
        var sut = new DeviceRepository(_dbProvider);

        // Act
        await sut.Upsert("TestDeviceId");
        var device = await sut.GetByDeviceId("TestDeviceId");

        // Assert
        Assert.NotNull(device);
        Assert.NotEqual(0, device.Id);
        Assert.Equal("TestDeviceId", device.DeviceId);
    }
}
