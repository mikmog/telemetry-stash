using System;
using System.Collections;

namespace TelemetryStash.Inaxxx.Sensor
{
    public class InaxxxSensorSettings
    {
        private const string DataPinKey = "InaxxxSensor.DataPin";
        private const string ClockPinKey = "InaxxxSensor.ClockPin";

        public void Configure(IDictionary dictionary)
        {
            object Setting(string key) => dictionary[key] ?? throw new ArgumentException(nameof(InaxxxSensorSettings), key);

            DataPin = (int)Setting(DataPinKey);
            ClockPin = (int)Setting(ClockPinKey);
        }

        public int DataPin { get; set; }

        public int ClockPin { get; set; }
    }
}
