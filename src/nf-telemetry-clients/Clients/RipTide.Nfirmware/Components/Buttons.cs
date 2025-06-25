using RipTide.Nfirmware.Components.Common;
using System.Device.Gpio;

namespace RipTide.Nfirmware.Components
{
    internal class Buttons : Component
    {
        public Buttons(GpioController gpioController, ErrorHandler errorHandler) : base(gpioController, errorHandler) { }

        public override void Initialize(AppSettings appSettings)
        {
            Start(Runner);
        }

        private void Runner()
        {
        }
    }
}
