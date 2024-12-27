using nanoFramework.Hardware.Esp32;
using System;
using System.Collections;

namespace TelemetryStash.Aidon.Sensor
{
    public class AidonSensorSettings
    {
        private const string ComPortKey = "AidonSensor.ComPort";
        private const string RxPinKey = "AidonSensor.RxPin";
        private const string RxComPortKey = "AidonSensor.RxComPort";
        
        public void Configure(IDictionary dictionary)
        {
            object Setting(string key) => dictionary[key] ?? throw new ArgumentException(nameof(AidonSensorSettings), key);

            ComPort = (string)Setting(ComPortKey);
            RxPin = (int)Setting(RxPinKey);
            RxComPort = (DeviceFunction)Setting(RxComPortKey);
        }

        public string ComPort { get; set; }

        public int RxPin { get; set; }

        public DeviceFunction RxComPort { get; set; }
    }
}
