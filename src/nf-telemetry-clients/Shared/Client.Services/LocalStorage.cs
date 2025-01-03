using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TelemetryStash.NfClient.Services
{
    public class LocalStorage
    {
        private readonly string _storageFile;

        public LocalStorage(string storageFile)
        {
            _storageFile = storageFile;
            FileExists = File.Exists(_storageFile);
        }

        public bool FileExists { get; private set; }
        public float FileSize { get; private set; } = -1;

        public void AppendLocalStorage(object[] entities)
        {
            using var file = File.OpenWrite(_storageFile);
            FileExists = true;
            file.Seek(0, SeekOrigin.End);

            for (int i = 0; i < entities.Length; i++)
            {
                var serialized = BinaryFormatter.Serialize(entities[i]);
                var header = BitConverter.GetBytes((int)serialized.Length);

                // Note. When storage run out(?) (around 150 KB on current device) an exception will be thrown
                // 'System.IndexOutOfRangeException' occurred in System.IO.FileSystem.dll
                file.Write(header, 0, header.Length);
                file.Write(serialized, 0, serialized.Length);
            }

            FileSize = file.Length / 1024f;
            file.Close();
        }

        public delegate void OnReadCallback(object entity);

        public void ReadLocalStorage(OnReadCallback onReadCallback)
        {
            if (!FileExists)
            {
                return;
            }

            using var file = File.OpenRead(_storageFile);
            Debug.WriteLine("ReadLocalStorage: '" + _storageFile + "', File size KB: " + file.Length / 1024f);
            if (file.Length == 0)
            {
                file.Close();
                return;
            }

            var headerBuff = new byte[sizeof(int)];
            while (true)
            {
                var headerReadResult = file.Read(headerBuff, 0, headerBuff.Length);
                if (headerReadResult == 0)
                {
                    break;
                }

                var telemetryLength = BitConverter.ToInt32(headerBuff, 0);
                var telemetryBuff = new byte[telemetryLength];
                file.Read(telemetryBuff, 0, telemetryLength);

                var obj = BinaryFormatter.Deserialize(telemetryBuff);
                onReadCallback.Invoke(obj);
            }

            file.Close();
        }

        public void DeleteLocalStorage()
        {
            if (File.Exists(_storageFile))
            {
                Debug.WriteLine("Deleting: " + _storageFile);
                File.Delete(_storageFile);
                FileSize = -1;
                FileExists = false;
            }
        }
    }
}
