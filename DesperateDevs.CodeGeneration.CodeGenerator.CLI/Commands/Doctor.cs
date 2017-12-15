using System;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class Doctor : AbstractPreferencesCommand {

        public override string trigger { get { return "doctor"; } }
        public override string description { get { return "Check the config for potential problems"; } }
        public override string example { get { return "jenny doctor"; } }

        public Doctor() : base(typeof(Doctor).Name) {
        }

        protected override void run() {
            new Status().Run(_rawArgs);

            diagnose();

            _logger.Debug("Dry Run");
            CodeGeneratorUtil
                .CodeGeneratorFromPreferences(_preferences)
                .DryRun();

            _logger.Info("👨‍🔬  No problems detected. Happy coding :)");
        }

        void diagnose() {
            var doctors = AppDomain.CurrentDomain.GetInstancesOf<IDoctor>();
            foreach (var doctor in doctors.OfType<IConfigurable>()) {
                doctor.Configure(_preferences);
            }

            var diagnoses = doctors
                .Select(doctor => doctor.Diagnose())
                .ToArray();

            foreach (var diagnosis in diagnoses.Where(d => d.severity == DiagnosisSeverity.Hint)) {
                _logger.Info("👨‍⚕️  Symptoms: " + diagnosis.symptoms);
                _logger.Info("💊  Treatment: " + diagnosis.treatment);
            }

            foreach (var diagnosis in diagnoses.Where(d => d.severity == DiagnosisSeverity.Warning)) {
                _logger.Warn("👨‍⚕️  Symptoms: " + diagnosis.symptoms);
                _logger.Warn("💊  Treatment: " + diagnosis.treatment);
            }

            var errors = string.Join("\n", diagnoses
                .Where(d => d.severity == DiagnosisSeverity.Error)
                .Select(d => "👨‍⚕️  Symptoms: " + d.symptoms + "\n💊  Treatment: " + d.treatment)
                .ToArray());

            if (!string.IsNullOrEmpty(errors)) {
                throw new Exception(errors);
            }
        }
    }
}
