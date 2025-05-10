using RipTide.Nfirmware.Components.Common;

namespace RipTide.Nfirmware.Components
{
    internal class DepthMonitor : Component
    {
        public DepthMonitor(ErrorHandler errorHandler) : base(errorHandler) { }

        public override void Initialize(AppSettings appSettings)
        {
            Start(Runner);
        }

        private void Runner()
        {
        }
    }
}
