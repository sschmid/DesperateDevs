using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace DesperateDevs.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class FluentApiBenchmarks
    {
        object _object;

        [GlobalSetup]
        public void GlobalSetup() => _object = new object();

        [Benchmark]
        public void NonFluent() => _object.NonFluent();

        [Benchmark]
        public object Fluent() => _object.Fluent();
    }

    public static class EntityExtension
    {
        public static void NonFluent(this object obj) { }
        public static object Fluent(this object obj) => obj;
    }
}
