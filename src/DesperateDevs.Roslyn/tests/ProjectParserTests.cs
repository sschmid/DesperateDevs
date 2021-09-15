using System.IO;
using System.Linq;
using DesperateDevs.Tests;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Roslyn.Tests
{
    public class ProjectParserTests
    {
        static readonly string TestProject = Path.Combine(
            TestHelper.GetProjectRoot(), "DesperateDevs.Roslyn", "fixtures", "DesperateDevs.Roslyn.Tests.Project", "DesperateDevs.Roslyn.Tests.Project.csproj"
        );

        [Fact]
        public void GetsAllTypes()
        {
            new ProjectParser(TestProject)
                .GetTypes()
                .Any(c => c.ToCompilableString() == "DesperateDevs.Roslyn.Tests.Project.TestClass")
                .Should().BeTrue();
        }
    }
}
