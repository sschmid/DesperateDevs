using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace DesperateDevs.Serialization.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class PropertiesBenchmarks
    {
        [Params(100, 10000)]
        public int Lines { get; set; }

        [Benchmark]
        public Properties AddProperties()
        {
            var properties = new Properties();
            for (var i = 0; i < Lines; i++)
                properties["key" + i] = "value" + i;

            return properties;
        }

        [Benchmark]
        public Properties GetProperties()
        {
            var properties = new Properties();
            for (var i = 0; i < Lines; i++)
                properties["key" + i] = "value" + i;

            for (var i = 0; i < Lines; i++)
            {
                var value = properties["key" + i];
            }

            return properties;
        }

        [Benchmark]
        public string PropertiesToString()
        {
            var properties = new Properties();
            for (var i = 0; i < Lines; i++)
                properties["key" + i] = "value" + i;

            return properties.ToString();
        }

        [Benchmark]
        public string PropertiesToMinifiedString()
        {
            var properties = new Properties();
            for (var i = 0; i < Lines; i++)
                properties["key" + i] = "value" + i;

            return properties.ToMinifiedString();
        }
    }
}
