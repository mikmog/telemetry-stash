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
        private static TelemetryService _telemetryService;

        public static void Main()
        {
            try
            {
                Thread.Sleep(2000);
                Wifi.EnsureConnected();
                Thread.Sleep(2000);

                DeviceMetrics.RegisterDeviceStart();

                PrintStartupMessage();

                var settings = new AppSettings(new ConfigurationReader());

                var mqttService = new MqttService(settings.Mqtt);
                mqttService.Connect();
                _telemetryService = new(mqttService, bufferSize: 5);

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

                Debug.WriteLine("Error occured. Rebooting device");
                Thread.Sleep(5000);
                Power.RebootDevice(1000);
            }
        }

        private static void AidonSensor_DataReceived(Telemetry telemetry)
        {
            try
            {
                _telemetryService.PublishTelemetry(telemetry);
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
                _telemetryService.PublishTelemetry(new Telemetry
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
                _telemetryService.PublishTelemetry(new Telemetry
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
            var printer = new Debugformation();
            printer.PrintStartupMessage();
            printer.PrintSystemInfo();
        }
    }
}
