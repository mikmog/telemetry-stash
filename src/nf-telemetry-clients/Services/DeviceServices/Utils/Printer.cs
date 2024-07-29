using nanoFramework.Hardware.Esp32;
using nanoFramework.Runtime.Native;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;

namespace TelemetryStash.DeviceServices
{
    public class Printer
    {
        [Conditional("DEBUG")]
        public void PrintStartupMessage()
        {
            // https://patorjk.com/software/taag/#p=display&h=1&f=Small%20Slant&t=Telemetry%20%20Stash
            const string msg =
@"
 ______      __                 __                ____ __              __ 
/_  __/___  / /___  __ _  ___  / /_ ____ __ __   / __// /_ ___ _ ___  / / 
 / /  / -_)/ // -_)/  ' \/ -_)/ __// __// // /  _\ \ / __// _ `/(_-< / _ \
/_/   \__//_/ \__//_/_/_/\__/ \__//_/   \_, /  /___/ \__/ \_,_//___//_//_/
                                       /___/                              
";
            Debug.WriteLine(msg);
        }

        [Conditional("DEBUG")]
        public void PrintSystemInfo()
        {
            Debug.WriteLine();
            Debug.WriteLine("---------------------------------------------------------------------------");

            Debug.WriteLine("Platform: " + SystemInfo.Platform);
            Debug.WriteLine("Manufacturer: " + SystemInfo.OEMString);
            Debug.WriteLine("TargetName: " + SystemInfo.TargetName);
            Debug.WriteLine("NanoFramework version: " + SystemInfo.Version);
            Debug.WriteLine("Floating point precision: " + SystemInfo.FloatingPointSupport);

            Debug.WriteLine();
            
            PrintMemoryInfo(NativeMemory.MemoryType.Internal);
            Debug.WriteLine();
            PrintMemoryInfo(NativeMemory.MemoryType.SpiRam);

            Debug.WriteLine();
            Debug.WriteLine("Serial ports:");
            foreach (var serialPort in SerialPort.GetPortNames())
            {
                Debug.WriteLine("> Port: " + serialPort);
            }

            Debug.WriteLine();
            foreach (var drive in DriveInfo.GetDrives())
            {
                Debug.WriteLine("Drive: " + drive.Name);
                Debug.WriteLine("> Drive type: " + drive.DriveType);
                Debug.WriteLine("> Total size: " + drive.TotalSize);

                foreach (var file in Directory.GetFiles(drive.Name))
                {
                    Debug.WriteLine(
                        "> File: " + file + ", Attributes: " + File.GetAttributes(file));
                }

                foreach (var directory in Directory.GetDirectories(drive.Name))
                {
                    Debug.WriteLine("> Directory: " + directory);
                }
            }

            Debug.WriteLine("---------------------------------------------------------------------------");
            Debug.WriteLine();
        }

        private void PrintMemoryInfo(NativeMemory.MemoryType memoryType)
        {
            NativeMemory.GetMemoryInfo(memoryType, out uint totalSize, out uint totalFreeSize, out uint largestFreeBlock);
            
            var memoryTypeName = memoryType == NativeMemory.MemoryType.Internal ? "Internal" : "SpiRam";
            Debug.WriteLine(memoryTypeName + " memory");
            Debug.WriteLine("> Total size KB: " + (totalSize / 1024f).ToString("n1"));
            Debug.WriteLine("> Total free KB: " + (totalFreeSize / 1024f).ToString("n1"));
            Debug.WriteLine("> Largest free block KB: " + (largestFreeBlock / 1024f).ToString("n1"));
        }
    }
}
