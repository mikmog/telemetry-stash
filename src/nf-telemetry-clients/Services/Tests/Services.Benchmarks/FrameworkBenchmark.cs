using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using System.Text;
using TelemetryStash.ServiceModels;

/*
    Platform: ESP32
    Manufacturer: MinSizeRel build, chip rev. >= 0, support for PSRAM, support for BLE
    TargetName: ESP32_S3_ALL
    NanoFramework version: 1.10.0.49
    Floating point precision: 1
    | MethodName           | IterationCount | Mean                  | Ratio  | Min    | Max    |
    | ---------------------------------------------------------------------------------------- |
    | Concat_Plus          | 10             | 20 ms                 | 1.0    | 20 ms  | 20 ms  |
    | ConcatStringBuilder  | 10             | 49 ms                 | 2.4500 | 40 ms  | 50 ms  |
    | Concat_Interpolation | 10             | 162.99999999999999 ms | 8.1500 | 160 ms | 170 ms |


    Platform: ESP32
    Manufacturer: MinSizeRel build, chip rev. >= 3, without support for PSRAM
    TargetName: XIAO_ESP32C3
    NanoFramework version: 1.10.0.62
    Floating point precision: 1
    | MethodName           | IterationCount | Mean                  | Ratio  | Min    | Max    |
    | ---------------------------------------------------------------------------------------- |
    | Concat_Plus          | 10             | 50.999999999999996 ms | 1.0    | 50 ms  | 60 ms  |
    | ConcatStringBuilder  | 10             | 116.99999999999999 ms | 2.2941 | 110 ms | 120 ms |
    | Concat_Interpolation | 10             | 329.99999999999998 ms | 6.4706 | 320 ms | 340 ms |
 */

namespace TelemetryStash.Services.Benchmarks
{
    [ConsoleParser]
    [IterationCount(10)]
    public class FrameworkBenchmark
    {
        Telemetry _telemetry;

        [Setup]
        public void Setup()
        {
            _telemetry = TestData.StaticKeyTelemetry;
        }

        [Benchmark, Baseline]
        public void Concat_Plus()
        {
            var json = "{'Timestamp':" + _telemetry.Timestamp;

            foreach (RegisterSet set in _telemetry.RegisterSets)
            {
                foreach (Register reg in set.Registers)
                {
                    json += "{'Obis':'" + reg.Identifier + "','IntPart':" + reg.IntPart + ",'Fractions':" + reg.Fractions + "}";
                }
            }

            json += "]}";
        }

        [Benchmark]
        public void ConcatStringBuilder()
        {
            var sb = new StringBuilder();

            sb.Append("{");

            sb.Append("\"Timestamp\":");
            sb.Append(_telemetry.Timestamp);

            sb.Append(",\"Numbers\":[");
            foreach (RegisterSet set in _telemetry.RegisterSets)
            {
                foreach (Register number in set.Registers)
                {
                    sb.Append("{");
                    sb.Append("\"Obis\":");
                    sb.Append("\"");
                    sb.Append(number.Identifier);
                    sb.Append("\"");
                    sb.Append(",\"IntPart\":");
                    sb.Append(number.IntPart);
                    sb.Append(",\"Fractions\":");
                    sb.Append(number.Fractions);
                    sb.Append("}");
                }
            }

            sb.Append("]");
            sb.Append("}");

            var json = sb.ToString();
        }

        [Benchmark]
        public void Concat_Interpolation()
        {
            var json = $"{{'Timestamp':{_telemetry.Timestamp}";

            foreach (RegisterSet set in _telemetry.RegisterSets)
            {
                foreach (Register number in set.Registers)
                {
                    json += $"{{'Obis':'{number.Identifier}','IntPart':{number.IntPart},'Fractions':{number.Fractions}}}";
                }
            }

            json += "]}";
        }
    }
}
