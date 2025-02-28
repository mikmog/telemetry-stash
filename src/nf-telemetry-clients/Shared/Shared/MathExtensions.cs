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
    }
}
