using RipTide.Nfirmware.Components.Common;
using System.Threading;
using TelemetryStash.AdcSensor;

namespace RipTide.Nfirmware.Components
{
    internal class Throttle : Component
    {
        private readonly Ss49eHallSensor _thrustSensor = new();
        private readonly Ss49eHallSensor _thrustLockSensor = new();

        public Throttle(ErrorHandler errorHandler) : base(errorHandler) { }

        public override void Initialize(AppSettings appSettings)
        {
            _thrustSensor.Initialize(appSettings.Throttle.ThrustSensorPins, appSettings.Throttle.AdcReadScale, reverseScale: true);
            _thrustSensor.CalibrateAdcChannelOffsets();

            _thrustLockSensor.Initialize(appSettings.Throttle.ThrustLockSensorPins, adcReadScale: 10, reverseScale: true);
            _thrustLockSensor.CalibrateAdcChannelOffsets();

            Start(Runner);
        }

        private void Runner()
        {
            while (true)
            {
                var value = _thrustSensor.Read();
                Thread.Sleep(100);
                OnThrustChanged?.Invoke(value);
            }
        }

        public delegate void ThrustChanged(int thrust);
        public event ThrustChanged OnThrustChanged;
    }
}
