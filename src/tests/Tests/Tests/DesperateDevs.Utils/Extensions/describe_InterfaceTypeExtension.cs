using DesperateDevs.Utils;
using NSpec;
using Shouldly;

class describe_InterfaceTypeExtension : nspec {

    void when_type() {

        it["return false if type doesn't implement interface"] = () => {
            typeof(object).ImplementsInterface<ITestInterface>().ShouldBeFalse();
        };

        it["return false if type is same"] = () => {
            typeof(ITestInterface).ImplementsInterface<ITestInterface>().ShouldBeFalse();
        };

        it["return false if type is interface"] = () => {
            typeof(ITestSubInterface).ImplementsInterface<ITestInterface>().ShouldBeFalse();
        };

        it["return true if type implements interface"] = () => {
            typeof(TestInterfaceClass).ImplementsInterface<ITestInterface>().ShouldBeTrue();
        };
    }
}
