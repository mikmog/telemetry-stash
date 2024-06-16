namespace TelemetryStash.Database.Models;

public record Timestamp(short DeviceId, DateTimeOffset Ts, DateTimeOffset Created)
{
    public int Id { get; set; }
}
