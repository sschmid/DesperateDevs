using BenchmarkDotNet.Running;
using DesperateDevs.Logging.Benchmarks;

namespace DesperateDevs.Extensions.Benchmarks
{
    class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run(typeof(LoggerBenchmarks));
        }
    }
}
