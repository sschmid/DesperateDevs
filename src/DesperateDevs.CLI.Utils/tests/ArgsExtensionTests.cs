using System;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.CLI.Utils.Tests
{
    public class ArgsExtensionTests
    {
        static void AssertArg(string parameter, Func<string[], bool> method)
        {
            method(new[] {"value"}).Should().BeFalse();
            method(new[] {"value", parameter}).Should().BeTrue();
        }

        [Fact]
        public void IsVerbose() => AssertArg("-v", ArgsExtension.IsVerbose);

        [Fact]
        public void IsSilent() => AssertArg("-s", ArgsExtension.IsSilent);

        [Fact]
        public void IsDebug() => AssertArg("-d", ArgsExtension.IsDebug);

        [Fact]
        public void IsDebugSetsIsVerbose() => AssertArg("-d", ArgsExtension.IsVerbose);

        [Fact]
        public void FiltersDefaultParameter()
        {
            var filtered = new[] {"-v", "-s", "-d", "value"}.WithoutDefaultParameter();
            filtered.Length.Should().Be(1);
            filtered.Should().Contain("value");
        }

        [Fact]
        public void KeepsCustomParameter()
        {
            var filtered = new[] {"-v", "-s", "-d", "-f"}.WithoutDefaultParameter();
            filtered.Length.Should().Be(1);
            filtered.Should().Contain("-f");
        }

        [Fact]
        public void FiltersAllParameter()
        {
            var filtered = new[] {"-v", "-s", "-d", "-x", "-y", "value"}.WithoutParameter();
            filtered.Length.Should().Be(1);
            filtered.Should().Contain("value");
        }

        [Fact]
        public void FiltersTrigger()
        {
            var filtered = new[] {"value1", "-p", "value2"}.WithoutTrigger();
            filtered.Length.Should().Be(2);
            filtered.Should().Contain("-p");
            filtered.Should().Contain("value2");
        }
    }
}
