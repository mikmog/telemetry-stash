namespace TelemetryStash.Database.Models;

public record TimestampHistory(short DeviceId, DateTimeOffset Ts, DateTimeOffset Created)
{
    public int Id { get; set; }
}
