using Iot.Device.Buzzer;
using nanoFramework.Hardware.Esp32;
using System.Threading;

namespace TelemetryStash.BuzzerPeripheral
{
    public class PiezoBuzzer
    {
        const int buzzerPin = 21;

        public PiezoBuzzer()
        {
            Configuration.SetPinFunction(buzzerPin, DeviceFunction.PWM1);
        }

        public void RunDemo()
        {
            using var buzzer = new Buzzer(buzzerPin);

            while (true)
            {
                buzzer.StartPlaying(440);
                Thread.Sleep(1000);
                buzzer.StartPlaying(880);
                Thread.Sleep(1000);
                buzzer.StopPlaying();
            }
        }
    }
}
