using TelemetryStash.Database.Repositories;
using TelemetryStash.Functions.TelemetryTrigger;

namespace TelemetryStash.Functions.Services;

public interface ITelemetryService
{
    Task Process(string deviceId, TelemetryRequest telemetry);
}

public class TelemetryService(
    ITelemetryRepository telemetryRepository,
    IDeviceService deviceService,
    IRegisterSetService registerSetService,
    IRegisterService registerService,
    IRegisterKeyService registerKeyService) : ITelemetryService
{
    public async Task Process(string deviceId, TelemetryRequest telemetry)
    {
        var telemetryMap = new List<(int RegisterKey, decimal Value)>(telemetry.Registers.Sum(r => r.Value.Count));

        var device = await deviceService.GetOrAdd(deviceId);
        foreach (var registerRequests in telemetry.Registers)
        {
            var registerSetRequest = telemetry.RegisterSet ?? registerRequests.Key;
            var registerSubset = registerRequests.Key;

            var registerSet = await registerSetService.GetOrAdd(device.Id, registerSetRequest);

            foreach (var registerRequest in registerRequests.Value)
            {
                var register = await registerService.GetOrAdd(registerSet.Id, registerRequest.Key);
                var registerKey = await registerKeyService.GetOrAdd(register.Id, registerSubset);
                telemetryMap.Add((registerKey.Id, registerRequest.Value));
            }
        }

        await telemetryRepository.UpsertTelemetry(device.Id, telemetry.Timestamp, telemetryMap);
    }
}
