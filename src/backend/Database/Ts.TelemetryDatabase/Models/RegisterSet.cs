namespace TelemetryStash.Database.Models;

public record RegisterSet(int DeviceId, string Identifier)
{ 
    public int Id { get; set; }
};
