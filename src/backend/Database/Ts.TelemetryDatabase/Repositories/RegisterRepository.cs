using TelemetryStash.Database.Models;

namespace TelemetryStash.Database.Repositories;

public interface IRegisterRepository
{
    Task<Register?> GetByTemplateAndSubset(int registerTemplateId, string subset, CancellationToken token = default);

    Task<Register> Upsert(int registerTemplateId, string subset, CancellationToken token = default);
}

public class RegisterRepository(IDbProvider dbProvider) : IRegisterRepository
{
    public async Task<Register?> GetByTemplateAndSubset(int registerTemplateId, string subset, CancellationToken token = default)
    {
        return await dbProvider
            .ExecuteStoredProcedure<Register>("dbo.GetRegister", new { RegisterTemplateId = registerTemplateId, Subset = subset }, token);
    }

    public async Task<Register> Upsert(int registerTemplateId, string subset, CancellationToken token = default)
    {
        return await dbProvider
            .ExecuteStoredProcedure<Register>("dbo.UpsertRegister", new { RegisterTemplateId = registerTemplateId, Subset = subset }, token)
            ?? throw new Exception("UpsertRegister not null expected");
    }
}
