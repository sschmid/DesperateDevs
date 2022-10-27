using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace DesperateDevs.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class CollectionBenchmarks
    {
        [Params(100, 10000)]
        public int N { get; set; }

        string[] _values;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _values = new string[N];
            for (var i = 0; i < N; i++) _values[i] = i.ToString();
        }

        [Benchmark]
        public void ReturnAsArray()
        {
            foreach (var unused in GetAsArray()) { }
        }

        [Benchmark]
        public void ReturnAsIEnumerable()
        {
            foreach (var unused in GetAsIEnumerable()) { }
        }

        [Benchmark]
        public void ReturnAsIEnumerableToArray()
        {
            foreach (var unused in GetAsIEnumerable().ToArray()) { }
        }

        string[] GetAsArray() => _values;
        IEnumerable<string> GetAsIEnumerable() => _values;
    }
}
