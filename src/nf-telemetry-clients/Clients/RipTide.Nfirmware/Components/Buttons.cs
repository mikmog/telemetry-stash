using RipTide.Nfirmware.Components.Common;
using System.Device.Adc;
using System.Device.Gpio;

namespace RipTide.Nfirmware.Components
{
    internal class Buttons : Component
    {
        public Buttons(AdcController adc, GpioController gpio, ErrorHandler errorHandler) : base(adc, gpio, errorHandler) { }

        public override void Initialize(AppSettings appSettings)
        {
            Start(Runner);
        }

        private void Runner()
        {
        }
    }
}
