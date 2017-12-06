using DesperateDevs.CodeGeneration;
using NSpec;

class describe_CodeGeneratorData : nspec {

    void when_providing() {

        CodeGeneratorData data = null;

        before = () => {
            data = new CodeGeneratorData();
            data["Key"] = "World!";
        };

        it["contains added object"] = () => {
            data["Key"].should_be("World!");
        };

        it["replaces placeholders"] = () => {
            const string template = "Hello, ${Key}";
            data.ReplacePlaceholders(template).should_be("Hello, World!");
        };

        it["respects case of placeholder"] = () => {
            const string template = "Hello, ${key}";
            data.ReplacePlaceholders(template).should_be("Hello, world!");
        };
    }
}
