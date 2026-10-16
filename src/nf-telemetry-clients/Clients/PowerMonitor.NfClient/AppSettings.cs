using System.Collections;
using TelemetryStash.Ds18b20Sensor;
using TelemetryStash.Inaxxx.Sensor;
using TelemetryStash.NfClient.Services;

namespace TelemetryStash.PowerMonitor.NfClient
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
            Ds18b20Sensor.Configure(dictionary);
            InaxxxSensor.Configure(dictionary);
            Mqtt.Configure(dictionary);
        }

        public Ds18b20SensorSettings Ds18b20Sensor { get; set; } = new();

        public InaxxxSensorSettings InaxxxSensor { get; set; } = new();

        public MqttSettings Mqtt { get; set; } = new();
    }
}
