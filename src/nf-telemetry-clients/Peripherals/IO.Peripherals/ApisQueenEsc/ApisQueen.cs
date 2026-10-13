using nanoFramework.Hardware.Esp32;
using System.Device.Pwm;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.Shared;

namespace TelemetryStash.IO.Peripherals.ApisQueen
{
    /*
        1ms - 1.5ms     1.5ms       1.5ms - 2ms
     <---Reverse--|    Neutral      |--Forward-->

     REVERSE 50%
            __     __     __ 
        ___|  |___|  |___|  |___
    
     NEUTRAL 75%
          ____   ____    ____ 
        _|    |_|    |__|    |_
    
     FORWARD 100%

        __|______|______|______|_
    */
    public class ApisQueen
    {
        private readonly PwmChannel _pwm1;
        private readonly PwmChannel _pwm2;

        private Thread _throttleThread;
        private int _currentThrottle = 0;
        private int _requestedThrottle = 0;

        private const double neutralDutyCycle = 0.75;
        private const double oneStepDutyCycle = 0.0025;

        public ApisQueen(ApisQueenSettings settings)
        {
            Configuration.SetPinFunction(settings.LeftMotorPin, settings.LeftMotorPwmChannel);
            Configuration.SetPinFunction(settings.RightMotorPin, settings.RightMotorPwmChannel);
            _pwm1 = PwmChannel.CreateFromPin(settings.LeftMotorPin, 500, neutralDutyCycle);
            _pwm2 = PwmChannel.CreateFromPin(settings.RightMotorPin, 500, neutralDutyCycle);
            Stop();
        }

        public void Initialize()
        {
            _pwm1.Start();
            _pwm2.Start();

            _throttleThread = new Thread(SetThrottle);
            _throttleThread.Start();
        }

        public void Stop()
        {
            _pwm1.Stop();
            _pwm2.Stop();
        }

        public void SetThrottle(int percentage)
        {
            Debug.WriteLine($"Setting throttle to {percentage} %");

            const int minThrottle = 0;
            const int maxThrottle = 100;

            _requestedThrottle = MathExtensions.Clamp(minThrottle, percentage, maxThrottle);
        }

        private void SetThrottle()
        {
            while (true)
            {
                if (_currentThrottle == _requestedThrottle)
                {
                    Thread.Sleep(1);
                    continue;
                }

                var currentThrottle = _currentThrottle;
                var requestedThrottle = _requestedThrottle;

                // Speed up
                while (requestedThrottle > currentThrottle)
                {
                    var dutyCycle = neutralDutyCycle + ++currentThrottle * oneStepDutyCycle;
                    if (currentThrottle % 2 == 0)
                    {
                        _pwm1.DutyCycle = dutyCycle;
                        _pwm2.DutyCycle = dutyCycle;
                    }
                    else
                    {
                        _pwm2.DutyCycle = dutyCycle;
                        _pwm1.DutyCycle = dutyCycle;
                    }
                }

                // Slow down
                while (requestedThrottle < currentThrottle)
                {
                    var dutyCycle = neutralDutyCycle + --currentThrottle * oneStepDutyCycle;

                    if (currentThrottle % 2 == 0)
                    {
                        _pwm1.DutyCycle = dutyCycle;
                        _pwm2.DutyCycle = dutyCycle;
                    }
                    else
                    {
                        _pwm2.DutyCycle = dutyCycle;
                        _pwm1.DutyCycle = dutyCycle;
                    }
                }

                _currentThrottle = currentThrottle;
            }
        }
    }
}
