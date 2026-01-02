using TelemetryStash.Database.Repositories;
using Xunit;

namespace TelemetryStash.Database.Tests.Ts.TelemetryDatabase;

[Collection(CollectionState.SharedTestDbServer)]
public class HealthRepositoryTests(SharedTestDbFixture dbFixture) : TelemetryDbSeeder(dbFixture)
{
    [Fact]
    public async Task HealthCheck_does_not_throw()
    {
        // Arrange
        var sut = new HealthRepository(GetDbProvider());

        // Act
        var exception = await Record.ExceptionAsync(() => sut.HealthCheck(CancellationToken.None));

        // Assert
        Assert.Null(exception);
    }
}
