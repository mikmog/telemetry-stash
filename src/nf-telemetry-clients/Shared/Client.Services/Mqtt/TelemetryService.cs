using nanoFramework.Runtime.Native;
using System;
using System.Diagnostics;
using System.Threading;
using TelemetryStash.Shared;

namespace TelemetryStash.NfClient.Services
{
    public class TelemetryService
    {
        private MqttService _mqttService;
        private ConcurrentQueue _telemetryQueue;
        private readonly Timer _uploadTimer;
        private bool _uploadInProgresss = false;
        private readonly short _telemetryBufferSize;

        public TelemetryService(MqttService mqttService, short inMemoryBufferSize)
        {
            _mqttService = mqttService;
            _telemetryQueue = new();
            _telemetryBufferSize = inMemoryBufferSize;
            _uploadTimer = new Timer(UploadTelemetry, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        public void PublishTelemetry(Telemetry telemetry)
        {
            if (_telemetryQueue.Count >= _telemetryBufferSize)
            {
                Debug.WriteLine("Discarding telemetry. Digestion is slower than ingestion");
                return;
            }

            _telemetryQueue.Enqueue(telemetry);
        }

        public void Dispose()
        {
            _uploadTimer?.Dispose();
            _mqttService?.Dispose();
            _mqttService = null;
            _telemetryQueue = null;
        }

        private void UploadTelemetry(object _)
        {
            try
            {
                if (_uploadInProgresss)
                {
                    return;
                }
                _uploadInProgresss = true;

                var telemetryCount = _telemetryQueue.Count;
                if (telemetryCount == 0)
                {
                    return;
                }

                for (var i = 0; i < telemetryCount; i++)
                {
                    if (!_mqttService.IsConnected)
                    {
                        _mqttService.Connect();
                    }

                    var telemetry = (Telemetry)_telemetryQueue.Dequeue();
                    _mqttService.Publish(telemetry);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());

                Debug.WriteLine("Error occured. Rebooting device");
                Thread.Sleep(5000);
                Power.RebootDevice(1000);
            }
            finally
            {
                _uploadInProgresss = false;
            }
        }
    }
}
