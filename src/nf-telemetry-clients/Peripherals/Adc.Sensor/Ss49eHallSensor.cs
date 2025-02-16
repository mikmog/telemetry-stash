using System.Device.Adc;
using System.Diagnostics;
using System.Threading;

namespace TelemetryStash.AdcSensor
{
    public class Ss49eHallSensor
    {
        public void RunDemo()
        {
            var adc = new AdcController();
            var channel = adc.OpenChannel(0);

            Debug.WriteLine($"ADC max: {channel.Controller.MaxValue}");
            Debug.WriteLine($"ADC min: {channel.Controller.MinValue}");
            Debug.WriteLine($"ADC resolution: {channel.Controller.ResolutionInBits}");
            Debug.WriteLine($"ADC channel count: {channel.Controller.ChannelCount}");
            Debug.WriteLine($"ADC channel mode: {channel.Controller.ChannelMode}");
            Debug.WriteLine($"ADC IsChannelModeSupported SingleEnded: {channel.Controller.IsChannelModeSupported(AdcChannelMode.SingleEnded)}");
            Debug.WriteLine($"ADC IsChannelModeSupported Differential: {channel.Controller.IsChannelModeSupported(AdcChannelMode.Differential)}");

            var currentValue = 0;
            while (true)
            {
                var value = channel.ReadValue();
                var rounded = value - (value % 50);
                if (rounded != currentValue)
                {
                    currentValue = rounded;
                    Debug.WriteLine(rounded.ToString());
                }
                
                Thread.Sleep(10);
            }
        }
    }
}
