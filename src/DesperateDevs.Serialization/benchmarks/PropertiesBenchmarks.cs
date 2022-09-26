using System.Text;
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

        string _input;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < Lines; i++)
            {
                var value = i.ToString();
                sb.Append("key");
                sb.Append(value);
                sb.Append(" = value");
                sb.AppendLine(value);
            }

            _input = sb.ToString();
        }

        [Benchmark]
        public Properties CreateProperties() => new Properties(_input);

        [Benchmark]
        public Properties AddProperties()
        {
            var properties = new Properties();
            for (var i = 0; i < Lines; i++)
                properties[$"key{i}"] = $"value{i}";

            return properties;
        }

        [Benchmark]
        public void GetProperties()
        {
            var properties = new Properties(_input);
            for (var i = 0; i < Lines; i++)
            {
                var unused = properties[$"key{i}"];
            }
        }

        [Benchmark]
        public string PropertiesToString() => new Properties(_input).ToString();

        [Benchmark]
        public string PropertiesToMinifiedString() => new Properties(_input).ToMinifiedString();
    }
}
