using nanoFramework.Hardware.Esp32;
using nanoFramework.UI;
using nanoFramework.UI.GraphicDrivers;
using System.Device.Pwm;
using System.Drawing;
using System.Threading;

namespace TelemetryStash.IliDisplay
{
	public class Ili9341Display
    {
        public void RunDemo()
        {
            int backLightPin = 5;
            int chipSelect = 6;
            int dataCommand = 7;
            int reset = 15;

            // MOSI => FSPID
            // MISO => FSPIQ
            // SCK => FSPICLK
            Configuration.SetPinFunction(13, DeviceFunction.SPI1_MISO);
            Configuration.SetPinFunction(11, DeviceFunction.SPI1_MOSI);
            Configuration.SetPinFunction(12, DeviceFunction.SPI1_CLOCK);

            var spi = new SpiConfiguration(1, chipselect: chipSelect, dataCommand: dataCommand, reset: reset, backLight: backLightPin);
            var driver = Ili9341.GraphicDriver;
            DisplayControl.Initialize(spi, new ScreenConfiguration(0, 0, 320, 240, driver), 2*1024*1024);
            
            Configuration.SetPinFunction(backLightPin, DeviceFunction.PWM1);
            var pwm = PwmChannel.CreateFromPin(backLightPin, 500);
            pwm.DutyCycle = 0;
            pwm.Start();
            for (double i = 0; i <= 0.8; i += 0.01)
            {
                pwm.DutyCycle = i;
                Thread.Sleep(10);
            }

            Thread.Sleep(1000);

            var fullScreen = DisplayControl.FullScreen;
            fullScreen.DrawRectangle(0, 0, DisplayControl.ScreenWidth, DisplayControl.ScreenHeight, 1, Color.Red);
            fullScreen.Flush();

            Thread.Sleep(1000);
            fullScreen.Clear();
            fullScreen.Flush();

            Thread.Sleep(100);

            fullScreen.DrawLine(Color.Red, 20, 50, 50, 200, 200);
            fullScreen.Flush();

            Thread.Sleep(1000);

            for (ushort i = 0; i < 300; i++)
            {
                fullScreen.SetPixel(i, 150, Color.Blue);
                fullScreen.Flush();
            }

            fullScreen.DrawEllipse(Color.SeaShell, 50, 50, 50, 50);
            fullScreen.Flush();

            Thread.Sleep(1000);

            for (double i = 0.8; i >= 0.2; i -= 0.01)
            {
                pwm.DutyCycle = i;
                Thread.Sleep(10);
            }

            Thread.Sleep(100000);
        }
    }
}
