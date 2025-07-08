using RipTide.Nfirmware.Components.Common;
using System.Device.Adc;
using System.Device.Gpio;
using TelemetryStash.MpuGyroSensor;

namespace RipTide.Nfirmware.Components
{
    internal class Gyro : Component
    {
        public Gyro(AdcController adc, GpioController gpio, ErrorHandler errorHandler) : base(adc, gpio, errorHandler) { }

        private Mpu6050Gyro _mpu6050 = new();

        public override void Initialize(AppSettings appSettings)
        {
            _mpu6050.Initialize(appSettings.MpuGyro);

            Start(Runner);
        }

        private void Runner()
        {
        }
    }
}
