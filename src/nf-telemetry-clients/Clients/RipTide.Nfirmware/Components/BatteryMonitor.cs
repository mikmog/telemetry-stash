using RipTide.Nfirmware.Components.Common;
using System;
using System.Device.Adc;
using System.Device.Gpio;
using System.Threading;
using TelemetryStash.Peripherals.Bms.Daly;

namespace RipTide.Nfirmware.Components
{
    public class BatteryMonitor : Component
    {
        public BatteryMonitor(AdcController adc, GpioController gpio, ErrorHandler errorHandler) : base(adc, gpio, errorHandler) { }

        private ActiveBalanceBms _activeBalanceBms;
        private const int ReadInterval = 1000; // 1 seconds

        private BatteryValue _batteryStatus;

        public override void Initialize(AppSettings appSettings)
        {
            _activeBalanceBms = new();
            _activeBalanceBms.Initialize(appSettings.ActiveBalanceBms);

            Start(Runner);
        }

        private void Runner()
        {
            while (true)
            {
                var currentStatus = new BatteryValue
                {
                    TotalVoltage = _activeBalanceBms.ReadValue(Registers.TotalVoltage, readFailedValue: 0),
                    TotalCurrent = _activeBalanceBms.ReadValue(Registers.TotalCurrent, readFailedValue: 0),
                    RemainingCapacity = _activeBalanceBms.ReadValue(Registers.RemainingCapacity, readFailedValue: 0),
                    PowerInWatts = _activeBalanceBms.ReadValue(Registers.PowerInWatts, readFailedValue: 0)
                };

                if (currentStatus.Equals(_batteryStatus))
                {
                    // No change
                    Thread.Sleep(ReadInterval);
                    continue;
                }

                _batteryStatus = currentStatus;
                OnBatteryChanged?.Invoke(_batteryStatus);

                Thread.Sleep(ReadInterval);
            }
        }

        public delegate void BatteryChanged(BatteryValue batteryValue);
        public event BatteryChanged OnBatteryChanged;
    }

    public struct BatteryValue
    {
        public double TotalVoltage { get; set; }
        public double TotalCurrent { get; set; }
        public double RemainingCapacity { get; set; }
        public double PowerInWatts { get; set; }

        public override readonly bool Equals(object obj)
        {
            var other = (BatteryValue)obj;

            return TotalVoltage == other.TotalVoltage &&
                    TotalCurrent == other.TotalCurrent &&
                    RemainingCapacity == other.RemainingCapacity &&
                    PowerInWatts == other.PowerInWatts;
        }

        public override readonly int GetHashCode()
        {
            throw new NotImplementedException("GetHashCode is not implemented");
        }
    }
}
