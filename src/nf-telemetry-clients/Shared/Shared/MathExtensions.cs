using System;

namespace TelemetryStash.Shared
{
    public static class MathExtensions
    {
        public static int Abs(this int value)
        {
            return value < 0 ? -value : value;
        }

        public static int Sum(this int[] array)
        {
            var total = 0;
            foreach (var value in array)
            {
                total += value;
            }
            return total;
        }

        public static void MinMax(this int[] array, out int minValue, out int maxValue)
        {
            minValue = array[0];
            maxValue = minValue;

            for (var i = 1; i < array.Length; i++)
            {
                var next = array[i];
                if (next < minValue)
                {
                    minValue = next;
                }
                else if (next > maxValue)
                {
                    maxValue = next;
                }
            }
        }

        public static int Max(int value1, int value2)
        {
            return value1 > value2 ? value1 : value2;
        }

        public static int Average(this int[] array)
        {
            return array.Sum() / array.Length;
        }

        public static int[] TrimArray(this int[] array, int count)
        {
            var trimmedArray = new int[array.Length - count * 2];
            Array.Copy(array, count, trimmedArray, 0, trimmedArray.Length);
            return trimmedArray;
        }

        public static int[] SortArray(this int[] array)
        {
            return SortArray(array, 0, array.Length - 1);
        }

        public static int Clamp(int min, int value, int max)
        {
            if (min > max)
            {
                throw new ArgumentException("Min cannot be greater than max");
            }

            if (value < min && value > max)
            {
                throw new ArgumentException("Value is outside the range of min and max");
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

        private static int[] SortArray(int[] array, int leftIndex, int rightIndex)
        {
            var i = leftIndex;
            var j = rightIndex;
            var pivot = array[leftIndex];
            while (i <= j)
            {
                while (array[i] < pivot)
                {
                    i++;
                }

                while (array[j] > pivot)
                {
                    j--;
                }

                if (i <= j)
                {
                    var temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                    i++;
                    j--;
                }
            }

            if (leftIndex < j)
                SortArray(array, leftIndex, j);
            if (i < rightIndex)
                SortArray(array, i, rightIndex);
            return array;
        }
    }
}
