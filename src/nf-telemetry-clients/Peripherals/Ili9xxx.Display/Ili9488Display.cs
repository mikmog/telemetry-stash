using nanoFramework.Hardware.Esp32;
using nanoFramework.UI;
using nanoFramework.UI.GraphicDrivers;
using System;
using System.Collections;
using System.Device.Pwm;
using System.Drawing;
using System.Threading;
using TelemetryStash.Shared;

namespace TelemetryStash.IliDisplay
{
    /*
        Top left: 0,0
        Top right: 480,0
        Bottom left: 0,320
        Bottom right: 480,320
     */

    public class Ili9488Display
    {
        private PwmChannel _backlightPwm;
        private nanoFramework.UI.Font _defaultFont;

        public Bitmap Screen { get; private set; }


        public void Initialize(IliDisplaySettings settings)
        {
            // MOSI => FSPID => SDI
            // MISO => FSPIQ => SDO
            // SCK => FSPICLK
            Configuration.SetPinFunction(settings.SpiMisoPin, DeviceFunction.SPI1_MISO);
            Configuration.SetPinFunction(settings.SpiMosiPin, DeviceFunction.SPI1_MOSI);
            Configuration.SetPinFunction(settings.SpiClockPin, DeviceFunction.SPI1_CLOCK);

            var spi = new SpiConfiguration(spiBus: 1, settings.ChipSelectPin, settings.DataCommandPin, settings.ResetPin, settings.BackLightPin);
            var driver = Ili9488.GraphicDriver;

            // 480X320
            DisplayControl.Initialize(spi, new ScreenConfiguration(0, 0, 480, 320, driver), 480 * 320 * 3);
            Thread.Sleep(1000); // Give som breathing room to initialize

            _defaultFont = Resource.GetFont(Resource.FontResources.Consolas16);

            Configuration.SetPinFunction(settings.BackLightPin, DeviceFunction.PWM1);
            _backlightPwm = PwmChannel.CreateFromPin(settings.BackLightPin, dutyCyclePercentage: 1);

            Screen = DisplayControl.FullScreen;
        }

        /// <summary>
        /// Intensity as a value between 0.0 and 1.0.
        /// </summary>
        public void Fade(double from, double to, TimeSpan duration)
        {
            const double step = 0.01;

            var max = from > to ? from : to;
            var min = from < to ? from : to;
            var steps = (max - min) / step;
            var delay = (int)(duration.TotalMilliseconds / steps);

            _backlightPwm.DutyCycle = from;
            _backlightPwm.Start();

            var goingUp = from < to;
            for (; goingUp && from <= to; from += step)
            {
                _backlightPwm.DutyCycle = from;
                Thread.Sleep(delay);
            }

            for (; !goingUp && from >= to; from -= step)
            {
                _backlightPwm.DutyCycle = from;
                Thread.Sleep(delay);
            }
        }

        public void Clear(Color color)
        {
            Screen.Clear();
            Screen.DrawRectangle(
                colorOutline: color,
                thicknessOutline: 0,
                x: 0,
                y: 0,
                width: 480,
                height: 320,
                xCornerRadius: 0,
                yCornerRadius: 0,
                colorGradientStart: color,
                xGradientStart: 0,
                yGradientStart: 0,
                colorGradientEnd: color,
                xGradientEnd: 480,
                yGradientEnd: 320,
                opacity: Bitmap.OpacityOpaque);

            Screen.Flush();
        }

        public void Clear(Bitmap bitmap)
        {
            Screen.Clear();
            Screen.DrawImage(
               xDst: 0,
               yDst: 0,
               bitmap: bitmap,
               xSrc: 0,
               ySrc: 0,
               width: 480,
               height: 320,
               opacity: Bitmap.OpacityOpaque);
            Screen.Flush();
        }


        private readonly Hashtable _textHistory = new();

        public void Text(string text, Color color, System.Drawing.Point point)
        {
            // Consolas16
            // Height: 19
            // Max character width: 9

            var prevLength = (int)(_textHistory[point] ?? 0);
            var currentLength = 9 * text.Length;

            _textHistory[point] = currentLength;

            var width = MathExtensions.Max(currentLength, prevLength);
            var height = 19;

            Screen.FillRectangle(point.X, point.Y, width, height, Color.Brown);
            Screen.DrawText(text, _defaultFont, color, point);
            Screen.Flush(point.X, point.Y, width, height);
        }

        public void RunDemo(Bitmap logo)
        {
            Console.WriteLine("Ili9488Display.RunDemo()");
            var start = DateTime.UtcNow;
            Screen.Clear();
            Screen.Flush();

            Thread.Sleep(500);

            Screen.DrawRectangle(Color.White, 2, 0, 0, 480, 320, 0, 0, Color.White, 0, 0, Color.Black, 480, 320, Bitmap.OpacityOpaque);
            Screen.Flush();

            Screen.DrawLine(Color.Red, 20, 50, 0, 50, 300);
            Screen.Flush();

            Screen.DrawLine(Color.Blue, 20, 100, 0, 100, 300);
            Screen.Flush();

            Screen.DrawLine(Color.Green, 20, 150, 0, 150, 300);
            Screen.Flush();

            Screen.DrawLine(Color.Yellow, 20, 200, 0, 200, 300);
            Screen.Flush();

            Thread.Sleep(500);

            for (var y = 100; y < 104; y++)
            {
                for (var x = 10; x < 440; x++)
                {
                    Screen.SetPixel(x, y, Color.Yellow);
                    Screen.Flush(x, y, 1, 1);
                }
            }

            for (var y = 110; y < 114; y++)
            {
                for (var x = 10; x < 440; x++)
                {
                    Screen.SetPixel(x, y, Color.Blue);
                    Screen.Flush(x, y, 1, 1);
                }
            }

            for (var y = 120; y < 124; y++)
            {
                for (var x = 10; x < 440; x++)
                {
                    Screen.SetPixel(x, y, Color.Red);
                    Screen.Flush(x, y, 1, 1);
                }
            }

            for (var y = 130; y < 134; y++)
            {
                for (var x = 10; x < 440; x++)
                {
                    Screen.SetPixel(x, y, Color.Green);
                    Screen.Flush(x, y, 1, 1);
                }
            }

            Thread.Sleep(2000);

            Screen.Flush();

            Screen.DrawImage(0, 0, logo, 0, 0, 480, 320);
            Screen.Flush();

            Thread.Sleep(500);

            var totalTime = DateTime.UtcNow - start;

            Screen.DrawText("0x0", _defaultFont, Color.Black, 10, 0);
            Screen.DrawText("480x0", _defaultFont, Color.Black, 420, 0);
            Screen.DrawText("480x320", _defaultFont, Color.Black, 405, 300);
            Screen.DrawText("0x320", _defaultFont, Color.Black, 10, 300);
            Screen.DrawText("Took: " + totalTime.TotalSeconds, _defaultFont, Color.WhiteSmoke, 240, 160);

            Screen.Flush();

            Thread.Sleep(3000);
        }
    }
}
