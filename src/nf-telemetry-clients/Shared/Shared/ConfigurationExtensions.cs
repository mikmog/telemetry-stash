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
                "COM2_RX" => DeviceFunction.COM2_RX,
                "COM2_TX" => DeviceFunction.COM2_TX,
                "COM2_RTS" => DeviceFunction.COM2_RTS,

                "COM3_RX" => DeviceFunction.COM3_RX,
                "COM3_TX" => DeviceFunction.COM3_TX,
                "COM3_RTS" => DeviceFunction.COM3_RTS,

                "PWM1" => DeviceFunction.PWM1,
                "PWM2" => DeviceFunction.PWM2,
                "PWM3" => DeviceFunction.PWM3,
                "PWM4" => DeviceFunction.PWM4,
                "PWM5" => DeviceFunction.PWM5,
                "PWM6" => DeviceFunction.PWM6,

                "SPI1_MISO" => DeviceFunction.SPI1_MISO,
                "SPI1_MOSI" => DeviceFunction.SPI1_MOSI,
                "SPI1_CLOCK" => DeviceFunction.SPI1_CLOCK,

                "I2C1_DATA" => DeviceFunction.I2C1_DATA,
                "I2C1_CLOCK" => DeviceFunction.I2C1_CLOCK,

                _ => throw new Exception($"Invalid device function: {deviceFunction}")
            };
        }
    }
}
