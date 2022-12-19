using BenchmarkDotNet.Running;

namespace DesperateDevs.Extensions.Benchmarks
{
    static class Program
    {
        public static void Main(string[] args)
        {
            // BenchmarkRunner.Run(typeof(DictionaryExtensionBenchmarks));
            // BenchmarkRunner.Run(typeof(StringExtensionBenchmarks));
            BenchmarkRunner.Run(typeof(TypeExtensionBenchmarks));
        }
    }
}
