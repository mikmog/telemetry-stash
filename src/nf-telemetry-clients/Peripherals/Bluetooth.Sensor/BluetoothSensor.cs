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
            
            _notificationTimer = new Timer((_) => NotifyDataReceived(forceNotification: true), null, _timerInerval, _timerInerval);
            _bluetoothWatcher.Start();
        }

        public void Stop()
        {
            _notificationTimer.Dispose();
            _notificationTimer = null;

            NotifyDataReceived(forceNotification: true);
            _running = false;

            // Note. Stopping the watcher sometimes causing the device to reboot
            _bluetoothWatcher.Stop();

            TrimEventHistory();
        }

        private void Advertisement_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            if(args.BluetoothAddress == 0)
            {
                return;
            }

            if (!_running)
            {
                return;
            }

            lock(_eventHistory)
            {
                var utcNow = DateTime.UtcNow.Ticks;
                var expiry = _eventHistory[args.BluetoothAddress];
                if (expiry == null || (long)expiry < utcNow)
                {
                    _eventHistory[args.BluetoothAddress] = args.Timestamp.Add(_retentionInterval).Ticks;
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
                NotifyDataReceived();
            }
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

                var now = DateTime.UtcNow.Ticks;
                var keys = new ArrayList();
                foreach (DictionaryEntry entry in _eventHistory)
                {
                    if ((long)entry.Value < now)
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
            var advert = args.Advertisement;

            var registers = new ArrayList
            {
                new Register("AddrType", (int)args.BluetoothAddressType),
                new Register("AdvType", (int)args.AdvertisementType),
                new Register("DBm", args.RawSignalStrengthInDBm),
                new Register("Flags", (short)advert.Flags)
            };
            
            // Add local name
            var localName  = advert.LocalName;
            if (!string.IsNullOrEmpty(localName))
            {
                registers.Add(new Register("Name", localName));
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
                registers.Add(new Register(AppendIndex("Company", i), manufacturer.CompanyId));

                if (manufacturer.Data.Length > 0)
                {
                    var dr = DataReader.FromBuffer(manufacturer.Data);
                    var bytes = new byte[manufacturer.Data.Length];
                    dr.ReadBytes(bytes);

                    registers.Add(new Register(AppendIndex("Data", i), bytes.ToHexString()));
                }
            }

            // Append 'index' if > 0
            static string AppendIndex(string identifier, int index)
            {
                var serial = index > 0 ? "_" + index : null;
                return identifier + serial;
            }

            var macAddress = args.BluetoothAddress.ToString("x");
            registers.Add(new Register("Mac", macAddress));

            var identifier = "bluetooth:" + macAddress;
            var registerSet = new RegisterSet
            {
                Identifier = identifier,
                Registers = (Register[])registers.ToArray(typeof(Register))
            };

            return registerSet;
        }

        public delegate void DataReceivedEventHandler(ArrayList registerSets);
        public event DataReceivedEventHandler DataReceived;
    }
}
