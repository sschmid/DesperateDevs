namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class DryRun : AbstractPreferencesCommand {

        public override string trigger { get { return "dry"; } }
        public override string description { get { return "Run the code generator in dry mode"; } }
        public override string example { get { return "jenny dry"; } }

        public DryRun() :base(typeof(DryRun).Name) {
        }

        protected override void run() {
            var codeGenerator = CodeGeneratorUtil.CodeGeneratorFromPreferences(_preferences);

            codeGenerator.OnProgress += (title, info, progress) => {
                var p = (int)(progress * 100);
                _logger.Debug(string.Format("{0}: {1} ({2}%)", title, info, p));
            };
            codeGenerator.DryRun();
        }
    }
}
