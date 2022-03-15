using FluentAssertions;
using Xunit;

namespace DesperateDevs.Tests
{
    public class InterfaceTypeExtensionTests
    {
        [Fact]
        public void ReturnsFalseIfTypeDoesNotOmplementInterface()
        {
            typeof(object).ImplementsInterface<ITestInterface>().Should().BeFalse();
        }

        [Fact]
        public void ReturnsFalseIfTypeIsSame()
        {
            typeof(ITestInterface).ImplementsInterface<ITestInterface>().Should().BeFalse();
        }

        [Fact]
        public void ReturnFalseIfTypeIsInterface()
        {
            typeof(ITestSubInterface).ImplementsInterface<ITestInterface>().Should().BeFalse();
        }

        [Fact]
        public void ReturnsTrueIfTypeImplementsInterface()
        {
            typeof(TestInterfaceClass).ImplementsInterface<ITestInterface>().Should().BeTrue();
        }
    }

    public interface ITestInterface { }

    public interface ITestSubInterface : ITestInterface { }

    public class TestInterfaceClass : ITestInterface { }
}
