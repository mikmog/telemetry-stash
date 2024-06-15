namespace TelemetryStash.Aidon.Sensor
{
    public static class Obis
    {
        public static ObisCode Timestamp = new("TS", "0-0:1.0.0", 13, "UtcDateTime");
        public static ObisCode TotActiveEnergy = new("TAE", "1-0:1.8.0", 12, "kWh");
        public static ObisCode TotActiveEnergyInput = new("TEI", "1-0:2.8.0", 12, "kWh");
        public static ObisCode TotReactiveEnergyDraw = new("TRD", "1-0:3.8.0", 12, "kVArh");
        public static ObisCode TotReactiveEnergyInput = new("TRI", "1-0:4.8.0", 12, "kVArh");

        public static ObisCode ActiveEnergyDraw = new("ED", "1-0:1.7.0", 8, "kW");
        public static ObisCode ActiveEnergyInput = new("EI", "1-0:2.7.0", 8, "kW");

        public static ObisCode ReactiveEnergyDraw = new("RED", "1-0:3.7.0", 8, "kVArh");
        public static ObisCode ReactiveEnergyInput = new("REI", "1-0:4.7.0", 8, "kvkVArhar");

        public static ObisCode L1EnergyDraw = new("1ED", "1-0:21.7.0", 8, "kW");
        public static ObisCode L1EnergyInput = new("1EI", "1-0:22.7.0", 8, "kW");
        public static ObisCode L2EnergyDraw = new("2ED", "1-0:41.7.0", 8, "kW");
        public static ObisCode L2EnergyInput = new("2EI", "1-0:42.7.0", 8, "kW");
        public static ObisCode L3EnergyDraw = new("3ED", "1-0:61.7.0", 8, "kW");
        public static ObisCode L3EnergyInput = new("3EI", "1-0:62.7.0", 8, "kW");

        public static ObisCode L1ReactiveEnergyDraw = new("1RD", "1-0:23.7.0", 8, "kVArh");
        public static ObisCode L1ReactiveEnergyInput = new("1RI", "1-0:24.7.0", 8, "kVArh");
        public static ObisCode L2ReactiveEnergyDraw = new("2RD", "1-0:43.7.0", 8, "kVArh");
        public static ObisCode L2ReactiveEnergyInput = new("2RI", "1-0:44.7.0", 8, "kVArh");
        public static ObisCode L3ReactiveEnergyDraw = new("3RD", "1-0:63.7.0", 8, "kVArh");
        public static ObisCode L3ReactiveEnergyInput = new("3RI", "1-0:64.7.0", 8, "kVArh");

        public static ObisCode L1Voltage = new("1V", "1-0:32.7.0", 5, "v");
        public static ObisCode L2Voltage = new("2V", "1-0:52.7.0", 5, "v");
        public static ObisCode L3Voltage = new("3V", "1-0:72.7.0", 5, "v");

        public static ObisCode L1Current = new("1C", "1-0:31.7.0", 5, "a");
        public static ObisCode L2Current = new("2C", "1-0:51.7.0", 5, "a");
        public static ObisCode L3Current = new("3C", "1-0:71.7.0", 5, "a");
    }

    public class ObisCode
    {
        public ObisCode(string name, string code, int fieldLength, string unit)
        {
            Name = name;
            Code = code;
            FieldLength = fieldLength;
            Unit = unit;
        }
        public string Name { get; }

        public string Code { get; }

        public int FieldLength { get; }

        public int CodeLength => Code.Length + 1; // Skip right parentheses

        public string Unit { get; }
    }
}
