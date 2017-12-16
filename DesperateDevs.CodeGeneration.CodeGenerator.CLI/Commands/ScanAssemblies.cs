using System.Linq;
using DesperateDevs.Serialization.CLI;
using DesperateDevs.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class ScanAssemblies : AbstractPreferencesCommand {

        public override string trigger { get { return "scan"; } }
        public override string description { get { return "Scan and print available types found in specified assemblies"; } }
        public override string example { get { return "scan"; } }

        public ScanAssemblies() : base(typeof(ScanAssemblies).Name) {
        }

        protected override void run() {
            var types = CodeGeneratorUtil.LoadTypesFromPlugins(_preferences);
            var orderedTypes = types
                .Where(type => type.ImplementsInterface<ICodeGeneratorBase>())
                .OrderBy(type => type.Assembly.GetName().Name)
                .ThenBy(type => type.FullName);

            foreach (var type in orderedTypes) {
                _logger.Info(type.Assembly.GetName().Name + ": " + type);
            }
        }
    }
}
