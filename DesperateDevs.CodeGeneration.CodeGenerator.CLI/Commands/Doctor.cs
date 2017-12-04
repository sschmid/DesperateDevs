namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class Doctor : AbstractPreferencesCommand {

        public override string trigger { get { return "doctor"; } }
        public override string description { get { return "Check the config for potential problems"; } }
        public override string example { get { return "jenny doctor"; } }

        public Doctor() : base(typeof(Doctor).Name) {
        }

        protected override void run() {
            new Status().Run(_rawArgs);
            _logger.Debug("Dry Run");
            CodeGeneratorUtil
                .CodeGeneratorFromPreferences(_preferences)
                .DryRun();

            _logger.Info("👨‍🔬  No problems detected. Happy coding :)");
        }
    }
}
