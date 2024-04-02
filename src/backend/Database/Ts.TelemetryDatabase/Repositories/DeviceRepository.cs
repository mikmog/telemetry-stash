using Microsoft.EntityFrameworkCore;
using TEntity = TelemetryStash.Database.Models.Device;

namespace TelemetryStash.Database.Repositories;

public interface IDeviceRepository : IDbRepository<TEntity>
{
    Task<TEntity?> GetByDeviceId(string deviceId, Opts<TEntity>? opts = null);
}

public class DeviceRepository(TelemetryDbContext context) : RepositoryBase<TEntity>(context), IDeviceRepository
{
    public async Task<TEntity?> GetByDeviceId(string deviceId, Opts<TEntity>? opts = null)
    {
        return await Context
            .Set<TEntity>()
            .WithOptions(opts)
            .SingleOrDefaultAsync(d => d.DeviceId == deviceId);
    }
}
