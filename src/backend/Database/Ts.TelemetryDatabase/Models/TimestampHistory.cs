namespace TelemetryStash.Database.Models;

public class TimestampHistory
{
    public int Id { get; set; }
    public short DeviceId { get; set; }
    public DateTimeOffset Ts { get; set; }
    public DateTimeOffset Created { get; set; }
}
