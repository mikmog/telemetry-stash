using nanoFramework.Hardware.Esp32;
using System.Collections;
using TelemetryStash.Shared;

namespace TelemetryStash.Peripherals.Bms.Daly
{
    public class ActiveBalanceBmsSettings
    {
        public const string BmsComKey = "ActiveBalanceBms.ComPort";
        public const string BmsComRtsKey = "ActiveBalanceBms.ComRts";
        public const string BmsComRtsPinKey = "ActiveBalanceBms.ComRtsPin";
        public const string BmsComTxKey = "ActiveBalanceBms.ComTx";
        public const string BmsComTxPinKey = "ActiveBalanceBms.ComTxPin";
        public const string BmsComRxKey = "ActiveBalanceBms.ComRx";
        public const string BmsComRxPinKey = "ActiveBalanceBms.ComRxPin";


        public void Configure(IDictionary dictionary)
        {
            ComPort = dictionary.String(BmsComKey);
            ComRts = dictionary.DeviceFunction(BmsComRtsKey);
            ComRtsPin = dictionary.Int32(BmsComRtsPinKey);
            ComTx = dictionary.DeviceFunction(BmsComTxKey);
            ComTxPin = dictionary.Int32(BmsComTxPinKey);
            ComRx = dictionary.DeviceFunction(BmsComRxKey);
            ComRxPin = dictionary.Int32(BmsComRxPinKey);
        }

        public string ComPort { get; set; }

        public DeviceFunction ComRts { get; set; }
        public int ComRtsPin { get; set; }
        public DeviceFunction ComTx { get; set; }
        public int ComTxPin { get; set; }
        public DeviceFunction ComRx { get; set; }
        public int ComRxPin { get; set; }
    }
}
