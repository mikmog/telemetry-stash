using CCSWE.nanoFramework.NeoPixel;
using CCSWE.nanoFramework.NeoPixel.Drivers;
using System;
using System.Drawing;
using System.Threading;

// https://docid81hrs3j1.cloudfront.net/medialibrary/2018/10/WS2812B_V1.4_EN_18090714224701.pdf

namespace NeoPixel.Peripheral
{
    public class NeoPixelGauge
    {
        private readonly Color[] _colors;
        private readonly NeoPixelStrip _pixels;
        private readonly Thread _gaugeThread;
        
        private volatile int _currentPosition;
        private volatile int _requestedPosition;

        public NeoPixelGauge(ushort pixelsCount, Color[] gaugeColors, byte pin)
        {
            // TODO: Decrease pulse transfer times
            _pixels = new NeoPixelStrip(pin, pixelsCount, new Ws2812B());
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
            for (var i = 0; i < _pixels.Count; i++)
            {
                _pixels.SetLed(i, _colors[i]);
                _pixels.Update();
                Thread.Sleep(1);
            }
            
            for (var i = 0; i < _pixels.Count; i++)
            {
                _pixels.SetLed(i, Color.Black);
                _pixels.Update();
                Thread.Sleep(1);
            }
        }

        public void SetPosition(int position)
        {
            _requestedPosition = Math.Min(position, _colors.Length-1);
        }

        private void SetPosition()
        {
            while (true)
            {
                if (_currentPosition == _requestedPosition)
                {
                    Thread.Sleep(1);
                    continue;
                }

                var currentPosition = _currentPosition;
                var requestedPosition = _requestedPosition;

                var goingUp = currentPosition < requestedPosition;
                if (goingUp)
                {
                    while (currentPosition < requestedPosition)
                    {
                        // Increase the speed of the gauge if the requested position is far away
                        var increments = (requestedPosition - currentPosition) switch
                        {
                            > 10 => 3,
                            > 5 => 2,
                            _ => 1,
                        };

                        _pixels.SetLed(++currentPosition, _colors[currentPosition]);
                        if (currentPosition % increments == 0 || currentPosition >= requestedPosition || requestedPosition != _requestedPosition)
                        {
                            _pixels.Update();
                            Thread.Sleep(0);
                        }
                        if (requestedPosition != _requestedPosition)
                        {
                            break;
                        }
                    }
                }

                // going down
                else
                {
                    while (currentPosition > requestedPosition)
                    {
                        var increments = (currentPosition - requestedPosition) switch
                        {
                            > 5 => 2,
                            _ => 1,
                        };

                        _pixels.SetLed(currentPosition--, Color.Black);
                        if (currentPosition % increments == 0 || requestedPosition == currentPosition || requestedPosition != _requestedPosition)
                        { 
                            _pixels.Update();
                            Thread.Sleep(0);
                        }

                        if (requestedPosition != _requestedPosition)
                        {
                            break;
                        }
                    }
                }

                _currentPosition = currentPosition;
            }
        }
    }
}
