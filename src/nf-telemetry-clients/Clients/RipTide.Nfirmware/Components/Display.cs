using nanoFramework.UI;
using RipTide.Nfirmware.Assets;
using RipTide.Nfirmware.Components.Common;
using System;
using System.Device.Gpio;
using System.Drawing;
using System.Threading;
using TelemetryStash.IliDisplay;
using TelemetryStash.Shared;

namespace RipTide.Nfirmware.Components
{
    public class Display : Component
    {
        private int _thrust = -1;
        private Screen _screen;
        private static Bitmap _logo = null;
        private readonly Ili9488Display _ili9488Display = new();

        public Display(GpioController gpioController, ErrorHandler errorHandler) : base(gpioController, errorHandler) { }

        public override void Initialize(AppSettings appSettings)
        {
            _ili9488Display.Initialize(appSettings.IliDisplay);

            var splash = new FileReader().ReadFile(Asset.RipTideSplash, $"Error loading {Asset.RipTideSplash}");
            _logo = new Bitmap(splash, Bitmap.BitmapImageType.Jpeg);

            Start(Runner);
        }

        public void SetScreen(Screen screen)
        {
            _screen = screen;
        }

        public void SetText(string text)
        {
            // TODO
            _ili9488Display.Text(text, Color.White, new System.Drawing.Point(50, 100));
        }

        public void Fade(double from, double to, TimeSpan duration)
        {
            _ili9488Display.Fade(from, to, duration);
        }

        public void SetThrust(int thrust)
        {
            _thrust = thrust;
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
            var currentThrust = _thrust;

            while (true)
            {
                if (currentScreen != _screen)
                {
                    currentScreen = _screen;
                    SetScreen(_ili9488Display, currentScreen);
                }

                if (currentThrust != _thrust)
                {
                    currentThrust = _thrust;
                    _ili9488Display.Text(currentThrust.ToString(), Color.White, new System.Drawing.Point(240, 160));
                }

                Thread.Sleep(10);
            }
        }
    }

    public enum Screen
    {
        Empty,
        Demo,
        Splash,
        Dash
    }
}
