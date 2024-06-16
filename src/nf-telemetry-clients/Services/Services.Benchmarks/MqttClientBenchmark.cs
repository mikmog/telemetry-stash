using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using System.Threading;
using TelemetryStash.DeviceServices;
using TelemetryStash.DeviceServices.Mqtt;
using TelemetryStash.ServiceModels;

namespace TelemetryStash.Services.Benchmarks
{
    [ConsoleParser]
    [IterationCount(10)]
    public class MqttClientBenchmark
    {
        private MqttService _mqttService;
        private Telemetry _telemetry;

        [Setup]
        public void Setup()
        {
            Wifi.EnsureConnected();
            var configService = new ConfigurationService();
            var settings = configService.ReadConfiguration();

            Thread.Sleep(1000);
            _telemetry = TestData.StaticKeyTelemetry;
            _mqttService = new(new MqttSettings().Configure(settings));
            _mqttService.Connect();
        }

        [Benchmark, Baseline]
        public void TestMqtt()
        {
            _mqttService.Publish(_telemetry);
        }

        [Benchmark]
        public void DisconnectMqttService()
        {
            _mqttService.Dispose();
        }
    }
}
