using DataCollector.Application.P1Meter;

namespace TelemetryStash.DeviceService.Properties
{
    public static class Json
    {
        /// <summary>
        /// Serialize Telemetry as JSON string
        /// <code>
        /// {
        ///    "ts": "2309011013361234567",
        ///    "reg": {
        ///        "P1": {
        ///            "C1": 7,
        ///            "C2": 8,
        ///            "C3": 9
        ///        },
        ///        "Am2320": {
        ///            "Hum": 80.8,
        ///            "Temp": 21.1
        ///        }
        ///    }
        /// }
        /// </code>
        /// </summary>
        public static string Serialize(Telemetry telemetry)
        {
            string registerSetsJson = null;
            foreach (RegisterSet registerSet in telemetry.RegisterSets)
            {
                // "P1": {
                registerSetsJson += "\"" + registerSet.Identifier + "\":{";

                // "C1": 7,
                // "C2": 8,
                // "C3": 9
                string registersJson = null;
                foreach (var register in registerSet.Registers)
                {
                    registersJson += "\"" + register.Identifier + "\":" + register.ToNumberString() + ",";
                }

                registerSetsJson += registersJson?.TrimEnd(',');

                // },
                registerSetsJson += "},";
            }

            var json = "{\"ts\":\"" + telemetry.Timestamp.ToString("yyMMddHHmmss") +
                        "\",\"reg\":{" +
                                registerSetsJson?.TrimEnd(',') +
                            "}}";

            return json;
        }
    }
}
