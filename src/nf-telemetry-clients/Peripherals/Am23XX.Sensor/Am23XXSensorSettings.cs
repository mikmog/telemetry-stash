using System;
using System.Collections;

namespace TelemetryStash.Am23XX.Sensor
{
    public class Am23XXSensorSettings
    {
        private const string RegisterSetIdentifierKey = "Am23XXSensor.RegisterSetIdentifier";
        private const string DataPinKey = "Am23XXSensor.DataPin";
        private const string ClockPinKey = "Am23XXSensor.ClockPin";

        public void Configure(IDictionary dictionary)
        {
            object Setting(string key) => dictionary[key] ?? throw new ArgumentException(key);

            RegisterSetIdentifier = (string)Setting(RegisterSetIdentifierKey);
            DataPin = (int)Setting(DataPinKey);
            ClockPin = (int)Setting(ClockPinKey);
        }

        public string RegisterSetIdentifier { get; set; }
        
        public int DataPin { get; set; }
        
        public int ClockPin { get; set; }
    }
}
