using nanoFramework.Runtime.Native;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.Aidon.Sensor;
using TelemetryStash.Bmxx80.Sensor;
using TelemetryStash.NfClient.Communication;
using TelemetryStash.NfClient.Communication.Mqtt;
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
                Wifi.EnsureConnected();
                DeviceMetrics.RegisterDeviceStart();

                PrintStartupMessage();

                var settings = new AppSettings(new ConfigurationReader());
                _telemetryService = new(new MqttService(settings.Mqtt));

                var aidonSensor = new AidonSensor(settings.AidonSensor);
                aidonSensor.DataReceived += AidonSensor_DataReceived;

                var bmxx80Sensor = new Bme680Sensor(settings.Bme680Sensor, TimeSpan.FromSeconds(60), new string[] { "Out" });
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
                Power.RebootDevice(10000);
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
            }
        }

        private static void Bmxx80Sensor_DataReceived(RegisterSet registerSet)
        {
            try
            {
                _telemetryService.AddTelemetry(new Telemetry
                {
                    Timestamp = DateTime.UtcNow,
                    RegisterSets = new ArrayList { registerSet }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private static void DeviceMetrics_TimerElapsed(DateTime timestamp)
        {
            try
            {
                var deviceMetrics = DeviceMetrics.GetDeviceMetrics();
                _telemetryService.AddTelemetry(new Telemetry
                {
                    Timestamp = timestamp,
                    RegisterSets = deviceMetrics
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        [Conditional("DEBUG")]
        private static void PrintStartupMessage()
        {
            var printer = new DebugInformation();
            printer.PrintStartupMessage();
            printer.PrintSystemInfo();
        }
    }
}
