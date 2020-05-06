using DesperateDevs.CodeGeneration;
using NSpec;
using Shouldly;

class describe_CodeGeneratorData : nspec {

    void when_providing() {

        CodeGeneratorData data = null;

        before = () => {
            data = new CodeGeneratorData();
            data["Key"] = "World!";
        };

        it["contains added object"] = () => {
            data["Key"].ShouldBe("World!");
        };

        it["replaces placeholders"] = () => {
            const string template = "Hello, ${Key}";
            data.ReplacePlaceholders(template).ShouldBe("Hello, World!");
        };

        it["respects case of placeholder"] = () => {
            const string template = "Hello, ${key}";
            data.ReplacePlaceholders(template).ShouldBe("Hello, world!");
        };

        it["clones data"] = () => {
            var newData = new CodeGeneratorData(data);
            newData.Count.ShouldBe(data.Count);
            newData["Key"].ShouldBe("World!");
        };
    }
}
