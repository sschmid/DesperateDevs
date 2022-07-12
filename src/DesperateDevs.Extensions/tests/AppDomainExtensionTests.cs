using System;
using System.Linq;
using DesperateDevs.Extensions.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Extensions.Tests
{
    public class AppDomainExtensionTests
    {
        [Fact]
        public void GetsAllTypes()
        {
            var types = AppDomain.CurrentDomain.GetAllTypes().ToArray();
            types.Should().Contain(typeof(object));
            types.Should().Contain(typeof(AppDomainExtensionTests));
            types.Should().Contain(typeof(AppDomainExtension));
        }

        [Fact]
        public void GetNonAbstractTypes()
        {
            var types = AppDomain.CurrentDomain.GetNonAbstractTypes<ITestInterface>().ToArray();
            types.Should().HaveCount(1);
            types[0].Should().Be(typeof(TestInterfaceClass));
        }

        [Fact]
        public void GetInstancesOf()
        {
            var types = AppDomain.CurrentDomain.GetInstancesOf<ITestInterface>().ToArray();
            types.Should().HaveCount(1);
            types[0].GetType().Should().Be(typeof(TestInterfaceClass));
        }
    }
}
