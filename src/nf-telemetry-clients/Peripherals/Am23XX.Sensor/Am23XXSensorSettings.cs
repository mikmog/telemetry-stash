using System.Collections;
using TelemetryStash.Shared;

namespace TelemetryStash.Am23XX.Sensor
{
    public class Am23XXSensorSettings
    {
        private const string DataPinKey = "Am23XXSensor.DataPin";
        private const string ClockPinKey = "Am23XXSensor.ClockPin";
        private const string TemperatureOffsetKey = "Am23XXSensor.TemperatureOffset";
        private const string HumidityOffsetKey = "Am23XXSensor.HumidityOffset";

        public void Configure(IDictionary dictionary)
        {
            DataPin = dictionary.Int32(DataPinKey);
            ClockPin = dictionary.Int32(ClockPinKey);
            TemperatureOffset = dictionary.Double(TemperatureOffsetKey);
            HumidityOffset = dictionary.Double(HumidityOffsetKey);
        }

        public int DataPin { get; set; }

        public int ClockPin { get; set; }

        public double TemperatureOffset { get; set; }

        public double HumidityOffset { get; set; }
    }
}
