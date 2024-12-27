using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using TelemetryStash.NfClient.Services;
using TelemetryStash.Shared;

namespace TelemetryStash.Services.Benchmarks
{
    [ConsoleParser]
    [IterationCount(10)]
    public class MqttClientBenchmark
    {
        private MqttService _mqttService;
        private Telemetry _numbersOnlyTelemetry;
        Telemetry _textAndNumbersTelemetry;

        [Setup]
        public void Setup()
        {
            Wifi.EnsureConnected();
            var configReader = new ConfigurationReader();
            var settings = configReader.ReadConfiguration();
            
            _numbersOnlyTelemetry = TestData.NumbersOnlyTelemetry;
            _textAndNumbersTelemetry = TestData.TextOnlyTelemetry;

            _mqttService = new(new MqttSettings().Configure(settings));
            _mqttService.Connect();
        }

        [Benchmark, Baseline]
        public void Publish_Numbers_only_telemetry()
        {
            _mqttService.Publish(_numbersOnlyTelemetry);
        }

        [Benchmark]
        public void Publish_Text_and_numbersTelemetry()
        {
            _mqttService.Publish(_textAndNumbersTelemetry);
        }

        //[Benchmark]
        //public void MqttService_Dispose()
        //{
        //    _mqttService.Dispose();
        //}
    }
}
