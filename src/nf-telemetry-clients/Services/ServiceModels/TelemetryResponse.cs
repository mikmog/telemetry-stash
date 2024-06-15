using System;
using System.Collections;

namespace TelemetryStash.ServiceModels
{
    [Serializable]
    public class Telemetry
    {
        public DateTime Timestamp { get; set; }

        public ArrayList RegisterSets { get; set; }
    };

    [Serializable]
    public class RegisterSet
    {
        public string Identifier { get; set; }

        public Register[] Registers { get; set; }
    }

    [Serializable]
    public class Register
    {
        public Register(string identifier, int intPart, short fractions)
        {
            Identifier = identifier;
            IntPart = intPart;
            Fractions = fractions;
        }

        public string Identifier { get; set; }
        public int IntPart { get; set; }
        public short Fractions { get; set; }
    }

    public static class RegisterExtension
    {
        public static string ToNumberString(this Register number)
        {
            if (number.Fractions == 0)
            {
                return number.IntPart.ToString();
            }

            string fractionsReversed = null;
            var fractions = number.Fractions.ToString();
            for (var i = fractions.Length - 1; i >= 0; i--)
            {
                fractionsReversed += fractions[i];
            }

            return number.IntPart + "." + fractionsReversed;
        }

        public static Register ToRegister(string identifier, double value, ushort numberOfDecimals)
        {
            if (numberOfDecimals <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfDecimals), "Number of decimals must be 0 or greater");
            }

            if (value == 0d)
            {
                return new Register(identifier, 0, 0);
            }

            var fractions = value
                .ToString("N" + numberOfDecimals)
                .Split('.')[1]
                .TrimEnd('0');

            if (fractions == "")
            {
                return new Register(identifier, (int)value, 0);
            }

            string fractionsReversed = null;
            for (var i = fractions.Length - 1; i >= 0; i--)
            {
                fractionsReversed += fractions[i];
            }

            return new Register(identifier, (int)value, short.Parse(fractionsReversed));
        }
    }
}
