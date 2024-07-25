namespace TelemetryStash.Database.Tests.Ts.TelemetryDatabase;

public class TelemetryDbSeeder(SharedTestDbFixture dbFixture)
{
    protected IDbProvider GetDbProvider(string? databaseName = null)
    {
        var name = databaseName ?? GetType().Name;
        return dbFixture.GetTestDbProvider(name);
    }

    protected async Task<Device> SeedDevice(string deviceId, IDbProvider? dbProvider = null)
    {
        var repository = new DeviceRepository(dbProvider ?? GetDbProvider());
        return await repository.Upsert(deviceId);
    }

    protected async Task<RegisterSet> SeedRegisterSet(Device device, string identifier, IDbProvider? dbProvider = null)
    {
        var repository = new RegisterSetRepository(dbProvider ?? GetDbProvider());
        return await repository.Upsert(device.Id, identifier);
    }

    protected async Task<RegisterTemplate> SeedRegisterTemplate(RegisterSet registerSet, string identifier, IDbProvider? dbProvider = null)
    {
        var repository = new RegisterTemplateRepository(dbProvider ?? GetDbProvider());
        return await repository.Upsert(registerSet.Id, identifier);
    }

    protected async Task<Register> SeedRegister(RegisterTemplate registerTemplate, string subset, IDbProvider? dbProvider = null)
    {
        var repository = new RegisterRepository(dbProvider ?? GetDbProvider());
        return await repository.Upsert(registerTemplate.Id, subset);
    }
}
