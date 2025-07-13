using nanoFramework.Hardware.Esp32;
using System.Collections;
using TelemetryStash.Shared;

namespace TelemetryStash.Bmxx80.Sensor
{
    public class Bme680SensorSettings
    {
        private const string DataPinKey = "Bme680Sensor.I2cDataPin";
        private const string DataKey = "Bme680Sensor.I2cData";

        private const string ClockPinKey = "Bme680Sensor.I2cClockPin";
        private const string ClockKey = "Bme680Sensor.I2cClock";

        public void Configure(IDictionary dictionary)
        {
            I2cDataPin = dictionary.Int32(DataPinKey);
            I2cData = dictionary.DeviceFunction(DataKey);

            I2cClockPin = dictionary.Int32(ClockPinKey);
            I2cClock = dictionary.DeviceFunction(ClockKey);
        }

        public int I2cDataPin { get; set; }
        public DeviceFunction I2cData { get; set; }

        public int I2cClockPin { get; set; }
        public DeviceFunction I2cClock { get; set; }
    }
}
