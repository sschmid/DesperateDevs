using System.Linq;
using DesperateDevs.Logging;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace DesperateDevs.Roslyn
{
    public class ProjectParser
    {
        static readonly Logger _logger = Sherlog.GetLogger(nameof(ProjectParser));

        readonly Project _project;
        INamedTypeSymbol[] _types;

        public ProjectParser(string projectPath)
        {
            _logger.Debug("Opening " + projectPath);

            if (!MSBuildLocator.IsRegistered) MSBuildLocator.RegisterDefaults();
            using (var workspace = MSBuildWorkspace.Create())
            {
                _project = workspace.OpenProjectAsync(projectPath).Result;
                _logger.Debug("Opened " + _project.Name);
            }
        }

        public INamedTypeSymbol[] GetTypes()
        {
            if (_types == null)
            {
                _logger.Debug("Parsing " + _project.Name);

                _types = _project.GetCompilationAsync().Result
                    .GetSymbolsWithName(name => true, SymbolFilter.Type)
                    .OfType<INamedTypeSymbol>()
                    .ToArray();

                _logger.Debug("Complete " + _project.Name);
            }

            return _types;
        }
    }
}
