using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using TelemetryStash.Shared;

namespace TelemetryStash.NfClient.Services
{
    public class DebugInformation
    {
        [Conditional("DEBUG")]
        public void PrintStartupMessage()
        {
            // https://patorjk.com/software/taag/#p=display&h=1&f=Small%20Slant&t=Telemetry%20%20Stash
            Debug.WriteLine(
@"
 ______      __                 __                ____ __              __ 
/_  __/___  / /___  __ _  ___  / /_ ____ __ __   / __// /_ ___ _ ___  / / 
 / /  / -_)/ // -_)/  ' \/ -_)/ __// __// // /  _\ \ / __// _ `/(_-< / _ \
/_/   \__//_/ \__//_/_/_/\__/ \__//_/   \_, /  /___/ \__/ \_,_//___//_//_/
                                       /___/                              
"
                );
        }

        [Conditional("DEBUG")]
        public void PrintSystemInfo()
        {
            Debug.WriteLine();
            Debug.WriteLine("---------------------------------------------------------------------------");

            static string BytesToKb(string bytes) => Round.ToTwoDecimalString(int.Parse(bytes) / 1024d);

            var metrics = DeviceMetrics.GetDeviceMetrics();
            foreach (RegisterSet registerSet in metrics)
            {
                Debug.WriteLine(registerSet.Identifier);
                foreach (var register in registerSet.Registers)
                {
                    if (registerSet.Identifier == "Memory")
                    {
                        Debug.WriteLine("> " + register.Identifier + ": " + BytesToKb(register.Value) + " KB");
                    }
                    else
                    {
                        Debug.WriteLine("> " + register.Identifier + ": " + register.Value);
                    }
                }

                Debug.WriteLine();
            }

            Debug.WriteLine("Serial ports:");
            foreach (var serialPort in SerialPort.GetPortNames())
            {
                Debug.WriteLine("> Port: " + serialPort);
            }

            Debug.WriteLine();
            foreach (var drive in DriveInfo.GetDrives())
            {
                Debug.WriteLine("Drive " + drive.Name);
                Debug.WriteLine("> Drive type: " + drive.DriveType);
                Debug.WriteLine("> Total size: " + drive.TotalSize);

                foreach (var file in Directory.GetFiles(drive.Name))
                {
                    Debug.WriteLine("> File: " + file + ", Attributes: " + File.GetAttributes(file));
                }

                foreach (var directory in Directory.GetDirectories(drive.Name))
                {
                    Debug.WriteLine("> Directory: " + directory);
                }
            }

            Debug.WriteLine("---------------------------------------------------------------------------");
            Debug.WriteLine();
        }
    }
}
