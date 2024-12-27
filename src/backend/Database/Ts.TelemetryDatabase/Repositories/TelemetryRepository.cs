using RepoDb;
using System.Data;
using System.Runtime.CompilerServices;

namespace TelemetryStash.Database.Repositories;

public interface ITelemetryRepository
{
    IAsyncEnumerable<List<TelemetryRow>> GetTelemetry(int deviceId, DateTimeOffset fromClientTimestamp, DateTimeOffset toClientTimestamp, int preferredBatchSize = 1000, CancellationToken token = default);
    Task Upsert(int deviceId, DateTimeOffset timestamp, IEnumerable<(int RegisterId, string Value)> registerValues, CancellationToken token = default);
}

public class TelemetryRepository(IDbProvider dbProvider) : ITelemetryRepository
{
    public async IAsyncEnumerable<List<TelemetryRow>> GetTelemetry(int deviceId, DateTimeOffset fromClientTimestamp, DateTimeOffset toClientTimestamp, int preferredBatchSize = 1000, [EnumeratorCancellation] CancellationToken token = default)
    {
        using var connection = dbProvider.CreateConnection();

        var skip = 0;
        var take = preferredBatchSize;

        while(true)
        {
            var telemetries = (await connection.ExecuteQueryAsync<TelemetryRow>
            (
                commandText: "dbo.GetTelemetryValues",
                param: new
                {
                    DeviceId = deviceId,
                    FromClientTimestamp = fromClientTimestamp,
                    ToClientTimestamp = toClientTimestamp,
                    Skip = skip,
                    Take = take
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: token
            )).ToList();

            if(telemetries.Count == 0)
            {
                break;
            }

            skip += telemetries.Count;

            yield return telemetries;
        }

        yield return [];
    }

    public async Task Upsert(int deviceId, DateTimeOffset timestamp, IEnumerable<(int RegisterId, string Value)> registerValues, CancellationToken token = default)
    {
        var telemetries = new DataTable()
        {
            TableName = "dbo.TelemetriesType"
        };

        telemetries.Columns.Add("RegisterId", typeof(int));
        telemetries.Columns.Add("Value", typeof(string));

        foreach (var (RegisterId, Value) in registerValues)
        {
            telemetries.Rows.Add(RegisterId, Value);
        }

        await dbProvider
            .ExecuteScalar
            (
                storedProcedure: "dbo.UpsertTelemetry",
                parameters: new { DeviceId = deviceId, ClientTimestamp = timestamp, Telemetry = telemetries },
                cancellationToken: token
            );
    }
}
