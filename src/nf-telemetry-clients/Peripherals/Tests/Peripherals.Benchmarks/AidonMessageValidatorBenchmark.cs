using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using TelemetryStash.Aidon.Sensor;
using TelemetryStash.Shared;

/*
    Platform: ESP32
    Manufacturer: MinSizeRel build, chip rev. >= 0, support for PSRAM, support for BLE
    TargetName: ESP32_S3_ALL
    NanoFramework version: 1.10.0.49
    Floating point precision: 1
    | MethodName | IterationCount | Mean | Ratio | Min  | Max   |
    | --------------------------------------------------------- |
    | Validate   | 10             | 1 ms | 1.0   | 0 ms | 10 ms |
*/

namespace TelemetryStash.Peripherals.Benchmarks
{
    [ConsoleParser]
    [IterationCount(10)]
    public class AidonMessageValidatorBenchmark
    {
        private string _testMessage;

        [Setup]
        public void Setup()
        {
            _testMessage = TestData.AidonMessage;
        }

        [Benchmark, Baseline]
        public void Validate()
        {
            AidonMessageValidator.IsValid(_testMessage, out var _);
        }
    }
}
