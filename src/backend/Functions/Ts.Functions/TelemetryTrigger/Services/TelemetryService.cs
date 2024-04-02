using TelemetryStash.Database.Repositories;

namespace TelemetryStash.Functions.TelemetryTrigger.Services
{
    public class TelemetryService(
        ITelemetryRepository telemetryRepository,
        CachedDeviceService deviceService,
        CachedRegisterSetService registerSetService,
        CachedRegisterService registerService)
    {
        public async Task Process(string deviceId, TelemetryRequest telemetry)
        {
            var telemetryMap = new List<(int RegisterKey, decimal Value)>(telemetry.Registers.Sum(r => r.Value.Count));

            var device = CachedDeviceService.Get(deviceId) ?? await deviceService.AddAndGet(deviceId);
            foreach (var reg in telemetry.Registers)
            {
                var registerSet = telemetry.RegisterSet ?? reg.Key;
                var registerSubset = reg.Key;

                foreach (var register in reg.Value)
                {
                    var regSetId = CachedRegisterSetService.GetRegisterSetId(device.Id, registerSet) ?? await registerSetService.AddAndGet(device.Id, registerSet);
                    var registerKeyId = CachedRegisterService.GetRegisterKey(regSetId, registerSubset, register.Key) ?? await registerService.AddAndGet(regSetId, registerSubset, register.Key);

                    telemetryMap.Add((registerKeyId, register.Value));
                }
            }

            await telemetryRepository.UpsertTelemetry(device.Id, telemetry.Timestamp, telemetryMap);
        }
    }
}
