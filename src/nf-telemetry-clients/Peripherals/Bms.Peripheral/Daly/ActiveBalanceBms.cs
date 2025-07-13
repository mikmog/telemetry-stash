using Iot.Device.Modbus.Client;
using nanoFramework.Hardware.Esp32;

namespace TelemetryStash.Peripherals.Bms.Daly
{
    // Use a MAX485 Modbus Module

    public class ActiveBalanceBms
    {
        const byte DeviceAddress = 0xD2;

        private ModbusClient _modbusClient;

        public void Initialize(ActiveBalanceBmsSettings settings)
        {
            Configuration.SetPinFunction(settings.ComRtsPin, settings.ComRts);
            Configuration.SetPinFunction(settings.ComTxPin, settings.ComTx);
            Configuration.SetPinFunction(settings.ComRxPin, settings.ComRx);

            _modbusClient = new ModbusClient(settings.ComPort);
            _modbusClient.ReadTimeout = _modbusClient.WriteTimeout = 1000;
        }

        public double ReadValue(Register register, double readFailedValue = double.NaN)
        {
            var data = _modbusClient.ReadHoldingRegisters(DeviceAddress, register.Address, count: 1);
            if (data == null || data.Length == 0)
            {
                return readFailedValue;
            }

            return register.Value(data[0]);
        }
    }
}
