using System;

namespace TelemetryStash.Shared
{
    public static class MathExtensions
    {
        public static int Sum(this int[] array)
        {
            var total = 0;
            foreach (var value in array)
            {
                total += value;
            }
            return total;
        }

        public static int Average(this int[] array)
        {
            return array.Sum() / array.Length;
        }

        public static ushort Clamp(ushort min, ushort value, ushort max)
        {
            if (min > max)
            {
                throw new ArgumentException("Min cannot be greater than max");
            }

            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }

            return value;
        }
    }
}
