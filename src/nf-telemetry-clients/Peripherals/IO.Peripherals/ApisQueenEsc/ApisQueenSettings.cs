using nanoFramework.Hardware.Esp32;
using System.Collections;
using TelemetryStash.Shared;

namespace TelemetryStash.IO.Peripherals.ApisQueen
{
    public class ApisQueenSettings
    {
        public const string LeftMotorPinKey = "ApisQueen.LeftMotorPin";
        public const string LeftMotorPwmChannelKey = "ApisQueen.LeftMotorPwmChannel";
        public const string RightMotorPinKey = "ApisQueen.RightMotorPin";
        public const string RightMotorPwmChannelKey = "ApisQueen.RightMotorPwmChannel";

        public void Configure(IDictionary dictionary)
        {
            LeftMotorPin = dictionary.Int32(LeftMotorPinKey);
            LeftMotorPwmChannel = dictionary.DeviceFunction(LeftMotorPwmChannelKey);

            RightMotorPin = dictionary.Int32(RightMotorPinKey);
            RightMotorPwmChannel = dictionary.DeviceFunction(RightMotorPwmChannelKey);
        }

        public int LeftMotorPin { get; set; }
        public DeviceFunction LeftMotorPwmChannel { get; set; }

        public int RightMotorPin { get; set; }
        public DeviceFunction RightMotorPwmChannel { get; set; }
    }
}
