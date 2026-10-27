using Iot.Device.Adc;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.I2c;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.Shared;

namespace TelemetryStash.Inaxxx.Sensor
{
    public class Ina219Sensor
    {
        private readonly TimeSpan _notificationInterval;
        private Timer _notificationTimer;

        private readonly string[] _tags;

        private Ina219 _ina219;

        public Ina219Sensor(InaxxxSensorSettings settings, TimeSpan notificationInterval, string[] tags)
        {
            Configuration.SetPinFunction(settings.DataPin, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(settings.ClockPin, DeviceFunction.I2C1_CLOCK);

            _notificationInterval = notificationInterval;
            _tags = tags;
        }

        /// <summary>
        /// Init sensor and start notification timer.
        /// </summary>
        public void Start()
        {
            if (_ina219 == null)
            {
                InitSensor();
            }

            _notificationTimer ??= new Timer(OnNotificationEvent, null, _notificationInterval, _notificationInterval);
        }

        public void Stop()
        {
            _notificationTimer.Dispose();
            _notificationTimer = null;
        }

        public bool TryReadVoltage(out double volts)
        {
            ArgumentNullException.ThrowIfNull(_ina219, $"{nameof(Ina219Sensor)} must be initialized before reading");

            lock (_ina219)
            {
                try
                {
                    volts = _ina219.ReadBusVoltage().Volts;
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed reading {nameof(Ina219Sensor)}");
                    Debug.WriteLine(ex.ToString());
                    volts = double.NaN;
                    return false;
                }
            }
        }

        public void InitSensor()
        {
            const byte Ina219_I2cAddress = 0x40;
            const byte Ina219_I2cBus = 0x1;

            _ina219 = new Ina219(new I2cConnectionSettings(Ina219_I2cBus, Ina219_I2cAddress));
            _ina219.Reset();

            _ina219.BusVoltageRange = Ina219BusVoltageRange.Range16v;
            _ina219.PgaSensitivity = Ina219PgaSensitivity.PlusOrMinus40mv;
        }

        private void OnNotificationEvent(object state)
        {
            if (!TryReadVoltage(out var volts))
            {
                return;
            }

            var registerSet = new RegisterSet
            {
                Identifier = "Ina219",
                Tags = _tags,
                Registers = new Register[]
                {
                    new ("Volts", volts, DecimalPrecision.Two)
                }
            };

            DataReceived?.Invoke(registerSet);
        }

        public delegate void DataReceivedEventHandler(RegisterSet registerSet);
        public event DataReceivedEventHandler DataReceived;
    }
}
