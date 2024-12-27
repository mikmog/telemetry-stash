using nanoFramework.Json;
using nanoFramework.TestFramework;
using System;
using System.Collections;
using TelemetryStash.Shared;

namespace TelemetryStash.NfClient.Services.Tests
{
    [TestClass]
    public class JsonSerializerTests
    {
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        [TestMethod]
        public void JsonSerializer_SerializeTelemetry_serializes_tags()
        {
            // Arrange
            var telemetry = NewTelemetry;
            telemetry.RegisterSets = new()
            {
                new RegisterSet
                {
                    Identifier = "RegisterSet1",
                    Tags = new[] { "Tag1", "Tag2" },
                    Registers = new Register[] { new("Register1", 1) }
                }
            };

            // Act
            var telemetryJson = JsonSerialize.SerializeTelemetry(telemetry);

            /// Deserialize JSON string to temporary object via nanoFramework.Json
            var telemetryTest = JsonConvert.DeserializeObject(telemetryJson, typeof(TelemetryTest), _jsonOptions) as TelemetryTest;

            var registerSetExpected = (RegisterSet)telemetry.RegisterSets[0];
            var registerSetActual = (Hashtable)telemetryTest.Set["RegisterSet1"];
            var tagsActual = registerSetActual["tag"] as ArrayList;

            // Assert
            Assert.IsNotNull(tagsActual);
            Assert.AreEqual(2, tagsActual.Count);
            foreach (var tag in registerSetExpected.Tags)
            {
                Assert.IsTrue(tagsActual.Contains(tag));
            }
        }


        [TestMethod]
        public void JsonSerializer_SerializeTelemetry_serializes_telemetry_tags()
        {
            // Arrange
            var telemetry = NewTelemetry;
            telemetry.Tags = new[] { "Tag1", "Tag2" };
            telemetry.RegisterSets = new()
            {
                new RegisterSet
                {
                    Identifier = "RegisterSet1",
                    Registers = new Register[] { new("Register1", 1) }
                }
            };

            // Act
            var telemetryJson = JsonSerialize.SerializeTelemetry(telemetry);

            /// Deserialize JSON string to temporary object via nanoFramework.Json
            var telemetryTest = JsonConvert.DeserializeObject(telemetryJson, typeof(TelemetryTest), _jsonOptions) as TelemetryTest;

            var tagsActual = telemetryTest.Tag;

            // Assert
            Assert.IsNotNull(tagsActual);
            Assert.AreEqual(2, tagsActual.Length);
            for(var i = 0; i < telemetry.Tags.Length; i++)
            {
                Assert.AreEqual(telemetry.Tags[i], tagsActual[i]);
            }
        }

        [TestMethod]
        public void JsonSerializer_SerializeTelemetry_serializes_numbers_only()
        {
            // Arrange
            var telemetry = NewTelemetry;

            telemetry.RegisterSets = new()
            {
                new RegisterSet
                {
                    Identifier = "RegisterSet1",
                    Registers = new[]
                    {
                        new Register("Register1", 1),
                    }
                }
            };

            // Act
            var telemetryJson = JsonSerialize.SerializeTelemetry(telemetry);

            /// Deserialize JSON string to temporary object via nanoFramework.Json
            var telemetryTest = JsonConvert.DeserializeObject(telemetryJson, typeof(TelemetryTest), _jsonOptions) as TelemetryTest;

            var registerSetExpected = (RegisterSet)telemetry.RegisterSets[0];
            var registerExpected = registerSetExpected.Registers[0];
            var registerSetActual = (Hashtable)telemetryTest.Set["RegisterSet1"];
            var registersActual = (Hashtable)registerSetActual["reg"];

            // Assert
            Assert.AreEqual(1, registersActual.Count);
            Assert.AreEqual(registerExpected.RegisterValue, double.Parse(registersActual["Register1"].ToString()));
        }

