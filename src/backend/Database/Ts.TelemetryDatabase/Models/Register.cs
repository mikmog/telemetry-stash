namespace TelemetryStash.Database.Models;

public record Register(
    int RegisterSetId,
    string RegisterIdentifier,
    string? Name = null,
    string? Type = null,
    string? Unit = null,
    string? Description = null
)
{
    public int Id { get; set; }
}
