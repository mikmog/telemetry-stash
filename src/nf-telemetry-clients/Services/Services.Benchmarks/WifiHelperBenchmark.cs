using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using TelemetryStash.DeviceServices;

/*
    Platform: ESP32
    Manufacturer: MinSizeRel build, chip rev. >= 0, without support for PSRAM
    TargetName: ESP32_REV0
    NanoFramework version: 1.9.1.0
    Floating point precision: 1

    Console export: WifiHelperBenchmark benchmark class.
    | MethodName    | IterationCount | Mean | Ratio | Min  | Max   |
    | ------------------------------------------------------------ |
    | ConnectToWifi | 10             | 2 ms | 1.0   | 0 ms | 20 ms |
 */

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
