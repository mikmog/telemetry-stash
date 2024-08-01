using nanoFramework.Json;
using nanoFramework.TestFramework;
using System;
using System.Collections;
using TelemetryStash.DeviceServices;
using TelemetryStash.ServiceModels;

namespace TelemetryStash.Services.Tests.DeviceServices
{
    [TestClass]
    public class JsonSerializerTests
    {
        [TestMethod]
        public void JsonSerializer_Serialize_telemetry_is_not_null()
        {
            // Arrange
            var telemetry = GetTelemetry();

            // Act
            var json = Json.Serialize(telemetry);

            // Assert
            Assert.IsNotNull(json);
        }

        [TestMethod]
        public void JsonSerializer_Serialized_telemetry_can_be_deserialized()
        {
            // Arrange
            var telemetry = GetTelemetry();

            // Act
            var json = Json.Serialize(telemetry);
            var telemetryDeserialized = JsonConvert.DeserializeObject(json, typeof(ArrayList));
            var jsonDeserialized = JsonConvert.SerializeObject(telemetryDeserialized).TrimStart('[').TrimEnd(']');

            // Assert
            Assert.AreEqual(json, jsonDeserialized);
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
            var dictionary = Json.DeserializeToDictionary(json);

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
            var dictionary = Json.DeserializeToDictionary(json);

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
            var dictionary = Json.DeserializeToDictionary(json);

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
            var dictionary = Json.DeserializeToDictionary(json);

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

        private static Telemetry GetTelemetry() => new()
        {
            Timestamp = new DateTime(2024, 6, 01, 12, 0, 0),
            RegisterSets = new()
                {
                    new RegisterSet
                    {
                        Identifier = "P1",
                        Registers = new[]
                        {
                            new Register("C1", 7, 0),
                            new Register("C2", 8, 0),
                            new Register("C3", 9, 0)
                        }
                    },
                    new RegisterSet
                    {
                        Identifier = "Am2320",
                        Registers = new[]
                        {
                            new Register("Hum", 80, 8),
                            new Register("Temp", 21, 1)
                        }
                    }
                }
        };
    }
}
