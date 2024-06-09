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
        Telemetry _telemetry;

        [Setup]
        public void Setup()
        {
            Wifi.EnsureConnected();
            _bufferedTelemetryService = new(new MqttService(null));

            // TODO: Create a Telemetry object
            //_telemetry = AidonMessageParser.Parse(TestData.MessageAsString, "P1");
        }

        [Benchmark, Baseline]
        public void AddTelemetry()
        {
            //_bufferedTelemetryService.AddTelemetry(_telemetry);
        }
    }
}
