using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace DesperateDevs.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class StringBenchmarks
    {
        Random _random1;
        Random _random2;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _random1 = new Random(0);
            _random2 = new Random(0);
        }

        [Benchmark]
        public string Concat() => "message: " + _random1.Next() + _random1.Next() + _random1.Next();

        [Benchmark]
        public string Interpolated() => $"message: {_random2.Next()}{_random2.Next()}{_random2.Next()}";
    }
}
