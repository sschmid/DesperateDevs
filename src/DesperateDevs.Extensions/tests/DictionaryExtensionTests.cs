using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Extensions.Tests
{
    public class DictionaryExtensionTests
    {
        [Fact]
        public void MergesIntoDictionary()
        {
            var d1 = new Dictionary<string, string>
            {
                {"k1", "v1"},
                {"k2", "v2"}
            };

            var d2 = new Dictionary<string, string>
            {
                {"k1", "v2"},
                {"k3", "v3"}
            };

            var d3 = new Dictionary<string, string>
            {
                {"k1", "v3"},
                {"k4", "v4"}
            };

            var merged = d1.Merge(new[] {d2, d3});
            merged.Should().BeSameAs(d1);
            merged.Count.Should().Be(4);
            merged.Keys.Should().Contain("k1");
            merged.Keys.Should().Contain("k2");
            merged.Keys.Should().Contain("k3");
            merged.Keys.Should().Contain("k4");
            merged["k1"].Should().Be("v3");
        }
    }
}
