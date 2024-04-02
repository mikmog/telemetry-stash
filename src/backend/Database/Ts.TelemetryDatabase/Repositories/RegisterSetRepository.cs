using Microsoft.EntityFrameworkCore;
using TEntity = TelemetryStash.Database.Models.RegisterSet;

namespace TelemetryStash.Database.Repositories;

public interface IRegisterSetRepository : IDbRepository<TEntity>
{
    Task<TEntity?> GetRegisterSet(int deviceId, string identifier, Opts<TEntity>? opts = null);
}

public class RegisterSetRepository(TelemetryDbContext context) : RepositoryBase<TEntity>(context), IRegisterSetRepository
{
    public async Task<TEntity?> GetRegisterSet(int deviceId, string identifier, Opts<TEntity>? opts = null)
    {
        return await Context
            .Set<TEntity>()
            .WithOptions(opts)
            .Where(reg => reg.DeviceId == deviceId)
            .Where(reg => reg.Identifier == identifier)
            .SingleOrDefaultAsync();
    }
}
