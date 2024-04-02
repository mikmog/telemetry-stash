namespace TelemetryStash.Database.Models;

public record RegisterKey (int RegisterId, string Subset)
{
    public int Id { get; set; }
}
