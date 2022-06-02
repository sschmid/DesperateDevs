using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Cli.Utils.Tests
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
        public void IsForce() => AssertArg("-f", ArgsExtension.IsForce);

        [Fact]
        public void IsYes() => AssertArg("-y", ArgsExtension.IsYes);

        [Fact]
        public void IsNo() => AssertArg("-n", ArgsExtension.IsNo);

        [Fact]
        public void FiltersDefaultParameter()
        {
            var filtered = new[] {"-v", "-s", "-d", "-f", "-y", "-n", "value"}.WithoutDefaultParameter().ToArray();
            filtered.Length.Should().Be(1);
            filtered.Should().Contain("value");
        }

        [Fact]
        public void KeepsCustomParameter()
        {
            var filtered = new[] {"-v", "-s", "-d", "-g"}.WithoutDefaultParameter().ToArray();
            filtered.Length.Should().Be(1);
            filtered.Should().Contain("-g");
        }

        [Fact]
        public void FiltersAllParameter()
        {
            var filtered = new[] {"-v", "-s", "-d", "-x", "-y", "value"}.WithoutParameter().ToArray();
            filtered.Length.Should().Be(1);
            filtered.Should().Contain("value");
        }

        [Fact]
        public void FiltersTrigger()
        {
            var filtered = new[] {"value1", "-p", "value2"}.WithoutTrigger().ToArray();
            filtered.Length.Should().Be(2);
            filtered.Should().Contain("-p");
            filtered.Should().Contain("value2");
        }
    }
}
