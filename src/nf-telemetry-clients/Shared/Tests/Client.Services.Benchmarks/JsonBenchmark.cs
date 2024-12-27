using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using nanoFramework.Json;
using TelemetryStash.NfClient.Services;
using TelemetryStash.Shared;

namespace TelemetryStash.Services.Benchmarks
{
    /*
    
    System Information
    HAL build info: nanoCLR running @ ESP32 built with ESP-IDF c9763f6
      Target:   ESP32_PSRAM_BLE_GenericGraphic_
      Platform: ESP32

    Firmware build Info:
      Date:        Dec  7 2024
      Type:        MinSizeRel build, chip rev. >= 3, support for PSRAM, support for BLE
      CLR Version: 1.12.1.54
      Compiler:    GNU ARM GCC v13.2.0

    | MethodName                        | IterationCount | Mean    | Ratio   | Min      | Max      |
    | -------------------------------------------------------------------------------------------- |
    | SerializeTelemetry_Numbers_only   | 10             | 37 ms   | 1.0     | 30 ms    | 40 ms    |
    | SerializeTelemetry_Text_only      | 10             | 61 ms   | 1.6486  | 50 ms    | 70 ms    |
    | SerializeTelemetry_Long_text_only | 10             | 15 ms   | 0.4054  | 10 ms    | 20 ms    |
    | SerializeObject_Numbers_only      | 10             | 1536 ms | 41.5135 | 1,440 ms | 1,620 ms |
    | SerializeObject_Text_only         | 10             | 924 ms  | 24.9730 | 890 ms   | 940 ms   |
    | SerializeObject_Long_text_only    | 10             | 164 ms  | 4.4324  | 150 ms   | 170 ms   |

    */

    [ConsoleParser]
    [IterationCount(10)]
    public class JsonBenchmark
    {
        Telemetry _numbersOnlyTelemetry;
        Telemetry _textOnlyTelemetry;
        Telemetry _longTextOnlyTelemetry;

        [Setup]
        public void Setup()
        {
            _numbersOnlyTelemetry = TestData.NumbersOnlyTelemetry;
            _textOnlyTelemetry = TestData.TextOnlyTelemetry;
            _longTextOnlyTelemetry = TestData.LongTextOnlyTelemetry;
        }

        [Benchmark, Baseline]
        public void SerializeTelemetry_Numbers_only()
        {
            JsonSerialize.SerializeTelemetry(_numbersOnlyTelemetry);
        }

        [Benchmark]
        public void SerializeTelemetry_Text_only()
        {
            JsonSerialize.SerializeTelemetry(_textOnlyTelemetry);
        }

        [Benchmark]
        public void SerializeTelemetry_Long_text_only()
        {
            JsonSerialize.SerializeTelemetry(_longTextOnlyTelemetry);
        }

        [Benchmark]
        public void SerializeObject_Numbers_only()
        {
            var json = JsonConvert.SerializeObject(_numbersOnlyTelemetry);
        }

        [Benchmark]
        public void SerializeObject_Text_only()
        {
            JsonConvert.SerializeObject(_textOnlyTelemetry);
        }

        [Benchmark]
        public void SerializeObject_Long_text_only()
        {
            JsonConvert.SerializeObject(_longTextOnlyTelemetry);
        }
    }
}
