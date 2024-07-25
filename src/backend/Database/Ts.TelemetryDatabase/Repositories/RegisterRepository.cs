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
            .QuerySingle<Register>
            (
                where: register => register.RegisterTemplateId == registerTemplateId && register.Subset == subset,
                cancellationToken: token
            );
    }

    public async Task<Register> Upsert(int registerTemplateId, string subset, CancellationToken token = default)
    {
        return await dbProvider
            .ExecuteSingle<Register>
            (
                storedProcedure: "dbo.GetOrCreateRegister",
                parameters: new { RegisterTemplateId = registerTemplateId, Subset = subset },
                cancellationToken: token
            );
    }
}
