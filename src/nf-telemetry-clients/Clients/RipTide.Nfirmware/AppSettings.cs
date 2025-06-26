using System;
using System.Collections;
using TelemetryStash.IliDisplay;
using TelemetryStash.MpuxxxxGyro.Sensor;
using TelemetryStash.Shared;

namespace RipTide.Nfirmware
{
    public class AppSettings
    {
        private readonly ConfigurationReader configurationReader = new();

        public void Configure()
        {
            var dictionary = configurationReader.ReadConfiguration();
            MpuGyro.Configure(dictionary);
            IliDisplay.Configure(dictionary);
            Throttle.Configure(dictionary);
        }

        public MpuGyroSettings MpuGyro { get; set; } = new();

        public IliDisplaySettings IliDisplay { get; set; } = new();

        public ThrottleSettings Throttle { get; set; } = new();
    }

    public class ThrottleSettings
    {
        private const string AdcReadScaleKey = "Throttle.ThrustScale";
        private const string PrimaryButtonPinKey = "Throttle.PrimaryButtonPin";
        private const string ThrustLockPinKey = "Throttle.ThrustLockPin";
        private const string ThrustSensorPinsKey = "Throttle.ThrustSensorPins";
        private const string LeftMotorPinKey = "Throttle.LeftMotorPin";
        private const string RightMotorPinKey = "Throttle.RightMotorPin";
        private const string LeftThrotledPinKey = "Throttle.LeftThrotledPin";

        public void Configure(IDictionary dictionary)
        {
            object Setting(string key) => dictionary[key] ?? throw new ArgumentException(key);

            AdcReadScale = (int)Setting(AdcReadScaleKey);
            PrimaryButtonPin = (int)Setting(PrimaryButtonPinKey);
            ThrustLockPin = (int)Setting(ThrustLockPinKey);
            LeftMotorPin = (int)Setting(LeftMotorPinKey);
            RightMotorPin = (int)Setting(RightMotorPinKey);
            LeftThrotledPin = (int)Setting(LeftThrotledPinKey);

            var setting = (ArrayList)Setting(ThrustSensorPinsKey);
            ThrustSensorPins = new int[setting.Count];
            setting.CopyTo(ThrustSensorPins, 0);
        }

        public int AdcReadScale { get; set; }

        public int PrimaryButtonPin { get; set; }

        public int ThrustLockPin { get; set; }

        public int[] ThrustSensorPins { get; set; }

        public int LeftMotorPin { get; set; }

        public int RightMotorPin { get; set; }

        public int LeftThrotledPin { get; set; }
    }
}
