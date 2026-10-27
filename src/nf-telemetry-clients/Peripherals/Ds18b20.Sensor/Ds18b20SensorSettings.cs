using nanoFramework.Hardware.Esp32;
using System;
using System.Collections;

namespace TelemetryStash.Ds18b20Sensor
{
    public class Ds18b20SensorSettings
    {
        private const string RxPinKey = "Ds18b20Sensor.RxPin";
        private const string TxPinKey = "Ds18b20Sensor.TxPin";
        private const string RxComPortKey = "Ds18b20Sensor.RxComPort";
        private const string TxComPortKey = "Ds18b20Sensor.TxComPort";

        public void Configure(IDictionary dictionary)
        {
            object Setting(string key) => dictionary[key] ?? throw new ArgumentException(nameof(Ds18b20SensorSettings), key);

            RxPin = (int)Setting(RxPinKey);
            TxPin = (int)Setting(TxPinKey);
            RxComPort = (DeviceFunction)Setting(RxComPortKey);
            TxComPort = (DeviceFunction)Setting(TxComPortKey);
        }

        public int RxPin { get; set; }
        public int TxPin { get; set; }

        public DeviceFunction RxComPort { get; set; }
        public DeviceFunction TxComPort { get; set; }
    }
}
