using RipTide.Nfirmware.Components.Common;
using System.Device.Adc;
using System.Device.Gpio;
using System.Numerics;
using System.Threading;
using TelemetryStash.MpuGyroSensor;

namespace RipTide.Nfirmware.Components
{
    public class Gyro : Component
    {
        public Gyro(AdcController adc, GpioController gpio, ErrorHandler errorHandler) : base(adc, gpio, errorHandler) { }

        private readonly Mpu6050Gyro _mpu6050 = new();

        public override void Initialize(AppSettings appSettings)
        {
            _mpu6050.Initialize(appSettings.MpuGyro);

            Start(Runner);
        }

        private void Runner()
        {
            // TODO needed everywhere?
            while (OnGyroChanged == null)
            {
                Thread.Sleep(100);
            }

            var oldReading = _mpu6050.ReadGyroscope();
            OnGyroChanged.Invoke(oldReading);

            while (true)
            {
                Thread.Sleep(100);

                var newReading = _mpu6050.ReadGyroscope();
                if (newReading == oldReading)
                {
                    continue; // No change, skip
                }

                oldReading = newReading;
                OnGyroChanged?.Invoke(newReading);
            }
        }

        public delegate void GyroChanged(Vector3 value);
        public event GyroChanged OnGyroChanged;
    }
}
