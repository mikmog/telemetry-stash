using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using System.Collections;
using System.Text;
using TelemetryStash.Shared;

namespace TelemetryStash.Services.Benchmarks
{
    [ConsoleParser]
    [IterationCount(10)]
    public class FrameworkBenchmark
    {
        const int Thousand = 1000;

        string[] _hundredWords;

        readonly int[] _array = new int[Thousand];
        readonly Hashtable _hashTable = new(Thousand);

        [Setup]
        public void Setup()
        {
            _hundredWords = TestData.OneHundredWords;

            for (var i = 0; i < Thousand; i++)
            {
                _array[i] = i;
                _hashTable.Add(i, i);
            }
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
            var _ = string.Concat(_hundredWords);
        }

        [Benchmark]
        public void ConcatString_StringConcatParams()
        {
            var _ = string.Concat("apple", "banana", "cherry", "date", "elderberry", "fig", "grape", "honeydew", "kiwi", "lemon",
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
        public void HashTable_Add_thousand()
        {
            var ht = new Hashtable();
            for(var i = 0; i < Thousand; i++)
            {
                ht.Add(i, i);
            }
        }

        [Benchmark]
        public void HashTable_Add_thousand_max_loadFactor()
        {
            var ht = new Hashtable(1, 1);
            for(var i = 0; i < Thousand; i++)
            {
                ht.Add(i, i);
            }
        }

        [Benchmark]
        public void HashTable_Add_thousand_initial_capacity_max_loadFactor()
        {
            var ht = new Hashtable(Thousand, 1);
            for (var i = 0; i < Thousand; i++)
            {
                ht.Add(i, i);
            }
        }

        [Benchmark]
        public void Array_Add_thousand()
        {
            var al = new int[Thousand];
            for (var i = 0; i < Thousand; i++)
            {
                al[i] = i;
            }
        }

        [Benchmark]
        public void ArrayList_Add_thousand()
        {
            var al = new ArrayList();
            for (var i = 0; i < Thousand; i++)
            {
                al.Add(i);
            }
        }


        [Benchmark]
        public void Array_Get_by_index()
        {
            for (var i = 0; i < Thousand; i++)
            {
                var _ = _array[i];
            }
        }

        [Benchmark]
        public void HashTable_Get_by_index()
        {
            for (var i = 0; i < Thousand; i++)
            {
                var _ = (int)_hashTable[i];
            }
        }
    }
}
