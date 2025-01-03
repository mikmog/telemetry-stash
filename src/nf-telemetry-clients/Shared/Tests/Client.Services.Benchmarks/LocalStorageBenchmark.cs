using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using System.Collections;
using TelemetryStash.NfClient.Services;
using TelemetryStash.Shared;

/*
    Platform: ESP32
    Manufacturer: MinSizeRel build, chip rev. >= 0, support for PSRAM, support for BLE
    TargetName: ESP32_S3_ALL
    NanoFramework version: 1.10.0.49
    Floating point precision: 1
    | MethodName        | IterationCount | Mean  | Min   | Max    |
    | ----------------------------------------------------------- |
    | AddToLocalStorage | 10             | 75 ms | 40 ms | 170 ms |
    | DeleteIfExist     | 10             | 10 ms | 0 ms  | 50 ms  |


    Platform: ESP32
    Manufacturer: MinSizeRel build, chip rev. >= 3, without support for PSRAM
    TargetName: XIAO_ESP32C3
    NanoFramework version: 1.10.0.62
    Floating point precision: 1
    | MethodName        | IterationCount | Mean                  | Min   | Max    |
    | --------------------------------------------------------------------------- |
    | AddToLocalStorage | 10             | 86.999999999999993 ms | 40 ms | 160 ms |
    | DeleteIfExist     | 10             | 8 ms                  | 0 ms  | 40 ms  |
 */

namespace TelemetryStash.Services.Benchmarks
{
    [ConsoleParser]
    [IterationCount(10)]
    public class LocalStorageBenchmark
    {
        private LocalStorage _localStorage;
        private readonly Queue _telemetries = new();

        [Setup]
        public void Setup()
        {
            _localStorage = new LocalStorage("I:\\test-localstorage.db");
            _localStorage.DeleteLocalStorage();

            _telemetries.Enqueue(TestData.NumbersOnlyTelemetry);
            _telemetries.Enqueue(TestData.NumbersOnlyTelemetry);
            _telemetries.Enqueue(TestData.NumbersOnlyTelemetry);
            _telemetries.Enqueue(TestData.NumbersOnlyTelemetry);
            _telemetries.Enqueue(TestData.NumbersOnlyTelemetry);
        }

        [Benchmark]
        public void AddToLocalStorage()
        {
            _localStorage.AppendLocalStorage(_telemetries.ToArray());
            _localStorage.ReadLocalStorage((telemetry) => { });
        }

        [Benchmark]
        public void DeleteIfExist()
        {
            _localStorage.DeleteLocalStorage();
        }
    }
}
