using nanoFramework.Hardware.Esp32;
using System.Collections;
using TelemetryStash.Shared;

namespace TelemetryStash.Ds18b20Sensor
{
    /*
      "Ds18b20.ComRx": "COM3_RX",
      "Ds18b20.ComRxPin": 39,
      "Ds18b20.ComTx": "COM3_TX",
      "Ds18b20.ComTxPin": 40,
      "Ds18b20.SensorMap": [
        "28B29B8400000067;Sensor1;0.2",
        "28B29B8400000067;Sensor2;0",
        "28B29B8400000067;Sensor3;0.2",
        "28B29B8400000067;Sensor4;0"
      ],
     */

    public class Ds18b20SensorSettings
    {
        private const string ComRxKey = "Ds18b20.ComRx";
        private const string ComRxPinKey = "Ds18b20.ComRxPin";
        private const string ComTxKey = "Ds18b20.ComTx";
        private const string ComTxPinKey = "Ds18b20.ComTxPin";
        private const string SensorMapKey = "Ds18b20.SensorMap";

        public void Configure(IDictionary dictionary)
        {
            ComRx = dictionary.DeviceFunction(ComRxKey);
            ComRxPin = dictionary.Int32(ComRxPinKey);

            ComTx = dictionary.DeviceFunction(ComTxKey);
            ComTxPin = dictionary.Int32(ComTxPinKey);

            var sensorMappings = dictionary.List(SensorMapKey);
            foreach (string sensorMapEntry in sensorMappings)
            {
                var parts = sensorMapEntry.Split(';');
                var address = parts[0].ToLower();
                var name = parts[1];
                var offset = double.Parse(parts[2]);

                SensorMap[address] = new SensorMap(name, offset);
            }
        }

        public DeviceFunction ComRx { get; set; }
        public int ComRxPin { get; set; }
        public DeviceFunction ComTx { get; set; }
        public int ComTxPin { get; set; }
        public Hashtable SensorMap { get; set; } = new();
    }

    public class SensorMap
    {
        public SensorMap(string name, double offset)
        {
            Name = name;
            Offset = offset;
        }

        public string Name { get; set; }
        public double Offset { get; set; }
    }
}
