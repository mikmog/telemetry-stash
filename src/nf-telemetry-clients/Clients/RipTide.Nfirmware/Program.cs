using Iot.Device.Button;
using NeoPixel.Peripheral;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using TelemetryStash.AdcSensor;
using TelemetryStash.Ds18b20Sensor;

namespace RipTide.Nfirmware
{
    public class Program
    {
        public static void Main()
        {
            var buttonPressed = false;
            var button = new GpioButton(buttonPin: 8, debounceTime: TimeSpan.FromMilliseconds(50))
            {
                IsDoublePressEnabled = false,
                IsHoldingEnabled = false
            };
            button.ButtonDown += (sender, e) => { buttonPressed = true; };

            Thread.Sleep(1000);
            if (buttonPressed)
            {
                Debug.WriteLine("Button pressed. Sleeping one minute");
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }

            //var ds18b20 = new Ds18b20Sensor();
            //ds18b20.RunDemo();

            //var neo = new NeoPixelGauge(pixelsCount: 45, new[] { Color.Green, Color.Yellow, Color.Red }, pin: 11);
            //neo.DemoRun();

            var ss49e = new Ss49eSensor();
            ss49e.RunDemo();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
