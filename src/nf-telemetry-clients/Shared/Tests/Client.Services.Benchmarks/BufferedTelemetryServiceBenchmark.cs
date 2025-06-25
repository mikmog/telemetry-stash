using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using TelemetryStash.NfClient.Communication;
using TelemetryStash.NfClient.Communication.Mqtt;
using TelemetryStash.Shared;

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

            var config = new ConfigurationReader();
            var dictionary = config.ReadConfiguration();

            var mqttSettings = new MqttSettings();
            mqttSettings.Configure(dictionary);

            _bufferedTelemetryService = new(new MqttService(mqttSettings));
        }

        [Benchmark, Baseline]
        public void AddTelemetry()
        {
            _bufferedTelemetryService.AddTelemetry(TestData.NumbersOnlyTelemetry);
        }
    }
}
