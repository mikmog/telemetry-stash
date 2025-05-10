using System;
using System.Collections;

namespace TelemetryStash.MpuxxxxGyro.Sensor
{
    public class MpuGyroSettings
    {
        private const string DataPinKey = "MpuGyro.DataPin";
        private const string ClockPinKey = "MpuGyro.ClockPin";

        public int I2cDataPin { get; set; }
        public int I2cClockPin { get; set; }

        public void Configure(IDictionary dictionary)
        {
            object Setting(string key) => dictionary[key] ?? throw new ArgumentException(key);

            I2cDataPin = (int)Setting(DataPinKey);
            I2cClockPin = (int)Setting(ClockPinKey);
        }
    }
}
