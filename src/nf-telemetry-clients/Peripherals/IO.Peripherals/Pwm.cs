using nanoFramework.Hardware.Esp32;
using System.Device.Gpio;
using System.Device.Pwm;
using System.Diagnostics;
using System.Threading;

namespace TelemetryStash.IO.Peripherals
{
    public class Pwm
    {
        private readonly GpioController _gpioController;

        public Pwm(GpioController gpioController)
        {
            _gpioController = gpioController;
        }

        public void RunDemo()
        {
            var buttonPressed = false;

            var button = new Button(8, _gpioController);
            button.OnButtonDown(() => buttonPressed = true);

            Configuration.SetPinFunction(17, DeviceFunction.PWM10);
            var pwm = PwmChannel.CreateFromPin(17, 500);
            pwm.DutyCycle = 0.75;
            pwm.Start();

            Debug.WriteLine("PWM started");
            Debug.WriteLine($"Duty cycle: {pwm.DutyCycle}. Freq: {pwm.Frequency} Hz");

            for (double i = 0.75; i <= 1; i += 0.01)
            {
                while (!buttonPressed)
                {
                    Thread.Sleep(50);
                }

                Debug.WriteLine("Button pressed, changing duty cycle");
                Thread.Sleep(500);
                pwm.DutyCycle = i;
                Debug.WriteLine($"Duty cycle: {pwm.DutyCycle}. Freq: {pwm.Frequency} Hz");
                buttonPressed = false;
            }
        }
    }
}
