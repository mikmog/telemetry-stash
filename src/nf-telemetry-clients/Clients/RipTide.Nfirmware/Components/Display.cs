using RipTide.Nfirmware.Components.Common;
using System;
using System.Threading;
using TelemetryStash.IliDisplay;

namespace RipTide.Nfirmware.Components
{
    public class Display : Component
    {
        private int _thrust;
        private Screen _screen;
        private readonly Ili9488Display _ili9488Display = new();

        public Display(ErrorHandler errorHandler) : base(errorHandler) { }

        public override void Initialize(AppSettings appSettings)
        {
            _ili9488Display.Initialize(appSettings.IliDisplay);

            // Add fonts

            Start(Runner);
        }

        public void SetScreen(Screen screen)
        {
            _screen = screen;
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
                        ili9488Display.RunDemo();
                        break;
                    case Screen.Splash:
                        //ili9488Display.DrawLogo();
                        break;
                    case Screen.Dash:
                        //ili9488Display.DrawInformationBar(0);
                        break;
                }

                ili9488Display.Screen.Flush();
            }

            var currentScreen = _screen;

            while (true)
            {
                if (currentScreen != _screen)
                {
                    currentScreen = _screen;
                    SetScreen(_ili9488Display, currentScreen);
                }

                Thread.Sleep(100);
                if (_screen == Screen.Demo)
                {
                    _ili9488Display.RunDemo();
                }

                if (_screen == Screen.Splash)
                {
                    _ili9488Display.DrawLogo();
                }

                if (_screen == Screen.Dash)
                {
                    _ili9488Display.DrawInformationBar(_thrust);
                }
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
