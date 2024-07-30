using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using TelemetryStash.Aidon.Sensor;
using TelemetryStash.ServiceModels;

/*
    Platform: ESP32
    Manufacturer: MinSizeRel build, chip rev. >= 0, support for PSRAM, support for BLE
    TargetName: ESP32_S3_ALL
    NanoFramework version: 1.10.0.49
    Floating point precision: 1
    | MethodName | IterationCount | Mean | Ratio | Min  | Max   |
    | --------------------------------------------------------- |
    | Validate   | 10             | 1 ms | 1.0   | 0 ms | 10 ms |
*/

namespace TelemetryStash.Peripherals.Benchmarks
{
    [ConsoleParser]
    [IterationCount(1)]
    public class AidonSensorBenchmark
    {
        private AidonSensor _aidonSensor;

        [Setup]
        public void Setup()
        {
            var settings = new AidonSensorSettings
            { 
                ComPort = "COM2", 
                RegisterSetIdentifier = "Test",
                RxPin = 20,
                RxComPort = nanoFramework.Hardware.Esp32.DeviceFunction.COM2_RX
            };
            _aidonSensor = new AidonSensor(settings);
        }

        [Benchmark, Baseline]
        public void CreateSerialPort()
        {
            _aidonSensor.Open();
        }
    }
}
