using DesperateDevs.Utils;
using NSpec;

class describe_InterfaceTypeExtension : nspec {

    void when_type() {

        it["return false if type doesn't implement interface"] = () => {
            typeof(object).ImplementsInterface<ITestInterface>().should_be_false();
        };

        it["return false if type is same"] = () => {
            typeof(ITestInterface).ImplementsInterface<ITestInterface>().should_be_false();
        };

        it["return false if type is interface"] = () => {
            typeof(ITestSubInterface).ImplementsInterface<ITestInterface>().should_be_false();
        };

        it["return true if type implements interface"] = () => {
            typeof(TestInterfaceClass).ImplementsInterface<ITestInterface>().should_be_true();
        };
    }
}
