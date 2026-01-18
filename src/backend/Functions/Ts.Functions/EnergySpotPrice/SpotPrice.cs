using Microsoft.Azure.Functions.Worker;
using TelemetryStash.Functions.EnergySpotPrice.Logic;

namespace TelemetryStash.Functions.EnergySpotPrice;

public class EnergySpotPrice(IEnergySpotPriceService energySpotPriceService)
{
    // Prices release at 12 utc
    private const string Run10MinutesPastBetween12To15 = "0 10 12-15 * * *";

    [Function("EnergySpotPrice")]
    public async Task Run([TimerTrigger(Run10MinutesPastBetween12To15, RunOnStartup = true)] TimerInfo timer, CancellationToken cancellationToken)
    {
        await energySpotPriceService.ProcessEnergySpotPrice(cancellationToken);
    }
}
