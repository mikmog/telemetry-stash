using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using System.Collections;
using TelemetryStash.DeviceServices;
using TelemetryStash.ServiceModels;

/*
    Platform: ESP32
    Manufacturer: MinSizeRel build, chip rev. >= 0, without support for PSRAM
    TargetName: ESP32_REV0
    NanoFramework version: 1.9.1.0
    Floating point precision: 1

    Console export: LocalStorageBenchmark benchmark class.
    | MethodName        | IterationCount | Mean   | Min    | Max    |
    | ------------------------------------------------------------- |
    | AddToLocalStorage | 10             | 216 ms | 200 ms | 230 ms |
    | DeleteIfExist     | 10             | 17 ms  | 10 ms  | 20 ms  |
 */

namespace TelemetryStash.Services.Benchmarks
{
    [ConsoleParser]
    [IterationCount(10)]
    public class LocalStorageBenchmark
    {
        private LocalStorageService _localStorage;
        private readonly Queue _telemetries = new();

        [Setup]
        public void Setup()
        {
            _localStorage = new LocalStorageService("I:\\test-localstorage.db");
            _localStorage.DeleteIfExist();

            _telemetries.Enqueue(TestData.StaticKeyTelemetry);
            _telemetries.Enqueue(TestData.StaticKeyTelemetry);
            _telemetries.Enqueue(TestData.StaticKeyTelemetry);
            _telemetries.Enqueue(TestData.StaticKeyTelemetry);
            _telemetries.Enqueue(TestData.StaticKeyTelemetry);
        }

        [Benchmark]
        public void AddToLocalStorage()
        {
            _localStorage.AddToLocalStorage(_telemetries);
            _localStorage.ReadFromLocalStorage((telemetry) => { });
        }

        [Benchmark]
        public void DeleteIfExist()
        {
            _localStorage.DeleteIfExist();
        }
    }
}
