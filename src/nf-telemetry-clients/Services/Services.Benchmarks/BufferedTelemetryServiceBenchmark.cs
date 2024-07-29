using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using TelemetryStash.DeviceServices;
using TelemetryStash.DeviceServices.Mqtt;
using TelemetryStash.ServiceModels;

namespace TelemetryStash.Services.Benchmarks
{
    [ConsoleParser]
    [IterationCount(1)]
    public class BufferedTelemetryServiceBenchmark
    {
        private BufferedTelemetryService _bufferedTelemetryService;

        [Setup]
        public void Setup()
        {
            Wifi.EnsureConnected();

            var config = new ConfigurationService();
            var dictionary = config.ReadConfiguration();

            var mqttSettings = new MqttSettings();
            mqttSettings.Configure(dictionary);

            _bufferedTelemetryService = new(new MqttService(mqttSettings));
        }

        [Benchmark, Baseline]
        public void AddTelemetry()
        {
            _bufferedTelemetryService.AddTelemetry(TestData.StaticKeyTelemetry);
        }
    }
}
