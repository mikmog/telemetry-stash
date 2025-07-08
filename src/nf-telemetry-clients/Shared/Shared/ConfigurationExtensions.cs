using nanoFramework.Hardware.Esp32;
using System;

namespace TelemetryStash.Shared
{
    public static class ConfigurationExtensions
    {
        public static DeviceFunction ParseDeviceFunction(string deviceFunction)
        {
            return deviceFunction switch
            {
                "COM1_RX" => DeviceFunction.COM1_RX,
                "COM1_TX" => DeviceFunction.COM1_TX,
                "COM2_RX" => DeviceFunction.COM2_RX,
                "COM2_TX" => DeviceFunction.COM2_TX,
                "COM3_RX" => DeviceFunction.COM3_RX,
                "COM3_TX" => DeviceFunction.COM3_TX,

                "PWM2" => DeviceFunction.PWM2,

                _ => throw new Exception($"Invalid device function: {deviceFunction}")
            };
        }
    }
}
