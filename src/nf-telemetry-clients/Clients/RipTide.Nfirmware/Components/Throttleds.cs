using RipTide.Nfirmware.Components.Common;
using System.Device.Adc;
using System.Device.Gpio;
using System.Threading;

namespace RipTide.Nfirmware.Components
{
    internal class Throttleds : Component
    {
        private int _thrust;

        public Throttleds(AdcController adc, GpioController gpio, ErrorHandler errorHandler) : base(adc, gpio, errorHandler) { }

        public override void Initialize(AppSettings appSettings)
        {
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
            }
        }

        public void ThrustChanged(int thrust)
        {
            _thrust = thrust;
        }
    }
}
