using System.Text.Json;
using TelemetryStash.Database.Repositories;
using Xunit;

namespace TelemetryStash.Database.Tests.Ts.TelemetryDatabase;

[Collection(CollectionState.SharedTestDbServer)]
public class WeatherPrognosisRepositoryTests(SharedTestDbFixture dbFixture) : TelemetryDbSeeder(dbFixture)
{
    [Fact]
    public async Task AddOrUpdate_Executes_without_exception()
    {
        // Arrange
        var timeStamp = DateTimeOffset.UtcNow;
        var sut = new WeatherPrognosisRepository(GetDbProvider());

        var json = JsonSerializer.Serialize(new
        {
            Test1 = 10,
            Test2 = "Test2",
        });

        // Act
        var exception1 = await Record.ExceptionAsync(() => sut.AddOrUpdate("name", timeStamp, json, CancellationToken.None));
        var exception2 = await Record.ExceptionAsync(() => sut.AddOrUpdate("name", timeStamp, json, CancellationToken.None));

        // Assert
        Assert.Null(exception1);
        Assert.Null(exception2);
    }
}
