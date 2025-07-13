using RipTide.Nfirmware.Components.Common;
using System;
using System.Device.Adc;
using System.Device.Gpio;
using System.Threading;
using TelemetryStash.Ds18b20Sensor;

namespace RipTide.Nfirmware.Components
{
    internal class TempMonitor : Component
    {
        private static readonly TimeSpan ReadInterval = TimeSpan.FromSeconds(1);
        private readonly Ds18b20SensorReader _ds18B20Sensors;
        TemperatureValue[] _currentReadings;


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

            MapToCurrentReading(_ds18B20Sensors.Readings);
            OnTemperatureChanged?.Invoke(_currentReadings);

            var waitInterval = (int)ReadInterval.TotalMilliseconds * 2;
            while (true)
            {
                Thread.Sleep(waitInterval);

                for (var i = 0; i < _currentReadings.Length; i++)
                {
                    if (_currentReadings[i].Temperature != _ds18B20Sensors.Readings[i].Temperature)
                    {
                        MapToCurrentReading(_ds18B20Sensors.Readings);
                        OnTemperatureChanged?.Invoke(_currentReadings);
                        break;
                    }
                }
            }
        }

        public delegate void TemperatureChanged(TemperatureValue[] temperatureValues);
        public event TemperatureChanged OnTemperatureChanged;

        private void MapToCurrentReading(Ds18b20SensorReading[] ds18B20SensorReadings)
        {
            if (_currentReadings == default)
            {
                _currentReadings = new TemperatureValue[ds18B20SensorReadings.Length];
            }

            for (var i = 0; i < ds18B20SensorReadings.Length; i++)
            {
                var reading = ds18B20SensorReadings[i];
                _currentReadings[i] = new TemperatureValue(reading.Name, reading.Temperature);
            }
        }
    }

    public struct TemperatureValue
    {
        public string Name { get; set; }
        public double Temperature { get; set; }

        public TemperatureValue(string name, double temperature)
        {
            Name = name;
            Temperature = temperature;
        }
    }
}
