using nanoFramework.Device.Bluetooth;
using nanoFramework.Device.Bluetooth.Advertisement;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.Shared;

// https://www.bluetooth.com/wp-content/uploads/Files/Specification/HTML/Assigned_Numbers/out/en/Assigned_Numbers.pdf?v=1722629534741

namespace TelemetryStash.Peripherals.BluetoothSensor
{
    public class BluetoothSensor
    {
        private readonly TimeSpan _timerInerval = TimeSpan.FromSeconds(5);
        private Timer _notificationTimer;

        private readonly ArrayList _telemetry = new();
        private readonly Hashtable _eventHistory = new(1, 1f);

        private bool _running = false;

        private readonly BluetoothLEAdvertisementWatcher _bluetoothWatcher;

        private readonly TimeSpan _retentionInterval;
        private readonly short _preferredBatchSize;

        public BluetoothSensor(TimeSpan deviceRententionInterval, short preferredBatchSize)
        {
            _bluetoothWatcher = new BluetoothLEAdvertisementWatcher
            {
                ScanningMode = BluetoothLEScanningMode.Active
            };
            _bluetoothWatcher.Received += Advertisement_Received;

            _retentionInterval = deviceRententionInterval;
            _preferredBatchSize = preferredBatchSize;
        }

        public void Start()
        {
            _running = true;

            TrimEventHistory();
            var utcNow = DateTime.UtcNow;
            
            _notificationTimer = new Timer((_) => NotifyDataReceived(utcNow, forceNotification: true), null, _timerInerval, _timerInerval);
            _bluetoothWatcher.Start();
        }

        public void Stop()
        {
            _bluetoothWatcher.Stop();
            _notificationTimer.Dispose();
            _notificationTimer = null;

            NotifyDataReceived(DateTime.UtcNow, forceNotification: true);
            _running = false;

            TrimEventHistory();
        }

        private void Advertisement_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            var utcNow = DateTime.UtcNow;

            lock(_eventHistory)
            {
                var expiry = _eventHistory[args.BluetoothAddress];
                if (expiry == null || (DateTime)expiry < utcNow)
                {
                    _eventHistory[args.BluetoothAddress] = args.Timestamp.Add(_retentionInterval);
                }
                else
                {
                    // Still in retention period
                    return;
                }
            }

            var telemetry = MapToTelemetry(args);
            lock (_telemetry)
            {
                _telemetry.Add(telemetry);
                NotifyDataReceived(utcNow);
            }
        }

        private void NotifyDataReceived(DateTime utcNow, bool forceNotification = false)
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
                    if(_notificationTimer != null)
                    {
                        _notificationTimer.Change(_timerInerval, _timerInerval);
                    }

                    var telemetry = (ArrayList)_telemetry.Clone();
                    _telemetry.Clear();
                    DataReceived?.Invoke(telemetry);
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

                var now = DateTime.UtcNow;
                var keys = new ArrayList();
                foreach (DictionaryEntry entry in _eventHistory)
                {
                    if ((DateTime)entry.Value < now)
                    {
                        keys.Add(entry.Key);
                    }
                }

                foreach (var key in keys)
                {
                    _eventHistory.Remove(key);
                }

                Debug.WriteLine($"Trimmed {keys.Count} Bluetooth event histories. Current count {_eventHistory.Count}");
            }
        }

        private RegisterSet MapToTelemetry(BluetoothLEAdvertisementReceivedEventArgs args)
        {
            var registers = new ArrayList
            {
                new Register("AddrType", (short)args.BluetoothAddressType),
                new Register("AdvType", (short)args.AdvertisementType),
                new Register("DBm", args.RawSignalStrengthInDBm),
            };

            var advert = args.Advertisement;

            // Add local name
            if (!string.IsNullOrEmpty(advert.LocalName))
            {
                registers.Add(new Register("Name", args.Advertisement.LocalName));
            }

            // Add service UUIDs
            for (var i = 0; i < advert.ServiceUuids.Length; i++)
            {
                // Remove hyphens from UUID
                var id = string.Concat(advert.ServiceUuids[i].ToString().Split('-'));
                registers.Add(new Register(AppendIndex("ServiceId", i), id));
            }

            // Add manufacturer data
            for (var i = 0; i < advert.ManufacturerData.Count; i++)
            {
                var manufacturer = (BluetoothLEManufacturerData)advert.ManufacturerData[i];
                if (manufacturer.Data.Length > 0)
                {
                    var dr = DataReader.FromBuffer(manufacturer.Data);
                    var bytes = new byte[manufacturer.Data.Length];
                    dr.ReadBytes(bytes);

                    registers.Add(new Register(AppendIndex("Data", i), bytes.ToHexString()));
                }
            }

            // Append index if > 0
            static string AppendIndex(string identifier, int index)
            {
                var serial = index > 0 ? "_" + index : null;
                return identifier + serial;
            }

            var registerSet = new RegisterSet
            {
                Identifier = args.BluetoothAddress.ToString("x"),
                Registers = (Register[])registers.ToArray(typeof(Register))
            };

            return registerSet;
        }

        public delegate void DataReceivedEventHandler(ArrayList registerSets);
        public event DataReceivedEventHandler DataReceived;
    }
}
