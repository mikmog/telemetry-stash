using System;
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
        private static Ds18b20Sensor2 _ds18b20Sensor;
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

            _ina219Sensor = new Ina219Sensor(settings.InaxxxSensor, TimeSpan.FromSeconds(1), new string[] { "Voltage" });
            _ina219Sensor.DataReceived += Ina219Sensor_DataReceived;
            _ina219Sensor.Start();

            _ds18b20Sensor = new Ds18b20Sensor2();
            _ds18b20Sensor.RunDemo();

            Thread.Sleep(Timeout.Infinite);
        }

        private static void Ina219Sensor_DataReceived(RegisterSet registerSet)
        {
            Debug.WriteLine($"Voltage: {registerSet.Registers[0].Value} V");
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
