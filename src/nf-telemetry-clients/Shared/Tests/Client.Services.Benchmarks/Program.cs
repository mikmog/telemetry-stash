using nanoFramework.Benchmark;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.NfClient.Services;

namespace TelemetryStash.Services.Benchmarks
{
    public class Program
    {
        public static void Main()
        {
            Thread.Sleep(1000);

            new Debugformation().PrintStartupMessage();
            new Debugformation().PrintSystemInfo();

            Debug.WriteLine("Running nanoFramework benchmarks...");

            BenchmarkRunner.RunClass(typeof(FrameworkBenchmark));
            BenchmarkRunner.RunClass(typeof(FrameworkBenchmark2));
            BenchmarkRunner.RunClass(typeof(FrameworkBenchmark3));
            BenchmarkRunner.RunClass(typeof(JsonBenchmark));
            //BenchmarkRunner.RunClass(typeof(LocalStorageBenchmark));
            //BenchmarkRunner.RunClass(typeof(BufferedTelemetryServiceBenchmark));
            //BenchmarkRunner.RunClass(typeof(MqttClientBenchmark));
            //BenchmarkRunner.RunClass(typeof(WifiHelperBenchmark));

            Debug.WriteLine("Benchmarks completed");
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
