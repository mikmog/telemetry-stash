using System;
using System.Device.Wifi;
using System.Diagnostics;
using System.Threading;

namespace TelemetryStash.Peripherals.WifiSensor
{
    internal class Scanner
    {
        private readonly ScannerCallback _callback;
        private readonly WifiAdapter[] _adapters;
        private bool _scanComplete = false;

        public Scanner(ScannerCallback callbackDelegate)
        {
            _callback = callbackDelegate;
            _adapters = WifiAdapter.FindAllAdapters();
            foreach (var adapter in _adapters)
            {
                adapter.AvailableNetworksChanged += Wifi_AvailableNetworksChanged;
            }
        }

        public void ScannerRunner()
        {
            while (true)
            {
                foreach (var adapter in _adapters)
                {
                    _scanComplete = false;

                    try
                    {
                        adapter.ScanAsync();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error scanning Wifi networks: {ex.Message}");
                        _scanComplete = true;
                    }

                    while (!_scanComplete)
                    {
                        Thread.Sleep(500);
                    }

                    // Give the adapter some breathing room
                    Thread.Sleep(500);
                }
            }
        }

        private void Wifi_AvailableNetworksChanged(WifiAdapter sender, object e)
        {
            if (Thread.CurrentThread.ThreadState == ThreadState.Suspended)
            {
                _scanComplete = true;
                return;
            }

            // Get report of all scanned Wifi networks
            var report = sender.NetworkReport;
            foreach (var network in report.AvailableNetworks)
            {
                _callback?.Invoke(network);
            }

            _scanComplete = true;
        }

        public delegate void ScannerCallback(WifiAvailableNetwork network);
    }
}
