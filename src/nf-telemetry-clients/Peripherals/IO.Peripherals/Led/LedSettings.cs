using System;
using System.Collections;

namespace TelemetryStash.IO.Peripherals.Led
{
    public class LedSettings
    {
        private const string Pin1Key = "Led1.Pin";

        public void Configure(IDictionary dictionary)
        {
            object Setting(string key) => dictionary[key] ?? throw new ArgumentException(key);

            Pin1 = (int)Setting(Pin1Key);
        }

        public int Pin1 { get; set; }
    }
}
