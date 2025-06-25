using RipTide.Nfirmware.Components.Common;
using System.Device.Gpio;

namespace RipTide.Nfirmware.Components
{
    internal class BatteryMonitor : Component
    {
        public BatteryMonitor(GpioController gpioController, ErrorHandler errorHandler) : base(gpioController, errorHandler) { }

        public override void Initialize(AppSettings appSettings)
        {
            Start(Runner);
        }

        private void Runner()
        {
        }
    }
}
