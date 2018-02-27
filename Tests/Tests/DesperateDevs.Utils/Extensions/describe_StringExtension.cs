using DesperateDevs.Utils;
using NSpec;

class describe_StringExtension : nspec {

    void when_string() {

        it["UppercaseFirst"] = () => {
            const string str = "hi";
            str.UppercaseFirst().should_be("Hi");
        };

        it["UppercaseFirst handles empty string"] = () => {
            const string str = "";
            str.UppercaseFirst().should_be("");
        };

        it["LowercaseFirst"] = () => {
            const string str = "Hi";
            str.LowercaseFirst().should_be("hi");
        };

        it["LowercaseFirst"] = () => {
            const string str = "";
            str.LowercaseFirst().should_be("");
        };

        it["ToUnixLineEndings"] = () => {
            const string str = "1\r\n2\r3\n";
            str.ToUnixLineEndings().should_be("1\n2\n3\n");
        };

        it["ToCSV"] = () => {
            var strings = new[] { "1", "2", "3" };
            strings.ToCSV().should_be("1, 2, 3");
        };

        it["ArrayFromCSV"] = () => {
            const string str = "1,2, 3";
            str.ArrayFromCSV().should_be(new[] { "1", "2", "3" });
        };

        it["ToSpacedCamelCase"] = () => {
            const string str = "ThisIsATest";
            str.ToSpacedCamelCase().should_be("This Is A Test");
        };
    }
}
