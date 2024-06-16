using System.Collections;
using TelemetryStash.Aidon.Sensor;
using TelemetryStash.Am23XX.Sensor;
using TelemetryStash.DeviceServices;
using TelemetryStash.DeviceServices.Mqtt;
using TelemetryStash.IO.Peripherals.Led;

namespace TelemetryStash.SmartPowerMeter.NfClient.Configuration
{
    public class AppSettings
    {
        public AppSettings() { }

        public AppSettings(ConfigurationService configurationService)
        {
            var dictionary = configurationService.ReadConfiguration();
            Configure(dictionary);
        }

        private void Configure(IDictionary dictionary)
        {
            Am23XXSensor.Configure(dictionary);
            AidonSensor.Configure(dictionary);
            Mqtt.Configure(dictionary);
        }

        public LedSettings Led { get; set; } = new();

        public Am23XXSensorSettings Am23XXSensor { get; set; } = new();

        public AidonSensorSettings AidonSensor { get; set; } = new();

        public MqttSettings Mqtt { get; set; } = new();
    }
}
