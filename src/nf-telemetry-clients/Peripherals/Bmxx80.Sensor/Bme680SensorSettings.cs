using System;
using System.Collections;

namespace TelemetryStash.Bmxx80.Sensor
{
    public class Bme680SensorSettings
    {
        private const string DataPinKey = "Bme680Sensor.DataPin";
        private const string ClockPinKey = "Bme680Sensor.ClockPin";

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
