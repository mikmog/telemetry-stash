using nanoFramework.Benchmark;
using System;
using System.Diagnostics;
using System.Threading;


namespace TelemetryStash.Peripherals.Benchmarks
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Running nanoFramework benchmarks...");

            //BenchmarkRunner.RunClass(typeof(Am23XXBenchmark));
            //BenchmarkRunner.RunClass(typeof(AidonMessageParserBenchmark));
            //BenchmarkRunner.RunClass(typeof(AidonMessageValidatorBenchmark));
            //BenchmarkRunner.RunClass(typeof(AidonSensorBenchmark));

            Debug.WriteLine("Benchmarks completed");
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
