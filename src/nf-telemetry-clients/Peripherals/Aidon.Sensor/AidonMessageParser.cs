using System;
using System.Collections;
using TelemetryStash.ServiceModels;

namespace TelemetryStash.Aidon.Sensor
{
    public static class AidonMessageParser
    {
        public static Telemetry Parse(string message, string registerSetIdentifier)
        {
            var telemetry = new Telemetry
            {
                Timestamp = ParseTimestamp(Obis.Timestamp),
                RegisterSets = new ArrayList()
                {
                    new RegisterSet
                    {
                        Identifier = registerSetIdentifier,
                        Registers =  new[]
                        {
                            ParseFloat(Obis.TotActiveEnergy),
                            ParseFloat(Obis.TotActiveEnergyInput),
                            ParseFloat(Obis.TotReactiveEnergyDraw),
                            ParseFloat(Obis.TotReactiveEnergyInput),
                            ParseFloat(Obis.ActiveEnergyDraw),
                            ParseFloat(Obis.ActiveEnergyInput),

                            ParseFloat(Obis.ReactiveEnergyDraw),
                            ParseFloat(Obis.ReactiveEnergyInput),
                            ParseFloat(Obis.L1EnergyDraw),
                            ParseFloat(Obis.L1EnergyInput),
                            ParseFloat(Obis.L2EnergyDraw),
                            ParseFloat(Obis.L2EnergyInput),
                            ParseFloat(Obis.L3EnergyDraw),
                            ParseFloat(Obis.L3EnergyInput),

                            ParseFloat(Obis.L1ReactiveEnergyDraw),
                            ParseFloat(Obis.L1ReactiveEnergyInput),
                            ParseFloat(Obis.L2ReactiveEnergyDraw),
                            ParseFloat(Obis.L2ReactiveEnergyInput),
                            ParseFloat(Obis.L3ReactiveEnergyDraw),
                            ParseFloat(Obis.L3ReactiveEnergyInput),

                            ParseFloat(Obis.L1Voltage),
                            ParseFloat(Obis.L2Voltage),
                            ParseFloat(Obis.L3Voltage),

                            ParseFloat(Obis.L1Current),
                            ParseFloat(Obis.L2Current),
                            ParseFloat(Obis.L3Current)
                        }
                    }
                }
            };

            return telemetry;

            Register ParseFloat(ObisCode obis)
            {
                message = message.Substring(message.IndexOf(obis.Code));

                var numberParts = message.Substring(obis.CodeLength, obis.FieldLength);
                var leftIntPart = numberParts.Substring(0, numberParts.IndexOf('.'));
                var rightFractionPart = numberParts.Substring(numberParts.IndexOf('.') + 1);

                if (!int.TryParse(leftIntPart, out var intPart))
                {
                    var msg =
                        "Error parsing integer part " + leftIntPart + " for" + obis.Code +
                        " from " + message.Substring(obis.CodeLength, obis.FieldLength);

                    throw new Exception(msg);
                }

                string fractionsReversed = null;
                for (var i = rightFractionPart.Length - 1; i >= 0; i--)
                {
                    fractionsReversed += rightFractionPart[i];
                }

                if (!short.TryParse(fractionsReversed, out var fractionPart))
                {
                    var msg =
                        "Error parsing fraction part " + fractionsReversed + " for " + obis.Code +
                        " from " + message.Substring(obis.CodeLength, obis.FieldLength);

                    throw new Exception(msg);
                }

                return new Register(obis.Name, intPart, fractionPart);
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
