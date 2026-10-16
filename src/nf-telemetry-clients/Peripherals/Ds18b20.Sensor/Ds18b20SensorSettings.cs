using nanoFramework.Hardware.Esp32;
using System;
using System.Collections;

namespace TelemetryStash.Inaxxx.Sensor
{
    public class Ds18b20SensorSettings
    {
        private const string ComPortKey = "Ds18b20Sensor.ComPort";
        private const string RxPinKey = "Ds18b20Sensor.RxPin";
        private const string RxComPortKey = "Ds18b20Sensor.RxComPort";

        public void Configure(IDictionary dictionary)
        {
            object Setting(string key) => dictionary[key] ?? throw new ArgumentException(nameof(Ds18b20SensorSettings), key);

            ComPort = (string)Setting(ComPortKey);
            RxPin = (short)Setting(RxPinKey);
            RxComPort = (DeviceFunction)Setting(RxComPortKey);
        }

        public string ComPort { get; set; }

        public short RxPin { get; set; }

        public DeviceFunction RxComPort { get; set; }
    }
}
