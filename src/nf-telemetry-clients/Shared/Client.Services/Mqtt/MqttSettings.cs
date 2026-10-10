using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace TelemetryStash.NfClient.Services
{
    public class MqttSettings
    {
        private const string BrokerHostNameKey = "Mqtt.BrokerHostName";
        private const string AzureCaCertKey = "Mqtt.AzureRootCert";

        private const string ClientCertificateNameKey = "Client.CertificateName";
        private const string ClientPemCertKey = "Client.PemCert";
        private const string ClientPrivateKeyKey = "Client.PrivateKey";

        public MqttSettings Configure(IDictionary dictionary)
        {
            string Setting(string key) => dictionary[key] as string ?? throw new ArgumentException("Setting not found", key);
            string ReadFile(string key) => File.ReadAllText(Setting(key));

            ClientICertificateName = Setting(ClientCertificateNameKey);
            BrokerHostName = Setting(BrokerHostNameKey);

            AzureCaCert = new X509Certificate(ReadFile(AzureCaCertKey));

            ClientCert = new X509Certificate2(
                rawData: ReadFile(ClientPemCertKey),
                key: ReadFile(ClientPrivateKeyKey),
                password: null);

            return this;
        }

        public string ClientICertificateName { get; set; }

        public string BrokerHostName { get; set; }

        public X509Certificate AzureCaCert { get; set; }

        public X509Certificate2 ClientCert { get; set; }
    }
}
