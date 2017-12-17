using System.IO;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Serialization.CLI.Utils;
using DesperateDevs.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class AutoImport : AbstractPreferencesCommand {

        public override string trigger { get { return "auto-import"; } }
        public override string description { get { return "Find and import all plugins"; } }
        public override string example { get { return "auto-import"; } }

        public AutoImport() : base(typeof(AutoImport).FullName) {
        }

        protected override void run() {
            _logger.Debug(_preferences.ToString());
            autoImport();
            new FixConfig().Run(_rawArgs);
        }

        void autoImport() {
            var config = _preferences.CreateAndConfigure<CodeGeneratorConfig>();

            var plugins = AssemblyResolver
                .GetAssembliesContainingType<ICodeGenerationPlugin>(true, config.searchPaths)
                .Select(assembly => assembly.CodeBase)
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
