namespace TelemetryStash.Database.Repositories;

public interface IDeviceRepository
{
    Task<DeviceRow?> GetByDeviceId(string deviceIdentifier, CancellationToken token = default);

    Task<DeviceRow> GetOrCreate(string deviceIdentifier, CancellationToken token = default);
}

public class DeviceRepository(IDbProvider dbProvider) : IDeviceRepository
{
    public async Task<DeviceRow?> GetByDeviceId(string deviceIdentifier, CancellationToken token = default)
    {
        return await dbProvider
            .QuerySingle<DeviceRow>
            (
                tableName: "[dbo].[Devices]",
                where: device => device.Identifier == deviceIdentifier,
                cancellationToken: token
            );
    }

    public async Task<DeviceRow> GetOrCreate(string deviceIdentifier, CancellationToken token = default)
    {
        return await dbProvider
            .ExecuteSingle<DeviceRow>
            (
                storedProcedure: "dbo.GetOrCreateDevice",
                parameters: new { Identifier = deviceIdentifier },
                cancellationToken: token
            );
    }
}
