using System.Device.Wifi;
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
                    adapter.ScanAsync();

                    while (!_scanComplete)
                    {
                        Thread.Sleep(500);
                    }
                }
            }
        }

        private void Wifi_AvailableNetworksChanged(WifiAdapter sender, object e)
        {
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
