namespace TelemetryStash.Peripherals.Bms.Daly
{
    public static class Registers
    {
#pragma warning disable format

        ///                                                  Address  SI-Factor Offset
        public static readonly Register TotalVoltage =      new(0x28, 10,       0);       // 35.0v
        public static readonly Register TotalCurrent =      new(0x29, 10,       -30000);  // 10.00A
        public static readonly Register RemainingCapacity = new(0x30, 10,       0);       // 0.1Ah
        public static readonly Register PowerInWatts =      new(0x39, 1,        0);       // 50W
        public static readonly Register Temperature1 =      new(0x20, 1,        -40);     // 25.0C
        public static readonly Register Temperature2 =      new(0x21, 1,        -40);     // 25.0C

#pragma warning restore format

        public static double Value(this Register register, short value)
        {
            return (value + register.Offset) / (double)register.ConversionFactor;
        }
    }

    public readonly struct Register
    {
        public Register(byte address, short conversionFactor, short offset)
        {
            Address = address;
            ConversionFactor = conversionFactor;
            Offset = offset;
        }

        public byte Address { get; }
        public short ConversionFactor { get; }
        public short Offset { get; }
    }
}
