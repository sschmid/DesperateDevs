using DesperateDevs.Serialization;
using DesperateDevs.Serialization.CLI.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI
{
    public class AutoImportCommand : AbstractPreferencesCommand
    {
        public override string trigger => "auto-import";
        public override string description => "Find and import all plugins";
        public override string group => CommandGroups.PLUGINS;
        public override string example => "auto-import";

        public AutoImportCommand() : base(typeof(AutoImportCommand).FullName)
        {
        }

        protected override void run()
        {
            _logger.Debug(_preferences.ToString());
            autoImport();
            new FixCommand().Run(_program, _rawArgs);
        }

        void autoImport()
        {
            var config = _preferences.CreateAndConfigure<CodeGeneratorConfig>();
            var searchPaths = CodeGeneratorUtil.BuildSearchPaths(config.searchPaths, new[] { "." });
            CodeGeneratorUtil.AutoImport(config, searchPaths);
            _preferences.Save();
        }
    }
}
