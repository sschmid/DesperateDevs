using BenchmarkDotNet.Running;

namespace Sherlog.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run(typeof(LoggerBenchmarks));
        }
    }
}
