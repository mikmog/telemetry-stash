using nanoFramework.Hardware.Esp32;
using System.Collections;
using TelemetryStash.Shared;

namespace TelemetryStash.Aidon.Sensor
{
    public class AidonSensorSettings
    {
        private const string ComPortKey = "AidonSensor.ComPort";
        private const string RxPinKey = "AidonSensor.RxPin";
        private const string RxComPortKey = "AidonSensor.RxComPort";

        public void Configure(IDictionary dictionary)
        {
            ComPort = dictionary.String(ComPortKey);
            RxPin = dictionary.Int32(RxPinKey);
            RxComPort = dictionary.DeviceFunction(RxComPortKey);
        }

        public string ComPort { get; set; }

        public int RxPin { get; set; }

        public DeviceFunction RxComPort { get; set; }
    }
}
