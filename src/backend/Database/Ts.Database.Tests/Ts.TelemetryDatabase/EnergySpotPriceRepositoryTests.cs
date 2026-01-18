using TelemetryStash.Database.Repositories;
using Xunit;

namespace TelemetryStash.Database.Tests.Ts.TelemetryDatabase;

[Collection(CollectionState.SharedTestDbServer)]
public class EnergySpotPriceRepositoryTests(SharedTestDbFixture dbFixture) : TelemetryDbSeeder(dbFixture)
{
    private static readonly DateTimeOffset From1Timestamp = DateTimeOffset.Parse("2026-01-10 12:00:00");
    private static readonly DateTimeOffset From2Timestamp = DateTimeOffset.Parse("2026-01-10 12:15:00");
    private static List<EnergySpotPrice> TestEnergySpotPrices =>
    [
        new (DeliveryArea: "Area1", From: From1Timestamp, Price: 10m, Unit: "SWEDISH_ORE/KWH"),
        new (DeliveryArea: "Area1", From: From2Timestamp, Price: 20m, Unit: "SWEDISH_ORE/KWH"),
    ];

    [Fact]
    public async Task UpsertEnergySpotPrice_insert()
    {
        // Arrange
        var sut = new EnergySpotPriceRepository(GetDbProvider());

        // Act
        await sut.AddOrUpdate(TestEnergySpotPrices, CancellationToken.None);

        // Assert
        var spotPrices = await sut.GetByDeliveryArea("Area1", From1Timestamp, From2Timestamp, CancellationToken.None);

        Assert.Equal(TestEnergySpotPrices.Count, spotPrices.Count);
        for (var i = 0; i < spotPrices.Count; i++)
        {
            Assert.Equal(TestEnergySpotPrices[i].DeliveryArea, spotPrices[i].DeliveryArea);
            Assert.Equal(TestEnergySpotPrices[i].From, spotPrices[i].From);
            Assert.Equal(TestEnergySpotPrices[i].Price, spotPrices[i].Price);
            Assert.Equal(TestEnergySpotPrices[i].Unit, spotPrices[i].Unit);
        }
    }

    [Fact]
    public async Task UpsertEnergySpotPrice_update()
    {
        // Arrange
        var sut = new EnergySpotPriceRepository(GetDbProvider());

        var from1Timestamp = DateTimeOffset.UtcNow.Date;
        var from2Timestamp = from1Timestamp.AddMinutes(15);

        await sut.AddOrUpdate(TestEnergySpotPrices, CancellationToken.None); // Insert

        var updateEnergySpotPrices = TestEnergySpotPrices
            .Select((spotPrice, index) => index == 0 ? spotPrice with { Price = 15m, Unit = "Test update" } : spotPrice) // Update only the first one
        .ToList();

        // Act
        await sut.AddOrUpdate(updateEnergySpotPrices, CancellationToken.None); // Update

        // Assert
        var spotPrices = await sut.GetByDeliveryArea("Area1", From1Timestamp, From2Timestamp, CancellationToken.None);

        Assert.Equal(updateEnergySpotPrices.Count, spotPrices.Count);
        for (var i = 0; i < spotPrices.Count; i++)
        {
            Assert.Equal(updateEnergySpotPrices[i].DeliveryArea, spotPrices[i].DeliveryArea);
            Assert.Equal(updateEnergySpotPrices[i].From, spotPrices[i].From);
            Assert.Equal(updateEnergySpotPrices[i].Price, spotPrices[i].Price);
            Assert.Equal(updateEnergySpotPrices[i].Unit, spotPrices[i].Unit);
        }
    }

    [Fact]
    public async Task GetByDeliveryArea()
    {
        // Arrange
        var sut = new EnergySpotPriceRepository(GetDbProvider());

        await sut.AddOrUpdate(TestEnergySpotPrices, CancellationToken.None);

        // Act
        var spotPrices = await sut.GetByDeliveryArea("Area1", From1Timestamp, From1Timestamp, CancellationToken.None);

        // Assert
        var spotPrice = Assert.Single(spotPrices);
        Assert.Equal(TestEnergySpotPrices[0].DeliveryArea, spotPrice.DeliveryArea);
        Assert.Equal(TestEnergySpotPrices[0].From, spotPrice.From);
        Assert.Equal(TestEnergySpotPrices[0].Price, spotPrice.Price);
        Assert.Equal(TestEnergySpotPrices[0].Unit, spotPrice.Unit);
    }

    [Fact]
    public async Task GetMostRecentByDeliveryArea_returns_null_if_missing()
    {
        // Arrange
        var sut = new EnergySpotPriceRepository(GetDbProvider());

        await sut.AddOrUpdate(TestEnergySpotPrices, CancellationToken.None);

        // Act
        var spotPrice = await sut.GetMostRecentByDeliveryArea("AreaDoesNotExist", CancellationToken.None);

        // Assert
        Assert.Null(spotPrice);
    }

    [Fact]
    public async Task GetMostRecentByDeliveryArea_returns_most_recent()
    {
        // Arrange
        var sut = new EnergySpotPriceRepository(GetDbProvider());

        await sut.AddOrUpdate(TestEnergySpotPrices, CancellationToken.None);
        var newestSpotPrice = TestEnergySpotPrices.MaxBy(sp => sp.From)!;

        // Act
        var spotPrice = await sut.GetMostRecentByDeliveryArea("Area1", CancellationToken.None);

        // Assert
        Assert.NotNull(spotPrice);
        Assert.Equal(newestSpotPrice.DeliveryArea, spotPrice.DeliveryArea);
        Assert.Equal(newestSpotPrice.From, spotPrice.From);
        Assert.Equal(newestSpotPrice.Price, spotPrice.Price);
        Assert.Equal(newestSpotPrice.Unit, spotPrice.Unit);
    }
}
