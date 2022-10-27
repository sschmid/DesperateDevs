using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace DesperateDevs.Reflection.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class AppDomainExtensionBenchmarks
    {
        [Benchmark]
        public void GetAllTypes()
        {
            AppDomain.CurrentDomain.GetAllTypes();
        }
    }
}
