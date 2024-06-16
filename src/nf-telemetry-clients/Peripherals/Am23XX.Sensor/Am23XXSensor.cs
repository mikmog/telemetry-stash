using Iot.Device.Am2320;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.I2c;
using System.Diagnostics;
using TelemetryStash.ServiceModels;

namespace TelemetryStash.Am23XX.Sensor
{
    public class Am23XXSensor : IDisposable
    {
        private Am2320 _am2320;
        private DateTime _lastRead = DateTime.MinValue;
        private readonly string _registerSetIdentifier;

        public Am23XXSensor(Am23XXSensorSettings settings)
        {
            if(settings == null)
            {
                return;
            }

            Configuration.SetPinFunction(settings.DataPin, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(settings.ClockPin, DeviceFunction.I2C1_CLOCK);
            _registerSetIdentifier = settings.RegisterSetIdentifier;
            _am2320 = new(new I2cDevice(new I2cConnectionSettings(1, 0x5C, I2cBusSpeed.StandardMode)));
        }

        public RegisterSet ReadTempAndHumidity()
        {
            if (_am2320 == null)
            {
                Debug.WriteLine(nameof(Am23XXSensor) + " not initialized");
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
                Debug.WriteLine($"Temp = {temp.DegreesCelsius.ToString("N1")} C, Hum = {hum.Percent.ToString("N1")} %");

                return new RegisterSet
                {
                    Identifier = _registerSetIdentifier,
                    Registers = new Register[]
                    {
                       RegisterExtension.ToRegister("Temp", temp.DegreesCelsius, 1),
                       RegisterExtension.ToRegister("Hum", hum.Percent, 1)
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
