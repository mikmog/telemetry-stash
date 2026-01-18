namespace TelemetryStash.Functions.EnergySpotPrice.Logic;

public class EnergySpotPriceConfiguration
{
    public required HashSet<string> DeliveryAreas { get; set; }
    public bool Enabled { get; set; }
}
