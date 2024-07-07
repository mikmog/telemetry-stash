namespace TelemetryStash.Database.Models;

public record Register(int RegisterId, string Subset)
{
    public int Id { get; set; }
}