        [TestMethod]
        public void JsonSerializer_SerializeTelemetry_serializes_text_only()
        {
            // Arrange
            var telemetry = NewTelemetry;

            telemetry.RegisterSets = new()
            {
                new RegisterSet
                {
                    Identifier = "RegisterSet1",
                    Registers = new[]
                    {
                        new Register("Register1", "TextValue1"),
                    }
                }
            };

            // Act
            var telemetryJson = JsonSerialize.SerializeTelemetry(telemetry);

            /// Deserialize JSON string to temporary object via nanoFramework.Json
            var telemetryTest = JsonConvert.DeserializeObject(telemetryJson, typeof(TelemetryTest), _jsonOptions) as TelemetryTest;

            var registerSetExpected = (RegisterSet)telemetry.RegisterSets[0];
            var registerExpected = registerSetExpected.Registers[0];
            var registerSetActual = (Hashtable)telemetryTest.Set["RegisterSet1"];
            var registersActual = (Hashtable)registerSetActual["reg"];

            // Assert
            Assert.AreEqual(1, registersActual.Count);
            Assert.AreEqual(registerExpected.RegisterValue, registersActual["Register1"]);
        }

        [TestMethod]
        public void JsonSerializer_SerializeTelemetry_serializes_numbers_and_strings()
        {
            // Arrange
            var telemetry = NewTelemetry;

            // Act
            var telemetryJson = JsonSerialize.SerializeTelemetry(telemetry);

            /// Deserialize JSON string to temporary object via nanoFramework.Json
            var telemetryTest = JsonConvert.DeserializeObject(telemetryJson, typeof(TelemetryTest), _jsonOptions) as TelemetryTest;

            // Assert
            Assert.AreEqual("250101120000", telemetryTest.Ts);
            Assert.AreEqual(telemetry.RegisterSets.Count, telemetryTest.Set.Count);

            /// Enumerate RegisterSets and Registers
            foreach (RegisterSet registerSetExpected in telemetry.RegisterSets)
            {
                Assert.IsTrue(telemetryTest.Set.Contains(registerSetExpected.Identifier));

                var registerSetActual = (Hashtable)telemetryTest.Set[registerSetExpected.Identifier];
                var registersActual = (Hashtable)registerSetActual["reg"];
                
                /// Assert counts
                Assert.AreEqual(registerSetExpected.Registers.Length, registersActual.Count);

                /// Assert registers
                foreach (var registerExpected in registerSetExpected.Registers)
                {
                    Assert.IsTrue(registersActual.Contains(registerExpected.Identifier));

                    if (registerExpected.ValueType == RegisterValueType.Number)
                    {
                        Assert.AreEqual(registerExpected.RegisterValue, double.Parse(registersActual[registerExpected.Identifier].ToString()));
                    }
                    else
                    {
                        Assert.AreEqual(registerExpected.RegisterValue, registersActual[registerExpected.Identifier]);
                    }
                }
            }
        }

        [TestMethod]
        public void JsonSerializer_DeserializeToDictionary_deserializes_single_string()
        {
            // Arrange
            var json = @"
                    {
                        ""property1"": ""value1""
                    }";

            // Act
            var dictionary = JsonSerialize.DeserializeToDictionary(json);

            // Assert
            Assert.IsNotNull(dictionary);
            Assert.AreEqual(1, dictionary.Count);
            Assert.AreEqual("value1", dictionary["property1"]);
        }

        [TestMethod]
        public void JsonSerializer_DeserializeToDictionary_deserializes_file_path()
        {
            // Arrange
            var json = @"
                    {
                        ""property1"": ""I:\\Test.pem""
                    }";

            // Act
            var dictionary = JsonSerialize.DeserializeToDictionary(json);

            // Assert
            Assert.AreEqual(@"I:\Test.pem", dictionary["property1"]);
        }

        [TestMethod]
        public void JsonSerializer_DeserializeToDictionary_deserializes_multiple_strings()
        {
            // Arrange
            var json = @"
                    {
                        ""property1"": ""value1"",
                        ""property2"": ""value2""
                    }";

            // Act
            var dictionary = JsonSerialize.DeserializeToDictionary(json);

            // Assert
            Assert.IsNotNull(dictionary);
            Assert.AreEqual(2, dictionary.Count);
            Assert.AreEqual("value1", dictionary["property1"]);
            Assert.AreEqual("value2", dictionary["property2"]);
        }

