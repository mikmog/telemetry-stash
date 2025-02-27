using nanoFramework.Hardware.Esp32;
using nanoFramework.System.IO.FileSystem;
using System;
using System.Diagnostics;
using System.Threading;

namespace TelemetryStash.Peripherals.SdCard
{
    public class MicroSdCard
    {
        public MicroSdCard()
        {
            // MOSI => FSPID
            // MISO => FSPIQ
            // SCK => FSPICLK
            Configuration.SetPinFunction(13, DeviceFunction.SPI1_MISO);
            Configuration.SetPinFunction(11, DeviceFunction.SPI1_MOSI);
            Configuration.SetPinFunction(12, DeviceFunction.SPI1_CLOCK);
        }

        public void RunDemo()
        {
            uint chipSelect = 6;

            var spiParameters = new SDCardSpiParameters { spiBus = 1, chipSelectPin = chipSelect, slotIndex = 0 };
            var microSd = new SDCard(spiParameters);

            Debug.WriteLine("SDcard inited. Mounted: " + microSd.IsMounted);
            
            Thread.Sleep(5000);

            for (int i = 0; i < 100; i++)
            {
                try
                {
                    Debug.WriteLine($"Mounting card");

                    // TODO: Mounting the card is not working..

                    microSd.Mount();
                    Debug.WriteLine("Card Mounted!");
                    Thread.Sleep(Timeout.Infinite);
                    microSd.Unmount();
                    microSd.Dispose();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Card failed to mount : {ex.Message}");
                    Debug.WriteLine($"IsMounted {microSd.IsMounted}");
                    Debug.WriteLine($"IsCardDetected {microSd.IsCardDetected}");
                    Debug.WriteLine($"CardType {microSd.CardType}");
                    Thread.Sleep(2000);
                }
            }

            Debug.WriteLine("Proceding");
        }
    }
}
