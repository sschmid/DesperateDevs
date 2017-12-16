using System;
using System.IO;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Serialization.CLI.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class AutoImport : AbstractPreferencesCommand {

        public override string trigger { get { return "auto-import"; } }
        public override string description { get { return "Find and import all plugins"; } }
        public override string example { get { return "auto-import"; } }

        public AutoImport() : base(typeof(AutoImport).Name) {
        }

        protected override void run() {
            _logger.Debug(_preferences.ToString());
            autoImport();
            new FixConfig().Run(_rawArgs);
        }

        void autoImport() {
            var config = _preferences.CreateAndConfigure<CodeGeneratorConfig>();

            var plugins = config.searchPaths
                .SelectMany(path => Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories))
                .Where(path => path.EndsWith(".plugins.dll", StringComparison.OrdinalIgnoreCase))
                .ToArray();

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
