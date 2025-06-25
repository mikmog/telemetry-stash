using System;
using System.IO;

namespace TelemetryStash.Shared
{
    public class FileReader
    {
        public byte[] ReadFile(string filePath, string errorMessage)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException(errorMessage, filePath);
            }

            var bytes = File.ReadAllBytes(filePath);

            var fileBuffer = new byte[bytes.Length + 1];
            Array.Copy(bytes, fileBuffer, bytes.Length);
            fileBuffer[fileBuffer.Length - 1] = 0;

            return fileBuffer;
        }
    }
}
