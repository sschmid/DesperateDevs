using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace DesperateDevs.Extensions.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class DictionaryExtensionBenchmarks
    {
        [Params(10, 100)]
        public int Factor { get; set; }

        [Benchmark]
        public Dictionary<string, string> Merge()
        {
            var d1 = CreateDict(0, 1);
            var d2 = CreateDict(1, 2);
            var d3 = CreateDict(2, 3);
            var d4 = CreateDict(3, 4);
            var d5 = CreateDict(4, 5);
            return d1.Merge(d2, d3, d4, d5);
        }

        Dictionary<string, string> CreateDict(int start, int end)
        {
            start *= Factor;
            end *= Factor;
            var dict = new Dictionary<string, string>(end - start);
            for (var i = start; i < end; i++)
            {
                var key = i.ToString();
                dict[key] = key;
            }

            return dict;
        }
    }
}
