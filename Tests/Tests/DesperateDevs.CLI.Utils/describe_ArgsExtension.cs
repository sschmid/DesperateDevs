using System;
using DesperateDevs.CLI.Utils;
using NSpec;
using Shouldly;

class describe_ArgsExtension : nspec {

    void when_Args() {

        detectsParameter("verbose", "-v", ArgsExtension.IsVerbose);
        detectsParameter("silent", "-s", ArgsExtension.IsSilent);
        detectsParameter("debug", "-d", ArgsExtension.IsDebug);
        detectsParameter("verbose because of debug", "-d", ArgsExtension.IsVerbose);

        it["filters default parameter starting with -"] = () => {
            var args = new[] { "-v", "-s", "-d", "value" };
            var filtered = args.WithoutDefaultParameter();

            filtered.Length.ShouldBe(1);
            filtered.ShouldContain("value");
        };

        it["keeps custom parameter starting with -"] = () => {
            var args = new[] { "-v", "-s", "-d", "-f" };
            var filtered = args.WithoutDefaultParameter();

            filtered.Length.ShouldBe(1);
            filtered.ShouldContain("-f");
        };

        it["filters all parameter starting with -"] = () => {
            var args = new[] { "-v", "-s", "-d", "-x", "-y", "value" };
            var filtered = args.WithoutParameter();

            filtered.Length.ShouldBe(1);
            filtered.ShouldContain("value");
        };

        it["filters trigger"] = () => {
            var args = new[] { "value1", "-p", "value2" };
            var filtered = args.WithoutTrigger();

            filtered.Length.ShouldBe(2);
            filtered.ShouldContain("-p");
            filtered.ShouldContain("value2");
        };
    }

    void detectsParameter(string name, string parameter, Func<string[], bool> method) {

        context[name + " " + parameter] = () => {

            it["doesn't detect parameter " + parameter + " when it doesn't exist"] = () => {
                method(new[] { "value" }).ShouldBeFalse();
            };

            it["detects parameter " + parameter + " when it exist"] = () => {
                method(new[] { "value", parameter }).ShouldBeTrue();
            };
        };
    }
}
