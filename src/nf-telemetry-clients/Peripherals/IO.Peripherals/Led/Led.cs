using System.Device.Gpio;
using System.Threading;

namespace TelemetryStash.IO.Peripherals.Led
{
    public class Led
    {
        private static readonly object _lock = new();

        private readonly GpioPin _led;
        private bool _isOn;

        public Led(int pin, GpioController gpioController)
        {
            _led = gpioController.OpenPin(pin, PinMode.Output);
        }

        public void Signal(LedSignal signal)
        {
            lock (_lock)
            {
                switch (signal)
                {
                    case LedSignal.On:
                        TurnOn();
                        break;
                    case LedSignal.Off:
                        TurnOff();
                        break;
                    case LedSignal.Blink5ForStart:
                        Blink(5, msOn: 100, msOff: 50);
                        break;
                    case LedSignal.BlinkSosFor60Sec:
                        {
                            for (var i = 0; i < 20; i++)
                            {
                                Blink(3, msOn: 150, msOff: 50); // 600
                                Thread.Sleep(200);              // 200
                                Blink(2, msOn: 450, msOff: 50); // 1000
                                Thread.Sleep(200);              // 200
                                Blink(3, msOn: 150, msOff: 50); // 600
                                Thread.Sleep(500);              // 500
                            }
                        }
                        break;
                    case LedSignal.Blink2ForError:
                        Blink(2, msOn: 100, msOff: 50);
                        break;
                }
            }
        }

        private void Blink(ushort times, int msOn, int msOff)
        {
            if (_isOn)
            {
                TurnOff();
                Thread.Sleep(50);
            }

            for (ushort i = 0; i < times; i++)
            {
                TurnOn();
                Thread.Sleep(msOn);
                TurnOff();
                Thread.Sleep(msOff);
            }
        }

        private void TurnOn()
        {
            _led.Write(PinValue.High);
            _isOn = true;
        }

        private void TurnOff()
        {
            _led.Write(PinValue.Low);
            _isOn = false;
        }
    }

    public enum LedSignal
    {
        On,
        Off,
        Blink2ForError,
        Blink5ForStart,
        BlinkSosFor60Sec,
    }
}
