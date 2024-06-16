using System;
using System.Collections;
using System.Diagnostics;
using TelemetryStash.ServiceModels;

namespace TelemetryStash.DeviceServices
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

        /// <summary>
        /// Deserialize basic JSON to Dictionary. Only primitive types supported.
        /// Quotes not supported in strings.
        /// </summary>
        public static IDictionary DeserializeToDictionary(string json)
        {
            const int maxProperties = 1000;

            var dictionary = new Hashtable();
            for (int i = 0; i < maxProperties; i++)
            {               
                var key = ExtractJsonSubstring(json);
                if (key == null)
                {
                    break;
                }

                // Advance to name separator. 
                json = json.Substring(json.IndexOf(':') + 1).TrimStart();

                // Char starting with "
                if (json[0] == '"')
                {
                    var value = ExtractJsonSubstring(json);
                    
                    // Advance length + 2 for the quotes
                    json = json.Substring(value.Length + 2);

                    // Replace \\ with \
                    while (value.Contains(@"\\"))
                    {
                        var index = value.IndexOf('\\');
                        value = value.Substring(0, index) + value.Substring(index + 1);
                    }

                    dictionary.Add(key, value);
                }

                // Char between 0-9 or -
                else if ((byte)json[0] >= 48 && (byte)json[0] <= 57 || json[0] == '-')
                {
                    var value = ExtractJsonNumber(json);
                    dictionary.Add(key, value);
                } 
                
                // Char is { or [
                else
                {
                    throw new NotImplementedException("Only value types supported");
                }
            }
            
            return dictionary;
        }

        private static string ExtractJsonSubstring(string json)
        {
            var keyStart = json.IndexOf('"');
            if (keyStart == -1)
            {
                return null;
            }

            var val = json.Substring(keyStart + 1, json.IndexOf('"', keyStart + 1) - keyStart - 1);
            return val;
        }

        private static object ExtractJsonNumber(string json)
        {
            var number = json.Substring(0, json.IndexOfAny(new char[] { ',', '}', ' ' }));
            if (number.IndexOf('.') >= 0)
            {
                return double.Parse(number);
            }
            else
            {
                return int.Parse(number);
            }
        }
    }
}
