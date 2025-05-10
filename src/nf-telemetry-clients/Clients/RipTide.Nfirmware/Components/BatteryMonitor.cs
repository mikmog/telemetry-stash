using RipTide.Nfirmware.Components.Common;

namespace RipTide.Nfirmware.Components
{
    internal class BatteryMonitor : Component
    {
        public BatteryMonitor(ErrorHandler errorHandler) : base(errorHandler) { }

        public override void Initialize(AppSettings appSettings)
        {
            Start(Runner);
        }

        private void Runner()
        {
        }
    }
}
