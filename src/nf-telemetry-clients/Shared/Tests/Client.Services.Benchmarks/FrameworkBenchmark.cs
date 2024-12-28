using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using TelemetryStash.Shared;

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

    | MethodName                          | IterationCount | Mean   | Ratio   | Min    | Max    |
    | ----------------------------------------------------------------------------------------- |
    | ConcatString_Plus                   | 10             | 12 ms  | 1.0     | 10 ms  | 20 ms  |
    | ConcatString_StringBuilder          | 10             | 56 ms  | 4.6667  | 50 ms  | 60 ms  |
    | ConcatString_Interpolation          | 10             | 18 ms  | 1.5000  | 10 ms  | 20 ms  |
    | ConcatString_StringConcatArray      | 10             | 2 ms   | 0.1667  | 0 ms   | 10 ms  |
    | ConcatString_StringConcatParams     | 10             | 2 ms   | 0.1667  | 0 ms   | 10 ms  |
    | ConcatString_StringConcatForEach    | 10             | 13 ms  | 1.0833  | 10 ms  | 20 ms  |
    | HashTable_Add_min_loadFactor        | 10             | 108 ms | 9.0000  | 100 ms | 120 ms |
    | HashTable_Add_max_loadFactor        | 10             | 116 ms | 9.6667  | 100 ms | 160 ms |
    | HashTable_Add_remove_max_loadFactor | 10             | 354 ms | 29.5000 | 340 ms | 360 ms |
    | ArrayList_Add_remove                | 10             | 312 ms | 26.0000 | 300 ms | 320 ms |

 */

namespace TelemetryStash.Services.Benchmarks
{
    [ConsoleParser]
    [IterationCount(10)]
    public class FrameworkBenchmark
    {
        Telemetry _telemetry;
        string[] _hundredWords;

        [Setup]
        public void Setup()
        {
            _telemetry = TestData.NumbersOnlyTelemetry;
            _hundredWords = TestData.OneHundredWords;
        }

        [Benchmark, Baseline]
        public void ConcatString_Plus()
        {
            string text = null;
            foreach (var word in _hundredWords)
            {
                text += word;
            }
        }

        [Benchmark]
        public void ConcatString_StringBuilder()
        {
            var sb = new StringBuilder();
            foreach (var word in _hundredWords)
            {
                sb.Append(word);
            }

            var _ = sb.ToString();
        }

        [Benchmark]
        public void ConcatString_Interpolation()
        {
            string text = null;
            foreach (var word in _hundredWords)
            {
                text += $" {word} ";
            }
        }

        [Benchmark]
        public void ConcatString_StringConcatArray()
        {
            var text = string.Concat(_hundredWords);
        }

        [Benchmark]
        public void ConcatString_StringConcatParams()
        {
            var text = string.Concat("apple", "banana", "cherry", "date", "elderberry", "fig", "grape", "honeydew", "kiwi", "lemon",
            "mango", "nectarine", "orange", "papaya", "quince", "raspberry", "strawberry", "tangerine", "ugli", "vanilla",
            "watermelon", "xigua", "yellowfruit", "zucchini", "apricot", "blackberry", "blueberry", "cantaloupe", "dragonfruit", "grapefruit",
            "jackfruit", "kumquat", "lime", "lychee", "mandarin", "mulberry", "olive", "peach", "pear", "pineapple",
            "plum", "pomegranate", "pumpkin", "rhubarb", "starfruit", "tomato", "avocado", "coconut", "cranberry", "currant",
            "gooseberry", "guava", "kiwifruit", "loquat", "mangosteen", "passionfruit", "persimmon", "plantain", "prune", "sapodilla",
            "soursop", "tamarind", "yuzu", "bilberry", "boysenberry", "cloudberry", "dewberry", "elderflower", "feijoa", "huckleberry",
            "jabuticaba", "jostaberry", "longan", "marionberry", "medlar", "miraclefruit", "naranjilla", "pawpaw", "pitaya", "rambutan",
            "redcurrant", "salak", "santol", "serviceberry", "sloe", "sugarapple", "surinamcherry", "whitecurrant", "wineberry", "wolfberry",
            "yumberry", "ziziphus", "acerola", "ackee", "açaí", "bignay", "burdekinplum", "calamondin", "camu", "cempedak");
        }

        [Benchmark]
        public void ConcatString_StringConcatForEach()
        {
            string text = null;
            foreach(var word in _hundredWords)
            {
                text = string.Concat(text, word);
            }
        }

        [Benchmark]
        public void HashTable_Add_min_loadFactor()
        {
            var ht = new Hashtable(1, 0.1f);
            for(var i = 0; i < 100; i++)
            {
                ht.Add(Guid.NewGuid(), i);
            }
        }

        [Benchmark]
        public void HashTable_Add_max_loadFactor()
        {
            var ht = new Hashtable(1, 0.1f);
            for(var i = 0; i < 100; i++)
            {
                ht.Add(Guid.NewGuid(), i);
            }
        }

        private static readonly object _lock = new();
        private enum Work { Add, Remove, Print }

        [Benchmark]
        public void HashTable_Add_remove_max_loadFactor()
        {
            var ht = new Hashtable(1, 1f);
            const int count = 100;

            void WorkHt(string key, Work work)
            {
                lock (_lock)
                {
                    if (work == Work.Add)
                    {
                        ht.Add(key.ToString(), key);
                    }

                    if (work == Work.Remove)
                    {
                        ht.Remove(key.ToString());
                    }

                    if (work == Work.Print)
                    {
                        Debug.WriteLine("Hashtable count: " + ht.Count);
                    }
                }
            }

            for (var i = 0; i < count; i++)
            {
                WorkHt(i.ToString(), Work.Add);
            }

            for (var i = 0; i < count; i++)
            {
                WorkHt(i.ToString(), Work.Remove);
            }

            //Debug.WriteLine("Hashtable count: " + ht.Count);
            //WorkHt("", Work.Print);
        }

        [Benchmark]
        public void ArrayList_Add_remove()
        {
            var list = new ArrayList();
            const int count = 100;

            void WorkList(string key, Work work)
            {
                lock (_lock)
                {
                    if (work == Work.Add)
                    {
                        list.Add(key.ToString());
                    }

                    if (work == Work.Remove)
                    {
                        list.Remove(key.ToString());
                    }

                    if (work == Work.Print)
                    {
                        Debug.WriteLine("List count: " + list.Count);
                    }
                }
            }

            for (var i = 0; i < count; i++)
            {
                WorkList(i.ToString(), Work.Add);
            }

            for (var i = 0; i < count; i++)
            {
                WorkList(i.ToString(), Work.Remove);
            }

            //Debug.WriteLine("List count: " + list.Count);
            //WorkList("", Work.Print);
        }
    }
}
