using DataCollector.Application.P1Meter;
using DataCollector.Application.Utils;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace DataCollector.Application.Services
{
    public class BufferedTelemetryService
    {
        private const int MaxBeforeDiscardingTelemetry = 10;
        private const int MaxBeforeBufferToDisk = 6;

        private readonly LocalStorageService _localStorage;
        private readonly MqttService _mqttService;

        private readonly Queue _telemetryQueue = new();

        public BufferedTelemetryService(MqttService mqttService)
        {
            _mqttService = mqttService;

            // I:\insight.db
            var telemetryDb = Directory.GetLogicalDrives()[0] + "insight.db";
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

        private void UploadAllTelemetry()
        {
            if (_localStorage.FileExists)
            {
                Debug.WriteLine("ReadFromLocalStorage: '" + "', Filesize (KB): " + _localStorage.FileSize);
                _localStorage.ReadFromLocalStorage(UploadSingleTelemetry);
                _localStorage.DeleteIfExist();
            }

            for(var i = 0; i < _telemetryQueue.Count; i++)
            {
                UploadSingleTelemetry(_telemetryQueue.Peek());
                _telemetryQueue.Dequeue();
            }
        }

        private short _connectCounter = 0;
        private void UploadSingleTelemetry(object telemetry)
        {
            if (!_mqttService.IsConnected)
            {
                _connectCounter++;
                _mqttService.Connect();
            } else
            {
                _connectCounter = 0;
            }

            if (_connectCounter > 5)
            {
                throw new Exception("MQTT broker not responding");
            }

            _mqttService.Publish((Telemetry)telemetry);
            Debug.WriteLine("Telemetry published. Remaining RAM (KB): " + nanoFramework.Runtime.Native.GC.Run(true) / 1024f);
        }
    }
}
