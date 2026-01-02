using TelemetryStash.Database.Repositories;
using Xunit;

namespace TelemetryStash.Database.Tests.Ts.TelemetryDatabase;

[Collection(CollectionState.SharedTestDbServer)]
public class TelemetryRepositoryTests(SharedTestDbFixture dbFixture) : TelemetryDbSeeder(dbFixture)
{
    [Fact]
    public async Task RegisterRepository_Upsert_telemetry_is_added()
    {
        // Arrange
        var timestamp = DateTimeOffset.Parse("2025-01-01T12:00:00Z");
        var (deviceId, registerValues) = await GetTelemetry(5, 3, timestamp);

        var sut = new TelemetryRepository(GetDbProvider());

        // Act
        await sut.Upsert(deviceId, timestamp, registerValues);

        var telemetryRows = new List<TelemetryRow>();
        await foreach (var rows in sut.GetTelemetry(deviceId, timestamp, timestamp))
        {
            telemetryRows.AddRange(rows);
        }

        // Assert
        Assert.Equal(registerValues.Count, telemetryRows.Count);
        Assert.Single(telemetryRows.DistinctBy(v => v.TimestampId));
        foreach (var (RegisterId, TelemetryValue) in registerValues)
        {
            var telemetryValue = telemetryRows.Single(x => x.RegisterId == RegisterId);

            Assert.NotEqual(0, telemetryValue.RegisterId);
            Assert.NotEqual(0, telemetryValue.TimestampId);
            Assert.Equal(TelemetryValue, telemetryValue.Value);
            Assert.Equal(timestamp, telemetryValue.ClientTimestamp);
        }
    }

    //[Fact]
    //public async Task RegisterRepository_Upsert_telemetry_is_appended_when_equal_timestamp()
    //{
    //    // Arrange
    //    var timestamp = DateTimeOffset.Parse("2025-01-02T12:00:00Z");
    //    var (device, telemetry) = await GetTelemetry(10, timestamp);

    //    var sut = new TelemetryRepository(GetDbProvider());
    //    var telemetry1 = telemetry.Take(5).ToList();
    //    var telemetry2 = telemetry.Skip(5).ToList();

    //    // Act
    //    await sut.Upsert(device.Id, timestamp, telemetry1);
    //    await sut.Upsert(device.Id, timestamp, telemetry2);

    //    var telemetryValues = new List<TelemetryRow>();
    //    await foreach (var values in sut.GetTelemetry(device.Id, timestamp, timestamp))
    //    {
    //        telemetryValues.AddRange(values);
    //    }

    //    // Assert
    //    Assert.Equal(10, telemetryValues.Count);
    //    Assert.Single(telemetryValues.DistinctBy(v => v.TimestampId));
    //}


    //[Fact]
    //public async Task RegisterRepository_Upsert_telemetry_is_updated_when_exist()
    //{
    //    // Arrange
    //    var timestamp = DateTimeOffset.Parse("2025-01-03T12:00:00Z");
    //    var (device, telemetry) = await GetTelemetry(10, timestamp);
    //    var telemetry2 = telemetry.Select((t, index) => (t.RegisterId, $"{index + 1.1m}")).ToList();

    //    var sut = new TelemetryRepository(GetDbProvider());

    //    // Act
    //    await sut.Upsert(device.Id, timestamp, telemetry);
    //    await sut.Upsert(device.Id, timestamp, telemetry2);

    //    var telemetryValues = new List<TelemetryRow>();
    //    await foreach (var values in sut.GetTelemetry(device.Id, timestamp, timestamp))
    //    {
    //        telemetryValues.AddRange(values);
    //    }

    //    // Assert
    //    Assert.Equal(10, telemetryValues.Count);
    //    Assert.Single(telemetryValues.DistinctBy(v => v.TimestampId));
    //    foreach (var (RegisterId, Value) in telemetry2)
    //    {
    //        var telemetryValue = telemetryValues.Single(x => x.RegisterId == RegisterId);

    //        Assert.NotEqual(0, telemetryValue.TimestampId);
    //        Assert.Equal(Value, telemetryValue.Value);
    //        Assert.Equal(timestamp, telemetryValue.ClientTimestamp);
    //    }
    //}

    private async Task<(int DeviceId, List<(int RegisterId, string Value)> Telemetry)> GetTelemetry(int numberCount, int textCount, DateTimeOffset timestamp)
    {
        var device = await SeedDevice($"MyDevice{Guid.NewGuid()}");

        var registers =
            Enumerable.Range(0, numberCount).ToList().Select(i => $"NumberRegister{i}")
            .Concat(Enumerable.Range(0, textCount).ToList().Select(i => $"TextRegister{i}"));

        var registerRows = await SeedRegister(device.Id, $"Registerset1{Guid.NewGuid()}", registers);
        var registerValues = registerRows.Select(r => (r.Id, r.Register)).ToList();

        return (device.Id, registerValues);
    }
}
