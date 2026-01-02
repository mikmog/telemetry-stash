namespace TelemetryStash.Functions.Weather.Logic;

public class WeatherPrognosisConfiguration
{
    public required string Name { get; set; }
    public required string Longitude { get; set; }
    public required string Latitude { get; set; }
    public bool Enabled { get; set; }
}
