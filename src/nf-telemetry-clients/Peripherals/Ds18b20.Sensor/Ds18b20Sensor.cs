using Iot.Device.Ds18b20;
using nanoFramework.Device.OneWire;
using nanoFramework.Hardware.Esp32;
using System;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.Shared;

namespace TelemetryStash.Ds18b20Sensor
{
    public class Ds18b20Sensor
    {
        private Timer _notificationTimer;

        private readonly string[] _tags;

        private Ds18b20 _ds18b20;

        public Ds18b20Sensor(Ds18b20SensorSettings settings, string[] tags)
        {
            Configuration.SetPinFunction(settings.RxPin, settings.RxComPort);
            Configuration.SetPinFunction(settings.TxPin, settings.TxComPort);

            _tags = tags;
        }

        /// <summary>
        /// Init sensor and start notification timer.
        /// </summary>
        public void Start(TimeSpan notificationInterval)
        {
            if (_ds18b20 == null)
            {
                InitSensor();
            }

            _notificationTimer ??= new Timer(OnNotificationEvent, null, notificationInterval, notificationInterval);
        }

        public void Stop()
        {
            _notificationTimer?.Dispose();
            _notificationTimer = null;
        }

        public bool TryReadTemperature(out double temperature)
        {
            ArgumentNullException.ThrowIfNull(_ds18b20, $"{nameof(Ds18b20Sensor)} must be initialized before reading");

            lock (_ds18b20)
            {
                if (!_ds18b20.TryReadTemperature(out var temp))
                {
                    Debug.WriteLine($"Failed reading {nameof(Ds18b20Sensor)}");
                    temperature = double.NaN;
                    return false;
                }

                temperature = temp.DegreesCelsius;
                return true;
            }
        }

        public bool TryReadTemperature(out RegisterSet registerSet)
        {
            if (TryReadTemperature(out double temperature))
            {
                registerSet = new RegisterSet
                {
                    Identifier = "Ds18b20",
                    Tags = _tags,
                    Registers = new Register[]
                    {
                        new ("Temp", temperature, DecimalPrecision.One)
                    }
                };
                return true;
            }

            registerSet = null;
            return false;
        }

        public void InitSensor()
        {
            var host = new OneWireHost();
            _ds18b20 = new Ds18b20(host, null, false, TemperatureResolution.Low)
            {
                IsAlarmSearchCommandEnabled = false
            };

            if (!_ds18b20.Initialize())
            {
                throw new Exception("Failed to initialize DS18B20 sensor.");
            }
        }

        private void OnNotificationEvent(object state)
        {
            if (TryReadTemperature(out RegisterSet registerSet))
            {
                DataReceived?.Invoke(registerSet);
            }
        }

        public delegate void DataReceivedEventHandler(RegisterSet registerSet);
        public event DataReceivedEventHandler DataReceived;
    }
}
