using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Order;

namespace DesperateDevs.Extensions.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class StringExtensionBenchmarks
    {
        readonly string[] _values;
        readonly string _csv;

        public StringExtensionBenchmarks()
        {
            _values = new string[10000];
            _csv = string.Empty;
            for (var i = 0; i < _values.Length; i++)
            {
                _values[i] = $"Number {i.ToString()}";
                _csv += $"Number {i},";
            }
        }

        [Benchmark]
        public string ToCSV() => _values.ToCSV(false, true);

        [Benchmark]
        public string[] FromCSV() => _csv.FromCSV(true).ToArray();
    }
}
