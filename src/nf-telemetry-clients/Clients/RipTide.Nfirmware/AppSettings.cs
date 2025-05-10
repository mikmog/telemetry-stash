using System;
using System.Collections;
using TelemetryStash.IliDisplay;
using TelemetryStash.MpuxxxxGyro.Sensor;
using TelemetryStash.NfClient.Services;

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
        private const string ThrustSensorPinsKey = "Throttle.ThrustSensorPins";
        private const string ThrustLockSensorPinsKey = "Throttle.ThrustLockSensorPins";

        public void Configure(IDictionary dictionary)
        {
            object Setting(string key) => dictionary[key] ?? throw new ArgumentException(key);

            AdcReadScale = (int)Setting(AdcReadScaleKey);
            PrimaryButtonPin = (int)Setting(PrimaryButtonPinKey);

            var setting = (ArrayList)Setting(ThrustSensorPinsKey);
            ThrustSensorPins = new int[setting.Count];
            setting.CopyTo(ThrustSensorPins, 0);

            setting = (ArrayList)Setting(ThrustLockSensorPinsKey);
            ThrustLockSensorPins = new int[setting.Count];
            setting.CopyTo(ThrustLockSensorPins, 0);
        }

        public int AdcReadScale { get; set; }

        public int PrimaryButtonPin { get; set; }

        public int[] ThrustSensorPins { get; set; }

        public int[] ThrustLockSensorPins { get; set; }
    }
}
