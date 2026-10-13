using CCSWE.nanoFramework.NeoPixel;
using CCSWE.nanoFramework.NeoPixel.Drivers;
using System;
using System.Drawing;
using System.Threading;

// https://docid81hrs3j1.cloudfront.net/medialibrary/2018/10/WS2812B_V1.4_EN_18090714224701.pdf
// https://learn.adafruit.com/adafruit-neopixel-uberguide/best-practices

namespace NeoPixel.Peripheral
{
    public class MirroredNeoPixelGauge
    {
        private readonly int _pixelsCount;
        private readonly Color[] _pixelColors;
        private readonly NeoPixelStrip _pixels;

        private int _currentPosition;

        public MirroredNeoPixelGauge(ushort pixelsCount, Color[] gaugeColors, byte pin, float maxBrightness = 0.3f)
        {
            _pixelsCount = pixelsCount;
            _pixels = new NeoPixelStrip(pin, (ushort)(pixelsCount * 2), new Ws2812B());
            _pixelColors = new Color[pixelsCount];

            var brightnessScale = maxBrightness / pixelsCount;

            for (var i = 0; i < pixelsCount; i++)
            {
                var brightness = i * brightnessScale;

                var colorIndex = (int)(((float)i / pixelsCount) * gaugeColors.Length);
                var color = ColorConverter.ScaleBrightness(gaugeColors[colorIndex], brightness);
                _pixelColors[i] = color;
            }
        }

        public void Initialize()
        {
            LightUp();
        }

        public void LightUp()
        {
            _pixels.Fill(Color.Black);
            for (var i = 0; i < _pixelsCount; i++)
            {
                var color = _pixelColors[i];
                _pixels.SetLed(i, color);
                _pixels.SetLed(i + _pixelsCount, color);
                _pixels.Update();
                Thread.Sleep(10);
            }

            for (var i = 0; i < _pixelsCount; i++)
            {
                _pixels.SetLed(i, Color.Black);
                _pixels.SetLed(i + _pixelsCount, Color.Black);
                _pixels.Update();
                Thread.Sleep(10);
            }
        }

        public void TravelToPosition(int position)
        {
            var requestedPosition = Math.Min(position, _pixelsCount);

            if (_currentPosition == requestedPosition)
            {
                return;
            }

            // Going up
            while (_currentPosition < requestedPosition)
            {
                var color = _pixelColors[++_currentPosition];
                _pixels.SetLed(_currentPosition, color);
                _pixels.SetLed(_currentPosition + _pixelsCount, color);
                _pixels.Update();

                Thread.Sleep(5);
            }

            // Going down
            while (_currentPosition > requestedPosition)
            {
                _pixels.SetLed(_currentPosition, Color.Black);
                _pixels.SetLed(_currentPosition + _pixelsCount, Color.Black);
                _currentPosition--;
                _pixels.Update();
                Thread.Sleep(5);
            }
        }
    }
}
