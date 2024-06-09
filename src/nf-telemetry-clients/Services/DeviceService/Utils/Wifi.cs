using nanoFramework.Networking;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace TelemetryStash.DeviceService.Properties
{
    // https://github.com/nanoframework/System.Device.Wifi

    public class Wifi
    {
        public static void ConnectToWifi()
        {
            // The wifi credentials are already stored on the device
            // VisualStudio -> Device Explorer -> Network Configuration
            if (WifiNetworkHelper.Status != NetworkHelperStatus.NetworkIsReady)
            {
                // Give 60 seconds to the wifi join to happen
                var cs = new CancellationTokenSource(60000);
                var success = WifiNetworkHelper.Reconnect(requiresDateTime: true, token: cs.Token);
                if (success)
                {
                    Debug.WriteLine("Connected to Wifi, IP: " + IPAddress.GetDefaultLocalAddress());
                }
                else
                {
                    var message = "Can't connect to Wifi, error: " + WifiNetworkHelper.Status;
                    throw new Exception(message, WifiNetworkHelper.HelperException);
                }
            }
            else
            {
                Debug.WriteLine("Already connected to Wifi, IP: " + IPAddress.GetDefaultLocalAddress());
            }
        }
    }
}
