using BenchmarkDotNet.Running;

namespace DesperateDevs.Benchmarks
{
    static class Program
    {
        public static void Main(string[] args)
        {
            // BenchmarkRunner.Run(typeof(CollectionBenchmarks));
            // BenchmarkRunner.Run(typeof(FluentApiBenchmarks));
            BenchmarkRunner.Run(typeof(StringBenchmarks));
        }
    }
}
