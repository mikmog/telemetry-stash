using RipTide.Nfirmware.Components;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;


namespace RipTide.Nfirmware
{
    public class Program
    {
        private static AppSettings _appSettings = new();
        private static readonly ErrorHandler _errorHandler = new();
        private static readonly GpioController _gpioController = new();

        private static readonly BatteryMonitor _batteryMonitor = new(_gpioController, _errorHandler);
        private static readonly Buttons _buttons = new(_gpioController, _errorHandler);
        private static readonly DepthMonitor _depthMonitor = new(_gpioController, _errorHandler);
        private static readonly Gyro _gyro = new(_gpioController, _errorHandler);
        private static readonly Display _display = new(_gpioController, _errorHandler);
        private static readonly TempMonitor _tempMonitor = new(_gpioController, _errorHandler);
        private static readonly Throttle _throttle = new(_gpioController, _errorHandler);
        private static readonly Throttleds _throttleds = new(_gpioController, _errorHandler);

        public static void Main()
        {
            Thread.Sleep(5000);

            try
            {
                PrintStartupMessage();

                _appSettings.Configure();

                // Initialize display
                _display.Initialize(_appSettings);
                _errorHandler.Initialize(_display);

                // Show splash screen
                _display.SetScreen(Screen.Splash);
                _display.Fade(0, 0.8, TimeSpan.FromMilliseconds(1000));

                // Initialize throttle leds
                _throttleds.Initialize(_appSettings);

                // Initialize throttle
                _throttle.Initialize(_appSettings);

                Thread.Sleep(500);
                _display.SetScreen(Screen.Empty);
                Thread.Sleep(500);

                _throttle.OnThrustChanged += _display.SetThrust;
                _throttle.OnThrustChanged += _throttleds.ThrustChanged;

                _throttle.CalibrateThrustRange(
                    onMessage: (message, sleep) =>
                    {
                        Debug.WriteLine(message);
                        _display.SetText(message);

                        Thread.Sleep(sleep);
                    },
                    onError: (errorMessage, sleep) =>
                    {
                        Debug.WriteLine($"Error: {errorMessage}");
                        _display.SetText($"Error: {errorMessage}");

                        Thread.Sleep(sleep);
                        throw new Exception(errorMessage);
                    });

                _display.SetText("");

                _appSettings = null;

                /////// I2C
                ////Configuration.SetPinFunction(8, DeviceFunction.I2C1_DATA);
                ////Configuration.SetPinFunction(9, DeviceFunction.I2C1_CLOCK);
                //var gyro = new Mpu6050Gyro(new MpuGyroSettings { I2cDataPin = 8, I2cClockPin = 9 });
                //gyro.RunDemo();

                //var displaySettings = new IliDisplaySettings
                //{
                //    BackLightPin = 38,
                //    ChipSelectPin = 6,
                //    DataCommandPin = 7,
                //    ResetPin = 34,
                //    SpiMisoPin = 37,
                //    SpiMosiPin = 35,
                //    SpiClockPin = 36
                //};
                //var ili9488Display = new Ili9488Display(displaySettings);
                //ili9488Display.RunDemo();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }


            //var gpioController = new GpioController();
            //var adcController = new AdcController();

            //// Display
            //int backLightPin = 38;
            //int chipSelect = 6;
            //gpioController.OpenPin(chipSelect, PinMode.Output);

            //int dataCommand = 7;
            //gpioController.OpenPin(dataCommand, PinMode.Output);

            //int reset = 34;
            //gpioController.OpenPin(reset, PinMode.Output);

            //Configuration.SetPinFunction(backLightPin, DeviceFunction.PWM2);
            //var pwm = PwmChannel.CreateFromPin(backLightPin, 500);
            //pwm.DutyCycle = 0.5;
            //pwm.Start();

            ///// SPI
            //Configuration.SetPinFunction(37, DeviceFunction.SPI1_MISO);
            //Configuration.SetPinFunction(36, DeviceFunction.SPI1_CLOCK);
            //Configuration.SetPinFunction(35, DeviceFunction.SPI1_MOSI);

            //// ESC PWM
            //Configuration.SetPinFunction(40, DeviceFunction.PWM10);
            //var pwmEsc = PwmChannel.CreateFromPin(40, 400);
            //pwmEsc.DutyCycle = 0.5;
            //pwmEsc.Start();

            //Configuration.SetPinFunction(39, DeviceFunction.PWM9);
            //var pwmEsc2 = PwmChannel.CreateFromPin(39, 400);
            //pwmEsc2.DutyCycle = 0.5;
            //pwmEsc2.Start();


            //// OneWire
            //Configuration.SetPinFunction(42, DeviceFunction.COM3_RX);
            //Configuration.SetPinFunction(41, DeviceFunction.COM3_TX);

            //// Gyro
            ///// I2C
            //Configuration.SetPinFunction(8, DeviceFunction.I2C1_DATA);
            //Configuration.SetPinFunction(9, DeviceFunction.I2C1_CLOCK);

            //// Pixels
            //int pixelsPin = 1;
            //gpioController.OpenPin(pixelsPin, PinMode.Output);

            //// Throttleds
            //int throttledsPin = 2;
            //gpioController.OpenPin(throttledsPin, PinMode.Output);

            //// Buzzer
            //Configuration.SetPinFunction(3, DeviceFunction.PWM2);
            //var pwmBuzzer = PwmChannel.CreateFromPin(3, 400);
            //pwmBuzzer.DutyCycle = 0.5;
            //pwmBuzzer.Start();

            //// Pressure
            ///// Pin 4
            //var channel = adcController.OpenChannel(3);
            //channel.ReadValue();

            //// BMS
            ///// Serial port
            //Configuration.SetPinFunction(0, DeviceFunction.COM2_RX);
            //Configuration.SetPinFunction(21, DeviceFunction.COM2_TX);

            //// Extra
            //gpioController.OpenPin(21, PinMode.InputPullUp);
            //gpioController.OpenPin(0, PinMode.InputPullUp);

            //// Throttle Ss49e
            //adcController.OpenChannel(12);
            //adcController.OpenChannel(11);

            //// Throttle Buttons
            //gpioController.OpenPin(15, PinMode.InputPullUp);
            //gpioController.OpenPin(14, PinMode.InputPullUp);


            //// Mosfets
            //Configuration.SetPinFunction(16, DeviceFunction.PWM3);
            //var pwmMosfet = PwmChannel.CreateFromPin(16, 400);
            //pwmMosfet.DutyCycle = 0.5;
            //pwmMosfet.Start();

            //Configuration.SetPinFunction(5, DeviceFunction.PWM4);
            //var pwmMosfet2 = PwmChannel.CreateFromPin(5, 400);
            //pwmMosfet2.DutyCycle = 0.5;
            //pwmMosfet2.Start();

            //var port = CreateSerialPort();
            //port.Open();
            //port.WriteLine("Hello from RipTide Nfirmware!");
            //var existing = port.ReadExisting();
            //port.Close();

            //var port3 = new SerialPort("COM3")
            //{
            //    StopBits = StopBits.One,
            //    BaudRate = 115200,
            //    DataBits = 8,
            //    Parity = Parity.None,
            //    ReadBufferSize = 900,
            //    ReceivedBytesThreshold = 650,
            //    Handshake = Handshake.None
            //};
            //port3.Open();
            //port3.WriteLine("Hello from RipTide Nfirmware!");
            //var existing3 = port3.ReadExisting();
            //port3.Close();

            //PrintStartupMessage();

            //var apisQueenEsc = new ApisQueenEsc(pinLeftMotor: 41, pinRightMotor: 42);
            //apisQueenEsc.Initialize();

            ////var gauge = new NeoPixelGauge(pixelsCount: 45, new[] { Color.Green, Color.Yellow, Color.Red }, pin: 11);
            ////gauge.Initialize();

            ////var ili9488Display = new Ili9488Display();
            ////ili9488Display.RunDemo();

            ////var ss49e = new Ss49eHallSensor(new int[] { 0, 1 }, adcReadScale: 45, true);
            //var ss49e = new Ss49eHallSensor(new int[] { 14, 15 }, adcReadScale: 45, true);
            //ss49e.CalibrateAdcChannelOffsets();
            //while (true)
            //{
            //    var value = ss49e.Read();
            //    if (value == -1)
            //    {
            //        ss49e.CalibrateAdcChannelOffsets();
            //        continue;
            //    }

            //    Thread.Sleep(100);

            //    //gauge.SetPosition(value);
            //    apisQueenEsc.SetThrottle((ushort)(value * 2.23d));
            //}

            Debug.WriteLine("Zzzz");
            Thread.Sleep(Timeout.Infinite);
        }

        [Conditional("DEBUG")]
        private static void PrintStartupMessage()
        {
            //var printer = new Debugformation();
            //printer.PrintStartupMessage();
            //printer.PrintSystemInfo();
        }

        private static SerialPort CreateSerialPort()
        {
            return new SerialPort("COM2")
            {
                StopBits = StopBits.One,
                BaudRate = 115200,
                DataBits = 8,
                Parity = Parity.None,
                ReadBufferSize = 900,
                ReceivedBytesThreshold = 650,
                Handshake = Handshake.None
            };
        }
    }
}
