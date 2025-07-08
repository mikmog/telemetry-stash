using System.Collections;
using TelemetryStash.Shared;

namespace TelemetryStash.IliDisplay
{
    public class IliDisplaySettings
    {
        private const string BackLightPinKey = "Ili9.BackLightPin";
        private const string ChipSelectPinKey = "Ili9.ChipSelectPin";
        private const string DataCommandPinKey = "Ili9.DataCommandPin";
        private const string ResetPinKey = "Ili9.ResetPin";
        private const string SpiMisoPinKey = "Ili9.SpiMisoPin";
        private const string SpiMosiPinKey = "Ili9.SpiMosiPin";
        private const string SpiClockPinKey = "Ili9.SpiClockPin";

        public int BackLightPin { get; set; }
        public int ChipSelectPin { get; set; }
        public int DataCommandPin { get; set; }
        public int ResetPin { get; set; }

        public int SpiMisoPin { get; set; }
        public int SpiMosiPin { get; set; }
        public int SpiClockPin { get; set; }

        public void Configure(IDictionary dictionary)
        {
            BackLightPin = dictionary.Int32(BackLightPinKey);
            ChipSelectPin = dictionary.Int32(ChipSelectPinKey);
            DataCommandPin = dictionary.Int32(DataCommandPinKey);
            ResetPin = dictionary.Int32(ResetPinKey);
            SpiMisoPin = dictionary.Int32(SpiMisoPinKey);
            SpiMosiPin = dictionary.Int32(SpiMosiPinKey);
            SpiClockPin = dictionary.Int32(SpiClockPinKey);
        }

        /* pros3
         "Ili9.BackLightPin": 38,
          "Ili9.ChipSelectPin": 6,
          "Ili9.DataCommandPin": 7,
          "Ili9.ResetPin": 34,
          "Ili9.SpiMisoPin": 37,
          "Ili9.SpiMosiPin": 35,
          "Ili9.SpiClockPin": 36,
         */

        /* wrover
         
            int backLightPin = 5;
            int chipSelect = 6;
            int dataCommand = 7;
            int reset = 15;

            // MOSI => FSPID => sdi
            // MISO => FSPIQ => sdo
            // SCK => FSPICLK
            Configuration.SetPinFunction(13, DeviceFunction.SPI1_MISO);
            Configuration.SetPinFunction(11, DeviceFunction.SPI1_MOSI);
            Configuration.SetPinFunction(12, DeviceFunction.SPI1_CLOCK);
         
         */
    }
}
