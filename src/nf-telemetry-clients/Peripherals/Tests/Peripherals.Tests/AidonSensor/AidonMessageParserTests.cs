using nanoFramework.TestFramework;
using System;
using System.Collections;
using TelemetryStash.Aidon.Sensor;
using TelemetryStash.Shared;

namespace TelemetryStash.Peripherals.Tests.AidonSensor
{
    [TestClass]
    public class AidonMessageParserTests
    {
        [TestMethod]
        public void Parse_Parses_raw_aidon_message()
        {
            var response = AidonMessageParser.Parse(TestData.AidonMessage);

            Assert.IsNotNull(response);
            Assert.AreEqual(new DateTime(2024, 02, 17, 16, 40, 50), response.Timestamp);

            var registers = ((RegisterSet)response.RegisterSets[0]).Registers;
            foreach (var register in registers)
            {
                Assert.AreEqual(ObisTable[register.Identifier], register.Value, register.Identifier);
            }
        }

        private Hashtable ObisTable => new()
        {
            { Obis.TotActiveEnergy.Name,         "7805.332" },
            { Obis.TotActiveEnergyInput.Name,    "11111111.111" },
            { Obis.TotReactiveEnergyDraw.Name,   "3.239" },
            { Obis.TotReactiveEnergyInput.Name,  "1945.988" },

            { Obis.ActiveEnergyDraw.Name,        "0.87" },
            { Obis.ActiveEnergyInput.Name,       "1.001" },

            { Obis.ReactiveEnergyDraw.Name,      "2" },
            { Obis.ReactiveEnergyInput.Name,      "0.551" },

            { Obis.L1EnergyDraw.Name,            "0.269" },
            { Obis.L1EnergyInput.Name,           "0.345" },
            { Obis.L2EnergyDraw.Name,            "0.526" },
            { Obis.L2EnergyInput.Name,           "3" },
            { Obis.L3EnergyDraw.Name,            "0.06" },
            { Obis.L3EnergyInput.Name,           "4" },

            { Obis.L1ReactiveEnergyDraw.Name,    "5" },
            { Obis.L1ReactiveEnergyInput.Name,   "0.277" },
            { Obis.L2ReactiveEnergyDraw.Name,    "6" },
            { Obis.L2ReactiveEnergyInput.Name,   "0.184" },
            { Obis.L3ReactiveEnergyDraw.Name,    "0" },
            { Obis.L3ReactiveEnergyInput.Name,   "0.081" },

            { Obis.L1Voltage.Name,               "234.7" },
            { Obis.L2Voltage.Name,               "234.6" },
            { Obis.L3Voltage.Name,               "234.5" },

            { Obis.L1Current.Name,               "1.6" },
            { Obis.L2Current.Name,               "2.3" },
            { Obis.L3Current.Name,               "0.4" }
        };
    }
}
