namespace TelemetryStash.Database;

public record DeviceRow(short Id, string Identifier);

public record RegisterRow(int Id, int DeviceId, string RegisterSet, string Register);

public record TelemetryRow(int RegisterId, int TimestampId, DateTimeOffset ClientTimestamp, string Value);
