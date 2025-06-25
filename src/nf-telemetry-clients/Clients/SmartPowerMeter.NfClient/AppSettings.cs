using System.Collections;
using TelemetryStash.Aidon.Sensor;
using TelemetryStash.Bmxx80.Sensor;
using TelemetryStash.IO.Peripherals.Led;
using TelemetryStash.NfClient.Communication.Mqtt;
using TelemetryStash.Shared;

namespace TelemetryStash.SmartPowerMeter.NfClient
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
            AidonSensor.Configure(dictionary);
            Bme680Sensor.Configure(dictionary);
            Mqtt.Configure(dictionary);
        }

        public LedSettings Led { get; set; } = new();

        public Bme680SensorSettings Bme680Sensor { get; set; } = new();

        public AidonSensorSettings AidonSensor { get; set; } = new();

        public MqttSettings Mqtt { get; set; } = new();
    }
}
