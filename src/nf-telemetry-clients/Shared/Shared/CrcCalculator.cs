namespace TelemetryStash.Shared
{
    public static class CrcCalculator
    {
        private static readonly ushort[] _crc16ArcTable = BuildCrc16ArcTable();

        public static ushort CalculateCrc16Arc(byte[] data)
        {
            ushort crc = 0x0000;
            var table = _crc16ArcTable;
            for (int i = 0; i < data.Length; i++)
            {
                crc = (ushort)((crc >> 8) ^ table[(crc ^ data[i]) & 0xFF]);
            }
            return crc;
        }

        private static ushort[] BuildCrc16ArcTable()
        {
            var table = new ushort[256];
            for (int n = 0; n < 256; n++)
            {
                ushort crc = (ushort)n;
                for (int i = 0; i < 8; i++)
                {
                    crc = (crc & 1) != 0
                        ? (ushort)((crc >> 1) ^ 0xA001)
                        : (ushort)(crc >> 1);
                }
                table[n] = crc;
            }
            return table;
        }
    }
}
