using System.Linq;
using DesperateDevs.Roslyn;
using DesperateDevs.Utils;
using NSpec;
using Shouldly;

class describe_ProjectParser : nspec {

    void when_parsing() {

        var projectRoot = TestExtensions.GetProjectRoot();
        var projectPath = projectRoot + "/Tests/Fixtures/TestFixtures/TestFixtures.csproj";

        context["project"] = () => {

            ProjectParser parser = null;

            before = () => {
                parser = new ProjectParser(projectPath);
            };

            it["gets all types"] = () => {
                var types = parser.GetTypes();
                var hasComponent = types.Any(c => c.ToCompilableString() == typeof(TestClass).ToCompilableString());
                hasComponent.ShouldBeTrue();
            };
        };
    }
}
