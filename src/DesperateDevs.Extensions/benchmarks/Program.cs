using BenchmarkDotNet.Running;

namespace DesperateDevs.Extensions.Benchmarks
{
    class Program
    {
        public static void Main(string[] args)
        {
            // BenchmarkRunner.Run(typeof(AppDomainExtensionBenchmarks));
            // BenchmarkRunner.Run(typeof(DictionaryExtensionBenchmarks));
            BenchmarkRunner.Run(typeof(StringExtensionBenchmarks));
        }
    }
}
