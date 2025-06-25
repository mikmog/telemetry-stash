using System;
using System.Collections;
using TelemetryStash.Shared;

namespace TelemetryStash.Aidon.Sensor
{
    public static class AidonMessageParser
    {
        public static Telemetry Parse(string message)
        {
            var telemetry = new Telemetry
            {
                Timestamp = ParseTimestamp(Obis.Timestamp),
                RegisterSets = new ArrayList()
                {
                    new RegisterSet
                    {
                        Identifier = "P1",
                        Registers =  new[]
                        {
                            Parse(Obis.TotActiveEnergy),
                            Parse(Obis.TotActiveEnergyInput),
                            Parse(Obis.TotReactiveEnergyDraw),
                            Parse(Obis.TotReactiveEnergyInput),
                            Parse(Obis.ActiveEnergyDraw),
                            Parse(Obis.ActiveEnergyInput),

                            Parse(Obis.ReactiveEnergyDraw),
                            Parse(Obis.ReactiveEnergyInput),
                            Parse(Obis.L1EnergyDraw),
                            Parse(Obis.L1EnergyInput),
                            Parse(Obis.L2EnergyDraw),
                            Parse(Obis.L2EnergyInput),
                            Parse(Obis.L3EnergyDraw),
                            Parse(Obis.L3EnergyInput),

                            Parse(Obis.L1ReactiveEnergyDraw),
                            Parse(Obis.L1ReactiveEnergyInput),
                            Parse(Obis.L2ReactiveEnergyDraw),
                            Parse(Obis.L2ReactiveEnergyInput),
                            Parse(Obis.L3ReactiveEnergyDraw),
                            Parse(Obis.L3ReactiveEnergyInput),

                            Parse(Obis.L1Voltage),
                            Parse(Obis.L2Voltage),
                            Parse(Obis.L3Voltage),

                            Parse(Obis.L1Current),
                            Parse(Obis.L2Current),
                            Parse(Obis.L3Current)
                        }
                    }
                }
            };

            return telemetry;
            
            Register Parse(ObisCode obis)
            {
                message = message.Substring(message.IndexOf(obis.Code));
                var number = Round.TrimZeroes(message.Substring(obis.CodeLength, obis.FieldLength));

                return new Register(obis.Name, number, RegisterValueType.Number);
            }

            DateTime ParseTimestamp(ObisCode obis)
            {
                message = message.Substring(message.IndexOf(obis.Code));
                var dateStr = message.Substring(obis.CodeLength, obis.FieldLength);

                var timestamp = new DateTime(
                   int.Parse(dateStr.Substring(0, 2)) + 2000,
                   int.Parse(dateStr.Substring(2, 2)),
                   int.Parse(dateStr.Substring(4, 2)),
                   int.Parse(dateStr.Substring(6, 2)) - (dateStr.Substring(12, 1) == "W" ? 1 : 2), // Wintertime/Summertime
                   int.Parse(dateStr.Substring(8, 2)),
                   int.Parse(dateStr.Substring(10, 2))
               );

                return timestamp;
            }
        }
    }
}
