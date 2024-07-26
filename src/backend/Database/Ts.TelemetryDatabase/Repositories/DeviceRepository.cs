namespace TelemetryStash.Database.Repositories;

public interface IDeviceRepository
{
    Task<Device?> GetByDeviceId(string identifier, CancellationToken token = default);

    Task<Device> Upsert(string identifier, CancellationToken token = default);
}

public class DeviceRepository(IDbProvider dbProvider) : IDeviceRepository
{
    public async Task<Device?> GetByDeviceId(string identifier, CancellationToken token = default)
    {
        return await dbProvider
            .QuerySingle<Device>
            (
                where: device => device.Identifier == identifier,
                cancellationToken: token
            );
    }

    public async Task<Device> Upsert(string identifier, CancellationToken token = default)
    {
        return await dbProvider
            .ExecuteSingle<Device>
            (
                storedProcedure: "dbo.GetOrCreateDevice",
                parameters: new { Identifier = identifier },
                cancellationToken: token
            );
    }
}
