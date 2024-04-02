using Microsoft.EntityFrameworkCore;
using TEntity = TelemetryStash.Database.Models.RegisterKey;

namespace TelemetryStash.Database.Repositories;

public interface IRegisterKeyRepository : IDbRepository<TEntity>
{
    Task<TEntity?> GetRegisterKey(int registerId, string subset, Opts<TEntity>? opts = null);
}

public class RegisterKeyRepository(TelemetryDbContext context) : RepositoryBase<TEntity>(context), IRegisterKeyRepository
{
    public async Task<TEntity?> GetRegisterKey(int registerId, string subset, Opts<TEntity>? opts = null)
    {
        return await Context
            .Set<TEntity>()
            .WithOptions(opts)
            .Where(reg => reg.RegisterId == registerId)
            .Where(reg => reg.Subset == subset)
            .SingleOrDefaultAsync();
    }
}
