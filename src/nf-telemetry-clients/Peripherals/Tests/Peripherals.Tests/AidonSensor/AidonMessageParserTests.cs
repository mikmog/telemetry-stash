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
            Assert.AreEqual(new DateTime(2023, 11, 07, 19, 30, 00), response.Timestamp);

            var registers = ((RegisterSet)response.RegisterSets[0]).Registers;
            foreach (var register in registers)
            {
                Assert.AreEqual(ObisTable[register.Identifier], register.Value, register.Identifier);
            }
        }

        private Hashtable ObisTable => new()
        {
            { Obis.TotActiveEnergy.Name,         "3501.926" },
            { Obis.TotActiveEnergyInput.Name,    "0" },
            { Obis.TotReactiveEnergyDraw.Name,   "2.464" },
            { Obis.TotReactiveEnergyInput.Name,  "1178.103" },

            { Obis.ActiveEnergyDraw.Name,        "3.304" },
            { Obis.ActiveEnergyInput.Name,       "0" },

            { Obis.ReactiveEnergyDraw.Name,      "0" },
            { Obis.ReactiveEnergyInput.Name,     "0.561" },

            { Obis.L1EnergyDraw.Name,            "1.222" },
            { Obis.L1EnergyInput.Name,           "0" },
            { Obis.L2EnergyDraw.Name,            "1.02" },
            { Obis.L2EnergyInput.Name,           "0" },
            { Obis.L3EnergyDraw.Name,            "1.067" },
            { Obis.L3EnergyInput.Name,           "0" },

            { Obis.L1ReactiveEnergyDraw.Name,    "0" },
            { Obis.L1ReactiveEnergyInput.Name,   "0.215" },
            { Obis.L2ReactiveEnergyDraw.Name,    "0" },
            { Obis.L2ReactiveEnergyInput.Name,   "0.246" },
            { Obis.L3ReactiveEnergyDraw.Name,    "0" },
            { Obis.L3ReactiveEnergyInput.Name,   "0.088" },

            { Obis.L1Voltage.Name,               "229.5" },
            { Obis.L2Voltage.Name,               "229.6" },
            { Obis.L3Voltage.Name,               "229.5" },

            { Obis.L1Current.Name,               "5.4" },
            { Obis.L2Current.Name,               "4.5" },
            { Obis.L3Current.Name,               "4.6" }
        };
    }
}
