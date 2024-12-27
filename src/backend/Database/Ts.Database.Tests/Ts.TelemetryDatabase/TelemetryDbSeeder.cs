namespace TelemetryStash.Database.Tests.Ts.TelemetryDatabase;

public class TelemetryDbSeeder(SharedTestDbFixture dbFixture)
{
    protected IDbProvider GetDbProvider(string? databaseName = null)
    {
        var name = databaseName ?? GetType().Name;
        return dbFixture.GetTestDbProvider(name);
    }

    protected async Task<DeviceRow> SeedDevice(string deviceId, IDbProvider? dbProvider = null)
    {
        var repository = new DeviceRepository(dbProvider ?? GetDbProvider());
        return await repository.GetOrCreate(deviceId);
    }

    protected async Task<List<RegisterRow>> SeedRegister(int deviceId, string registerSet, IEnumerable<string> registers, IDbProvider? dbProvider = null)
    {
        var repository = new RegisterRepository(dbProvider ?? GetDbProvider());
        return await repository.GetOrCreate(deviceId, registerSet, registers, default);
    }
}
