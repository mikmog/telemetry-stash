namespace TelemetryStash.Database.Tests;

public class RegisterKeyRepositoryTests : SqLiteTestBase
{
    [Fact]
    public async Task Repository_returns_all()
    {
        // Arrange
        using var context = GetDbContext();

        var deviceRepo = new DeviceRepository(context);
        var registerSetRepo = new RegisterSetRepository(context);
        var registerRepo = new RegisterTemplateRepository(context);
        var sut = new RegisterKeyRepository(context);

        var device = deviceRepo.Add(new Device(DeviceId: "ESP32"));
        var regSet = registerSetRepo.Add(new RegisterSet(DeviceId: device.Id, Identifier: "P1"));
        var reg = registerRepo.Add(new Register(RegisterSetId: regSet.Id, RegisterIdentifier: "L1"));
        var regKey = sut.Add(new Register(RegisterId: reg.Id, Subset: "K1"));

        await deviceRepo.SaveChangesAsync();

        // Act
        var result = await sut.All();

        // Assert
        Assert.Single(result);
    }
}
