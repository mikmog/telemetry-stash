using nanoFramework.Networking;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace TelemetryStash.DeviceServices
{
    // https://github.com/nanoframework/System.Device.Wifi

    public class Wifi
    {
        public static void EnsureConnected()
        {
            // The wifi credentials are already stored on the device
            // VisualStudio -> Device Explorer -> Network Configuration
            Debug.WriteLine("Ensuring connected to WiFi" + IPAddress.GetDefaultLocalAddress());

            if (WifiNetworkHelper.Status != NetworkHelperStatus.NetworkIsReady)
            {
                // Give 60 seconds to join
                var cs = new CancellationTokenSource(60000);
                var success = WifiNetworkHelper.Reconnect(requiresDateTime: true, token: cs.Token);
                if (success)
                {
                    Debug.WriteLine("Connected to WiFi, IP: " + IPAddress.GetDefaultLocalAddress());
                }
                else
                {
                    var message = "Can't connect to WiFi, error: " + WifiNetworkHelper.Status;
                    throw new Exception(message, WifiNetworkHelper.HelperException);
                }
            }
            else
            {
                Debug.WriteLine("Connected, IP: " + IPAddress.GetDefaultLocalAddress());
            }
            
            Debug.WriteLine("");
        }
    }
}
