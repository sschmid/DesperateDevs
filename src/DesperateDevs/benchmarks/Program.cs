using BenchmarkDotNet.Running;

namespace DesperateDevs.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // BenchmarkRunner.Run(typeof(CollectionBenchmarks));
            BenchmarkRunner.Run(typeof(FluentApiBenchmarks));
        }
    }
}
