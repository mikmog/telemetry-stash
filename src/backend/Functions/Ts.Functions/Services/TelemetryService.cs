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
    IRegisterTemplateService registerTemplateService,
    IRegisterService registerService) : ITelemetryService
{
    public async Task Process(string deviceId, TelemetryRequest telemetry)
    {
        var telemetryMap = new List<(int RegisterKey, decimal Value)>(telemetry.Registers.Sum(r => r.Value.Count));

        var device = await deviceService.GetOrCreate(deviceId);
        foreach (var registerRequests in telemetry.Registers)
        {
            var registerSetRequest = telemetry.RegisterSet ?? registerRequests.Key;
            var registerSubset = registerRequests.Key;

            var registerSet = await registerSetService.GetOrCreate(device.Id, registerSetRequest);

            foreach (var registerRequest in registerRequests.Value)
            {
                var registerTemplate = await registerTemplateService.GetOrCreate(registerSet.Id, registerRequest.Key);
                var register = await registerService.GetOrCreate(registerTemplate.Id, registerSubset);
                telemetryMap.Add((register.Id, registerRequest.Value));
            }
        }

        await telemetryRepository.Upsert(device.Id, telemetry.Timestamp, telemetryMap);
    }
}
