using RipTide.Nfirmware.Components.Common;
using System;
using System.Device.Adc;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.AdcSensor;
using TelemetryStash.IO.Peripherals;
using TelemetryStash.Shared;

namespace RipTide.Nfirmware.Components
{
    internal class Throttle : Component
    {
        public Throttle(AdcController adc, GpioController gpio, ErrorHandler errorHandler) : base(adc, gpio, errorHandler) { }

        private int _adcReadScale;
        private double _thrustScaleFactor;
        private int _currentThrust = -1;
        private bool _thrustLockEngaged = true;

        private GpioPin _thrustLockPin;
        private Ss49eHallSensor _thrustSensor;
        private ApisQueenEsc _apisQueenEsc;

        public bool ThrustRangeCalibrationComplete { get; private set; }

        public override void Initialize(AppSettings appSettings)
        {
            var settings = appSettings.Throttle;

            _adcReadScale = settings.AdcReadScale;
            _thrustScaleFactor = 100d / _adcReadScale;

            _apisQueenEsc = new ApisQueenEsc(settings.LeftMotorPin, settings.RightMotorPin);
            _apisQueenEsc.Initialize();

            _thrustSensor = new(_adcController);
            _thrustSensor.Initialize(settings.ThrustSensorPins, _adcReadScale, reverseScale: true);

            _thrustLockPin = _gpioController.OpenPin(settings.ThrustLockPin, PinMode.InputPullDown);
            _thrustLockPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            _thrustLockEngaged = _thrustLockPin.Read() == PinValue.High;
            _thrustLockPin.ValueChanged += (sender, args) =>
            {
                _thrustLockEngaged = args.ChangeType == PinEventTypes.Rising;
                Debug.WriteLine($"Thrust lock engaged: {_thrustLockEngaged}");
            };

            Start(Runner);
        }

        public void CalibrateThrustRange(ActionMessageDelegate onMessage, ActionMessageDelegate onError)
        {
            if (!AwaitThrustSensorInitialization())
            {
                _errorHandler.OnError("Thrust sensor initialization failed.");
                onError.Invoke("Thrust sensor initialization failed.");
                return;
            }

            int thrustRangeZeroThrust;
            int thrustRangeFullThrust;

            // Calibrate zero thrust
            while (true)
            {
                if (!_thrustLockEngaged)
                {
                    onMessage.Invoke("Thrust calibration. Let go of the throttle", sleep: 1000);
                    continue;
                }

                var success = GetRawCalibrationValue(onMessage, out var rawZeroThrustValue);
                if (!success)
                {
                    onError("Failed reading thrust sensor");
                    return;
                }

                if (!_thrustLockEngaged)
                {
                    onMessage.Invoke("Thrust calibration. Zero throttle please", sleep: 1000);
                    continue;
                }

                thrustRangeZeroThrust = rawZeroThrustValue;
                break;
            }

            // Calibrate full thrust
            var message = "Thrust calibration. Give full throttle!";
            while (true)
            {
                onMessage.Invoke(message, sleep: 1000);
                if (_thrustLockEngaged)
                {
                    continue;
                }

                // Give user time to engage full thrust
                Thread.Sleep(1000);

                var success = GetRawCalibrationValue(onMessage, out var rawFullThrustValue);
                if (!success)
                {
                    onError("Failed reading thrust sensor full thrust");
                    return;
                }

                if (_thrustLockEngaged)
                {
                    continue;
                }

                // Minimum delta to consider thrust calibration valid
                const int minimumThrustRange = 600;
                var thrustRange = MathExtensions.Abs(rawFullThrustValue - thrustRangeZeroThrust);
                if (thrustRange < minimumThrustRange)
                {
                    Debug.WriteLine($"Invalid Thrust range. Expected => {minimumThrustRange}. Actual: {thrustRange}");
                    message = "Thrust calibration. Floor it and hold still!";
                    continue;
                }

                thrustRangeFullThrust = rawFullThrustValue;
                break;
            }

            while (!_thrustLockEngaged)
            {
                onMessage.Invoke("Thrust calibration complete", sleep: 1000);
                onMessage.Invoke($"Thrust range. From {thrustRangeZeroThrust} to {thrustRangeFullThrust}", sleep: 1000);
            }

            _thrustSensor.SetAdcRangeValues(thrustRangeFullThrust, thrustRangeZeroThrust);
            ThrustRangeCalibrationComplete = true;
        }

