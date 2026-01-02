namespace TelemetryStash.Database.Repositories;

public interface IWeatherPrognosisRepository
{
    public Task AddOrUpdate(string name, DateTimeOffset timeStamp, string data, CancellationToken token);
}

public class WeatherPrognosisRepository(IDbProvider dbProvider) : IWeatherPrognosisRepository
{
    public async Task AddOrUpdate(string name, DateTimeOffset timeStamp, string data, CancellationToken token)
    {
        await dbProvider.ExecuteScalar(
            storedProcedure: "dbo.UpsertWeatherPrognosis",
            parameters: new
            {
                Name = name,
                TimeStamp = timeStamp,
                Data = data
            },
            cancellationToken: token
        );
    }
}
