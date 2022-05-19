using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Sherlog.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class LoggerBenchmarks
    {
        public LoggerBenchmarks()
        {
            Logger.AddAppender((logger, level, message) => Thread.Sleep(10));
        }

        [Benchmark]
        public void Log()
        {
            var logger = Logger.GetLogger(typeof(LoggerBenchmarks));
            logger.Debug(new string('.', 1000));
        }
    }
}
