using nanoFramework.Hardware.Esp32;
using System;
using System.Collections;

namespace TelemetryStash.Shared
{
    public static class Extensions
    {
        public static string ToHexString(this byte[] bytes)
        {
            char[] c = new char[bytes.Length * 2];

            byte b;

            for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx)
            {
                b = ((byte)(bytes[bx] >> 4));
                c[cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);

                b = ((byte)(bytes[bx] & 0x0F));
                c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
            }

            return new string(c);
        }

        public static byte[] HexStringToBytes(this string hexString)
        {
            var bytes = new byte[hexString.Length / 2]; // Each byte is represented by two characters
            for (var i = 0; i < bytes.Length; i++)
            {
                var startIndex = i * 2;
                bytes[i] = Convert.ToByte(hexString.Substring(startIndex, 2), fromBase: 16);
            }
            return bytes;
        }

        public static string[] SplitAndTrim(this string str, char separator)
        {
            var parts = str.Split(separator);
            for (var i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Trim();
            }

            return parts;
        }

        public static string String(this IDictionary dictionary, string key)
        {
            var value = dictionary[key] ?? throw new ArgumentException(key);
            return (string)value;
        }

        public static int Int32(this IDictionary dictionary, string key)
        {
            var value = dictionary[key] ?? throw new ArgumentException(key);
            return (int)value;
        }

        public static double Double(this IDictionary dictionary, string key)
        {
            var value = dictionary[key] ?? throw new ArgumentException(key);
            return (double)value;
        }

        public static DeviceFunction DeviceFunction(this IDictionary dictionary, string key)
        {
            var value = dictionary[key] ?? throw new ArgumentException(key);
            return ConfigurationExtensions.ParseDeviceFunction((string)value);
        }

        public static ArrayList List(this IDictionary dictionary, string key)
        {
            var value = dictionary[key] ?? throw new ArgumentException(key);
            return (ArrayList)value;
        }
    }
}
