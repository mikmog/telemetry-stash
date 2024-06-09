using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using TelemetryStash.Aidon.Sensor;
using TelemetryStash.ServiceModels;

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
