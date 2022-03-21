using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace DesperateDevs.Logging.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class LoggerBenchmarks
    {
        public LoggerBenchmarks()
        {
            Sherlog.AddAppender((logger, level, message) => Thread.Sleep(10));
        }

        [Benchmark]
        public void Log()
        {
            Sherlog.Debug(new string('.', 1000));
        }
    }
}
