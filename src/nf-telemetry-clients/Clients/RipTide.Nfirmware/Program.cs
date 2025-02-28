using System.Diagnostics;
using System.Threading;
using TelemetryStash.AdcSensor;


namespace RipTide.Nfirmware
{
    public class Program
    {
        public static void Main()
        {
            Thread.Sleep(2000);

            var ss49e = new Ss49eHallSensor(new int[] { 0, 1 }, adcReadScale: 45, true);
            ss49e.CalibrateAdcChannelOffsets();
            while (true)
            {
                var value = ss49e.Read();
                if (value == -1)
                {
                    ss49e.CalibrateAdcChannelOffsets();
                    continue;
                }

                Debug.WriteLine($"Value: {value}");
            }

            Debug.WriteLine("Zzzz");
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
