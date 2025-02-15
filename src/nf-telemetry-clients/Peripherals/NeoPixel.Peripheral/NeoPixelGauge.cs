using CCSWE.nanoFramework.NeoPixel;
using CCSWE.nanoFramework.NeoPixel.Drivers;
using System.Drawing;
using System.Threading;

// https://docid81hrs3j1.cloudfront.net/medialibrary/2018/10/WS2812B_V1.4_EN_18090714224701.pdf
// Enable LDO2 on UM FeatherS3
//var gpioController = new GpioController();
//gpioController.OpenPin(39, PinMode.Output).Write(PinValue.High);

namespace NeoPixel.Peripheral
{
    public class NeoPixelGauge
    {
        private readonly Color[] _colors;
        private readonly NeoPixelStrip _pixels;

        public NeoPixelGauge(ushort pixelsCount, Color[] gaugeColors, byte pin)
        {
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
        }

        public void DemoRun()
        {
            var delay = 500;

            while (true)
            {
                _pixels.Clear();
                _pixels.Update();

                for (var i = 0; i < _pixels.Count; i++)
                {
                    _pixels.SetLed(i, _colors[i]);
                    _pixels.Update();
                    Thread.Sleep(10);
                }

                Thread.Sleep(delay * 5);
            }
        }
    }
}
