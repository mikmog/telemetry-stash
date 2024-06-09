using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DataCollector.Application.Services
{
    public class LocalStorageService
    {
        private readonly string _telemetryDb;

        public LocalStorageService(string telemetryDb)
        {
            _telemetryDb = telemetryDb;
            FileExists = File.Exists(_telemetryDb);
        }

        public bool FileExists { get; private set; }
        public float FileSize { get; private set; } = -1;

        public void AddToLocalStorage(Queue telemetries)
        {
            using var file = File.OpenWrite(_telemetryDb);
            FileExists = true;
            file.Seek(0, SeekOrigin.End);

            for (int i = 0; i < telemetries.Count; i++)
            {
                var telemetrySerialized = BinaryFormatter.Serialize(telemetries.Peek());
                var header = BitConverter.GetBytes((short)telemetrySerialized.Length);

                // Note. When storage run out(?) (around 150 KB on current device) an exception will be thrown
                // 'System.IndexOutOfRangeException' occurred in System.IO.FileSystem.dll
                file.Write(header, 0, header.Length);
                file.Write(telemetrySerialized, 0, telemetrySerialized.Length);

                telemetries.Dequeue();
            }

            FileSize = file.Length / 1024f;
            file.Close();
        }

        public delegate void OnReadCallback(object telemetry);

        public void ReadFromLocalStorage(OnReadCallback onReadCallback)
        {
            if(!FileExists)
            {
                return;
            }

            using var file = File.OpenRead(_telemetryDb);
            Debug.WriteLine("LocalStorage, ReadFromLocalStorage: '" + _telemetryDb + "', Filesize KB: " + file.Length / 1024f);

            if (file.Length == 0)
            {
                file.Close();
                return;
            }

            var headerBuff = new byte[2];
            while (true)
            {
                var headerReadResult = file.Read(headerBuff, 0, headerBuff.Length);
                if (headerReadResult == 0)
                {
                    break;
                }

                var telemetryLength = BitConverter.ToInt16(headerBuff, 0);
                var telemetryBuff = new byte[telemetryLength];
                file.Read(telemetryBuff, 0, telemetryLength);

                var obj = BinaryFormatter.Deserialize(telemetryBuff);
                onReadCallback.Invoke(obj);
            }

            file.Close();

            File.Delete(_telemetryDb);
            FileSize = 0;
        }

        public void DeleteIfExist()
        {
            if (File.Exists(_telemetryDb))
            {
                Debug.WriteLine("Deleting: " + _telemetryDb);
                File.Delete(_telemetryDb);
                FileSize = 0;
                FileExists = false;
            }
        }
    }
}
