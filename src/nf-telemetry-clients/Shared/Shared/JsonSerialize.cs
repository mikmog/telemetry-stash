using System;
using System.Collections;
using RegisterValueType = TelemetryStash.Shared.RegisterValueType;

namespace TelemetryStash.Shared
{
    public static class JsonSerialize
    {
        /// <summary>
        /// Serialize Telemetry as a custom JSON string
        /// <code>
        /// {
        ///    "ts": "240130120000",
        ///    "set": {
        ///        "RegisterSet1": {
        ///             "tag": ["Tag1", "Tag2"],
        ///             "reg": {
        ///                 "Register1": 1,
        ///                 "Register2": 2,
        ///                 "Register3": "Value3"
        ///            }
        ///        },
        ///        "Am2320": {
        ///             "tag": ["Tag2"],
        ///             "reg": {
        ///                 "Humidity": 80.8,
        ///                 "Temperature": 21.1
        ///             }
        ///         }
        ///     }
        /// }
        /// </code>
        /// </summary>
        public static string SerializeTelemetry(Telemetry telemetry)
        {
            if (telemetry.RegisterSets == null || telemetry.RegisterSets.Count == 0)
            {
                throw new ArgumentException("RegisterSets is required");
            }

            string telemetryJson = null;
            foreach (RegisterSet registerSet in telemetry.RegisterSets)
            {
                if (registerSet.Registers == null || registerSet.Registers.Length == 0)
                {
                    throw new ArgumentException("Registers is required");
                }

                string registerSetsJson = null;

                // "Register1": 1,
                // "Register2": 2,
                // "Register3": "Value3"
                string registersJson = null;
                foreach (var register in registerSet.Registers)
                {
                    registersJson += "\"" + register.Identifier + "\":" + GetRegisterValueJson(register) + ",";
                }

                // "tag": ["Tag1", "Tag2"],
                string tagsJson = null;
                if (registerSet.Tags != null)
                {
                    foreach (var tag in registerSet.Tags)
                    {
                        tagsJson += "\"" + tag + "\",";
                    }
                    tagsJson = "\"tag\":[" + (tagsJson ?? string.Empty).TrimEnd(',') + "],";
                }

                //  "tag": ["Tag1", "Tag2"],
                //  "reg": {
                ///     "Register1": 1,
                ///     "Register2": 2,
                ///     "Register3": "Value3"
                //  },
                registerSetsJson += tagsJson;
                registerSetsJson += "\"reg\":{";
                registerSetsJson += registersJson.TrimEnd(',');
                registerSetsJson += "},";

                //  "RegisterSet1": {
                ///     "tag": ["Tag1", "Tag2"],
                ///     "reg": {
                ///         "Register1": 1,
                ///         "Register2": 2,
                ///         "Register3": "Value3"
                ///     }
                //  },
                telemetryJson += "\"" + registerSet.Identifier + "\":{";
                telemetryJson += registerSetsJson.TrimEnd(',');
                telemetryJson += "},";
            }

            // "tag": ["Tag1", "Tag2"],
            string telemetryTagsJson = null;
            if (telemetry.Tags != null)
            {
                foreach (var tag in telemetry.Tags)
                {
                    telemetryTagsJson += "\"" + tag + "\",";
                }
                telemetryTagsJson = "\"tag\":[" + (telemetryTagsJson ?? string.Empty).TrimEnd(',') + "],";
            }

            //  {
            //      "ts": "240130120000",
            //      "tag": ["Tag1", "Tag2"],
            //      "set": {
            ///         "RegisterSet1": {
            ///             "tag": ["Tag1", "Tag2"],
            ///             "reg": {
            ///                 "Register1": 1,
            ///                 "Register2": 2,
            ///                 "Register3": "Value3"
            ///             }
            ///         }
            //      }
            //  }
            var json = "{\"ts\":\"" + telemetry.Timestamp.ToString("yyMMddHHmmss") + "\"," +
                        telemetryTagsJson +
                        "\"set\":{" + telemetryJson.TrimEnd(',') + "}}";

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

                // Number array
                else if (json[0] == '[')
                {
                    var numbersAsText = json.Substring(1, json.IndexOf(']') - 1);
                    var array = new ArrayList();
                    foreach (var number in numbersAsText.Split(','))
                    {
                        var value = ExtractJsonNumber(number.Trim());
                        array.Add(value);
                    }
                    dictionary.Add(key, array);

                    // Advance total length of array
                    json = json.Substring(numbersAsText.Length + 2);
                }

                // Char is {
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
            var endPosition = json.IndexOfAny(new char[] { ',', '}', ' ' });
            endPosition = endPosition == -1 ? json.Length : endPosition;

            var number = json.Substring(0, endPosition);
            if (number.IndexOf('.') >= 0)
            {
                return double.Parse(number);
            }
            else
            {
                return int.Parse(number);
            }
        }

        private static string GetRegisterValueJson(Register register)
        {
            if (register.ValueType == RegisterValueType.Number)
            {
                return register.Value;
            }
            else
            {
                if (register.Value == null)
                {
                    return "null";
                }

                return "\"" + GetSafeJson(register.Value) + "\"";
            }
        }

        private static readonly char[] unsafeChars = new[] { '\\', '"', '\b', '\t', '\n', '\f', '\r' };
        private static string GetSafeJson(string unsafeJson)
        {
            if (unsafeJson.IndexOfAny(unsafeChars) == -1)
            {
                return unsafeJson;
            }

            string safeJson = null;
            foreach (char c in unsafeJson)
            {
                switch (c)
                {
                    case '\\':
                        safeJson += "\\\\";
                        break;
                    case '"':
                        safeJson += "\\\"";
                        break;
                    case '\b':
                        safeJson += "\\b";
                        break;
                    case '\t':
                        safeJson += "\\t";
                        break;
                    case '\n':
                        safeJson += "\\n";
                        break;
                    case '\f':
                        safeJson += "\\f";
                        break;
                    case '\r':
                        safeJson += "\\r";
                        break;
                    default:
                        safeJson += c;

                        // TODO can probably be removed
                        //if (c < ' ' || c > '~') // ASCII 'non printable' characters
                        //{
                        //    var t = "000" + string.Format("{0:X}", (int)c);
                        //    safeJson += "\\u" + t.Substring(t.Length - 4);
                        //}
                        //else
                        //{
                        //    safeJson += c;
                        //}
                        break;
                }
            }
            return safeJson;
        }
    }
}
