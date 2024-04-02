namespace TelemetryStash.Database.Tests;

public class RegisterSetRepositoryTests : SqLiteTestBase
{
    [Fact]
    public async Task Repository_returns_all()
    {
        // Arrange
        using var context = GetDbContext();

        var deviceRepo = new DeviceRepository(context);
        var sut = new RegisterSetRepository(context);

        var device = deviceRepo.Add(new Device(DeviceId: "ESP32"));
        var regSet = sut.Add(new RegisterSet(DeviceId: device.Id, Identifier: "P1"));

        await deviceRepo.SaveChangesAsync();

        // Act
        var result = await sut.All();

        // Assert
        Assert.Single(result);
    }
}
