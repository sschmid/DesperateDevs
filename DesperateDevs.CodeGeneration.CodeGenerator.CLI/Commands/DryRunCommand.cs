using DesperateDevs.Serialization.CLI.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI
{
    public class DryRunCommand : AbstractPreferencesCommand
    {
        public override string trigger => "dry";
        public override string description => "Run the code generator in dry mode";
        public override string group => CommandGroups.CODE_GENERATION;
        public override string example => "dry";

        public DryRunCommand() : base(typeof(DryRunCommand).FullName)
        {
        }

        protected override void run()
        {
            var codeGenerator = CodeGeneratorUtil.CodeGeneratorFromPreferences(_preferences);

            codeGenerator.OnProgress += (title, info, progress) =>
            {
                var p = (int)(progress * 100);
                _logger.Debug($"{title}: {info} ({p}%)");
            };
            codeGenerator.DryRun();
        }
    }
}
