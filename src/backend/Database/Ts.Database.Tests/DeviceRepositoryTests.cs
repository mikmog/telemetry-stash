namespace TelemetryStash.Database.Tests;

public class DeviceRepositoryTests : SqLiteTestBase
{
    [Fact]
    public async Task Repository_returns_all()
    {
        // Arrange
        using var context = GetDbContext();
        var sut = new DeviceRepository(context);

        // Act
        sut.Add(new Device(DeviceId: "ESP32"));
        await sut.SaveChangesAsync();
        var result = await sut.All();

        // Assert
        Assert.Single(result);
    }
}
