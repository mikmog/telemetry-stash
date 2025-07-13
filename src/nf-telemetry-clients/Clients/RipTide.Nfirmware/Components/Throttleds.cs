using NeoPixel.Peripheral;
using RipTide.Nfirmware.Components.Common;
using System.Device.Adc;
using System.Device.Gpio;
using System.Drawing;
using System.Threading;

namespace RipTide.Nfirmware.Components
{
    internal class Throttleds : Component
    {
        private DualNeoPixelGauge _gauge;
        private int _thrustValue;

        public Throttleds(AdcController adc, GpioController gpio, ErrorHandler errorHandler) : base(adc, gpio, errorHandler) { }

        public override void Initialize(AppSettings appSettings)
        {
            var settings = appSettings.Throttle;

            var colors = new Color[] { Color.Green, Color.Yellow, Color.Red };
            _gauge = new((ushort)settings.AdcReadScale, colors, pin: (byte)settings.ThrotledPin);
            _gauge.Initialize();

            Start(Runner);
        }

        private void Runner()
        {
            var requestedThrust = _thrustValue;
            while (true)
            {
                while (requestedThrust == _thrustValue)
                {
                    Thread.Sleep(10);
                }

                requestedThrust = _thrustValue;
                _gauge.SetPosition(_thrustValue);
            }
        }

        public void ThrustChanged(int thrustValue)
        {
            _thrustValue = thrustValue;
        }

        internal void LightUp()
        {
            _gauge.LightUp();
        }
    }
}
