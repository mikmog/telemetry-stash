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
        private int _thrust;

        public Throttleds(AdcController adc, GpioController gpio, ErrorHandler errorHandler) : base(adc, gpio, errorHandler) { }

        public override void Initialize(AppSettings appSettings)
        {
            var settings = appSettings.Throttle;

            var colors = new Color[] { Color.Green, Color.Yellow, Color.Red };
            _gauge = new((ushort)settings.AdcReadScale, colors, pin: (byte)settings.LeftThrotledPin);
            _gauge.Initialize();

            Start(Runner);
        }

        private void Runner()
        {
            var requestedThrust = _thrust;
            while (true)
            {
                while (requestedThrust == _thrust)
                {
                    Thread.Sleep(10);
                }

                requestedThrust = _thrust;
                _gauge.SetPosition(_thrust);
            }
        }

        public void ThrustChanged(int thrust)
        {
            _thrust = thrust;
        }
    }
}
