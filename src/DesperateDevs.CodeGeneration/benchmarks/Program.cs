using BenchmarkDotNet.Running;

namespace DesperateDevs.CodeGeneration.Benchmarks
{
    class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run(typeof(CodeGeneratorDataBenchmarks));
        }
    }
}
