using NeoPixel.Peripheral;
using System;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.AdcSensor;
using TelemetryStash.BuzzerPeripheral;
using TelemetryStash.Ds18b20Sensor;
using TelemetryStash.IliDisplay;
using TelemetryStash.IO.Peripherals;
using TelemetryStash.MpuxxxxGyro.Sensor;
using TelemetryStash.Peripherals.SdCard;


namespace RipTide.Nfirmware
{
    public class Program
    {
        public static void Main()
        {
            Thread.Sleep(2000);

            //var buttonPressed = false;

            //var button = new Button(8);
            //button.OnButtonDown(() =>  buttonPressed = true); 

            //if (buttonPressed)
            //{
            //    Debug.WriteLine("Button pressed. Sleeping one minute");
            //    Thread.Sleep(TimeSpan.FromMinutes(1));
            //}

            //var ds18b20 = new Ds18b20Sensor();
            //ds18b20.RunDemo();

            //var neo = new NeoPixelGauge(pixelsCount: 45, new[] { Color.Green, Color.Yellow, Color.Red }, pin: 11);
            //neo.DemoRun();

            //var ss49e = new Ss49eHallSensor();
            //ss49e.RunDemo();

            //var xdb401 = new Xdb401PressureSensor();
            //xdb401.RunDemo();

            //var buzzer = new PiezoBuzzer();
            //buzzer.RunDemo();

            //var pwm = new Pwm();
            //pwm.RunDemo();

            //var gyro = new Mpu6050Gyro();
            //gyro.RunDemo();

            //var display = new Ili9341Display();
            //display.RunDemo();

            var microSd = new MicroSdCard();
            microSd.RunDemo();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
