using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.ReadResult;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.I2c;
using System.Threading;
using TelemetryStash.Shared;
using UnitsNet;

namespace TelemetryStash.Bmxx80.Sensor
{
    // https://www.bosch-sensortec.com/media/boschsensortec/downloads/datasheets/bst-bme680-ds001.pdf

    public class Bme680Sensor
    {
        private Bme680 _bme680;
        private Bme680ReadResult _bme680ReadResult = null;

        private readonly TimeSpan _readTimerInterval = TimeSpan.FromSeconds(5);
        private Timer _readTimer;

        private readonly TimeSpan _notificationInterval;
        private Timer _notifyTimer;

        // Internet suggests 300 reads for gas sensor to stabilize.
        private readonly IndoorAirQuality _indoorAirQuality = new(burnInCycles: 300);

        private readonly string[] _tags;

        public Bme680Sensor(Bme680SensorSettings settings, TimeSpan notificationInterval, string[] tags)
        {
            Configuration.SetPinFunction(settings.I2cDataPin, settings.I2cData);
            Configuration.SetPinFunction(settings.I2cClockPin, settings.I2cClock);

            _notificationInterval = notificationInterval;
            _tags = tags;
        }

        public void Start()
        {
            if (_bme680 == null)
            {
                InitSensor();
            }

            // Never stop read timer. Keep a constant read interval
            _readTimer ??= new Timer(OnSensorReadvent, null, TimeSpan.FromMilliseconds(1), _readTimerInterval);
            _notifyTimer ??= new Timer(OnNotificationEvent, null, _readTimerInterval, _notificationInterval);
        }

        public void Stop()
        {
            _notifyTimer.Dispose();
            _notifyTimer = null;
        }

        private void InitSensor()
        {
            // The I2C bus ID on the MCU
            const int busId = 1;

            var i2cSettings = new I2cConnectionSettings(busId, Bme680.SecondaryI2cAddress, I2cBusSpeed.StandardMode);
            var i2cDevice = I2cDevice.Create(i2cSettings);

            _bme680 = new(i2cDevice, Temperature.FromDegreesCelsius(24.0))
            {
                TemperatureSampling = Sampling.Standard,
                PressureSampling = Sampling.Standard,
                HumiditySampling = Sampling.Standard,
            };
        }

        private void OnNotificationEvent(object state)
        {
            if (_bme680ReadResult == null)
            {
                return;
            }

            lock (_bme680)
            {
                if (TryMapToRegisterSet(_bme680ReadResult, _indoorAirQuality, _tags, out var registerSet))
                {
                    DataReceived?.Invoke(registerSet);
                }
            }
        }

        private void OnSensorReadvent(object state)
        {
            lock (_bme680)
            {
                _bme680ReadResult = _bme680.Read();
            }
        }

        private static bool TryMapToRegisterSet(Bme680ReadResult result, IndoorAirQuality indoorAirQuality, string[] tags, out RegisterSet registerSet)
        {
            var measurementsValid =
                result.TemperatureIsValid &&
                result.HumidityIsValid &&
                result.PressureIsValid &&
                result.GasResistanceIsValid;

            var iacValid = indoorAirQuality.TryCalculateIAC(result.Temperature.DegreesCelsius, result.Humidity.Value, result.GasResistance.Value, out var airQuality);

            if (measurementsValid)
            {
                registerSet = new RegisterSet
                {
                    Identifier = "BME680",
                    Tags = tags
                };

                if (iacValid)
                {
                    registerSet.Registers = new Register[] {
                        new ("Temp", result.Temperature.DegreesCelsius, DecimalPrecision.One),
                        new ("Hum", result.Humidity.Percent, DecimalPrecision.Half),
                        new ("Press", result.Pressure.Hectopascals, DecimalPrecision.Half),

                        // Gas sensor stable, add gas resistance and IAQ
                        new ("Gas", result.GasResistance.Ohms, DecimalPrecision.None),
                        new ("IAQ", airQuality, DecimalPrecision.None)
                    };
                }
                else
                {
                    registerSet.Registers = new Register[] {
                        new ("Temp", result.Temperature.DegreesCelsius, DecimalPrecision.One),
                        new ("Hum", result.Humidity.Percent, DecimalPrecision.Half),
                        new ("Press", result.Pressure.Hectopascals, DecimalPrecision.Half),
                    };
                }

                return true;
            }

            registerSet = null;
            return false;
        }

        public delegate void DataReceivedEventHandler(RegisterSet registerSet);
        public event DataReceivedEventHandler DataReceived;
    }
}
