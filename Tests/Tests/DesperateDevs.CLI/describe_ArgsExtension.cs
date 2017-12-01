using System;
using DesperateDevs.CLI;
using NSpec;

class describe_ArgsExtension : nspec {

    void when_Args() {

        detectsParameter("force", "-f", ArgsExtension.isForce);
        detectsParameter("verbose", "-v", ArgsExtension.isVerbose);
        detectsParameter("silent", "-s", ArgsExtension.isSilent);

        it["filters parameter starting with -"] = () => {
            var args = new[] { "value1", "-p", "value2" };
            var filtered = args.WithoutParameter();

            filtered.Length.should_be(2);
            filtered.should_contain("value1");
            filtered.should_contain("value2");
        };

        it["filters trigger"] = () => {
            var args = new[] { "value1", "-p", "value2" };
            var filtered = args.WithoutTrigger();

            filtered.Length.should_be(2);
            filtered.should_contain("-p");
            filtered.should_contain("value2");
        };
    }

    void detectsParameter(string name, string parameter, Func<string[], bool> method) {

        context[name + " " + parameter] = () => {

            it["doesn't detect parameter " + parameter + " when it doesn't exist"] = () => {
                method(new[] { "value" }).should_be_false();
            };

            it["detects parameter " + parameter + " when it exist"] = () => {
                method(new[] { "value", parameter }).should_be_true();
            };
        };
    }
}
