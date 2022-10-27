using BenchmarkDotNet.Running;

namespace DesperateDevs.Reflection.Benchmarks
{
    static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run(typeof(AppDomainExtensionBenchmarks));
        }
    }
}
