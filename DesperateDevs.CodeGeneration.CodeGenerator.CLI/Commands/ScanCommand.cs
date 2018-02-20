using System.Linq;
using DesperateDevs.Serialization.CLI.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class ScanCommand : AbstractPreferencesCommand {

        public override string trigger { get { return "scan"; } }
        public override string description { get { return "Scan and print available types found in specified assemblies"; } }
        public override string example { get { return "scan"; } }

        public ScanCommand() : base(typeof(ScanCommand).FullName) {
        }

        protected override void run() {
            var instances = CodeGeneratorUtil.LoadFromPlugins(_preferences);

            var orderedTypes = instances
                .Select(instance => instance.GetType())
                .OrderBy(type => type.Assembly.GetName().Name)
                .ThenBy(type => type.FullName);

            foreach (var type in orderedTypes) {
                _logger.Info(type.Assembly.GetName().Name + ": " + type);
            }
        }
    }
}
