using System.Collections;
using TelemetryStash.Ds18b20Sensor;
using TelemetryStash.IliDisplay;
using TelemetryStash.IO.Peripherals.Buzzer;
using TelemetryStash.MpuGyroSensor;
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
            Ds18b20.Configure(dictionary);
            PiezoBuzzer.Configure(dictionary);
        }

        public MpuGyroSettings MpuGyro { get; set; } = new();

        public IliDisplaySettings IliDisplay { get; set; } = new();

        public ThrottleSettings Throttle { get; set; } = new();

        public Ds18b20SensorSettings Ds18b20 { get; set; } = new();

        public PiezoBuzzerSettings PiezoBuzzer { get; set; } = new();
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
            AdcReadScale = dictionary.Int32(AdcReadScaleKey);
            PrimaryButtonPin = dictionary.Int32(PrimaryButtonPinKey);
            ThrustLockPin = dictionary.Int32(ThrustLockPinKey);
            LeftMotorPin = dictionary.Int32(LeftMotorPinKey);
            RightMotorPin = dictionary.Int32(RightMotorPinKey);
            LeftThrotledPin = dictionary.Int32(LeftThrotledPinKey);

            var setting = dictionary.List(ThrustSensorPinsKey);
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
