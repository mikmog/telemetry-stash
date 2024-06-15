using nanoFramework.Runtime.Native;
using System;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.Aidon.Sensor;
using TelemetryStash.Am23XX.Sensor;
using TelemetryStash.DeviceServices;
using TelemetryStash.DeviceServices.Mqtt;
using TelemetryStash.IO.Peripherals.Led;
using TelemetryStash.ServiceModels;
using TelemetryStash.SmartPowerMeter.NfClient.Configuration;

namespace SmartPowerMeter.Client
{
    public class Program
    {
        private static BufferedTelemetryService _telemetryService;
        private static AidonSensor _aidonSensor;
        private static Am23XXSensor _am23XXSensor;
        private static Led _led;

        public static void Main()
        {
            try
            {
                Thread.Sleep(1000);

                Wifi.EnsureConnected();
                PrintStartupMessage();

                var settings = new AppSettings(new ConfigurationService());
                _led = new(settings.Led.Pin1);
                _telemetryService = new(new MqttService(settings.Mqtt));
                _aidonSensor = new(settings.AidonSensor);
                _am23XXSensor = new(settings.Am23XXSensor);

                Run();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                DisposeAndRestart();
            }
        }

        private static void Run()
        {
            _led.Signal(LedSignal.Blink5ForStart);
            PrintStartupMessage();

            _led.Signal(LedSignal.On);
            Thread.Sleep(5000);
            _led.Signal(LedSignal.Off);

            _aidonSensor.DataReceived += AidonDataReceived;
            _aidonSensor.Open();

            Thread.Sleep(Timeout.Infinite);
        }

        private static void AidonDataReceived(Telemetry telemetry)
        {
            try
            {
                _led.Signal(LedSignal.On);
                var registerSet = _am23XXSensor.ReadTempAndHumidity();
                if (registerSet != null)
                {
                    telemetry.RegisterSets.Add(registerSet);
                }

                _telemetryService.AddTelemetry(telemetry);
                _led.Signal(LedSignal.Off);

                // TODO: Can probably be removed
                var remainingRam = nanoFramework.Runtime.Native.GC.Run(false);
                if (remainingRam <= 10000)
                {
                    DisposeAndRestart();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                DisposeAndRestart();
            }
        }

        private static void DisposeAndRestart()
        {
            _aidonSensor?.Dispose();
            _aidonSensor = null;

            _am23XXSensor?.Dispose();
            _am23XXSensor = null;

            _telemetryService?.Dispose();
            _telemetryService = null;

            Debug.WriteLine("Rebooting in 60 sec...");
            _led?.Signal(LedSignal.BlinkSosFor60Sec);
            Power.RebootDevice(100);
        }

        [Conditional("DEBUG")]
        private static void PrintStartupMessage()
        {
            var printer = new Printer();
            printer.PrintStartupMessage();
            printer.PrintSystemInfo();
        }
    }
}