        private void Runner()
        {
            while (!ThrustRangeCalibrationComplete || !_thrustSensor.IsInitialized)
            {
                Thread.Sleep(100);
            }

            while (true)
            {
                if (_thrustLockEngaged)
                {
                    if (_currentThrust > 0)
                    {
                        _currentThrust = 0;
                        _apisQueenEsc.SetThrottle(0);
                        OnThrustChanged.Invoke(0);
                    }

                    Thread.Sleep(10);
                    continue;
                }

                var requestedThrust = _thrustSensor.Read();
                Thread.Sleep(10);

                if (requestedThrust == Ss49eHallSensor.DirtyReadValue)
                {
                    _thrustSensor.AwaitAdcChannelOffsetCalibration();
                    continue;
                }

                // Sticky throttle
                if (requestedThrust != 0 && _currentThrust - requestedThrust == 1)
                {
                    continue;
                }

                // Debounce the read
                Thread.Sleep(Ss49eHallSensor.SensorReadInterval);
                if (requestedThrust != _thrustSensor.Read())
                {
                    continue;
                }

                if (requestedThrust != _currentThrust)
                {
                    _currentThrust = _thrustLockEngaged ? 0 : requestedThrust;

                    // Manual handling because of float precision. Round up to 100%
                    var thrustPercentage = _currentThrust == _adcReadScale ? 100 : (int)(_currentThrust * _thrustScaleFactor);

                    _apisQueenEsc.SetThrottle(thrustPercentage);
                    OnThrustChanged.Invoke(_currentThrust);
                }
            }
        }

        private bool AwaitThrustSensorInitialization()
        {
            const int maxAttempts = 500;
            for (var i = 0; i < maxAttempts; i++)
            {
                if (_thrustSensor.IsInitialized)
                {
                    return true;
                }

                Thread.Sleep(10);
            }

            return false;
        }

        private bool GetRawCalibrationValue(ActionMessageDelegate onMessage, out int rawValue)
        {
            const int totalReads = 30;
            const int readInterval = 50; // milliseconds

            var buffer = new int[totalReads];
            for (int i = 0; i < totalReads; i++)
            {
                Thread.Sleep(readInterval);

                // Read the raw sensor value
                var value = _thrustSensor.Read(scaledRange: false);
                buffer[i] = value;

                // Ensure the read is valid
                // If read is dirty, calibrate sensor offsets, reset attempts and restart calibration
                // If read is faulty, abort
                if (value == Ss49eHallSensor.DirtyReadValue)
                {
                    onMessage.Invoke("Dirty thrust sensor read. Retrying...");
                    _thrustSensor.AwaitAdcChannelOffsetCalibration();
                    i = -1;
                    continue;
                }
                else if (value == Ss49eHallSensor.FaultyReadValue)
                {
                    rawValue = Ss49eHallSensor.FaultyReadValue;
                    return false;
                }

                // If last read, ensure range min and max are within acceptable limits
                // If the range is too large, most likely the throttle is moving, reset attempts and restart calibration
                const int maxDeltaRange = 20;
                if (i == totalReads - 1)
                {
                    buffer.MinMax(out var minValue, out var maxValue);
                    if (maxValue - minValue > maxDeltaRange)
                    {
                        onMessage.Invoke("Bad thrust sensor read. Retrying...", sleep: 1000); // Give user time to stabilize throttle
                        i = -1;
                        continue;
                    }
                }
            }

            rawValue = buffer.Average();
            return true;
        }

        public delegate int FuncDelegate(int value);
        public delegate void ActionMessageDelegate(string message, int sleep = 0);
        public delegate void ActionNumberDelegate(int value);

        public delegate void ThrustChanged(int thrust);
        public event ThrustChanged OnThrustChanged;
    }
}
