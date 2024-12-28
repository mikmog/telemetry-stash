using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using TelemetryStash.NfClient.Services;

namespace TelemetryStash.Services.Benchmarks
{
    [ConsoleParser]
    [IterationCount(10)]
    public class WifiHelperBenchmark
    {
        [Setup]
        public void Setup() { }

        [Benchmark, Baseline]
        public void ConnectToWifi()
        {
            Wifi.EnsureConnected();
        }
    }
}
