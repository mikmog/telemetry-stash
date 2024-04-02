using Microsoft.EntityFrameworkCore;
using TEntity = TelemetryStash.Database.Models.Register;

namespace TelemetryStash.Database.Repositories;

public interface IRegisterRepository : IDbRepository<TEntity>
{
    Task<TEntity?> GetRegister(int registerSetId, string registerIdentifier, Opts<TEntity>? opts = null);
}

public class RegisterRepository(TelemetryDbContext context) : RepositoryBase<TEntity>(context), IRegisterRepository
{
    public async Task<TEntity?> GetRegister(int registerSetId, string registerIdentifier, Opts<TEntity>? opts = null)
    {
        return await Context
            .Set<TEntity>()
            .WithOptions(opts)
            .Where(reg => reg.RegisterSetId == registerSetId)
            .Where(reg => reg.RegisterIdentifier == registerIdentifier)
            .SingleOrDefaultAsync();
    }
}
