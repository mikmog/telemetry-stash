using System.Collections;
using TelemetryStash.Shared;

namespace TelemetryStash.MpuGyroSensor
{
    public class MpuGyroSettings
    {
        private const string DataPinKey = "MpuGyro.DataPin";
        private const string ClockPinKey = "MpuGyro.ClockPin";

        public int I2cDataPin { get; set; }
        public int I2cClockPin { get; set; }

        public void Configure(IDictionary dictionary)
        {
            I2cDataPin = dictionary.Int32(DataPinKey);
            I2cClockPin = dictionary.Int32(ClockPinKey);
        }
    }
}
