using System;
using System.Collections;

namespace TelemetryStash.Am23XX.Sensor
{
    public class Am23XXSensorSettings
    {
        private const string DataPinKey = "Am23XXSensor.DataPin";
        private const string ClockPinKey = "Am23XXSensor.ClockPin";

        public void Configure(IDictionary dictionary)
        {
            object Setting(string key) => dictionary[key] ?? throw new ArgumentException(key);

            DataPin = (int)Setting(DataPinKey);
            ClockPin = (int)Setting(ClockPinKey);
        }
        
        public int DataPin { get; set; }
        
        public int ClockPin { get; set; }
    }
}
