using DesperateDevs.Serialization;
using DesperateDevs.Serialization.CLI.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class AutoImportCommand : AbstractPreferencesCommand {

        public override string trigger { get { return "auto-import"; } }
        public override string description { get { return "Find and import all plugins"; } }
        public override string group { get { return "Plugins"; } }
        public override string example { get { return "auto-import"; } }

        public AutoImportCommand() : base(typeof(AutoImportCommand).FullName) {
        }

        protected override void run() {
            _logger.Debug(_preferences.ToString());
            autoImport();
            new FixCommand().Run(_rawArgs);
        }

        void autoImport() {
            var config = _preferences.CreateAndConfigure<CodeGeneratorConfig>();
            var searchPaths = CodeGeneratorUtil.BuildSearchPaths(config.searchPaths, new[] { "." });
            CodeGeneratorUtil.AutoImport(config, searchPaths);
            _preferences.Save();
        }
    }
}
