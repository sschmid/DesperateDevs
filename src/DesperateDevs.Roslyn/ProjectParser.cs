using System.Linq;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace DesperateDevs.Roslyn
{
    public class ProjectParser
    {
        readonly Project _project;
        INamedTypeSymbol[] _types;

        public ProjectParser(string projectPath)
        {
            if (!MSBuildLocator.IsRegistered) MSBuildLocator.RegisterDefaults();
            using (var workspace = MSBuildWorkspace.Create())
            {
                _project = workspace.OpenProjectAsync(projectPath).Result;
            }
        }

        public INamedTypeSymbol[] GetTypes()
        {
            if (_types == null)
            {
                _types = _project.GetCompilationAsync().Result
                    .GetSymbolsWithName(name => true, SymbolFilter.Type)
                    .OfType<INamedTypeSymbol>()
                    .ToArray();
            }

            return _types;
        }
    }
}
