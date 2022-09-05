using BenchmarkDotNet.Running;

namespace Jenny.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run(typeof(CodeGeneratorDataBenchmarks));
        }
    }
}
