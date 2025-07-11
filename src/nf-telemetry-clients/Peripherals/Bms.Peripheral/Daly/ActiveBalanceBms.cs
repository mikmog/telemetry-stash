using Iot.Device.Modbus.Client;
using nanoFramework.Hardware.Esp32;

namespace TelemetryStash.Peripherals.Bms.Daly
{
    // Uses a MAX485 Modbus Module
    // 
    public class ActiveBalanceBms
    {
        const byte DeviceAddress = 0xD2;

        private ModbusClient _modbusClient;

        public void Initialize()
        {
            Configuration.SetPinFunction(0, DeviceFunction.COM2_RTS);
            Configuration.SetPinFunction(43, DeviceFunction.COM2_TX);
            Configuration.SetPinFunction(44, DeviceFunction.COM2_RX);

            _modbusClient = new ModbusClient("COM2");
            _modbusClient.ReadTimeout = _modbusClient.WriteTimeout = 2000;
        }

        public double RadValue(Register register)
        {
            var data = _modbusClient.ReadHoldingRegisters(DeviceAddress, register.Address, count: 1);
            if (data == null || data.Length == 0)
            {
                return double.NaN;
            }

            return register.Value(data[0]);
        }
    }
}
