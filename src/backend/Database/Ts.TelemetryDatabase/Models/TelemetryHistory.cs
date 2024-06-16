namespace TelemetryStash.Database.Models;

public record TelemetryHistory(int RegisterKeyId, int FromTimestampHistoryId, int ToTimestampHistoryId, decimal Value)
{
    public int Id { get; set; }
}
