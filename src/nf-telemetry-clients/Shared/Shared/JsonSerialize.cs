using System;
using System.Collections;

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


        private const char KeyValueSeparator = '\f';
        private const char ArrayElementSeparator = '\a';

        /// <summary>
        /// Deserialize basic JSON to Dictionary. Only arrays and primitive types supported.
        /// </summary>
        public static IDictionary DeserializeToDictionary(string json)
        {
            json = SanitizeJson(json);
            var settings = json.SplitAndTrim(',');

            var dictionary = new Hashtable(settings.Length);
            foreach (var setting in settings)
            {
                if (string.IsNullOrEmpty(setting))
                {
                    continue;
                }

                var kvp = setting.SplitAndTrim(KeyValueSeparator);
                var settingName = ParseJsonString(kvp[0]);
                var settingValue = ParseJsonValue(kvp[1]);
                dictionary.Add(settingName, settingValue);
            }

            return dictionary;
        }

        private static string SanitizeJson(string json)
        {
            var jsonChars = json
                .Trim()
                .Trim('{', '}')
                .Trim()
                .ToCharArray();

            // Replace key:value colon separators with KeyValueSeparator
            // Replace [value1, value2] comma separators within arrays with ArrayElementSeparator
            bool isArray = false;
            for (var i = 0; i < jsonChars.Length; i++)
            {
                if (jsonChars[i] == '[')
                {
                    isArray = true;
                    continue;
                }

                if (isArray && jsonChars[i] == ']')
                {
                    isArray = false;
                    continue;
                }

                var commaSeparator = isArray && jsonChars[i] == ',';
                var colonSeparator = jsonChars[i] == ':' && jsonChars[i - 1] == '\"';

                if (commaSeparator)
                {
                    jsonChars[i] = ArrayElementSeparator;
                }

                if (colonSeparator)
                {
                    jsonChars[i] = KeyValueSeparator;
                }
            }

            return new string(jsonChars);
        }

        private static string ParseJsonString(string jsonString)
        {
            // Remove surrounding quotes
            jsonString = jsonString.Trim('\"');

            // Replace \\ with a single backslash \
            var parts = jsonString.Split('\\');
            jsonString = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                if (!string.IsNullOrEmpty(parts[i]))
                {
                    jsonString += '\\' + parts[i];
                }
            }

            return jsonString;
        }

        private static object ParseJsonValue(string jsonValue)
        {
            if (jsonValue.StartsWith("\""))
            {
                return ParseJsonString(jsonValue);
            }
            else if (jsonValue.StartsWith("["))
            {
                jsonValue = jsonValue.Trim('[', ']');
                var array = new ArrayList();
                foreach (var element in jsonValue.SplitAndTrim('\a'))
                {
                    var value = ParseJsonValue(element);
                    array.Add(value);
                }
                return array;
            }
            else if (int.TryParse(jsonValue, out var number))
            {
                return number;
            }
            else if (double.TryParse(jsonValue, out var intValue))
            {
                return intValue;
            }
            else
            {
                throw new NotSupportedException($"Unsupported JSON value: {jsonValue}");
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
