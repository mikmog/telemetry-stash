using Iot.Device.Ds18b20;
using nanoFramework.Device.OneWire;
using nanoFramework.Hardware.Esp32;
using System;
using System.Threading;

namespace TelemetryStash.Ds18b20Sensor
{
    public class Ds18b20Sensor
    {
        public Ds18b20Sensor()
        {
            Configuration.SetPinFunction(18, DeviceFunction.COM2_RX);
            Configuration.SetPinFunction(17, DeviceFunction.COM2_TX);
        }

        public void RunDemo()
        {
            var oneWire = new OneWireHost();

            var ds18b20 = new Ds18b20(oneWire, null, false, TemperatureResolution.VeryHigh)
            {
                IsAlarmSearchCommandEnabled = false
            };

            if (ds18b20.Initialize())
            {
                Console.WriteLine($"Is sensor parasite powered?:{ds18b20.IsParasitePowered}");
                string devAddrStr = "";
                foreach (var addrByte in ds18b20.Address)
                {
                    devAddrStr += addrByte.ToString("X2");
                }

                Console.WriteLine($"Sensor address:{devAddrStr}");

                while (true)
                {
                    if (!ds18b20.TryReadTemperature(out var currentTemperature))
                    {
                        Console.WriteLine("Can't read!");
                    }
                    else
                    {
                        Console.WriteLine($"Temperature: {currentTemperature.DegreesCelsius.ToString("F")}\u00B0C");
                    }

                    Thread.Sleep(5000);
                }
            }

            oneWire.Dispose();
        }
    }
}
