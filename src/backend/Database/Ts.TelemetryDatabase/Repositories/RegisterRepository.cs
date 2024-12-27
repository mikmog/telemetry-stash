using System.Data;

namespace TelemetryStash.Database.Repositories;

public interface IRegisterRepository
{
#pragma warning disable CA1716 // Identifiers should not match keywords
    Task<RegisterRow?> GetSingle(int deviceId, string registerSet, string register, CancellationToken token = default);
#pragma warning restore CA1716 // Identifiers should not match keywords

    Task<List<RegisterRow>> GetOrCreate(int deviceId, string registerSet, IEnumerable<string> registers, CancellationToken token = default);

}

public class RegisterRepository(IDbProvider dbProvider) : IRegisterRepository
{
    public async Task<RegisterRow?> GetSingle(int deviceId, string registerSet, string register, CancellationToken token = default)
    {
        return await dbProvider
            .QuerySingle<RegisterRow>
            (
                tableName: "[dbo].[Registers]",
                where: r => r.DeviceId == deviceId && r.RegisterSet == registerSet && r.Register == register,
                cancellationToken: token
            );
    }

    public async Task<List<RegisterRow>> GetOrCreate(int deviceId, string registerSet, IEnumerable<string> registers, CancellationToken token = default)
    {
        var telemetries = new DataTable()
        {
            TableName = "dbo.RegistersType"
        };

        telemetries.Columns.Add("RegisterSet", typeof(string));
        telemetries.Columns.Add("Register", typeof(string));

        foreach (var register in registers)
        {
            telemetries.Rows.Add(registerSet, register);
        }

        return await dbProvider
            .ExecuteMultiple<RegisterRow>
            (
                storedProcedure: "dbo.GetOrCreateRegisters",
                parameters: new { DeviceId = deviceId, Registers = telemetries },
                cancellationToken: token
            );
    }
}
