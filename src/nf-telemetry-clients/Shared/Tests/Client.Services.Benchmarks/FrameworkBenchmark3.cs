using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using System;
using System.Diagnostics;
using TelemetryStash.NfClient.Services;
using TelemetryStash.Shared;

namespace TelemetryStash.Services.Benchmarks
{
    [ConsoleParser]
    [IterationCount(1)]
    public class FrameworkBenchmark3
    {
        const UInt16 About65k = UInt16.MaxValue;

        [Benchmark, Baseline]
        public void FillArray_Int32()
        {
            PrintMemoryMetrics("Pre FillArray_Int32");
            var until = (int)About65k;
            var array = new int[until];
            for (int i = 0; i < until; i++)
            {
                array[i] = i;
            }
            PrintMemoryMetrics("Post FillArray_Int32");
        }

        [Benchmark]
        public void FillArray_UInt16()
        {
            PrintMemoryMetrics("Pre FillArray_UInt16");
            var array = new UInt16[About65k];
            for (UInt16 i = 0; i < About65k; i++)
            {
                array[i] = i;
            }
            PrintMemoryMetrics("Post FillArray_UInt16");
        }

        [Benchmark]
        public void FillArray_Int64()
        {
            PrintMemoryMetrics("Pre FillArray_Int64");
            var until = About65k;
            var array = new Int64[until];
            for (Int64 i = 0; i < About65k; i++)
            {
                array[i] = i;
            }
            PrintMemoryMetrics("Post FillArray_Int64");
        }

        private static void PrintMemoryMetrics(string info)
        {
            var metrics = DeviceMetrics.GetDeviceMetrics();
            var free = ((RegisterSet)metrics[2]).Registers[0].Value;
            Debug.WriteLine($"[{info}] Free Memory: {free}");
        }
    }
}
