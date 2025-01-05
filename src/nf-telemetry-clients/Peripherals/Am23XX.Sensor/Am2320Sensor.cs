using Iot.Device.Am2320;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.I2c;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.Shared;

namespace TelemetryStash.Am23XX.Sensor
{
    public class Am2320Sensor
    {
        private short _burnInCycles = 120; // Sensor (a fake one?) is driftning initially. Give it 10 minutes to stabalize
        private readonly TimeSpan _readTimerInterval = TimeSpan.FromSeconds(5);
        private Timer _readTimer;

        private readonly TimeSpan _notificationInterval;
        private Timer _notifyTimer;

        private double _temperature = -1;
        private double _humidity = -1;
        private DateTime _timestamp = DateTime.MinValue;

        private readonly string[] _tags;

        private readonly double _temperatureOffset;
        private readonly double _humidityOffset;

        private Am2320 _am2320;

        public Am2320Sensor(Am23XXSensorSettings settings, TimeSpan notificationInterval, string[] tags)
        {
            Configuration.SetPinFunction(settings.DataPin, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(settings.ClockPin, DeviceFunction.I2C1_CLOCK);
            
            _temperatureOffset = settings.TemperatureOffset;
            _humidityOffset = settings.HumidityOffset;

            _notificationInterval = notificationInterval;
            _tags = tags;
        }

        public void Start()
        {
            if (_am2320 == null)
            {
                InitSensor();
            }

            // Never stop read timer. Keep a constant read interval
            _readTimer ??= new Timer(OnSensorReadvent, null, TimeSpan.FromMilliseconds(100), _readTimerInterval);
            _notifyTimer ??= new Timer(OnNotificationEvent, null, _readTimerInterval, _notificationInterval);
        }

        public void Stop()
        {
            _notifyTimer.Dispose();
            _notifyTimer = null;
        }

        public SensorReading GetLastReading()
        {
            lock (_am2320)
            {
                if (TryMapToRegisterSet(_temperature, _temperatureOffset, _humidity, _humidityOffset, _tags, out var registerSet))
                {
                    return new SensorReading
                    {
                        Timestamp = _timestamp,
                        RegisterSet = registerSet
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        private void InitSensor()
        {
            // The I2C bus ID on the MCU
            const int busId = 1;

            var i2cSettings = new I2cConnectionSettings(busId, Am2320.DefaultI2cAddress, I2cBusSpeed.StandardMode);
            _am2320 = new(I2cDevice.Create(i2cSettings));
        }

        private void OnNotificationEvent(object state)
        {
            lock (_am2320)
            {
                if (TryMapToRegisterSet(_temperature, _temperatureOffset, _humidity, _humidityOffset, _tags, out var registerSet))
                {
                    DataReceived?.Invoke(registerSet);
                }
            }
        }

        private void OnSensorReadvent(object state)
        {
            lock (_am2320)
            {
                var temp = _am2320.Temperature;
                var hum = _am2320.Humidity;

                if(_burnInCycles > 0)
                {
                    _burnInCycles--;
                    return;
                }

                _timestamp = DateTime.UtcNow;

                if (_am2320.IsLastReadSuccessful)
                {
                    _temperature = temp.DegreesCelsius;
                    _humidity = hum.Percent;
                }
                else
                {
                    Debug.WriteLine("AM23XX Read failed");
                    _temperature = -1;
                    _humidity = -1;
                }
            }
        }

        private static bool TryMapToRegisterSet(double temperature, double temperatureOffset, double humidity, double humidityOffset, string[] tags, out RegisterSet registerSet)
        {
            if(temperature == -1 || humidity == -1)
            {
                registerSet = null;
                return false;
            }

            registerSet = new RegisterSet
            {
                Identifier = "Am2320",
                Tags = tags,
                Registers = new Register[]
                {
                       new ("Temp", temperature + temperatureOffset, DecimalPrecision.One),
                       new ("TempOffset", temperatureOffset, DecimalPrecision.One),
                       new ("Hum", humidity + humidityOffset, DecimalPrecision.One),
                       new ("HumOffset", humidityOffset, DecimalPrecision.One)
                }
            };

            return true;
        }

        public delegate void DataReceivedEventHandler(RegisterSet registerSet);
        public event DataReceivedEventHandler DataReceived;

        public class SensorReading
        {
            public DateTime Timestamp { get; set; }
            public RegisterSet RegisterSet { get; set; }
        }
    }
}
