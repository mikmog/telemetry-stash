using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.Am23XX.Sensor;
using TelemetryStash.ServiceModels;

namespace TelemetryStash.Peripherals.Benchmarks
{
    [ConsoleParser]
    [IterationCount(10)]
    public class Am23XXBenchmark
    {
        Am23XXSensor _am23XX;

        [Setup]
        public void Setup()
        {
            _am23XX = new(new Am23XXSensorSettings { DataPin = 6, ClockPin = 7, RegisterSetIdentifier = "Am23XX" } );
        }

        [Benchmark, Baseline]
        public void TestRead()
        {
            var response = _am23XX.ReadTempAndHumidity();

            var r1 = response.Registers[0];
            var r2 = response.Registers[1];
            Debug.WriteLine($"{response.Identifier} => {r1.Identifier}: {r1.ToNumberString()}, {r2.Identifier}: {r2.ToNumberString()}");
            Thread.Sleep(3000);
        }

        [Benchmark]
        public void Dispose()
        {
            _am23XX.Dispose();
        }
    }
}
