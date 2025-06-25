using Iot.Device.Button;
using System;
using System.Device.Gpio;

namespace TelemetryStash.IO.Peripherals
{
    public class Button
    {
        private readonly GpioButton _button;

        public Button(int buttonPin, GpioController gpioController)
        {
            _button = new GpioButton(buttonPin: buttonPin, gpioController, debounceTime: TimeSpan.FromMilliseconds(50))
            {
                IsDoublePressEnabled = false,
                IsHoldingEnabled = false
            };
        }

        public void OnButtonDown(Action action)
        {
            _button.ButtonDown += (sender, e) => action.Invoke();
        }
    }
}
