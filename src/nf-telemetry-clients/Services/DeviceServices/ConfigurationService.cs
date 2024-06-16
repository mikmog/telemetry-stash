using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace TelemetryStash.DeviceServices
{
    public class ConfigurationService
    {
        const string fileName = @"I:\AppSettings.json";

        public IDictionary ReadConfiguration()
        {
            if(!File.Exists(fileName))
            {
                throw new Exception($"Configuration file not found '{fileName}'");
            }

            var fileContent = File.ReadAllText(fileName);
            var dictionary = Json.DeserializeToDictionary(fileContent);
            PrintConfiguration(dictionary);

            return dictionary;
        }

        [Conditional("DEBUG")]
        private void PrintConfiguration(IDictionary dictionary)
        {
            Debug.WriteLine("Application settings: " + fileName);
            foreach (DictionaryEntry entry in dictionary)
            {
                Console.WriteLine($"> {entry.Key}: {entry.Value}");
            }
            Debug.WriteLine("");
        }
    }
}
