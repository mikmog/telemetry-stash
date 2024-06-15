using nanoFramework.Hardware.Esp32;
using nanoFramework.Runtime.Native;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;

namespace TelemetryStash.DeviceService.Properties
{
    public class Printer
    {
        [Conditional("DEBUG")]
        public void PrintStartupMessage()
        {
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
            Debug.WriteLine("----------------------------------------------");

            Debug.WriteLine("Platform: " + SystemInfo.Platform);
            Debug.WriteLine("Manufacturer: " + SystemInfo.OEMString);
            Debug.WriteLine("TargetName: " + SystemInfo.TargetName);
            Debug.WriteLine("NanoFramework version: " + SystemInfo.Version);
            Debug.WriteLine("Floating point precision: " + SystemInfo.FloatingPointSupport);

            Debug.WriteLine();
            PrintMemoryInfo(NativeMemory.MemoryType.Internal);
            PrintMemoryInfo(NativeMemory.MemoryType.SpiRam);

            Debug.WriteLine();
            Debug.WriteLine("Serial ports:");
            foreach (var serialPort in SerialPort.GetPortNames())
            {
                Debug.WriteLine("   Port: " + serialPort);
            }

            Debug.WriteLine();
            foreach (var drive in Directory.GetLogicalDrives())
            {
                Debug.WriteLine("Drive: " + drive);
                foreach (var file in Directory.GetFiles(drive))
                {
                    Debug.WriteLine("   File: " + file + ", " + File.GetLastWriteTime(file));
                }

                foreach (var file in Directory.GetDirectories(drive))
                {
                    Debug.WriteLine("   Directory: " + file);
                }
            }

            Debug.WriteLine("----------------------------------------------");
            Debug.WriteLine();
        }

        private void PrintMemoryInfo(NativeMemory.MemoryType memoryType = NativeMemory.MemoryType.Internal)
        {
            var memoryTypeName = memoryType == NativeMemory.MemoryType.Internal ? "Internal" : "SpiRam";
            NativeMemory.GetMemoryInfo(memoryType, out uint totalSize, out uint totalFreeSize, out uint largestFreeBlock);
            Debug.WriteLine(memoryTypeName + " memory, total size KB: " + totalSize / 1024f);
            Debug.WriteLine(memoryTypeName + " memory, total free KB: " + totalFreeSize / 1024f);
            Debug.WriteLine(memoryTypeName + " memory, largest free block KB: " + largestFreeBlock / 1024f);
        }
    }
}
