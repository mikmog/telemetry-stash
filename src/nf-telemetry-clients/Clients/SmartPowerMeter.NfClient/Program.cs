using nanoFramework.Runtime.Native;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.Aidon.Sensor;
using TelemetryStash.Bmxx80.Sensor;
using TelemetryStash.NfClient.Services;
using TelemetryStash.Shared;
using TelemetryStash.SmartPowerMeter.NfClient;

namespace SmartPowerMeter.Client
{
    public class Program
    {
        private static BufferedTelemetryService _telemetryService;

        public static void Main()
        {
            try
            {
                Thread.Sleep(1000);
                DeviceMetrics.RegisterDeviceStart();
                Wifi.EnsureConnected();

                PrintStartupMessage();

                var settings = new AppSettings(new ConfigurationReader());
                _telemetryService = new(new MqttService(settings.Mqtt));

                var aidonSensor = new AidonSensor(settings.AidonSensor);
                aidonSensor.DataReceived += AidonSensor_DataReceived;

                var bmxx80Sensor = new Bme680Sensor(settings.Bme680Sensor, TimeSpan.FromSeconds(60), new string[] { "Outdoor" });
                bmxx80Sensor.DataReceived += Bmxx80Sensor_DataReceived;

                var metricsSensor = new TimerRunner(TimeSpan.FromMinutes(60));
                metricsSensor.TimerElapsed += DeviceMetrics_TimerElapsed;

                Debug.WriteLine("Starting Aidon sensor");
                aidonSensor.Start();

                Debug.WriteLine("Starting Bme680 sensor");
                bmxx80Sensor.Start();

                Debug.WriteLine("Starting Device metrics sensor");
                metricsSensor.Start();

                settings = null;

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                Power.RebootDevice(1000);
            }
        }

        private static void AidonSensor_DataReceived(Telemetry telemetry)
        {
            try
            {
                _telemetryService.AddTelemetry(telemetry);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                Power.RebootDevice(1000);
            }
        }

        private static void Bmxx80Sensor_DataReceived(RegisterSet registerSet)
        {
            _telemetryService.AddTelemetry(new Telemetry
            {
                Timestamp = DateTime.UtcNow,
                RegisterSets = new ArrayList { registerSet }
            });
        }

        private static void DeviceMetrics_TimerElapsed(DateTime timestamp)
        {
            var deviceMetrics = DeviceMetrics.GetDeviceMetrics();
            _telemetryService.AddTelemetry(new Telemetry
            {
                Timestamp = timestamp,
                RegisterSets = deviceMetrics
            });
        }

        [Conditional("DEBUG")]
        private static void PrintStartupMessage()
        {
            var printer = new Debugformation();
            printer.PrintStartupMessage();
            printer.PrintSystemInfo();
        }
    }
}
