using System.Data;

namespace TelemetryStash.Database.Repositories;

public interface ITelemetryRepository
{
    Task Upsert(int deviceId, DateTimeOffset timestamp, List<(int RegisterId, decimal Value)> telemetry, CancellationToken token = default);
}

public class TelemetryRepository(IDbProvider dbProvider) : ITelemetryRepository
{
    public async Task Upsert(int deviceId, DateTimeOffset timestamp, List<(int RegisterId, decimal Value)> telemetry, CancellationToken token = default)
    {
        var telemetries = new DataTable()
        {
            TableName = "dbo.TelemetriesType"
        };

        telemetries.Columns.Add("RegisterId", typeof(int));
        telemetries.Columns.Add("Value", typeof(decimal));

        foreach (var (registerId, value) in telemetry)
        {
            telemetries.Rows.Add(registerId, value);
        }

        await dbProvider.ExecuteStoredProcedure("dbo.UpsertTelemetry",
            new
            {
                DeviceId = deviceId,
                Timestamp = timestamp,
                Telemetry = telemetries
            },
            token
            );
    }
}
