namespace TelemetryStash.Database.Models;

public class TelemetryHistory
{
    public int Id { get; set; }

    public int RegisterKeyId { get; set; }
    public required int FromTimestampHistoryId { get; set; }
    public required int ToTimestampHistoryId { get; set; }

    public required decimal Value { get; set; }
}