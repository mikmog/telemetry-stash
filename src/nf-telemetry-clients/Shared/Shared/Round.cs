using System;

namespace TelemetryStash.NfClient.Services
{
    public static class Round
    {
        public static double ToWhole(double dx)
        {
            return RoundTo(dx, 1);
        }

        public static double ToHalf(double dx)
        {
            return RoundTo(dx, 0.5);
        }

        public static double ToOneDecimal(double dx)
        {
            return RoundTo(dx, 0.1);
        }

        public static double ToTwoDecimals(double dx)
        {
            return RoundTo(dx, 0.01);
        }

        public static string ToTwoDecimalString(double dx)
        {
            return RoundTo(dx, 0.01).ToString("f2");
        }

        public static string TrimZeroes(string number)
        {
            var trimmedNumber = number.TrimStart('0').TrimEnd('0').TrimEnd('.');
            if (trimmedNumber == "")
            {
                trimmedNumber = "0";
            }
            else if (trimmedNumber[0] == '.')
            {
                trimmedNumber = "0" + trimmedNumber;
            }

            return trimmedNumber;
        }

        public static string TrimTrailingZeroes(string number)
        {
            var trimmedNumber = number.TrimEnd('0').TrimEnd('.');
            if (trimmedNumber == "")
            {
                trimmedNumber = "0";
            }
            else if (trimmedNumber[0] == '.')
            {
                trimmedNumber = "0" + trimmedNumber;
            }

            return trimmedNumber;
        }

        private static double RoundTo(double value, double snap)
        {
            if (snap <= 1f)
                value = Math.Floor(value) + (Math.Round((value - Math.Floor(value)) * (1f / snap)) * snap);
            else
                value = Math.Round(value / snap) * snap;

            return value;
        }
    }
}
