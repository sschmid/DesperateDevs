using BenchmarkDotNet.Running;

namespace Sherlog.Benchmarks
{
    static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run(typeof(LoggerBenchmarks));
        }
    }
}
