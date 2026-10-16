using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.Ds18b20Sensor;
using TelemetryStash.Inaxxx.Sensor;
using TelemetryStash.NfClient.Services;
using TelemetryStash.PowerMonitor.NfClient;
using TelemetryStash.Shared;

namespace PowerMonitor.NfClient
{
    public class Program
    {
        private static Ds18b20Sensor _ds18b20Sensor;
        private static Ina219Sensor _ina219Sensor;
        private static BufferedTelemetryService _telemetryService;

        public static void Main()
        {
            Thread.Sleep(2000);
            Wifi.EnsureConnected();
            Thread.Sleep(2000);

            DeviceMetrics.RegisterDeviceStart();

            PrintStartupMessage();

            var settings = new AppSettings(new ConfigurationReader());

            var mqttService = new MqttService(settings.Mqtt);
            mqttService.Connect();
            _telemetryService = new(mqttService);

            var metricsSensor = new TimerRunner(TimeSpan.FromMinutes(60));
            metricsSensor.TimerElapsed += DeviceMetrics_TimerElapsed;
            metricsSensor.Start();

            _ina219Sensor = new Ina219Sensor(settings.InaxxxSensor, TimeSpan.FromMinutes(5), new string[] { "Volts" });
            _ina219Sensor.DataReceived += Ina219Sensor_DataReceived;
            _ina219Sensor.Start();

            _ds18b20Sensor = new Ds18b20Sensor(settings.Ds18b20Sensor, new string[] { "Temp" });
            _ds18b20Sensor.InitSensor();

            settings = null;
            Thread.Sleep(Timeout.Infinite);
        }

        private static void Ina219Sensor_DataReceived(RegisterSet registerSet)
        {
            try
            {
                var telemetry = new Telemetry
                {
                    Timestamp = DateTime.UtcNow,
                    RegisterSets = new ArrayList { registerSet }
                };

                if (_ds18b20Sensor.TryReadTemperature(out RegisterSet temperature))
                {
                    telemetry.RegisterSets.Add(temperature);
                }

                _telemetryService.AddTelemetry(telemetry);
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
            var printer = new Debugformation();
            printer.PrintStartupMessage();
            printer.PrintSystemInfo();
        }
    }
}
