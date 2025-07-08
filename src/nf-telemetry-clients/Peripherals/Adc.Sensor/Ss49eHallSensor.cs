using System;
using System.Device.Adc;
using System.Threading;
using TelemetryStash.Shared;

namespace TelemetryStash.AdcSensor
{
    /*
        Important! To avoid frying the pins on ESP32.
        Magnet north pole facing label side of sensor when running 5v.

        Use a 0.1µF ceramic decoupling capacitor near sensor VCC & GND.
        Larger values like 1µF or 10µF can also be beneficial.
    */

    public class Ss49eHallSensor
    {
        private const int maxChannelsOffset = 100;
        public const int SensorReadInterval = 10;

        private int _adcRangeMinValue = 0;
        private int _adcRangeMaxValue = 4096;
        private double _adcRangeFactor = 0;

        private ChannelMode _channelMode;
        private AdcChannel[] _channels;
        private int[] _adcReadBuffer;
        private int _adcChannel1ValueOffset;
        private int _adcReadScale;
        private bool _reverseScale;

        private bool _calibrationRequested;

        private Thread _sensorReadRunner;
        private readonly AdcController _adcController;

        public const int DirtyReadValue = -1;
        public const int FaultyReadValue = -2;

        public bool IsInitialized { get; private set; }

        public Ss49eHallSensor(AdcController adcController)
        {
            _adcController = adcController;
        }

        // Will only affect dual channel mode
        public void AwaitAdcChannelOffsetCalibration()
        {
            _calibrationRequested = _channelMode == ChannelMode.Dual;
            while (_calibrationRequested)
            {
                Thread.Sleep(50);
            }
        }

        public void SetAdcRangeValues(int adcRangeFirstValue, int adcRangeLastValue)
        {
            _adcRangeMinValue = adcRangeFirstValue < adcRangeLastValue ? adcRangeFirstValue : adcRangeLastValue;
            _adcRangeMaxValue = adcRangeFirstValue > adcRangeLastValue ? adcRangeFirstValue : adcRangeLastValue;

            _adcRangeFactor = 1d / (_adcRangeMaxValue - _adcRangeMinValue);
        }

        /// <summary>
        /// One sensor per channel. Add more sensors to get a more stable reading.
        /// </summary>
        public void Initialize(int[] channels, int adcReadScale, bool reverseScale)
        {
            _channelMode = channels.Length switch
            {
                1 => ChannelMode.Single,
                2 => ChannelMode.Dual,
                _ => throw new ArgumentException("Max two channels are supported")
            };

            _channels = new AdcChannel[channels.Length];
            _adcReadBuffer = new int[channels.Length];

            _adcReadScale = adcReadScale;
            _reverseScale = reverseScale;

            for (var i = 0; i < channels.Length; i++)
            {
                _channels[i] = _adcController.OpenChannel(channels[i]);
            }

            _sensorReadRunner = new Thread(ReadRunner);
            _sensorReadRunner.Start();
        }

        /// <summary>
        /// Calculate average from all channels
        /// </summary>
        /// <returns>Scaled range. -1 if the read is dirty. -2 if read is faulty</returns>
        public int Read(bool scaledRange = true)
        {
            // If outside absolute error range sensor has probably a bad connection to VCC or GND
            const int adcAbsoluteMinValue = 500;
            const int adcAbsoluteMaxValue = 4000;

            if (_channelMode == ChannelMode.Single)
            {
                var v0 = _adcReadBuffer[0];
                if (v0 < adcAbsoluteMinValue || v0 > adcAbsoluteMaxValue)
                {
                    return FaultyReadValue;
                }

                return v0;
            }

            var v1 = _adcReadBuffer[0] + _adcChannel1ValueOffset;
            var v2 = _adcReadBuffer[1];
            if (v1 < adcAbsoluteMinValue || v1 > adcAbsoluteMaxValue || v2 < adcAbsoluteMinValue || v2 > adcAbsoluteMaxValue)
            {
                return FaultyReadValue;
            }

            // If the difference between sensor 1 and 2 is greater than maxChannelsOffset, read is dirty.
            // Sensors should be calibrated
            if (Math.Abs(v1 - v2) > maxChannelsOffset)
            {
                return DirtyReadValue;
            }

            var averageActual = (v1 + v2) / 2;

            if (!scaledRange)
            {
                return averageActual;
            }

            // Cap read to calibrated min/max range
            averageActual = MathExtensions.Clamp(_adcRangeMinValue, averageActual, _adcRangeMaxValue);

            // Calculate scale factor. E.g. 1000 becomes 0.5 (1000 * 0.0005)
            var scaleFactor = (averageActual - _adcRangeMinValue) * _adcRangeFactor;

            // If scale reversed, invert scale factor
            scaleFactor = _reverseScale ? 1 - scaleFactor : scaleFactor;

            return (int)Math.Round(_adcReadScale * scaleFactor);
        }

        // Keep a constant read of the sensor to avoid drift
        private void ReadRunner()
        {
            BurnInSensor();
            if (_channelMode == ChannelMode.Dual)
            {
                CalibrateDualChannelOffsetsInternal();
            }

            IsInitialized = true;

            // Read the sensor(s)
            while (true)
            {
                if (_calibrationRequested)
                {
                    CalibrateDualChannelOffsetsInternal();
                    _calibrationRequested = false;
                }

                _adcReadBuffer[0] = _channels[0].ReadValue();
                if (_channelMode == ChannelMode.Dual)
                {
                    _adcReadBuffer[1] = _channels[1].ReadValue();
                }

                Thread.Sleep(SensorReadInterval);
            }
        }

        private void BurnInSensor()
        {
            const int burnInCycles = 100;

            // Make a few reads to stabilize sensors
            for (var i = 0; i < burnInCycles; i++)
            {
                _ = _channels[0].ReadValue();
                if (_channelMode == ChannelMode.Dual)
                {
                    _ = _channels[1].ReadValue();
                }

                Thread.Sleep(SensorReadInterval);
            }
        }

        private void CalibrateDualChannelOffsetsInternal()
        {
            const int calibrationSampleCount = 10;
            const int trimCalibrationExtremeCounts = 2; // Remove min/max extremes from calibration

            // Create buffers for the samples
            var samples = new int[2][]
            {
                new int[calibrationSampleCount],
                new int[calibrationSampleCount]
            };

            // Read sensors for each channel n times
            for (var sampleIndex = 0; sampleIndex < calibrationSampleCount; sampleIndex++)
            {
                samples[0][sampleIndex] = _channels[0].ReadValue();
                samples[1][sampleIndex] = _channels[1].ReadValue();
                Thread.Sleep(SensorReadInterval);
            }

            // Remove extremes and calculate the average value for each channel
            var channel1Avg = samples[0]
                .SortArray()
                .TrimArray(trimCalibrationExtremeCounts)
                .Average();
            var channel2Avg = samples[1]
                .SortArray()
                .TrimArray(trimCalibrationExtremeCounts)
                .Average();

            // To make channel values as equal as possible we calculate channel offsets 
            // to be added/subtracted to the actual read values.
            _adcChannel1ValueOffset = channel2Avg - channel1Avg;
        }

        private enum ChannelMode
        {
            Single,
            Dual
        }
    }
}
