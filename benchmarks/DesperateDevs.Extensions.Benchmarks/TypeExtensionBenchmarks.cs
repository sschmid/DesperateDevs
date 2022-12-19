using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace DesperateDevs.Extensions.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class TypeExtensionBenchmarks
    {
        readonly string _typeString;

        public TypeExtensionBenchmarks()
        {
            _typeString = GetType().FullName;
        }

        [Benchmark]
        public string TypeNameSplit()
        {
            var split = _typeString.Split('.');
            return split[split.Length - 1];
        }

        [Benchmark]
        public string TypeNameIndexOf()
        {
            var index = _typeString.LastIndexOf(".", StringComparison.Ordinal) + 1;
            return _typeString.Substring(index, _typeString.Length - index);
        }
    }
}
