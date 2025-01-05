using System;
using System.Collections;
using System.Device.Wifi;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.Shared;

namespace TelemetryStash.Peripherals.WifiSensor
{
    public class WifiSensor
    {
        private readonly TimeSpan _timerInerval = TimeSpan.FromSeconds(5);
        private Timer _notificationTimer;

        private readonly ArrayList _telemetry = new();
        private readonly Hashtable _eventHistory = new(1, 1f);

        private bool _running = false;

        private Thread _scannerThread;

        private readonly TimeSpan _retentionInterval;
        private readonly short _preferredBatchSize;

        public WifiSensor(TimeSpan networkRetentionInterval, short preferredBatchSize)
        {
            _retentionInterval = networkRetentionInterval;
            _preferredBatchSize = preferredBatchSize;
        }

        public void Start()
        {
            _running = true;

            TrimEventHistory();
            _notificationTimer = new Timer((_) => NotifyDataReceived(forceNotification: true), null, _timerInerval, _timerInerval);

            if (_scannerThread == null)
            {
                var scanner = new Scanner(Wifi_NetworkFound);
                _scannerThread = new Thread(scanner.ScannerRunner);
                _scannerThread.Start();
            }
            else
            {
                _scannerThread.Resume();
            }
        }

        public void Stop()
        {
            _scannerThread.Suspend();

            _notificationTimer.Dispose();
            _notificationTimer = null;

            NotifyDataReceived(forceNotification: true);

            _running = false;
        }

        private void Wifi_NetworkFound(WifiAvailableNetwork network)
        {
            if (!_running)
            {
                return;
            }

            var utcNow = DateTime.UtcNow;

            lock (_eventHistory)
            {
                var expiry = _eventHistory[network.Bsid];
                if (expiry == null || (DateTime)expiry < utcNow)
                {
                    _eventHistory[network.Bsid] = utcNow.Add(_retentionInterval);
                }
                else
                {
                    // Still in retention period
                    return;
                }
            }

            var telemetry = MapNetworkToTelemetry(network);
            lock (_telemetry)
            {
                _telemetry.Add(telemetry);
            }

            NotifyDataReceived();
        }

        private void NotifyDataReceived(bool forceNotification = false)
        {
            if (!_running)
            {
                return;
            }

            lock (_telemetry)
            {
                if (_telemetry.Count == 0)
                {
                    return;
                }

                if (forceNotification || _telemetry.Count >= _preferredBatchSize)
                {
                    // Postpone next notification
                    if (_notificationTimer != null)
                    {
                        _notificationTimer.Change(_timerInerval, _timerInerval);
                    }

                    var telemetry = (ArrayList)_telemetry.Clone();
                    DataReceived?.Invoke(telemetry);
                    _telemetry.Clear();
                }
            }
        }

        private void TrimEventHistory()
        {
            lock (_eventHistory)
            {
                if (_eventHistory.Count == 0)
                {
                    return;
                }

                var utcNow = DateTime.UtcNow;
                var keys = new ArrayList();
                foreach (DictionaryEntry entry in _eventHistory)
                {
                    if ((DateTime)entry.Value < utcNow)
                    {
                        keys.Add(entry.Key);
                    }
                }

                foreach (var key in keys)
                {
                    _eventHistory.Remove(key);
                }

                Debug.WriteLine($"Trimmed {keys.Count} Wifi event histories. Current count {_eventHistory.Count}");
            }
        }

        private RegisterSet MapNetworkToTelemetry(WifiAvailableNetwork network)
        {
            var macAddress = string.Concat(network.Bsid.Split('-'));
            var identifier = string.IsNullOrEmpty(network.Ssid) ? macAddress : network.Ssid;

            return new RegisterSet
            {
                Identifier = "Wifi:" + identifier,
                Registers = new Register[]
                {
                    new ("RSSI", network.NetworkRssiInDecibelMilliwatts, DecimalPrecision.None),
                    new ("Bars", network.SignalBars),
                    new ("Freq", "2.4", RegisterValueType.Number),
                    new ("Ssid", network.Ssid),
                    new ("Mac", macAddress),
                    new ("Kind", (int)network.NetworkKind),
                }
            };
        }

        public delegate void DataReceivedEventHandler(ArrayList registerSets);
        public event DataReceivedEventHandler DataReceived;
    }
}
