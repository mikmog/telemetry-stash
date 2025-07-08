using nanoFramework.Hardware.Esp32;
using System.Collections;
using TelemetryStash.Shared;

namespace TelemetryStash.IO.Peripherals.Buzzer
{
    public class PiezoBuzzerSettings
    {
        public const string PinKey = "PiezoBuzzer.Pin";
        public const string DeviceFunctionKey = "PiezoBuzzer.DeviceFunction";

        public void Configure(IDictionary dictionary)
        {
            Pin = dictionary.Int32(PinKey);
            DeviceFunction = dictionary.DeviceFunction(DeviceFunctionKey);
        }

        public int Pin { get; set; }
        public DeviceFunction DeviceFunction { get; set; }
    }
}