        [TestMethod]
        public void JsonSerializer_DeserializeToDictionary_deserializes_multiple_numbers()
        {
            // Arrange
            var json = @"
                    {
                        ""property"": -1,
                        ""property0"": 0,
                        ""property1"": 0.01,
                        ""property2"": 2,
                        ""property3"": 3.33,
                        ""property4"": 2147483647,
                    }";

            // Act
            var dictionary = JsonSerialize.DeserializeToDictionary(json);

            // Assert
            Assert.IsNotNull(dictionary);
            Assert.AreEqual(6, dictionary.Count);
            Assert.AreEqual(-1, dictionary["property"]);
            Assert.AreEqual(0, dictionary["property0"]);
            Assert.AreEqual(0.01, dictionary["property1"]);
            Assert.AreEqual(2, dictionary["property2"]);
            Assert.AreEqual(3.33, dictionary["property3"]);
            Assert.AreEqual(2147483647, dictionary["property4"]);
        }

        private static Telemetry NewTelemetry => new()
        {
            Timestamp = new DateTime(2025, 01, 01, 12, 0, 0),
            RegisterSets = new()
                {
                    new RegisterSet
                    {
                        Identifier = "RegisterSet1",
                        Registers = new Register[]
                        {
                            new ("Register1", 1),
                            new ("Register2", "0.2", RegisterValueType.Number),
                            new ("Register3", 0),
                            new ("Register4", "-3.3", RegisterValueType.Number),

                            // Not supported by nanoFramework.Json
                            //new TextRegister("Backslash", "Text\\1"),
                            //new TextRegister("Backspace", "Text\b1"),
                            //new TextRegister("FormFeed", "Text\f1"),

                            new ("CarriageReturn",  "Text\r1"),
                            new ("DoubleQuote",     "Text1\""),
                            new ("Newline",         "Text\n1"),
                            new ("Tab",             "Text\t1"),
                            new ("OctalNull",       "Text\01"),
                            new ("NullChar",        "Tex\0t1"),
                            new ("Array",           "['Text3', 1]"),
                            new ("Json",            "{\"Text1\" : \"/Text1/\"}"),
                            new ("Unicode",         "ä"),
                            new ("Empty",           "")
                        }
                    },
                    new RegisterSet
                    {
                        Identifier = "RegisterSet2",
                        Registers = new Register[]
                        {
                            new ("Register21", 1),
                            new ("Register22", 2),
                            new ("TextRegister", "Text")
                        }
                    }
                }
        };

        //   {
        //       "ts": "250101120000",
        //       "tag": ["TagA", "TagB"],
        //       "set": {
        //           "RegisterSet1": {
        //               "tag": ["Tag1"],
        //               "reg": {
        //                   "Register1": 1,
        //                   "Register2": 0.20000000000000001,
        //                   "Register3": 0,
        //                   "Register4": -3.2999999999999998,
        //                   "CarriageReturn": "Text\r1",
        //                   "DoubleQuote": "Text1\"",
        //                   "Newline": "Text\n1",
        //                   "Tab": "Text\t1",
        //                   "OctalNull": "Text",
        //                   "Null": "Tex",
        //                   "Array": "['Text3', 1]",
        //                   "Json": "{\"Text1\" : \"/Text1/\"}",
        //                   "Unicode": "ä",
        //                   "Empty": ""
        //               }
        //           },
        //           "RegisterSet2": {
        //               "reg": {
        //                   "Register21": 1,
        //                   "Register22": 2,
        //                   "TextRegister": "Text"
        //               }
        //           }
        //       }
        //   }
        private class TelemetryTest
        {
            public string Ts { get; set; }

            public string[] Tag { get; set; }

            public Hashtable Set { get; set; }
        }
    }
}
