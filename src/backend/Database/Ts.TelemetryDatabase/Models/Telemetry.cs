namespace TelemetryStash.Database.Models;

public record Telemetry(int RegisterKeyId, int TimestampId, decimal Value)
{
    public int Id { get; set; }
}
