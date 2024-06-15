using nanoFramework.Benchmark;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.DeviceServices;

namespace TelemetryStash.Services.Benchmarks
{
    public class Program
    {
        public static void Main()
        {
            Thread.Sleep(1000);

            new Printer().PrintStartupMessage();
            new Printer().PrintSystemInfo();

            Debug.WriteLine("Running nanoFramework benchmarks...");

            //BenchmarkRunner.RunClass(typeof(FrameworkBenchmark));
            //BenchmarkRunner.RunClass(typeof(JsonBenchmark));
            //BenchmarkRunner.RunClass(typeof(LocalStorageBenchmark));
            //BenchmarkRunner.RunClass(typeof(BufferedTelemetryServiceBenchmark));
            //BenchmarkRunner.RunClass(typeof(MqttClientBenchmark));
            //BenchmarkRunner.RunClass(typeof(WifiHelperBenchmark));

            Debug.WriteLine("Benchmarks completed");
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
