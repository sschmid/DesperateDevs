using DesperateDevs.Utils;
using NSpec;
using Shouldly;

class describe_StringExtension : nspec {

    void when_string() {

        it["UppercaseFirst"] = () => {
            const string str = "hi";
            str.UppercaseFirst().ShouldBe("Hi");
        };

        it["UppercaseFirst handles empty string"] = () => {
            const string str = "";
            str.UppercaseFirst().ShouldBe("");
        };

        it["LowercaseFirst"] = () => {
            const string str = "Hi";
            str.LowercaseFirst().ShouldBe("hi");
        };

        it["LowercaseFirst"] = () => {
            const string str = "";
            str.LowercaseFirst().ShouldBe("");
        };

        it["ToUnixLineEndings"] = () => {
            const string str = "1\r\n2\r3\n";
            str.ToUnixLineEndings().ShouldBe("1\n2\n3\n");
        };

        it["ToUnixPath"] = () => {
            const string str = "a/b\\c\\d/e";
            str.ToUnixPath().ShouldBe("a/b/c/d/e");
        };

        it["ToCSV"] = () => {
            var strings = new[] { "1", "2", "3" };
            strings.ToCSV().ShouldBe("1, 2, 3");
        };

        it["ArrayFromCSV"] = () => {
            const string str = "1,2, 3";
            str.ArrayFromCSV().ShouldBe(new[] { "1", "2", "3" });
        };

        it["ToSpacedCamelCase"] = () => {
            const string str = "ThisIsATest";
            str.ToSpacedCamelCase().ShouldBe("This Is A Test");
        };
    }
}
