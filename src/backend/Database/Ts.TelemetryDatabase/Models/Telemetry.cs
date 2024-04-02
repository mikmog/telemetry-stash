namespace TelemetryStash.Database.Models;

public class Telemetry
{
    public int Id { get; set; }

    public required int RegisterKeyId { get; set; }
    public required int TimestampId { get; set; }

    public required decimal Value { get; set; }
}