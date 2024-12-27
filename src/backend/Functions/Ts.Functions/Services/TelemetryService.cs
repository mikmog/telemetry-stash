using System.Globalization;
using TelemetryStash.Database.Repositories;
using TelemetryStash.Functions.TelemetryTrigger;

namespace TelemetryStash.Functions.Services;

public interface ITelemetryService
{
    Task Process(string deviceIdentifier, TelemetryRequest telemetry);
}

public class TelemetryService(
    ITelemetryRepository telemetryRepository,
    IDeviceService deviceService,
    IRegisterService registerService) : ITelemetryService
{
    public async Task Process(string deviceIdentifier, TelemetryRequest telemetry)
    {
        var device = await deviceService.GetOrCreate(deviceIdentifier);

        foreach (var registerSetKvp in telemetry.RegisterSets)
        {
            var registerSetIdentifier = registerSetKvp.Key;
            var registerSet = registerSetKvp.Value;

            var requestRegisterValues = registerSet.RegisterValues.Select(r => (r.Key, r.Value));

            // TODO: registerSet.Tags

            var registerRows = await registerService.GetOrCreate(device.Id, registerSetIdentifier, requestRegisterValues.Select(r => r.Key));

            var telemetryMap = requestRegisterValues.Select(r => (registerRows[r.Key].Id, r.Value));
            await telemetryRepository.Upsert(device.Id, telemetry.Timestamp, telemetryMap);
        }
    }
}

public static class NumberHelper
{
    public static string ToStringWithoutTrailingZeroes(this decimal value)
    {
        return (value / 1.000000000000000000000000000000000m).ToString(CultureInfo.InvariantCulture);
    }
}
