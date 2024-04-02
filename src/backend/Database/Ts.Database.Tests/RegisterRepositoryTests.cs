namespace TelemetryStash.Database.Tests;

public class RegisterRepositoryTests : SqLiteTestBase
{
    [Fact]
    public async Task Repository_returns_all()
    {
        // Arrange
        using var context = GetDbContext();

        var deviceRepo = new DeviceRepository(context);
        var registerSetRepo = new RegisterSetRepository(context);
        var sut = new RegisterRepository(context);

        var device = deviceRepo.Add(new Device(DeviceId: "ESP32"));
        var regSet = registerSetRepo.Add(new RegisterSet(DeviceId: device.Id, Identifier: "P1"));
        var reg = sut.Add(new Register(RegisterSetId: regSet.Id, RegisterIdentifier: "L1"));
        await deviceRepo.SaveChangesAsync();

        // Act
        var result = await sut.All();

        // Assert
        Assert.Single(result);
    }
}
