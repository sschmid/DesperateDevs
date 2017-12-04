namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class Format : AbstractPreferencesCommand {

        public override string trigger { get { return "format"; } }
        public override string description { get { return "Format the config files"; } }
        public override string example { get { return "jenny format"; } }

        public Format() : base(typeof(Format).Name) {
        }

        protected override void run() {
            _preferences.Save();
        }
    }
}
