using nanoFramework.Hardware.Esp32;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using TelemetryStash.ServiceModels;

namespace TelemetryStash.Aidon.Sensor
{
    
    // https://aidon.com/wp-content/uploads/2023/06/AIDONFD_RJ12_HAN_Interface_SV.pdf
    // Aidon 6442 S

    public class AidonSensor
    {
        private SerialPort _serialPort;

        public readonly string _registerSetIdentifier;
        public readonly string _comPort;

        public AidonSensor(AidonSensorSettings settings)
        {
            _registerSetIdentifier = settings.RegisterSetIdentifier;
            Configuration.SetPinFunction(settings.RxPin, settings.RxComPort);
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

            Thread.Sleep(100);

            var message = _serialPort.ReadExisting();

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
                ReceivedBytesThreshold = 650,
                Handshake = Handshake.None
            };
        }

        public delegate void DataReceivedEventHandler(Telemetry telemetry);
        public event DataReceivedEventHandler DataReceived;
    }
}
