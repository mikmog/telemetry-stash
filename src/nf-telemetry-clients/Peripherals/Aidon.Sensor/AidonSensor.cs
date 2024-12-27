using nanoFramework.Hardware.Esp32;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using TelemetryStash.Shared;

namespace TelemetryStash.Aidon.Sensor
{
    public class AidonSensor
    {
        private SerialPort _serialPort;

        public readonly string _comPort;

        public AidonSensor(AidonSensorSettings settings)
        {
            Configuration.SetPinFunction(settings.RxPin, settings.RxComPort);
            _comPort = settings.ComPort;
        }

        public void Start()
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
            // Serial port spamming 0-bytes events. Filter out.
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

            var telemetry = AidonMessageParser.Parse(message);
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
