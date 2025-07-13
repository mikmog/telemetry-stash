using nanoFramework.Hardware.Esp32;
using System.Collections;
using TelemetryStash.Shared;

namespace TelemetryStash.IliDisplay
{
    public class IliDisplaySettings
    {
        private const string BackLightPinKey = "Ili9.BackLightPin";
        private const string BackLightPwmKey = "Ili9.BackLightPwm";
        private const string ChipSelectPinKey = "Ili9.ChipSelectPin";
        private const string DataCommandPinKey = "Ili9.DataCommandPin";
        private const string ResetPinKey = "Ili9.ResetPin";
        private const string SpiBusKey = "Ili9.SpiBus";
        private const string SpiMisoPinKey = "Ili9.SpiMisoPin";
        private const string SpiMisoKey = "Ili9.SpiMiso";
        private const string SpiMosiPinKey = "Ili9.SpiMosiPin";
        private const string SpiMosiKey = "Ili9.SpiMosi";
        private const string SpiClockPinKey = "Ili9.SpiClockPin";
        private const string SpiClockKey = "Ili9.SpiClock";

        public int BackLightPin { get; set; }
        public DeviceFunction BackLightPwm { get; set; }
        public int ChipSelectPin { get; set; }
        public int DataCommandPin { get; set; }
        public int ResetPin { get; set; }
        public byte SpiBus { get; set; }

        public DeviceFunction SpiMiso { get; set; }
        public int SpiMisoPin { get; set; }

        public DeviceFunction SpiMosi { get; set; }
        public int SpiMosiPin { get; set; }

        public DeviceFunction SpiClock { get; set; }
        public int SpiClockPin { get; set; }

        public void Configure(IDictionary dictionary)
        {
            BackLightPin = dictionary.Int32(BackLightPinKey);
            BackLightPwm = dictionary.DeviceFunction(BackLightPwmKey);
            ChipSelectPin = dictionary.Int32(ChipSelectPinKey);
            DataCommandPin = dictionary.Int32(DataCommandPinKey);
            ResetPin = dictionary.Int32(ResetPinKey);
            SpiBus = dictionary.Byte(SpiBusKey);

            SpiMisoPin = dictionary.Int32(SpiMisoPinKey);
            SpiMiso = dictionary.DeviceFunction(SpiMisoKey);

            SpiMosiPin = dictionary.Int32(SpiMosiPinKey);
            SpiMosi = dictionary.DeviceFunction(SpiMosiKey);

            SpiClockPin = dictionary.Int32(SpiClockPinKey);
            SpiClock = dictionary.DeviceFunction(SpiClockKey);
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
