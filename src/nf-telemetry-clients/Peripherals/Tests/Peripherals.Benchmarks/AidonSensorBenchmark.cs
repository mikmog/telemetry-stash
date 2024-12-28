using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using nanoFramework.Hardware.Esp32;
using TelemetryStash.Aidon.Sensor;

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
                RxPin = 20,
                RxComPort = DeviceFunction.COM2_RX
            };
            _aidonSensor = new AidonSensor(settings);
        }

        [Benchmark, Baseline]
        public void CreateSerialPort()
        {
            _aidonSensor.Start();
        }
    }
}
