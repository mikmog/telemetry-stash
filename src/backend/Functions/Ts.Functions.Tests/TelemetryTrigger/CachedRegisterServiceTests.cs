using TelemetryStash.Functions.TelemetryTrigger.Services;

namespace TelemetryStash.Functions.Tests.TelemetryTrigger;

[Collection("Serial")]
public class CachedRegisterServiceTests : SqLiteTestBase
{
    [Fact]
    public async Task CachedRegisterService_caches_register()
    {
        // Arrange
        using var context = GetDbContext();
        var deviceRepo = new DeviceRepository(context);
        var registerSetRepo = new RegisterSetRepository(context);
        var registerRepo = new RegisterRepository(context);
        var registerKeyRepo = new RegisterKeyRepository(context);

        var device = deviceRepo.Add(new Device(DeviceId: "ESP32"));
        var registerSet = registerSetRepo.Add(new RegisterSet(device.Id, "P1"));
        await deviceRepo.SaveChangesAsync();

        var sut = new CachedRegisterService(registerKeyRepo, registerRepo);

        // Act
        var fromCache1 = CachedRegisterService.GetRegisterKey(registerSet.Id, registerSubset: "P1", "L1");
        var result = await sut.AddAndGet(registerSet.Id, "P1", "L1");
        var fromCache2 = CachedRegisterService.GetRegisterKey(registerSet.Id, registerSubset: "P1", "L1");

        // Assert
        Assert.Null(fromCache1);
        Assert.True(result > 0);
        Assert.Equal(result, fromCache2);
    }

    [Fact]
    public async Task CachedRegisterSetService_AddAndGet_persist_register_to_database()
    {
        // Arrange
        using var context = GetDbContext();
        var deviceRepo = new DeviceRepository(context);
        var registerSetRepo = new RegisterSetRepository(context);
        var registerRepo = new RegisterRepository(context);
        var registerKeyRepo = new RegisterKeyRepository(context);

        var device = deviceRepo.Add(new Device(DeviceId: "ESP32"));
        var registerSet = registerSetRepo.Add(new RegisterSet(device.Id, "P2"));
        await deviceRepo.SaveChangesAsync();

        var sut = new CachedRegisterService(registerKeyRepo, registerRepo);

        // Act
        await sut.AddAndGet(registerSet.Id, "P2", "L2");
        var register = (await registerRepo.All()).Single();

        // Assert
        Assert.Equal(registerSet.Id, register.RegisterSetId);
        Assert.Equal("L2", register.RegisterIdentifier);
        Assert.True(registerSet.Id > 0);
    }

    [Fact]
    public async Task CachedRegisterSetService_AddAndGet_persist_register_key_to_database()
    {
        // Arrange
        using var context = GetDbContext();
        var deviceRepo = new DeviceRepository(context);
        var registerSetRepo = new RegisterSetRepository(context);
        var registerRepo = new RegisterRepository(context);
        var registerKeyRepo = new RegisterKeyRepository(context);

        var device = deviceRepo.Add(new Device(DeviceId: "ESP32"));
        var registerSet = registerSetRepo.Add(new RegisterSet(device.Id, "P1"));
        await deviceRepo.SaveChangesAsync();

        var sut = new CachedRegisterService(registerKeyRepo, registerRepo);

        // Act
        await sut.AddAndGet(registerSet.Id, "P3", "L1");
        var registerKey = (await registerKeyRepo.All()).Single();

        // Assert
        Assert.Equal("P3", registerKey.Subset);
        Assert.True(registerKey.Id > 0);
    }
}
