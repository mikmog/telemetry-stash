namespace TelemetryStash.Database.Tests.Ts.TelemetryDatabase;

[Collection("SharedTestDbServer")]
public class TelemetryRepositoryTests(SharedTestDbFixture dbFixture) : TelemetryDbSeeder(dbFixture)
{
    [Fact]
    public async Task RegisterRepository_Upsert_telemetry_is_added()
    {
        // Arrange
        var timestamp = DateTimeOffset.Parse("2025-01-01T12:00:00Z");
        var (device, telemetry) = await GetTelemetry(10, timestamp);

        var sut = new TelemetryRepository(GetDbProvider());

        // Act
        await sut.Upsert(device.Id, timestamp, telemetry);

        var telemetryValues = new List<TelemetryValue>();
        await foreach (var values in sut.GetTelemetry(device.Id, timestamp, timestamp))
        {
            telemetryValues.AddRange(values);
        }

        // Assert
        Assert.Equal(10, telemetryValues.Count);
        Assert.Single(telemetryValues.DistinctBy(v => v.TimestampId));
        foreach (var (RegisterId, Value) in telemetry)
        {
            var telemetryValue = telemetryValues.Single(x => x.RegisterId == RegisterId);

            Assert.NotEqual(0, telemetryValue.TimestampId);
            Assert.Equal(Value, telemetryValue.Value);
            Assert.Equal(timestamp, telemetryValue.ClientTimestamp);
        }
    }

    [Fact]
    public async Task RegisterRepository_Upsert_telemetry_is_appended_when_equal_timestamp()
    {
        // Arrange
        var timestamp = DateTimeOffset.Parse("2025-01-02T12:00:00Z");
        var (device, telemetry) = await GetTelemetry(10, timestamp);

        var sut = new TelemetryRepository(GetDbProvider());
        var telemetry1 = telemetry.Take(5).ToList();
        var telemetry2 = telemetry.Skip(5).ToList();

        // Act
        await sut.Upsert(device.Id, timestamp, telemetry1);
        await sut.Upsert(device.Id, timestamp, telemetry2);

        var telemetryValues = new List<TelemetryValue>();
        await foreach (var values in sut.GetTelemetry(device.Id, timestamp, timestamp))
        {
            telemetryValues.AddRange(values);
        }

        // Assert
        Assert.Equal(10, telemetryValues.Count);
        Assert.Single(telemetryValues.DistinctBy(v => v.TimestampId));
    }


    [Fact]
    public async Task RegisterRepository_Upsert_telemetry_is_updated_when_exist()
    {
        // Arrange
        var timestamp = DateTimeOffset.Parse("2025-01-03T12:00:00Z");
        var (device, telemetry) = await GetTelemetry(10, timestamp);
        var telemetry2 = telemetry.Select((t, index) => (t.RegisterId, index + 1.1m)).ToList();

        var sut = new TelemetryRepository(GetDbProvider());

        // Act
        await sut.Upsert(device.Id, timestamp, telemetry);
        await sut.Upsert(device.Id, timestamp, telemetry2);

        var telemetryValues = new List<TelemetryValue>();
        await foreach (var values in sut.GetTelemetry(device.Id, timestamp, timestamp))
        {
            telemetryValues.AddRange(values);
        }

        // Assert
        Assert.Equal(10, telemetryValues.Count);
        Assert.Single(telemetryValues.DistinctBy(v => v.TimestampId));
        foreach (var (RegisterId, Value) in telemetry2)
        {
            var telemetryValue = telemetryValues.Single(x => x.RegisterId == RegisterId);

            Assert.NotEqual(0, telemetryValue.TimestampId);
            Assert.Equal(Value, telemetryValue.Value);
            Assert.Equal(timestamp, telemetryValue.ClientTimestamp);
        }
    }

    private async Task<(Device Device, List<(int RegisterId, decimal Value)> Telemetry)> GetTelemetry(int count, DateTimeOffset timestamp)
    {
        var telemetry = new List<(int RegisterId, decimal Value)>();
        var device = await SeedDevice($"TestDeviceId{Guid.NewGuid()}");
        var registerSet = await SeedRegisterSet(device, "WeatherStation");

        for (var i = 0; i < count; i++)
        {
            var registerTemplate = await SeedRegisterTemplate(registerSet, $"Temperature{i}");
            var register = await SeedRegister(registerTemplate, $"Temperature{i}");
            telemetry.Add((register.Id, i));
        }

        return (device, telemetry);
    }
}
