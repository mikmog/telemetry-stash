using System.Collections.Concurrent;
using TelemetryStash.Database.Models;
using TelemetryStash.Database.Repositories;

namespace TelemetryStash.Functions.TelemetryTrigger.Services
{
    public class CachedRegisterService(IRegisterKeyRepository registerSetRepository, IRegisterRepository registerRepository)
    {
        private static readonly ConcurrentDictionary<string, int> _registerKeys = [];
        private static readonly ConcurrentDictionary<string, int> _registers = [];

        public static int? GetRegisterKey(int registerSetId, string registerSubset, string register)
        {
            var registerKeyKey = RegisterKeyKey(registerSetId, registerSubset, register);
            return _registerKeys.TryGetValue(registerKeyKey, out var id) ? id : null;
        }

        public async Task<int> AddAndGet(int registerSetId, string registerSubset, string register)
        {
            var registerKey = RegisterKey(registerSetId, register);
            if (!_registers.TryGetValue(registerKey, out var registerId))
            {
                var reg = await registerRepository.GetRegister(registerSetId, register);
                if (reg == null)
                {
                    reg = new Register(RegisterSetId: registerSetId, RegisterIdentifier: register);
                    registerRepository.Add(reg);
                    await registerRepository.SaveChangesAsync();
                }
                registerId = reg.Id;
                _registers.TryAdd(registerKey, registerId);
            }

            var key = await registerSetRepository.GetRegisterKey(registerId, registerSubset);
            if (key == null)
            {
                key = new RegisterKey(registerId, registerSubset);
                registerSetRepository.Add(key);
                await registerSetRepository.SaveChangesAsync();
            }

            _registerKeys.TryAdd(RegisterKeyKey(registerSetId, registerSubset, register), key.Id);

            return key.Id;
        }

        private static string RegisterKeyKey(int registerSetId, string registerSubset, string register)
        {
            return $"{registerSetId}{register}{registerSubset}";
        }

        private static string RegisterKey(int registerSetId, string register)
        {
            return $"{registerSetId}{register}";
        }
    }
}
