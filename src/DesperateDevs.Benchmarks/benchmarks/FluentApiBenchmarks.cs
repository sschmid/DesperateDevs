using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace DesperateDevs.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class FluentApiBenchmarks
    {
        SomeObject _object;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _object = new SomeObject();
        }

        [Benchmark]
        public void NonFluent() => _object.NonFluent();

        [Benchmark]
        public void Fluent() => _object.Fluent();
    }

    public class SomeObject { }

    public static class EntityExtension
    {
        public static void NonFluent(this SomeObject someObject)
        {
            // do sth
        }

        public static SomeObject Fluent(this SomeObject someObject)
        {
            // do sth
            return someObject;
        }
    }
}
