using Iot.Device.Am2320;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.I2c;
using System.Diagnostics;
using TelemetryStash.Shared;

namespace TelemetryStash.Am23XX.Sensor
{
    public class Am23XXSensor : IDisposable
    {
        private Am2320 _am2320;
        private DateTime _lastRead = DateTime.MinValue;
        private readonly string _registerSetIdentifier;

        public Am23XXSensor(Am23XXSensorSettings settings, string registerSetIdentifier)
        {
            if(settings == null)
            {
                return;
            }

            Configuration.SetPinFunction(settings.DataPin, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(settings.ClockPin, DeviceFunction.I2C1_CLOCK);
            _registerSetIdentifier = registerSetIdentifier;

            // The I2C bus ID on the MCU
            const int busId = 1;

            var i2cSettings = new I2cConnectionSettings(busId, Am2320.DefaultI2cAddress, I2cBusSpeed.StandardMode);
            _am2320 = new(I2cDevice.Create(i2cSettings));
        }

        public RegisterSet ReadTempAndHumidity()
        {
            if (_am2320 == null)
            {
                return null;
            }

            if (_lastRead.Add(Am2320.MinimumReadPeriod) > DateTime.UtcNow)
            {
                return null;
            }

            var temp = _am2320.Temperature;
            var hum = _am2320.Humidity;

            _lastRead = DateTime.UtcNow;
            if (_am2320.IsLastReadSuccessful)
            {
                return new RegisterSet
                {
                    Identifier = _registerSetIdentifier,
                    Registers = new Register[]
                    {
                       new ("Temp", temp.DegreesCelsius, DecimalPrecision.One),
                       new ("Hum", hum.Percent, DecimalPrecision.One)
                    }
                };
            }

            Debug.WriteLine("AM23XX Read failed");
            return null;
        }

        public void Dispose()
        {
            _am2320?.Dispose();
            _am2320 = null;
        }
    }
}
