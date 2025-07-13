using nanoFramework.Hardware.Esp32;
using System.Collections;
using TelemetryStash.Shared;

namespace TelemetryStash.Am23XX.Sensor
{
    public class Am23XXSensorSettings
    {
        private const string DataPinKey = "Am23XXSensor.I2cDataPin";
        private const string DataKey = "Am23XXSensor.I2cData";
        private const string ClockPinKey = "Am23XXSensor.I2cClockPin";
        private const string ClockKey = "Am23XXSensor.I2cClock";

        private const string TemperatureOffsetKey = "Am23XXSensor.TemperatureOffset";
        private const string HumidityOffsetKey = "Am23XXSensor.HumidityOffset";

        public void Configure(IDictionary dictionary)
        {
            I2cDataPin = dictionary.Int32(DataPinKey);
            I2cData = dictionary.DeviceFunction(DataKey);
            I2cClockPin = dictionary.Int32(ClockPinKey);
            I2cClock = dictionary.DeviceFunction(ClockKey);
            TemperatureOffset = dictionary.Double(TemperatureOffsetKey);
            HumidityOffset = dictionary.Double(HumidityOffsetKey);
        }

        public int I2cDataPin { get; set; }
        public DeviceFunction I2cData { get; set; }

        public int I2cClockPin { get; set; }
        public DeviceFunction I2cClock { get; set; }


        public double TemperatureOffset { get; set; }

        public double HumidityOffset { get; set; }
    }
}
