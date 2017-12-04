﻿using System.IO;
using System.Linq;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class AutoImport : AbstractPreferencesCommand{

        public override string trigger { get { return "auto-import"; } }
        public override string description { get { return "Find and import all plugins"; } }
        public override string example { get { return "jenny auto-import"; } }

        public AutoImport() : base(typeof(AutoImport).Name) {
        }

        protected override void run() {
            _logger.Debug(_preferences.ToString());
            autoImport();
            new FixConfig().Run(_rawArgs);
        }

        void autoImport() {
            var config = _preferences.CreateCodeGeneratorConfig();

            var plugins = config.searchPaths
                .SelectMany(path => Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories))
                .Where(path => path.ToLower().EndsWith(".plugins.dll"));

            config.searchPaths = config.searchPaths
                .Concat(plugins.Select(Path.GetDirectoryName))
                .Distinct()
                .ToArray();

            config.plugins = plugins
                .Select(Path.GetFileNameWithoutExtension)
                .Distinct()
                .ToArray();

            _preferences.Save();
        }
    }
}
