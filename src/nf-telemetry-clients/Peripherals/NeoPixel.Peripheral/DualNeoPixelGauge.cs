using CCSWE.nanoFramework.NeoPixel;
using CCSWE.nanoFramework.NeoPixel.Drivers;
using System;
using System.Drawing;
using System.Threading;

// https://docid81hrs3j1.cloudfront.net/medialibrary/2018/10/WS2812B_V1.4_EN_18090714224701.pdf
// https://learn.adafruit.com/adafruit-neopixel-uberguide/best-practices

namespace NeoPixel.Peripheral
{
    public class DualNeoPixelGauge
    {
        private int _pixelsCount;
        private readonly Color[] _colors;
        private readonly NeoPixelStrip _pixels;
        private readonly Thread _gaugeThread;

        private int _currentPosition;
        private int _requestedPosition;

        public DualNeoPixelGauge(ushort pixelsCount, Color[] gaugeColors, byte pin)
        {
            _pixelsCount = pixelsCount;
            _pixels = new NeoPixelStrip(pin, (ushort)(pixelsCount + pixelsCount), new Ws2812B());
            _colors = new Color[pixelsCount];

            var partitionSize = pixelsCount / gaugeColors.Length;
            var remaining = pixelsCount % gaugeColors.Length;
            var brightnessScale = 1f / pixelsCount;

            var index = 0;
            for (var i = 0; i < gaugeColors.Length; i++)
            {
                var partitionLength = partitionSize;
                if (i == 0)
                {
                    partitionLength += remaining;
                }

                for (var j = 0; j < partitionLength; j++)
                {
                    var brightness = (index + 1) * brightnessScale;
                    var color = ColorConverter.ScaleBrightness(gaugeColors[i], brightness);
                    _colors[index++] = color;
                }
            }

            _gaugeThread = new Thread(SetPosition);
        }

        public void Initialize()
        {
            _gaugeThread.Start();
            for (var i = 0; i < _pixelsCount; i++)
            {
                _pixels.SetLed(i, _colors[i]);
                _pixels.SetLed(i + _pixelsCount, _colors[i]);
                _pixels.Update();
                Thread.Sleep(1);
            }

            for (var i = 0; i < _pixelsCount; i++)
            {
                _pixels.SetLed(i, Color.Black);
                _pixels.SetLed(i + _pixelsCount, Color.Black);
                _pixels.Update();
                Thread.Sleep(1);
            }
        }

        public void SetPosition(int position)
        {
            _requestedPosition = Math.Min(position, _colors.Length - 1);
        }

        private void SetPosition()
        {
            while (true)
            {
                if (_currentPosition == _requestedPosition)
                {
                    Thread.Sleep(10);
                    continue;
                }

                var requestedPosition = _requestedPosition;

                // Going up
                while (_currentPosition < requestedPosition)
                {
                    var color = _colors[++_currentPosition];
                    _pixels.SetLed(_currentPosition, color);
                    _pixels.SetLed(_currentPosition + _pixelsCount, color);
                    _pixels.Update();

                    Thread.Sleep(10);

                    // Requested position has gone up. Continue
                    if (requestedPosition > _requestedPosition)
                    {
                        requestedPosition = _requestedPosition;
                    }
                    else
                    {
                        // Requested position reached, or has gone down.
                        break;
                    }
                }

                // going down
                while (_currentPosition > requestedPosition)
                {
                    _pixels.SetLed(_currentPosition, Color.Black);
                    _pixels.SetLed(_currentPosition + _pixelsCount, Color.Black);
                    _currentPosition--;
                    _pixels.Update();
                    Thread.Sleep(10);

                    // Requested position has gone down. Continue
                    if (requestedPosition < _requestedPosition)
                    {
                        requestedPosition = _requestedPosition;
                    }
                    else
                    {
                        // Requested position reached, or has gone up.
                        break;
                    }
                }
            }
        }
    }
}
