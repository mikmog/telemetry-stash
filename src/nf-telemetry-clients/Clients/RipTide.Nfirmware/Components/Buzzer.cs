using RipTide.Nfirmware.Components.Common;
using System;
using System.Device.Adc;
using System.Device.Gpio;
using TelemetryStash.IO.Peripherals.Buzzer;

namespace RipTide.Nfirmware.Components
{
    public class Buzzer : Component
    {
        public Buzzer(AdcController adc, GpioController gpio, ErrorHandler errorHandler) : base(adc, gpio, errorHandler) { }

        private readonly PiezoBuzzer _buzzer = new();

        public override void Initialize(AppSettings appSettings)
        {
            _buzzer.Initialize(appSettings.PiezoBuzzer);
        }

        public void BuzzWarning()
        {
            _buzzer.BuzzAsync(1000, TimeSpan.FromMilliseconds(500));
            _buzzer.BuzzAsync(1000, TimeSpan.FromMilliseconds(500));
        }

        public void BuzzError()
        {
            _buzzer.BuzzAsync(2000, TimeSpan.FromMilliseconds(500));
            _buzzer.BuzzAsync(1000, TimeSpan.FromMilliseconds(500));
            _buzzer.BuzzAsync(500, TimeSpan.FromMilliseconds(500));
        }
    }
}
