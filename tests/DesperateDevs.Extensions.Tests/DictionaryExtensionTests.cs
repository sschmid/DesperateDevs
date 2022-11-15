using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Extensions.Tests
{
    public class DictionaryExtensionTests
    {
        [Fact]
        public void MergesDictionary()
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

            var merged = d1.Merge(d2);
            merged.Should().BeSameAs(d1);
            merged.Should().HaveCount(3);
            merged.Keys.Should().Contain("k1");
            merged.Keys.Should().Contain("k2");
            merged.Keys.Should().Contain("k3");
            merged["k1"].Should().Be("v2");
        }

        [Fact]
        public void MergesDictionaries()
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
            merged.Should().HaveCount(4);
            merged.Keys.Should().Contain("k1");
            merged.Keys.Should().Contain("k2");
            merged.Keys.Should().Contain("k3");
            merged.Keys.Should().Contain("k4");
            merged["k1"].Should().Be("v3");
        }
    }
}
