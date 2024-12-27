using nanoFramework.Hardware.Esp32;
using nanoFramework.Runtime.Native;
using System;
using System.Collections;
using System.Net.NetworkInformation;
using TelemetryStash.Shared;
using GC = nanoFramework.Runtime.Native.GC;

namespace TelemetryStash.NfClient.Services
{
    public static class DeviceMetrics
    {
        private static DateTime _deviceStartUtc = DateTime.MinValue;

        public static void RegisterDeviceStart()
        {
            if (_deviceStartUtc == DateTime.MinValue)
            {
                _deviceStartUtc = DateTime.UtcNow;
            }
        }

        public static ArrayList GetDeviceMetrics()
        {
            RegisterDeviceStart();

            var freeRam = GC.Run(false);

            NativeMemory.GetMemoryInfo(NativeMemory.MemoryType.SpiRam, out uint totalSpiMem, out uint totalFreeSpiMem, out uint largestFreeSpiMemBlock);
            NativeMemory.GetMemoryInfo(NativeMemory.MemoryType.Internal, out uint totalInternalMem, out uint totalFreeInternalMem, out uint largestFreeInternalMemBlock);

            string macAddress = null;
            string ipAddress = null;

            // Only one nic on ESP32
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            if (interfaces.Length > 0)
            {
                var nic = interfaces[0];

                macAddress = nic.PhysicalAddress.ToHexString();
                ipAddress = nic.IPv4Address;
            }

            return new ArrayList
                {
                    new RegisterSet
                    {
                        Identifier = "Runtime",
                        Registers = new Register[]
                        {
                            new ("Startup", _deviceStartUtc.ToString("s")),
                            new ("Uptime", (DateTime.UtcNow - _deviceStartUtc).TotalSeconds, DecimalPrecision.None),
                            new ("Ip", ipAddress ?? "0"),
                            new ("Mac", macAddress ?? "0")
                        }
                    },
                    new RegisterSet
                    {
                        Identifier = "SysInfo",
                        Registers = new Register[]
                        {
                            new ("Platform", SystemInfo.Platform),
                            new ("OEM", SystemInfo.OEMString),
                            new ("Target", SystemInfo.TargetName),
                            new ("Ver", SystemInfo.Version.ToString()),
                            new ("Float", SystemInfo.FloatingPointSupport.ToString()),
                        }
                    },
                    new RegisterSet
                    {
                        Identifier = "Memory",
                        Registers = new Register[]
                        {
                            new ("FreeRam", freeRam),
                            new ("TotSpi", totalSpiMem),
                            new ("FreeSpi", totalFreeSpiMem),
                            new ("MaxSpiBlock", largestFreeSpiMemBlock),
                            new ("TotInt", totalInternalMem),
                            new ("IntFree", totalFreeInternalMem),
                            new ("MaxIntBlock", largestFreeInternalMemBlock)
                        }
                    },
            };
        }
    }
}
