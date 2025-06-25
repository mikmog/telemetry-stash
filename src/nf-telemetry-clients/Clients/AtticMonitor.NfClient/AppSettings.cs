using System.Collections;
using TelemetryStash.Am23XX.Sensor;
using TelemetryStash.Bmxx80.Sensor;
using TelemetryStash.IO.Peripherals.Led;
using TelemetryStash.NfClient.Communication.Mqtt;
using TelemetryStash.Shared;

namespace TelemetryStash.AtticMonitor.NfClient
{
    public class AppSettings
    {
        public AppSettings() { }

        public AppSettings(ConfigurationReader reader)
        {
            var dictionary = reader.ReadConfiguration();
            Configure(dictionary);
        }

        private void Configure(IDictionary dictionary)
        {
            Am23XXSensor.Configure(dictionary);
            Bme680Sensor.Configure(dictionary);
            Mqtt.Configure(dictionary);
        }

        public LedSettings Led { get; set; } = new();

        public Am23XXSensorSettings Am23XXSensor { get; set; } = new();

        public Bme680SensorSettings Bme680Sensor { get; set; } = new();

        public MqttSettings Mqtt { get; set; } = new();
    }
}
