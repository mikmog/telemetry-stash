namespace TelemetryStash.Database.Models;

public class Timestamp
{
    public int Id { get; set; }
    public short DeviceId { get; set; }
    public DateTimeOffset Ts { get; set; }
    public DateTimeOffset Created { get; set; }
}
