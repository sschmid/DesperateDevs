using DesperateDevs.Serialization.CLI.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class GenerateCommand : AbstractPreferencesCommand {

        public override string trigger { get { return "gen"; } }
        public override string description { get { return "Generate files based on properties file"; } }
        public override string example { get { return "gen"; } }

        public GenerateCommand() : base(typeof(GenerateCommand).FullName) {
        }

        protected override void run() {
            var codeGenerator = CodeGeneratorUtil.CodeGeneratorFromPreferences(_preferences);

            codeGenerator.OnProgress += (title, info, progress) => {
                var p = (int)(progress * 100);
                _logger.Debug(string.Format("{0}: {1} ({2}%)", title, info, p));
            };

            codeGenerator.Generate();
        }
    }
}
