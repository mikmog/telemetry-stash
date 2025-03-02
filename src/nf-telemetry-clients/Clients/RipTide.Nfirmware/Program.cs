using NeoPixel.Peripheral;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using TelemetryStash.AdcSensor;


namespace RipTide.Nfirmware
{
    public class Program
    {
        public static void Main()
        {
            Thread.Sleep(2000);

            var gauge = new NeoPixelGauge(pixelsCount: 45, new[] { Color.Green, Color.Yellow, Color.Red }, pin: 11);
            gauge.Initialize();

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

                gauge.SetPosition(value);
            }



            Debug.WriteLine("Zzzz");
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
