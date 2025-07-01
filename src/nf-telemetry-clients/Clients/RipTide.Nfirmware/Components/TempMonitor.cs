using RipTide.Nfirmware.Components.Common;
using System;
using System.Device.Adc;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.Ds18b20Sensor;
using TelemetryStash.Shared;

namespace RipTide.Nfirmware.Components
{
    internal class TempMonitor : Component
    {
        private static readonly TimeSpan ReadInterval = TimeSpan.FromSeconds(1);
        private readonly Ds18b20SensorReader _ds18B20Sensors;

        public TempMonitor(AdcController adc, GpioController gpio, ErrorHandler errorHandler) : base(adc, gpio, errorHandler)
        {
            _ds18B20Sensors = new Ds18b20SensorReader(ReadInterval);
        }

        public override void Initialize(AppSettings appSettings)
        {
            _ds18B20Sensors.Initialize(appSettings.Ds18b20);

            Start(Runner);
        }

        private void Runner()
        {
            while (!_ds18B20Sensors.Initialized)
            {
                Thread.Sleep(ReadInterval);
            }

            var timeStamp = DateTime.MinValue;

            while (true)
            {
                while (_ds18B20Sensors.TimeStamp == timeStamp)
                {
                    Thread.Sleep(ReadInterval);
                }

                timeStamp = _ds18B20Sensors.TimeStamp;

                // TODO: Expose readings to the system
                foreach (var reading in _ds18B20Sensors.Readings)
                {
                    if (reading.IsValid)
                    {
                        Debug.WriteLine($"DS18B20 [{timeStamp.TimeOfDay}] {reading.Name} - Temperature: {Round.ToOneDecimalString(reading.Temperature)}°C");
                    }
                    else
                    {
                        Debug.WriteLine($"DS18B20 Sensor {reading.Name} - Invalid reading");
                    }
                }
                Debug.WriteLine("");
            }
        }
    }
}
