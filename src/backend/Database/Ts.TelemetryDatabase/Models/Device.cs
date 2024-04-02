namespace TelemetryStash.Database.Models;

public record Device(string DeviceId)
{
    public short Id { get; set; }
};