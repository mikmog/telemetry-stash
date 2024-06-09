using nanoFramework.Hardware.Esp32;
using System;
using System.Collections;

namespace TelemetryStash.Aidon.Sensor
{
    public class AidonSensorSettings
    {
        private const string RegisterSetIdentifierKey = "AidonSensor.RegisterSetIdentifier";
        private const string ComPortKey = "AidonSensor.ComPort";
        //private const string TxPinKey = "AidonSensor.TxPin";
        private const string RxPinKey = "AidonSensor.RxPin";
        //private const string TxComPortKey = "AidonSensor.TxComPort";
        private const string RxComPortKey = "AidonSensor.RxComPort";
        
        public void Configure(IDictionary dictionary)
        {
            object Setting(string key) => dictionary[key] ?? throw new ArgumentException(nameof(AidonSensorSettings), key);

            RegisterSetIdentifier = (string)Setting(RegisterSetIdentifierKey);

            ComPort = (string)Setting(ComPortKey);

            //TxPin = (int)Setting(TxPinKey);
            RxPin = (int)Setting(RxPinKey);
            
            //TxComPort = (DeviceFunction)Setting(TxComPortKey);
            RxComPort = (DeviceFunction)Setting(RxComPortKey);
        }

        public string RegisterSetIdentifier { get; set; }

        //public int TxPin { get; set; }
        
        //public DeviceFunction TxComPort { get; set; }

        public string ComPort { get; set; }

        public int RxPin { get; set; }

        public DeviceFunction RxComPort { get; set; }
    }
}
