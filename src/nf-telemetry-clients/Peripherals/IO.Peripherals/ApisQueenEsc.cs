using nanoFramework.Hardware.Esp32;
using System.Device.Pwm;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.Shared;

namespace TelemetryStash.IO.Peripherals
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
    public class ApisQueenEsc
    {
        private readonly PwmChannel _pwm1;
        private readonly PwmChannel _pwm2;

        private readonly Thread _throttleThread;
        private int _currentThrottle = 0;
        private int _requestedThrottle = 0;

        private const double neutralDutyCycle = 0.75;
        private const double oneStepDutyCycle = 0.0025;

        public ApisQueenEsc(int pinLeftMotor, int pinRightMotor)
        {
            //Configuration.SetPinFunction(17, DeviceFunction.PWM9);
            //Configuration.SetPinFunction(18, DeviceFunction.PWM10);
            //_pwm1 = PwmChannel.CreateFromPin(17, 500, neutralDutyCycle);
            //_pwm2 = PwmChannel.CreateFromPin(18, 500, neutralDutyCycle);
            Configuration.SetPinFunction(pinLeftMotor, DeviceFunction.PWM9);
            Configuration.SetPinFunction(pinRightMotor, DeviceFunction.PWM10);
            _pwm1 = PwmChannel.CreateFromPin(pinLeftMotor, 500, neutralDutyCycle);
            _pwm2 = PwmChannel.CreateFromPin(pinRightMotor, 500, neutralDutyCycle);
            Stop();

            _throttleThread = new Thread(SetThrottle);
        }

        public void Initialize()
        {
            _pwm1.Start();
            _pwm2.Start();
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
                    //Thread.Sleep(0);
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

                    //Debug.WriteLine($"Duty cycle: {_pwm1.DutyCycle}. Throttle: {_currentThrottle} %");
                    //Thread.Sleep(0);
                }

                _currentThrottle = currentThrottle;
            }
        }
    }
}
