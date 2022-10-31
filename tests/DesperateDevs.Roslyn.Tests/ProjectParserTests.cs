using System.IO;
using DesperateDevs.Tests;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Roslyn.Tests
{
    public class ProjectParserTests
    {
        static readonly string TestProject = Path.Combine(
            TestHelper.GetProjectRoot(), "tests", "DesperateDevs.Roslyn.Tests.Fixture", "DesperateDevs.Roslyn.Tests.Fixture.csproj"
        );

        [Fact]
        public void GetsAllTypes()
        {
            new ProjectParser(TestProject)
                .GetTypes()
                .Should().Contain(c => c.ToCompilableString() == "DesperateDevs.Roslyn.Tests.Fixture.TestClass");
        }
    }
}
