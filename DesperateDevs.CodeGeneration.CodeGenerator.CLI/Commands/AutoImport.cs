using DesperateDevs.Serialization;
using DesperateDevs.Serialization.CLI.Utils;

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
            new FixPlugins().Run(_rawArgs);
        }

        void autoImport() {
            var config = _preferences.CreateAndConfigure<CodeGeneratorConfig>();
            CodeGeneratorUtil.AutoImport(config, config.searchPaths);
            _preferences.Save();
        }
    }
}
