using nanoFramework.Hardware.Esp32;
using System;
using System.Collections;
using System.Device.Pwm;
using System.Threading;

namespace TelemetryStash.IO.Peripherals.Buzzer
{
    // Wiring 5V active buzzer
    // https://www.sunrom.com/p/electromagnetic-active-buzzer-5v

    public class PiezoBuzzer
    {
        private Thread _buzzerRunner;
        private PwmChannel _pwmChannel = null;

        private readonly ArrayList _tones = new();

        public void Initialize(PiezoBuzzerSettings settings)
        {
            Configuration.SetPinFunction(settings.Pin, settings.DeviceFunction);
            _pwmChannel = PwmChannel.CreateFromPin(settings.Pin);

            _buzzerRunner = new Thread(BuzzerRunner);
            _buzzerRunner.Start();
        }

        public void BuzzAsync(int frequency, TimeSpan duration)
        {
            _tones.Add(new Tone(frequency, duration));
        }

        public void Buzz(int frequency, TimeSpan duration)
        {
            _pwmChannel.Frequency = frequency;
            _pwmChannel.Start();
            Thread.Sleep(duration);
            _pwmChannel.Stop();
        }

        private void BuzzerRunner()
        {
            while (true)
            {
                if (_tones.Count == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }

                var tonesArray = new Tone[_tones.Count];
                _tones.CopyTo(tonesArray);
                _tones.Clear();

                foreach (var tone in tonesArray)
                {
                    Buzz(tone.Frequency, tone.Duration);
                }
            }
        }

        private struct Tone
        {
            public Tone(int frequency, TimeSpan duration)
            {
                Frequency = frequency;
                Duration = duration;
            }

            public int Frequency { get; set; }
            public TimeSpan Duration { get; set; }
        }
    }
}
