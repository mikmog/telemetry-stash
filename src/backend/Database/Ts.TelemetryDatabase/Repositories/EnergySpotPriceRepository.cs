using RepoDb.Enumerations;
using System.Data;

namespace TelemetryStash.Database.Repositories;

public interface IEnergySpotPriceRepository
{
    public Task AddOrUpdate(List<EnergySpotPrice> energySpotPrices, CancellationToken token);
    public Task<List<EnergySpotPrice>> GetByDeliveryArea(string deliveryArea, DateTimeOffset validFrom, DateTimeOffset validTo, CancellationToken token);
    public Task<EnergySpotPrice?> GetMostRecentByDeliveryArea(string deliveryArea, CancellationToken token);
}

public class EnergySpotPriceRepository(IDbProvider dbProvider) : IEnergySpotPriceRepository
{
    public async Task AddOrUpdate(List<EnergySpotPrice> energySpotPrices, CancellationToken token)
    {
        var table = new DataTable("EnergySpotPriceType");
        table.Columns.Add(nameof(EnergySpotPrice.DeliveryArea), typeof(string));
        table.Columns.Add(nameof(EnergySpotPrice.From), typeof(DateTimeOffset));
        table.Columns.Add(nameof(EnergySpotPrice.Price), typeof(Decimal));
        table.Columns.Add(nameof(EnergySpotPrice.Unit), typeof(string));

        foreach (var energySpotPrice in energySpotPrices)
        {
            table.Rows.Add(
                energySpotPrice.DeliveryArea,
                energySpotPrice.From,
                energySpotPrice.Price,
                energySpotPrice.Unit
            );
        }

        await dbProvider.ExecuteScalar(
            storedProcedure: "UpsertEnergySpotPrice",
            parameters: new
            {
                EnergySpotPrice = table
            },
            cancellationToken: token
        );
    }

    public async Task<List<EnergySpotPrice>> GetByDeliveryArea(string deliveryArea, DateTimeOffset validFrom, DateTimeOffset validTo, CancellationToken token)
    {
        var result = await dbProvider
            .QueryMultiple<EnergySpotPrice>
            (
                where: row => row.DeliveryArea == deliveryArea && row.From >= validFrom && row.From <= validTo,
                cancellationToken: token
            );

        return result.ToList();
    }

    public async Task<EnergySpotPrice?> GetMostRecentByDeliveryArea(string deliveryArea, CancellationToken token)
    {
        var result = await dbProvider
            .QuerySingle<EnergySpotPrice>
            (
                where: row => row.DeliveryArea == deliveryArea,
                orderBy: [new(nameof(EnergySpotPrice.From), Order.Descending)],
                top: 1,
                cancellationToken: token
            );

        return result;
    }
}

public record EnergySpotPrice(string DeliveryArea, DateTimeOffset From, Decimal Price, string Unit);
