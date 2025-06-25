using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using TelemetryStash.Shared;

namespace TelemetryStash.NfClient.Communication.Mqtt
{
    public class MqttSettings
    {
        private const string BrokerHostNameKey = "Mqtt.BrokerHostName";
        private const string AzureCaCertKey = "Mqtt.AzureRootCert";

        private const string ClientCertificateNameKey = "Client.CertificateName";
        private const string ClientPemCertKey = "Client.PemCert";
        private const string ClientPrivateKeyKey = "Client.PrivateKey";
        private const string ClientCertificatePasswordKey = "Client.CertificatePassword";

        public MqttSettings Configure(IDictionary dictionary)
        {
            var reader = new FileReader();
            string Setting(string key) => dictionary[key] as string ?? throw new ArgumentException("Setting not found", key);
            byte[] ReadFile(string key) => reader.ReadFile(Setting(key), $"File not found '{key}'");

            ClientICertificateName = Setting(ClientCertificateNameKey);
            BrokerHostName = Setting(BrokerHostNameKey);

            AzureCaCert = new X509Certificate(ReadFile(AzureCaCertKey));

            ClientCert = new X509Certificate2(
                rawData: ReadFile(ClientPemCertKey),
                key: ReadFile(ClientPrivateKeyKey),
                password: Setting(ClientCertificatePasswordKey));

            return this;
        }

        public string ClientICertificateName { get; set; }

        public string BrokerHostName { get; set; }

        public X509Certificate AzureCaCert { get; set; }

        public X509Certificate2 ClientCert { get; set; }
    }
}
