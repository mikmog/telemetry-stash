namespace TelemetryStash.Database.Repositories;

public interface IRegisterTemplateRepository
{
    Task<RegisterTemplate?> GetRegisterTemplate(int registerSetId, string identifier, CancellationToken token = default);

    Task<RegisterTemplate> Upsert(int registerSetId, string identifier, CancellationToken token = default);
}

public class RegisterTemplateRepository(IDbProvider dbProvider) : IRegisterTemplateRepository
{
    public async Task<RegisterTemplate?> GetRegisterTemplate(int registerSetId, string identifier, CancellationToken token = default)
    {
        return await dbProvider
            .QuerySingle<RegisterTemplate>
            (
                where: template => template.RegisterSetId == registerSetId && template.Identifier == identifier,
                cancellationToken: token
            );
    }

    // TODO: Handle missing fields
    public async Task<RegisterTemplate> Upsert(int registerSetId, string identifier, CancellationToken token = default)
    {
        return await dbProvider
            .ExecuteSingle<RegisterTemplate>
            (
                storedProcedure: "dbo.GetOrCreateRegisterTemplate",
                parameters: new { RegisterSetId = registerSetId, Identifier = identifier },
                cancellationToken: token
            );
    }
}
