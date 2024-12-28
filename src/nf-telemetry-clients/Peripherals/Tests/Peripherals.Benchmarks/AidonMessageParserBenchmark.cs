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
    | MethodName               | IterationCount | Mean                  | Ratio | Min   | Max   |
    | ----------------------------------------------------------------------------------------- |
    | AidonMessageParser_Parse | 10             | 37.999999999999998 ms | 1.0   | 30 ms | 40 ms |


    Platform: ESP32
    Manufacturer: MinSizeRel build, chip rev. >= 3, without support for PSRAM
    TargetName: XIAO_ESP32C3
    NanoFramework version: 1.10.0.62
    Floating point precision: 1
    | MethodName               | IterationCount | Mean  | Ratio | Min   | Max   |
    | ------------------------------------------------------------------------- |
    | AidonMessageParser_Parse | 10             | 62 ms | 1.0   | 60 ms | 70 ms |
 */

namespace TelemetryStash.Peripherals.Benchmarks
{
    [ConsoleParser]
    [IterationCount(10)]
    public class AidonMessageParserBenchmark
    {
        private string _message;

        [Setup]
        public void Setup()
        {
            _message = TestData.AidonMessage;
        }

        [Baseline]
        [Benchmark]
        public void AidonMessageParser_Parse()
        {
            AidonMessageParser.Parse(_message);
        }
    }
}
