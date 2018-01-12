using System.Collections.Generic;

namespace DesperateDevs.Roslyn.CodeGeneration.Plugins {

    public static class PluginUtil {

        public const string PROJECT_PARSER_KEY = "DesperateDevs.Roslyn.CodeGeneration.Plugins.ProjectParser";

        public static ProjectParser GetCachedProjectParser(Dictionary<string, object> objectCache, string projectPath) {
            object cachedProjectParser;
            if (!objectCache.TryGetValue(PROJECT_PARSER_KEY, out cachedProjectParser)) {
                cachedProjectParser = new ProjectParser(projectPath);
                objectCache.Add(PROJECT_PARSER_KEY, cachedProjectParser);
            }

            return (ProjectParser)cachedProjectParser;
        }
    }
}
