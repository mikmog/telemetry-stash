using nanoFramework.Hardware.Esp32;
using System.Diagnostics;
using System.IO.Ports;
using TelemetryStash.ServiceModels;

namespace TelemetryStash.Aidon.Sensor
{
    public class AidonSensor
    {
        private SerialPort _serialPort;

        public readonly string _registerSetIdentifier;
        public readonly string _comPort;

        // TODO
        //public AidonSensor(string registerSetIdentifier, int rxPin, int txPin)
        //{
        //    _registerSetIdentifier = registerSetIdentifier;

        //    // ESP32 DevKit:
        //    //Configuration.SetPinFunction(16, DeviceFunction.COM3_RX);
        //    //Configuration.SetPinFunction(17, DeviceFunction.COM3_TX);

        //    // XIAO_ESP32C3
        //    //Configuration.SetPinFunction(20, DeviceFunction.COM2_
        //    RX);
        //    //Configuration.SetPinFunction(21, DeviceFunction.COM2_TX);

        //    Configuration.SetPinFunction(rxPin, DeviceFunction.COM2_RX);
        //    Configuration.SetPinFunction(txPin, DeviceFunction.COM2_TX);
        //}
        public AidonSensor(AidonSensorSettings settings)
        {
            _registerSetIdentifier = settings.RegisterSetIdentifier;
            Configuration.SetPinFunction(settings.RxPin, settings.RxComPort);
            //Configuration.SetPinFunction(settings.TxPin, settings.TxComPort);
            _comPort = settings.ComPort;
        }

        public void Open()
        {
            _serialPort = CreateSerialPort();
            _serialPort.DataReceived += Serial_DataReceived;
            _serialPort.Open();
        }

        public void Dispose()
        {
            if(_serialPort != null)
            {
                _serialPort.DataReceived -= Serial_DataReceived;
                _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null;
            }
        }

        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Serial port is spamming 0-bytes events. Filter out.
            if (_serialPort.BytesToRead <= 0)
            {
                return;
            }

            var message = _serialPort.ReadExisting();

            // TODO
            Debug.WriteLine(message);
            return;

            if (!AidonMessageValidator.IsValid(message, out var error))
            {
                Debug.WriteLine(error);
                return;
            }

            var telemetry = AidonMessageParser.Parse(message, _registerSetIdentifier);
            DataReceived?.Invoke(telemetry);
        }

        private SerialPort CreateSerialPort()
        {
            return new SerialPort(_comPort)
            {
                StopBits = StopBits.One,
                BaudRate = 115200,
                DataBits = 8,
                Parity = Parity.None,
                ReadBufferSize = 900,
                InvertSignalLevels = true,
                ReceivedBytesThreshold = 500,
                Handshake = Handshake.None
            };
        }

        public delegate void DataReceivedEventHandler(Telemetry telemetry);
        public event DataReceivedEventHandler DataReceived;
    }
}
