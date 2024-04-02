using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TEntity = TelemetryStash.Database.Models.Telemetry;

namespace TelemetryStash.Database.Repositories;

public interface ITelemetryRepository : IDbRepository<TEntity>
{
    Task UpsertTelemetry(int deviceId, DateTimeOffset timestamp, List<(int RegisterKey, decimal Value)> telemetry);
}

public class TelemetryRepository(TelemetryDbContext context) : RepositoryBase<TEntity>(context), ITelemetryRepository
{
    public async Task UpsertTelemetry(int deviceId, DateTimeOffset timestamp, List<(int RegisterKey, decimal Value)> telemetry)
    {
        var telemetryTable = new DataTable();
        telemetryTable.Columns.Add("RegisterKeyId", typeof(int));
        telemetryTable.Columns.Add("Value", typeof(decimal));
        foreach (var (registerKey, value) in telemetry)
        {
            telemetryTable.Rows.Add(registerKey, value);
        }

        await Context
            .Database
            .ExecuteSqlRawAsync($"EXEC [dbo].[uspUpsertTelemetry] @DeviceId, @Timestamp, @Telemetry",
                new SqlParameter("@DeviceId", deviceId),
                new SqlParameter("@Timestamp", timestamp),
                new SqlParameter
                {
                    ParameterName = "@Telemetry",
                    TypeName = "dbo.TelemetriesType",
                    SqlDbType = SqlDbType.Structured,
                    Value = telemetryTable
                });
    }
}
