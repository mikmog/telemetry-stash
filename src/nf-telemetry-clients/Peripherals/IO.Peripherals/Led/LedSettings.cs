using System.Collections;
using TelemetryStash.Shared;

namespace TelemetryStash.IO.Peripherals.Led
{
    public class LedSettings
    {
        private const string Pin1Key = "Led1.Pin";

        public void Configure(IDictionary dictionary)
        {
            Pin1 = dictionary.Int32(Pin1Key);
        }

        public int Pin1 { get; set; }
    }
}
