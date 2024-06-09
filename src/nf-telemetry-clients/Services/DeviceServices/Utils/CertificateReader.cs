using System;
using System.IO;

namespace TelemetryStash.DeviceServices.Utils
{
    public class CertificateReader
    {
        public byte[] ReadCertificate(string filePath, string errorMessage)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException(errorMessage, filePath);
            }
            
            var bytes = File.ReadAllBytes(filePath);
            
            var certificate = new byte[bytes.Length + 1];
            Array.Copy(bytes, certificate, bytes.Length);
            certificate[certificate.Length - 1] = 0;

            return certificate;
        }
    }
}
