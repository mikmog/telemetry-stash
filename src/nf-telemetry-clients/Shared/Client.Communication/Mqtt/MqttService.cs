using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Exceptions;
using nanoFramework.M2Mqtt.Messages;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;
using TelemetryStash.Shared;

namespace TelemetryStash.NfClient.Communication.Mqtt
{
    public class MqttService
    {
        private static MqttClient _mqttClient;
        private short _connectionAttempts;

        private readonly string _clientCertName;
        private readonly string _telemetryTopic;
        private readonly string _username;
        private readonly bool _discardMessages;

        public MqttService(MqttSettings settings, bool discardMessages = false)
        {
            _clientCertName = settings.ClientICertificateName;

            _telemetryTopic = "devices/" + _clientCertName + "/messages/events/";
            _username = settings.BrokerHostName + "/" + _clientCertName + "/?api-version=2021-04-12&DeviceClientType=nano";
            _discardMessages = discardMessages;

            _mqttClient = new(
                brokerHostName: settings.BrokerHostName,
                brokerPort: 8883,
                secure: true,
                caCert: settings.AzureCaCert,
                clientCert: settings.ClientCert,
                sslProtocol: MqttSslProtocols.TLSv1_2)
            {
                ProtocolVersion = MqttProtocolVersion.Version_3_1_1
            };

            SubscribeMqttEvents();
        }

        public bool IsConnected => _mqttClient != null && _mqttClient.IsConnected;

        public void Connect()
        {
            lock (_mqttClient)
            {
                if (_mqttClient.IsConnected)
                {
                    return;
                }

                const string willTopic = "$iothub/twin/GET/?$rid=999";
                const string willMessage = "Disconnected";
                _connectionAttempts++;

                try
                {
                    var response = _mqttClient.Connect(
                        clientId: _clientCertName,
                        username: _username,
                        password: null,
                        willRetain: false,
                        willQosLevel: MqttQoSLevel.AtMostOnce,
                        willFlag: false,
                        willTopic: willTopic,
                        willMessage: willMessage,
                        cleanSession: true,
                        keepAlivePeriod: 60);

                    _connectionAttempts = 0;

                    if (response != MqttReasonCode.Success)
                    {
                        throw new Exception("MQTT connection failed. ReasonCode: " + response + ", IsConnected: " + _mqttClient.IsConnected);
                    }

                    Debug.WriteLine("MQTT connected: " + _mqttClient.IsConnected + ", ReasonCode: " + response);

                }
                catch (MqttCommunicationException ex)
                {
                    if (_connectionAttempts < 5)
                    {
                        Debug.WriteLine("MQTT connection failed. Retrying...");
                        Thread.Sleep(1000 * _connectionAttempts);
                        Connect();
                    }
                    else
                    {
                        Debug.WriteLine("MQTT connection failed. Max attempts reached.");
                        throw ex;
                    }
                }

            }
        }

        public void Publish(Telemetry telemetry)
        {
            var json = JsonSerialize.SerializeTelemetry(telemetry);

            var messageId = -1;
            if (!_discardMessages)
            {
                messageId = _mqttClient.Publish(
                    topic: _telemetryTopic,
                    message: Encoding.UTF8.GetBytes(json),
                    contentType: null,
                    userProperties: null,
                    qosLevel: MqttQoSLevel.AtMostOnce,
                    retain: false);
            }

            Debug.WriteLine(
                DateTime.UtcNow.ToString("T") + 
                " - MQTT message " + 
                messageId +
                " published. Size: " +
                Round.ToTwoDecimalString(Encoding.UTF8.GetBytes(json).Length / 1024d) + " KB");
            Debug.WriteLine(json);
            Debug.WriteLine();
        }

        public void Dispose()
        {
            Thread.Sleep(1000);
            _mqttClient?.Dispose();
            _mqttClient = null;
        }

        [Conditional("DEBUG")]
        private void SubscribeMqttEvents()
        {
            _mqttClient.MqttMsgPublishReceived += (sender, e) => { Debug.WriteLine("MQTT message received: " + Encoding.UTF8.GetString(e.Message, 0, e.Message.Length)); };
            _mqttClient.MqttMsgPublished += (sender, e) => { Debug.WriteLine("MQTT message published: " + e.MessageId); };
            _mqttClient.MqttMsgSubscribed += (sender, e) => { Debug.WriteLine("MQTT message subscribed: " + e.MessageId); };
            _mqttClient.MqttMsgUnsubscribed += (sender, e) => { Debug.WriteLine("MQTT message unsubscribed: " + e.MessageId); };
            _mqttClient.ConnectionOpened += (object sender, ConnectionOpenedEventArgs e) => MqttConnectionOpened(sender, e);
            _mqttClient.ConnectionClosed += (sender, e) => { Debug.WriteLine("MQTT connection closed"); };
            _mqttClient.ConnectionClosedRequest += (sender, e) => { Debug.WriteLine("MQTT connection closed request: " + e.Message); };
        }

        [Conditional("DEBUG")]
        private void MqttConnectionOpened(object sender, ConnectionOpenedEventArgs e)
        {
            Debug.WriteLine($"MQTT connection opened:");
            Debug.WriteLine($"> ClientID: {((MqttClient)sender).ClientId}");
            Debug.WriteLine($"> Assigned client id: {e.Message.AssignedClientIdentifier}");
            Debug.WriteLine($"> Auth method: {e.Message.AuthenticationMethod}");
            Debug.WriteLine($"> Auth data length: {(e.Message.AuthenticationData == null ? 0 : e.Message.AuthenticationData.Length)}");
            Debug.WriteLine($"> Dup flag: {e.Message.DupFlag}");
            Debug.WriteLine($"> Max packet size: {e.Message.MaximumPacketSize}");
            Debug.WriteLine($"> Max QoS: {e.Message.MaximumQoS}");
            Debug.WriteLine($"> Msg ID: {e.Message.MessageId}");
            Debug.WriteLine($"> Qos level: {e.Message.QosLevel}");
            Debug.WriteLine($"> Reason: {e.Message.Reason}");
            Debug.WriteLine($"> Receive max: {e.Message.ReceiveMaximum}");
            Debug.WriteLine($"> Rep info: {e.Message.ResponseInformation}");
            Debug.WriteLine($"> Retain: {e.Message.Retain}");
            Debug.WriteLine($"> Retain available: {e.Message.RetainAvailable}");
            Debug.WriteLine($"> Return code: {e.Message.ReturnCode}");
            Debug.WriteLine($"> Server keep alive: {e.Message.ServerKeepAlive}");
            Debug.WriteLine($"> Server ref: {e.Message.ServerReference}");
            Debug.WriteLine($"> Session exp inter: {e.Message.SessionExpiryInterval}");
            Debug.WriteLine($"> Session present: {e.Message.SessionPresent}");
            Debug.WriteLine($"> Shared subs available: {e.Message.SharedSubscriptionAvailable}");
            Debug.WriteLine($"> Shared identifier available: {e.Message.SubscriptionIdentifiersAvailable}");
            Debug.WriteLine($"> Topic alias max: {e.Message.TopicAliasMaximum}");
            foreach (UserProperty prop in e.Message.UserProperties ?? new ArrayList())
            {
                Debug.WriteLine($"> User property key: {prop.Name}");
                Debug.WriteLine($"> User property value: {prop.Value}");
            }
            Debug.WriteLine($"> Wildcard available: {e.Message.WildcardSubscriptionAvailable}");
        }
    }
}
