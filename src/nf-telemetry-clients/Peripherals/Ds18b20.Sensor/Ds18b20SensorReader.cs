using Iot.Device.Ds18b20;
using nanoFramework.Device.OneWire;
using nanoFramework.Hardware.Esp32;
using System;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.Shared;

namespace TelemetryStash.Ds18b20Sensor
{
    // 4.7k pull-up resistor on the data line
    // 0.1μF ceramic decoupling capacitor between vcc and gnd near the sensor

    public class Ds18b20SensorReader
    {
        private Ds18b20 _ds18b20;

        private readonly TimeSpan _readInterval;
        private Thread _sensorReadRunner;

        public bool Initialized { get; private set; }

        public DateTime TimeStamp { get; private set; }

        public Ds18b20SensorReading[] Readings { get; private set; }

        public Ds18b20SensorReader(TimeSpan readInterval)
        {
            _readInterval = readInterval;
        }

        public void Initialize(Ds18b20SensorSettings settings)
        {
            Configuration.SetPinFunction(settings.ComRxPin, settings.ComRx);
            Configuration.SetPinFunction(settings.ComTxPin, settings.ComTx);

            var host = new OneWireHost();
            _ds18b20 = new Ds18b20(host, deviceAddress: null, manyDevicesConnected: true, TemperatureResolution.VeryHigh);

            if (!_ds18b20.Initialize())
            {
                throw new Exception("Failed to initialize the DS18B20 sensor.");
            }

            Readings = new Ds18b20SensorReading[_ds18b20.AddressNet.Length];

            for (var i = 0; i < _ds18b20.AddressNet.Length; i++)
            {
                var address = _ds18b20.AddressNet[i];
                _ds18b20.Address = address;
                if (_ds18b20.IsParasitePowered)
                {
                    throw new Exception("DS18B20 is parasite powered. Ensure pin connected to VCC");
                }

                var sensorMap = (settings.SensorMap[address.ToHexString()] as SensorMap) ?? new(address.ToHexString(), 0);
                Readings[i] = new Ds18b20SensorReading(address, sensorMap.Name, sensorMap.Offset);
            }

            _sensorReadRunner = new Thread(ReadRunner);
            _sensorReadRunner.Start();
        }

        private void ReadRunner()
        {
            while (true)
            {
                foreach (var reading in Readings)
                {
                    _ds18b20.Address = reading.Address;

                    // Note. Reading one sensor may take up to 750ms
                    if (!_ds18b20.TryReadTemperature(out var temperature))
                    {
                        Debug.WriteLine($"Error reading DS18B20 temp sensor {reading.Name}");
                        reading.SetTemperature(double.NaN);
                        continue;
                    }
                    reading.SetTemperature(temperature.DegreesCelsius);

                    // Ease off. No need to rush it
                    Thread.Sleep(100);
                }

                if (!Initialized)
                {
                    Initialized = true;
                    Debug.WriteLine("DS18B20 temp sensor initialized");
                }

                TimeStamp = DateTime.UtcNow;
                Thread.Sleep(_readInterval);
            }
        }
    }

    public class Ds18b20SensorReading
    {
        public Ds18b20SensorReading(byte[] address, string name, double offset)
        {
            Address = address;
            Name = name;
            Offset = offset;
        }

        private double _degreesCelsius = double.NaN;

        public byte[] Address { get; }

        public bool IsValid => _degreesCelsius != double.NaN;

        public string Name { get; }

        public double Offset { get; }

        public double Temperature => IsValid ? _degreesCelsius + Offset : 0;

        public void SetTemperature(double degreesCelsius)
        {
            _degreesCelsius = degreesCelsius;
        }
    }
}
