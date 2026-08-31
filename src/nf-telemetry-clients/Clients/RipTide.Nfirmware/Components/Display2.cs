using nanoFramework.UI;
using RipTide.Nfirmware.Assets;
using RipTide.Nfirmware.Components.Common;
using System;
using System.Device.Adc;
using System.Device.Gpio;
using System.Drawing;
using System.Numerics;
using System.Threading;
using TelemetryStash.IliDisplay;
using TelemetryStash.IliDisplay.GraphicDriver;
using TelemetryStash.Shared;

namespace RipTide.Nfirmware.Components
{
    public class Display2 : Component
    {
        private bool _running = false;

        private readonly Ili9488Display _ili9488Display = new();

        public Display2(AdcController adc, GpioController gpio, ErrorHandler errorHandler) : base(adc, gpio, errorHandler) { }

        public override void Initialize(AppSettings appSettings)
        {
            _ili9488Display.Initialize(appSettings.IliDisplay);
            var driver = new GraphicsDriver(_ili9488Display.Screen);

            var splash = new FileReader().ReadFile(Asset.RipTideSplash, $"Error loading {Asset.RipTideSplash}");
            var logo = new Bitmap(splash, Bitmap.BitmapImageType.Jpeg);

            var splashView = new SplashView(logo);
            driver.SetView(splashView);

            var dasboardView = new DashboardView(logo); // TODO
            driver.SetView(dasboardView);

            var font = Resource.GetFont(Resource.FontResources.Consolas16);
            var throttleText = new TextMetric(
                x: 240,
                y: 160,
                width: 100,
                height: 50,
                backgroundColor: Color.Black,
                font: font,
                fontHeight: 48,
                textColor: Color.White);

            dasboardView.UpdateMetric(throttleText, _ili9488Display.Screen, "Test 123");

            Start(Runner);
            while (_running == false)
            {
                Thread.Sleep(1);
            }
        }

        public void Fade(double from, double to, TimeSpan duration)
        {
            _ili9488Display.Fade(from, to, duration);
        }

        private void Runner()
        {
            static void SetScreen(Ili9488Display ili9488Display, Screen screen)
            {
                switch (screen)
                {
                    case Screen.Empty:
                        ili9488Display.Screen.Clear();
                        break;
                    case Screen.Demo:
                        ili9488Display.RunDemo(_logo);
                        break;
                    case Screen.Splash:
                        ili9488Display.Clear(_logo);
                        break;
                    case Screen.Dash:
                        ili9488Display.Clear(Color.Black);
                        break;
                }

                ili9488Display.Screen.Flush();
            }

            var currentScreen = _screen;
            var currentThrust = _thrustValue;
            _running = true;

            while (_running)
            {
                if (currentScreen != _screen)
                {
                    currentScreen = _screen;
                    SetScreen(_ili9488Display, currentScreen);
                }

                if (currentThrust != _thrustValue)
                {
                    currentThrust = _thrustValue;
                    _ili9488Display.Text(currentThrust.ToString(), Color.White, new System.Drawing.Point(240, 160));
                }

                Thread.Sleep(10);
            }
        }
    }

    public class Dashboard
    {
        private int _thrustValue = -1;
        private BatteryValue _batteryValue;
        private TemperatureValue[] _tempValues;
        private Vector3 _gyroValue;

        public void SetAsActive()
        {
            // TODO
        }

        public void ThrustChanged(int thrustValue)
        {
            _thrustValue = thrustValue;
        }

        public void BatteryChanged(BatteryValue batteryValue)
        {
            _batteryValue = batteryValue;
        }

        public void TemperatureChanged(TemperatureValue[] tempValues)
        {
            _tempValues = tempValues;
        }

        public void GyroChanged(Vector3 value)
        {
            _gyroValue = value;
        }
    }

    public class ThrustCalibration
    {
        public void SetAsActive()
        {
            // TODO
        }

        public void Text(string text)
        {
            // TODO
            _ili9488Display.Text(text, Color.White, new System.Drawing.Point(50, 100));
        }
    }

    public class Demo
    {
        public void SetAsActive()
        {
            // TODO
        }
    }

    public class TempBenchmark
    {
        private readonly Ili9488Display _ili9488Display;

        public TempBenchmark(Ili9488Display ili9488Display)
        {
            _ili9488Display = ili9488Display;
        }

        public void SetAsActive()
        {
            // TODO
        }

        public void RunDemo()
        {
            var logo = new Bitmap(new FileReader().ReadFile(Asset.RipTideSplash, $"Error loading {Asset.RipTideSplash}"), Bitmap.BitmapImageType.Jpeg);
            var screen = _ili9488Display.Screen;

            var start = DateTime.UtcNow;

            screen.Clear();
            screen.Flush();

            Thread.Sleep(500);

            screen.DrawRectangle(Color.White, 2, 0, 0, 480, 320, 0, 0, Color.White, 0, 0, Color.Black, 480, 320, Bitmap.OpacityOpaque);
            screen.Flush();

            screen.DrawLine(Color.Red, 20, 50, 0, 50, 300);
            screen.Flush();

            screen.DrawLine(Color.Green, 20, 100, 0, 100, 300);
            screen.Flush();

            screen.DrawLine(Color.Blue, 20, 150, 0, 150, 300);
            screen.Flush();

            screen.DrawLine(Color.Yellow, 20, 200, 0, 200, 300);
            screen.Flush();

            Thread.Sleep(500);

            for (var y = 100; y < 104; y++)
            {
                for (var x = 10; x < 440; x++)
                {
                    screen.SetPixel(x, y, Color.Yellow);
                    screen.Flush(x, y, 1, 1);
                }
            }

            for (var y = 110; y < 114; y++)
            {
                for (var x = 10; x < 440; x++)
                {
                    screen.SetPixel(x, y, Color.Blue);
                    screen.Flush(x, y, 1, 1);
                }
            }

            for (var y = 120; y < 124; y++)
            {
                for (var x = 10; x < 440; x++)
                {
                    screen.SetPixel(x, y, Color.Green);
                    screen.Flush(x, y, 1, 1);
                }
            }

            for (var y = 130; y < 134; y++)
            {
                for (var x = 10; x < 440; x++)
                {
                    screen.SetPixel(x, y, Color.Red);
                    screen.Flush(x, y, 1, 1);
                }
            }

            Thread.Sleep(2000);

            screen.Flush();

            screen.DrawImage(0, 0, logo, 0, 0, 480, 320);
            screen.Flush();

            Thread.Sleep(500);

            var totalTime = DateTime.UtcNow - start;

            var font = Resource.GetFont(Resource.FontResources.Consolas16);
            screen.DrawText("0x0", font, Color.Black, 10, 0);
            screen.DrawText("480x0", font, Color.Black, 420, 0);
            screen.DrawText("480x320", font, Color.Black, 405, 300);
            screen.DrawText("0x320", font, Color.Black, 10, 300);
            screen.DrawText("Took: " + totalTime.TotalSeconds, font, Color.WhiteSmoke, 240, 160);

            screen.Flush();

            Thread.Sleep(5000);
            logo.Dispose();
        }
    }
}
