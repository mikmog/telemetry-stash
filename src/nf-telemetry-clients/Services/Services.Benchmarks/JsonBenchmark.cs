using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using nanoFramework.Json;
using TelemetryStash.DeviceServices;
using TelemetryStash.ServiceModels;

namespace TelemetryStash.Services.Benchmarks
{
    /*
        Platform: ESP32
        Manufacturer: MinSizeRel build, chip rev. >= 0, without support for PSRAM
        TargetName: ESP32_REV0
        NanoFramework version: 1.9.1.0
        Floating point precision: 1

        Console export: JsonBenchmark benchmark class.
        | MethodName    | IterationCount | Mean  | Ratio | Min   | Max   |
        | -------------------------------------------------------------- |
        | SerializeJson | 10             | 45 ms | 1.0   | 40 ms | 50 ms |
    */

    [ConsoleParser]
    [IterationCount(10)]
    public class JsonBenchmark
    {
        Telemetry _telemetry;

        [Setup]
        public void Setup()
        {
            _telemetry = TestData.StaticKeyTelemetry;
        }

        [Benchmark, Baseline]
        public void SerializeJson()
        {
            var json = Json.Serialize(_telemetry);
        }

        [Benchmark]
        public void SerializeJsonConvertJson()
        {
            var json = JsonConvert.SerializeObject(_telemetry);
        }
    }
}
