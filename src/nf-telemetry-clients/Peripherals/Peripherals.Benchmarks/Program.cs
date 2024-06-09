using nanoFramework.Benchmark;
using System.Diagnostics;
using System.Threading;


namespace TelemetryStash.Peripherals.Benchmarks
{
    public class Program
    {
        public static void Main()
        {
            //new Printer().PrintSystemInfo();
            Debug.WriteLine("Running nanoFramework benchmarks...");

            BenchmarkRunner.RunClass(typeof(Am23XXBenchmark));
            BenchmarkRunner.RunClass(typeof(AidonMessageParserBenchmark));
            BenchmarkRunner.RunClass(typeof(AidonMessageValidatorBenchmark));

            Debug.WriteLine("Benchmarks completed");
            //new Printer().PrintSystemInfo();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
