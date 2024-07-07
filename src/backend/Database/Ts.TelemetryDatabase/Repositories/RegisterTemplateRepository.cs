using Microsoft.EntityFrameworkCore;
using TEntity = TelemetryStash.Database.Models.Register;

namespace TelemetryStash.Database.Repositories;

public interface IRegisterRepository : IDbRepository<TEntity>
{
    Task<TEntity?> GetRegister(int registerSetId, string registerIdentifier, Opts<TEntity>? opts = null, CancellationToken token = default);
}

public class RegisterTemplateRepository(TelemetryDbContext context) : RepositoryBase<TEntity>(context), IRegisterRepository
{
    public async Task<TEntity?> GetRegister(int registerSetId, string registerIdentifier, Opts<TEntity>? opts = null, CancellationToken token = default)
    {
        return await Context
            .Set<TEntity>()
            .WithOptions(opts)
            .Where(reg => reg.RegisterSetId == registerSetId)
            .Where(reg => reg.RegisterIdentifier == registerIdentifier)
            .SingleOrDefaultAsync(token);
    }
}
