using BenchmarkDotNet.Running;

namespace DesperateDevs.Logging.Benchmarks
{
    static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run(typeof(LoggerBenchmarks));
        }
    }
}
