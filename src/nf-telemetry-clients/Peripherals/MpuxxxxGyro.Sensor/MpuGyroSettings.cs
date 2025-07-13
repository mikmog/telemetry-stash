using nanoFramework.Hardware.Esp32;
using System.Collections;
using TelemetryStash.Shared;

namespace TelemetryStash.MpuGyroSensor
{
    public class MpuGyroSettings
    {
        private const string DataPinKey = "MpuGyro.I2cDataPin";
        private const string DataKey = "MpuGyro.I2cData";
        private const string ClockPinKey = "MpuGyro.I2cClockPin";
        private const string ClockKey = "MpuGyro.I2cClock";

        public int I2cDataPin { get; set; }
        public DeviceFunction I2cData { get; set; }
        public int I2cClockPin { get; set; }
        public DeviceFunction I2cClock { get; set; }

        public void Configure(IDictionary dictionary)
        {
            I2cDataPin = dictionary.Int32(DataPinKey);
            I2cData = dictionary.DeviceFunction(DataKey);
            I2cClockPin = dictionary.Int32(ClockPinKey);
            I2cClock = dictionary.DeviceFunction(ClockKey);
        }
    }
}
