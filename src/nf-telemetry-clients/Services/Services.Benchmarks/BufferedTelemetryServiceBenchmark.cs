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
            _bufferedTelemetryService = new(new MqttService(null));
        }

        [Benchmark, Baseline]
        public void AddTelemetry()
        {
            _bufferedTelemetryService.AddTelemetry(TestData.StaticKeyTelemetry);
        }
    }
}
