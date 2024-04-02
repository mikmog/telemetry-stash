using System.Collections.Concurrent;
using TelemetryStash.Database.Models;
using TelemetryStash.Database.Repositories;

namespace TelemetryStash.Functions.TelemetryTrigger.Services
{
    public class CachedRegisterSetService(IRegisterSetRepository registerSetRepository)
    {
        private static readonly ConcurrentDictionary<string, int> _registerSets = [];

        public static int? GetRegisterSetId(int deviceId, string registerSet)
        {
            return _registerSets.TryGetValue(GetRegisterSetKey(deviceId, registerSet), out var id) ? id : null;
        }

        public async Task<int> AddAndGet(int deviceId, string registerSet)
        {
            var set = await registerSetRepository.GetRegisterSet(deviceId, registerSet);
            if (set == null)
            {
                set = registerSetRepository.Add(new RegisterSet(deviceId, registerSet));
                await registerSetRepository.SaveChangesAsync();
                registerSetRepository.StopTracking(set);
            }

            _registerSets.TryAdd(GetRegisterSetKey(deviceId, registerSet), set.Id);

            return set.Id;
        }

        private static string GetRegisterSetKey(int deviceId, string registerSet)
        {
            return $"{deviceId}{registerSet}";
        }
    }
}
