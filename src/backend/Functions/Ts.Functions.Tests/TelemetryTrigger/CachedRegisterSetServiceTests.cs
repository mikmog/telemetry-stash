using TelemetryStash.Functions.TelemetryTrigger.Services;

namespace TelemetryStash.Functions.Tests.TelemetryTrigger;

public class CachedRegisterSetServiceTests : SqLiteTestBase
{
    [Fact]
    public async Task CachedRegisterSetService_caches_register_set()
    {
        // Arrange
        using var context = GetDbContext();
        var registerSetRepo = new RegisterSetRepository(context);
        var deviceRepo = new DeviceRepository(context);
        var device = deviceRepo.Add(new Device(DeviceId: "ESP32"));
        await deviceRepo.SaveChangesAsync();

        var sut = new CachedRegisterSetService(registerSetRepo);

        // Act
        var fromCache1 = CachedRegisterSetService.GetRegisterSetId(device.Id, registerSet: "P1");
        var result = await sut.AddAndGet(device.Id, "P1");
        var fromCache2 = CachedRegisterSetService.GetRegisterSetId(device.Id, registerSet: "P1");

        // Assert
        Assert.Null(fromCache1);
        Assert.True(result > 0);
        Assert.Equal(result, fromCache2);
    }

    [Fact]
    public async Task CachedRegisterSetService_AddAndGet_persist_to_database()
    {
        // Arrange
        using var context = GetDbContext();
        var registerSetRepo = new RegisterSetRepository(context);
        var deviceRepo = new DeviceRepository(context);
        var device = deviceRepo.Add(new Device(DeviceId: "ESP32"));
        await deviceRepo.SaveChangesAsync();

        var sut = new CachedRegisterSetService(registerSetRepo);

        // Act
        await sut.AddAndGet(device.Id, "P1");
        var regSet = (await registerSetRepo.All()).Single();

        // Assert
        Assert.Equal("P1", regSet.Identifier);
        Assert.True(regSet.DeviceId == device.Id);
        Assert.True(regSet.Id > 0);
    }
}
