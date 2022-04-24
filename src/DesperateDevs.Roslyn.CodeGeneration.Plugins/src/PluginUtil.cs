﻿using System.Collections.Generic;

namespace DesperateDevs.Roslyn.CodeGeneration.Plugins
{
    public static class PluginUtil
    {
        public static readonly string ProjectParserKey = $"{typeof(PluginUtil).Namespace}.ProjectParser";

        public static ProjectParser GetCachedProjectParser(Dictionary<string, object> objectCache, string projectPath)
        {
            if (!objectCache.TryGetValue(ProjectParserKey, out var cachedProjectParser))
            {
                cachedProjectParser = new ProjectParser(projectPath);
                objectCache.Add(ProjectParserKey, cachedProjectParser);
            }

            return (ProjectParser)cachedProjectParser;
        }
    }
}
