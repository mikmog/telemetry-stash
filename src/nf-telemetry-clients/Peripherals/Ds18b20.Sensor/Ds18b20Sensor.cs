//using nanoFramework.Hardware.Esp32;
//using System;
//using System.Threading;

//namespace TelemetryStash.Ds18b20.Sensor
//{
//    public class Ds18b20Sensor
//    {
//        private readonly TimeSpan _notificationInterval;
//        private Timer _notificationTimer;

//        private readonly string[] _tags;

//        private Ina219 _ina219;

//        public Ds18b20Sensor(Ds18b20SensorSettings settings, TimeSpan notificationInterval, string[] tags)
//        {
//            Configuration.SetPinFunction(settings.DataPin, DeviceFunction.I2C1_DATA);
//            Configuration.SetPinFunction(settings.ClockPin, DeviceFunction.I2C1_CLOCK);

//            _notificationInterval = notificationInterval;
//            _tags = tags;
//        }

//        public void Start()
//        {
//            if (_ina219 == null)
//            {
//                InitSensor();
//            }

//            _notificationTimer ??= new Timer(OnNotificationEvent, null, _notificationInterval, _notificationInterval);
//        }

//        public void Stop()
//        {
//            _notificationTimer.Dispose();
//            _notificationTimer = null;
//        }

//        public double ReadVoltage()
//        {
//            lock (_ina219)
//            {
//                return _ina219.ReadBusVoltage().Volts;
//            }
//        }

//        private void InitSensor()
//        {
//            const byte Ina219_I2cAddress = 0x40;
//            const byte Ina219_I2cBus = 0x1;

//            _ina219 = new Ina219(new I2cConnectionSettings(Ina219_I2cBus, Ina219_I2cAddress));
//            _ina219.Reset();

//            _ina219.BusVoltageRange = Ina219BusVoltageRange.Range16v;
//            _ina219.PgaSensitivity = Ina219PgaSensitivity.PlusOrMinus40mv;
//        }

//        private void OnNotificationEvent(object state)
//        {
//            var volts = ReadVoltage();

//            var registerSet = new RegisterSet
//            {
//                Identifier = "Ina219",
//                Tags = _tags,
//                Registers = new Register[]
//                {
//                    new ("Volts", volts, DecimalPrecision.Two)
//                }
//            };

//            DataReceived?.Invoke(registerSet);
//        }

//        public delegate void DataReceivedEventHandler(RegisterSet registerSet);
//        public event DataReceivedEventHandler DataReceived;
//    }
//}
