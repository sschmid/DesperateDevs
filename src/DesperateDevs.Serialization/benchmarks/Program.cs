using BenchmarkDotNet.Running;

namespace DesperateDevs.Serialization.Benchmarks
{
    class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run(typeof(PropertiesBenchmarks));
        }
    }
}
