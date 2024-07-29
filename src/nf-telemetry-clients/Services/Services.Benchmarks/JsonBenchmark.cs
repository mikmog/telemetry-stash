using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using nanoFramework.Json;
using TelemetryStash.DeviceServices;
using TelemetryStash.ServiceModels;

namespace TelemetryStash.Services.Benchmarks
{
    /*
        Platform: ESP32
        Manufacturer: MinSizeRel build, chip rev. >= 0, support for PSRAM, support for BLE
        TargetName: ESP32_S3_ALL
        NanoFramework version: 1.10.0.49
        Floating point precision: 1
        | MethodName               | IterationCount | Mean                  | Ratio   | Min    | Max    |
        | --------------------------------------------------------------------------------------------- |
        | SerializeJson            | 10             | 27.999999999999998 ms | 1.0     | 20 ms  | 30 ms  |
        | SerializeJsonConvertJson | 10             | 428 ms                | 15.2857 | 420 ms | 430 ms |


        Platform: ESP32
        Manufacturer: MinSizeRel build, chip rev. >= 3, without support for PSRAM
        TargetName: XIAO_ESP32C3
        NanoFramework version: 1.10.0.62
        Floating point precision: 1
        | MethodName               | IterationCount | Mean                  | Ratio   | Min    | Max    |
        | --------------------------------------------------------------------------------------------- |
        | SerializeJson            | 10             | 57 ms                 | 1.0     | 50 ms  | 60 ms  |
        | SerializeJsonConvertJson | 10             | 709.99999999999996 ms | 12.4561 | 710 ms | 710 ms |
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
