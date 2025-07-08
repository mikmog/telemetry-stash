using System.Collections;
using TelemetryStash.Shared;

namespace TelemetryStash.Bmxx80.Sensor
{
    public class Bme680SensorSettings
    {
        private const string DataPinKey = "Bme680Sensor.DataPin";
        private const string ClockPinKey = "Bme680Sensor.ClockPin";

        public void Configure(IDictionary dictionary)
        {
            DataPin = dictionary.Int32(DataPinKey);
            ClockPin = dictionary.Int32(ClockPinKey);
        }

        public int DataPin { get; set; }

        public int ClockPin { get; set; }
    }
}
