using DesperateDevs.CodeGeneration;
using NSpec;
using Shouldly;

class describe_CodeGenFile : nspec {

    void when_creating() {

        it["set fields"] = () => {
            var file = new CodeGenFile("name.cs", "content", "MyGenerator");
            file.fileName.ShouldBe("name.cs");
            file.fileContent.ShouldBe("content");
            file.generatorName.ShouldBe("MyGenerator");
        };

        it["converts new lines to unix"] = () => {
            var file = new CodeGenFile(null, "line1\r\nline2\r", null);
            file.fileContent.ShouldBe("line1\nline2\n");
        };

        it["converts new lines to unix when setting fileContent"] = () => {
            var file = new CodeGenFile(null, string.Empty, null);
            file.fileContent = "line1\r\nline2\r";
            file.fileContent.ShouldBe("line1\nline2\n");
        };
    }
}
