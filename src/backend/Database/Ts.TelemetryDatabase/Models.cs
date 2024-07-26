namespace TelemetryStash.Database;

public record Device(short Id, string Identifier);

public record Register(int Id, int RegisterTemplateId, string Subset);

public record RegisterSet(int Id, int DeviceId, string Identifier);

public record RegisterTemplate(
    int Id,
    int RegisterSetId,
    string Identifier,
    string? Name = null,
    string? Type = null,
    string? Unit = null,
    string? Description = null
);

public record TelemetryValue(int RegisterId, int TimestampId, DateTimeOffset ClientTimestamp, decimal Value);
