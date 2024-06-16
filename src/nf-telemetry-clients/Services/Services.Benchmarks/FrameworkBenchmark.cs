using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using System.Text;
using TelemetryStash.ServiceModels;

/*
    Platform: ESP32
    Manufacturer: MinSizeRel build, chip rev. >= 0, without support for PSRAM
    TargetName: ESP32_REV0
    NanoFramework version: 1.9.1.0
    Floating point precision: 1

    Console export: FrameworkBenchmark benchmark class.
    | MethodName           | IterationCount | Mean                  | Ratio  | Min    | Max    |
    | ---------------------------------------------------------------------------------------- |
    | Concat_Plus          | 10             | 36 ms                 | 1.0    | 30 ms  | 40 ms  |
    | Concat_PlusConstants | 10             | 36 ms                 | 1.0000 | 30 ms  | 40 ms  |
    | ConcatStringBuilder  | 10             | 107 ms                | 2.9722 | 100 ms | 110 ms |
    | Concat_Interpolation | 10             | 343.99999999999999 ms | 9.5556 | 330 ms | 370 ms |
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
