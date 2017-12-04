namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class Generate : AbstractPreferencesCommand {

        public override string trigger { get { return "gen"; } }
        public override string description { get { return "Generate files based on properties file"; } }
        public override string example { get { return "jenny gen"; } }

        public Generate() : base(typeof(Generate).Name) {
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
