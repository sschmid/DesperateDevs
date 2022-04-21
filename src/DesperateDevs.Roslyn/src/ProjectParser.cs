using System.Linq;
using DesperateDevs.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace DesperateDevs.Roslyn
{
    public class ProjectParser
    {
        static readonly Logger _logger = Sherlog.GetLogger(typeof(ProjectParser).Name);

        readonly Project _project;
        INamedTypeSymbol[] _types;

        public ProjectParser(string projectPath)
        {
            _logger.Debug("Opening " + projectPath);

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
