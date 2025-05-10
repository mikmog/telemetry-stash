using System;
using System.Device.Adc;
using System.Threading;
using TelemetryStash.Shared;

namespace TelemetryStash.AdcSensor
{
    public class Ss49eHallSensor
    {
        private AdcChannel[] _channels;
        private int[] _adcChannelOffset;
        private int[] _adcReadBuffer;
        private int[] _adcChannelReads;

        private int _adcReadScale;
        private double _adcReadScaleRange;
        private int _requiredSuccessfulReads;
        private bool _reverseScale;

        // Important.Magnet north pole facing label side of sensor when running 5v
        // Below sensor range when driven by 5V
        const int adcReadMinValue = 1000;
        const int adcReadMaxValue = 3000;

        // One sensor per channel. Add more sensors to get a more stable reading
        public void Initialize(int[] channels, int adcReadScale, bool reverseScale, int requiredSuccessfulReads = 12)
        {
            _channels = new AdcChannel[channels.Length];
            _adcChannelOffset = new int[channels.Length];
            _adcReadBuffer = new int[channels.Length];
            _adcChannelReads = new int[requiredSuccessfulReads];

            _adcReadScale = adcReadScale;

            // If effective sensor range is 0 to 2000, with a read scale of 50, the range is 40
            _adcReadScaleRange = (adcReadMaxValue - adcReadMinValue) / (double)adcReadScale;
            _requiredSuccessfulReads = requiredSuccessfulReads;
            _reverseScale = reverseScale;

            var adc = new AdcController();
            for (int i = 0; i < channels.Length; i++)
            {
                _channels[i] = adc.OpenChannel(channels[i]);
            }
        }

        /// <summary>
        /// Sample the sensor until required successful reads and calculate average
        /// </summary>
        /// <returns>Scaled range. -1 if the read is dirty</returns>
        public int Read()
        {
            var dirtyReadCount = 0;
            var successfulReadCount = 0;
            while (successfulReadCount < _requiredSuccessfulReads)
            {
                // Read all channels
                for (var i = 0; i < _channels.Length; i++)
                {
                    _adcReadBuffer[i] = _channels[i].ReadValue() + _adcChannelOffset[i];
                }

                var dirtyRead = false;
                for (var i = 0; i < _adcReadBuffer.Length && i + 1 < _adcReadBuffer.Length; i++)
                {
                    // If the difference between sensor 1 and sensor 2 is greater than read scale, the read is considered dirty
                    if (Math.Abs(_adcReadBuffer[i] - _adcReadBuffer[i + 1]) > _adcReadScaleRange)
                    {
                        dirtyRead = true;
                        dirtyReadCount++;
                        break;
                    }
                }

                if (dirtyRead)
                {
                    if (dirtyReadCount > 100)
                    {
                        return -1;
                    }

                    continue;
                }

                _adcChannelReads[successfulReadCount++] = _adcReadBuffer.Average();
            }

            var average = _adcChannelReads.Average();
            var normalized = average - adcReadMinValue; // Normalize 1000 -> 3000 to 0 -> 2000
            var scaled = Math.Round(normalized / _adcReadScaleRange);
            scaled = Math.Clamp(scaled, 0, _adcReadScale);
            return _reverseScale ? _adcReadScale - (int)scaled : (int)scaled;
        }

        public void CalibrateAdcChannelOffsets(int sampleCount = 10)
        {
            // If there is only one channel, there is no need to calibrate
            if (_channels.Length <= 1)
            {
                return;
            }

            var samples = new int[_channels.Length * sampleCount];

            for (var i = 0; i < sampleCount; i++)
            {
                for (var j = 0; j < _channels.Length; j++)
                {
                    // [c1, c1, c1, c2, c2, c2, c3, c3, c3, ...]
                    var window = (j * sampleCount);
                    samples[window + i] = _channels[j].ReadValue();
                }
                Thread.Sleep(1);
            }

            var samplingsAverage = new int[_channels.Length];
            for (var i = 0; i < _channels.Length; i++)
            {
                var samplingTotal = 0;
                int extremeMax = 0;
                int extremeMin = int.MaxValue;
                var window = i * sampleCount;
                for (var j = 0; j < sampleCount; j++)
                {
                    var value = samples[window + j];
                    extremeMax = Math.Max(extremeMax, value);
                    extremeMin = Math.Min(extremeMin, value);

                    samplingTotal += value;
                }

                // Remove min/max extreme values
                samplingsAverage[i] = (samplingTotal - extremeMin - extremeMax) / (sampleCount - 2);
            }

            for (var i = 1; i < samplingsAverage.Length; i++)
            {
                _adcChannelOffset[i - 1] = samplingsAverage[i] - samplingsAverage[i - 1];
            }
        }
    }
}
