using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using TelemetryStash.Aidon.Sensor;
using TelemetryStash.ServiceModels;

/*
    Platform: ESP32
    Manufacturer: MinSizeRel build, chip rev. >= 0, without support for PSRAM
    TargetName: ESP32_REV0
    NanoFramework version: 1.9.1.0
    Floating point precision: 1

    Console export: P1MessageParserBenchmark benchmark class.
    | MethodName            | IterationCount | Mean  | Ratio | Min   | Max   |
    | ---------------------------------------------------------------------- |
    | P1MessageParser_Parse | 10             | 52 ms | 1.0   | 50 ms | 60 ms |
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
            AidonMessageParser.Parse(_message, "Aidon");
        }
    }
}
