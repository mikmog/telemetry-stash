using System;
using System.Collections;
using System.Diagnostics;
using TelemetryStash.DeviceServices.Mqtt;
using TelemetryStash.ServiceModels;

namespace TelemetryStash.DeviceServices
{
    public class BufferedTelemetryService
    {
        private const int MaxBeforeDiscardingTelemetry = 10;
        private const int MaxBeforeBufferToDisk = 6;

        private LocalStorageService _localStorage;
        private MqttService _mqttService;
        private Queue _telemetryQueue;

        public BufferedTelemetryService(MqttService mqttService)
        {
            _mqttService = mqttService;
            _telemetryQueue = new();

            // I:\insight.db
            var telemetryDb = @"I:\insight.db";
            _localStorage = new(telemetryDb);
            _localStorage.DeleteIfExist();
        }

        public void AddTelemetry(Telemetry telemetry)
        {
            if (_telemetryQueue.Count >= MaxBeforeDiscardingTelemetry)
            {
                Debug.WriteLine("Skipping telemetry. Digestion is slower than ingestion");
                return;
            }

            _telemetryQueue.Enqueue(telemetry);

            if (_telemetryQueue.Count >= MaxBeforeBufferToDisk)
            {
                _localStorage.AddToLocalStorage(_telemetryQueue);
            }

            UploadAllTelemetry();
        }

        public void Dispose()
        {
            _mqttService?.Dispose();
            _mqttService = null;
            _telemetryQueue = null;
            _localStorage = null;
        }

        private void UploadAllTelemetry()
        {
            if (_localStorage.FileExists)
            {
                Debug.WriteLine("ReadFromLocalStorage: '" + "', File size (KB): " + _localStorage.FileSize);
                _localStorage.ReadFromLocalStorage(UploadSingleTelemetry);
                _localStorage.DeleteIfExist();
            }

            for (var i = 0; i < _telemetryQueue.Count; i++)
            {
                UploadSingleTelemetry(_telemetryQueue.Peek());
                _telemetryQueue.Dequeue();
            }
        }

        private short _connectCounter = 0;
        private void UploadSingleTelemetry(object telemetry)
        {
            if (_mqttService == null)
            {
                Debug.WriteLine(nameof(MqttService) + " service not initialized");
                return;
            }

            if (!_mqttService.IsConnected)
            {
                _connectCounter++;
                _mqttService.Connect();
            }
            else
            {
                _connectCounter = 0;
            }

            if (_connectCounter > 5)
            {
                throw new Exception("MQTT broker not responding");
            }

            _mqttService.Publish((Telemetry)telemetry);
            Debug.WriteLine("Telemetry published");
        }
    }
}
