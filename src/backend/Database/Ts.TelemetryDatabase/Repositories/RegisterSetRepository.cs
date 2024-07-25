namespace TelemetryStash.Database.Repositories;

public interface IRegisterSetRepository
{
    Task<RegisterSet?> GetByDeviceAndIdentifier(int deviceId, string identifier, CancellationToken token = default);

    Task<RegisterSet> Upsert(int deviceId, string identifier, CancellationToken token = default);
}

public class RegisterSetRepository(IDbProvider dbProvider) : IRegisterSetRepository
{
    public async Task<RegisterSet?> GetByDeviceAndIdentifier(int deviceId, string identifier, CancellationToken token = default)
    {
        return await dbProvider
            .QuerySingle<RegisterSet>
            (
                where: set => set.DeviceId == deviceId && set.Identifier == identifier,
                cancellationToken: token
            );
    }

    public async Task<RegisterSet> Upsert(int deviceId, string identifier, CancellationToken token = default)
    {
        return await dbProvider
            .ExecuteSingle<RegisterSet>
            (
                storedProcedure: "dbo.GetOrCreateRegisterSet",
                parameters: new { DeviceId = deviceId, Identifier = identifier },
                cancellationToken: token
            );
    }
}
