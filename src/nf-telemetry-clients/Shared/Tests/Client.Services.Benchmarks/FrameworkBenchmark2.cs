using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using System;

namespace TelemetryStash.Services.Benchmarks
{
    [ConsoleParser]
    [IterationCount(10)]
    public class FrameworkBenchmark2
    {
        const UInt16 About65k = UInt16.MaxValue;

        [Benchmark, Baseline]
        public void FillArray_Int32()
        {
            var until = (int)About65k;
            var array = new int[until];
            for (int j = 0; j < until; j++)
            {
                array[j] = j;
            }
        }

        [Benchmark]
        public void FillArray_UInt16()
        {
            var array = new UInt16[About65k];
            for (UInt16 j = 0; j < About65k; j++)
            {
                array[j] = j;
            }
        }

        [Benchmark]
        public void FillArray_Int64()
        {
            var until = (Int64)About65k;
            var array = new Int64[until];
            for (long j = 0; j < until; j++)
            {
                array[j] = j;
            }
        }
    }
}
