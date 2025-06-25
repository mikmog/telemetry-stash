using nanoFramework.Runtime.Native;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.Am23XX.Sensor;
using TelemetryStash.Bmxx80.Sensor;
using TelemetryStash.NfClient.Communication;
using TelemetryStash.NfClient.Communication.Mqtt;
using TelemetryStash.NfClient.Services;
using TelemetryStash.Peripherals.BluetoothSensor;
using TelemetryStash.Peripherals.WifiSensor;
using TelemetryStash.Shared;

namespace TelemetryStash.AtticMonitor.NfClient
{
    public class Program
    {
        private static BufferedTelemetryService _telemetryService;
        private static Am2320Sensor _am2320Sensor;

        public static void Main()
        {
            try
            {
                Thread.Sleep(1000);
                Wifi.EnsureConnected();
                DeviceMetrics.RegisterDeviceStart();

                PrintStartupMessage();

                var settings = new AppSettings(new ConfigurationReader());
                _telemetryService = new(new MqttService(settings.Mqtt, discardMessages: false));

                var metricsSensor = new TimerRunner(TimeSpan.FromMinutes(60));
                metricsSensor.TimerElapsed += DeviceMetrics_TimerElapsed;

                _am2320Sensor = new Am2320Sensor(settings.Am23XXSensor, TimeSpan.MaxValue, new string[] { "Firewall" });

                var bmxx80Sensor = new Bme680Sensor(settings.Bme680Sensor, TimeSpan.FromMinutes(10), new string[] { "Attic" });
                bmxx80Sensor.DataReceived += Bmxx80Sensor_DataReceived;

                var bluetoothSensor = new BluetoothSensor(TimeSpan.FromHours(2), preferredBatchSize: 10);
                bluetoothSensor.DataReceived += BluetoothSensor_DataReceived;

                var wifiSensor = new WifiSensor(TimeSpan.FromHours(2), preferredBatchSize: 10);
                wifiSensor.DataReceived += WifiSensor_DataReceived;

                Debug.WriteLine("Starting Device metrics sensor");
                metricsSensor.Start();

                Debug.WriteLine("Starting Bme680 sensor");
                bmxx80Sensor.Start();

                Debug.WriteLine("Starting Am2320 sensor");
                _am2320Sensor.Start();

                while (true)
                {
                    Debug.WriteLine("Starting Bluetooth sensor");
                    bluetoothSensor.Start();
                    Thread.Sleep(TimeSpan.FromSeconds(60));
                    Debug.WriteLine("60 sec pass. Stopping Bluetooth");
                    bluetoothSensor.Stop();
                    Debug.WriteLine();

                    Thread.Sleep(500);

                    Debug.WriteLine("Starting Wifi sensor");
                    wifiSensor.Start();
                    Thread.Sleep(TimeSpan.FromSeconds(30));
                    Debug.WriteLine("30 sec pass. Stopping Wifi");
                    wifiSensor.Stop();
                    Debug.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                Power.RebootDevice(10000);
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

        private static void Bmxx80Sensor_DataReceived(RegisterSet registerSet)
        {
            try
            {
                var telemetry = new Telemetry
                {
                    Timestamp = DateTime.UtcNow,
                    RegisterSets = new ArrayList { registerSet }
                };

                var am23Reading = _am2320Sensor.GetLastReading();
                if (am23Reading != null)
                {
                    telemetry.RegisterSets.Add(am23Reading.RegisterSet);
                }

                _telemetryService.AddTelemetry(telemetry);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private static void BluetoothSensor_DataReceived(ArrayList registerSets)
        {
            try
            {
                _telemetryService.AddTelemetry(new Telemetry
                {
                    Timestamp = DateTime.UtcNow,
                    Tags = new string[] { "Attic" },
                    RegisterSets = registerSets
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private static void WifiSensor_DataReceived(ArrayList registerSets)
        {
            try
            {
                _telemetryService.AddTelemetry(new Telemetry
                {
                    Timestamp = DateTime.UtcNow,
                    RegisterSets = registerSets
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
