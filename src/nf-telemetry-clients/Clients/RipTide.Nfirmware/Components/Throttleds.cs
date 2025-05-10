using RipTide.Nfirmware.Components.Common;
using System;
using System.Threading;

namespace RipTide.Nfirmware.Components
{
    internal class Throttleds : Component
    {
        private int _thrust;

        public Throttleds(ErrorHandler errorHandler) : base(errorHandler) { }

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
                Console.WriteLine($"Throttleds: Requested thrust: {requestedThrust}");
            }
        }

        public void ThrustChanged(int thrust)
        {
            _thrust = thrust;
        }
    }
}
