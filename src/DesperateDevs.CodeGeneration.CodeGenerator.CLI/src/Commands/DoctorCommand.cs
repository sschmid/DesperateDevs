using System;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Serialization.CLI.Utils;
using DesperateDevs.Extensions;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI
{
    public class DoctorCommand : AbstractPreferencesCommand
    {
        public override string trigger => "doctor";
        public override string description => "Check the config for potential problems";
        public override string group => CommandGroups.PLUGINS;
        public override string example => "doctor";

        public DoctorCommand() : base(typeof(DoctorCommand).FullName)
        {
        }

        protected override void run()
        {
            new StatusCommand().Run(_program, _rawArgs);

            diagnose();

            _logger.Info("Dry Run");
            CodeGeneratorUtil
                .CodeGeneratorFromPreferences(_preferences)
                .DryRun();

            _logger.Info("👨‍🔬  No problems detected. Happy coding :)");
        }

        void diagnose()
        {
            var doctors = AppDomain.CurrentDomain.GetInstancesOf<IDoctor>();
            foreach (var doctor in doctors.OfType<IConfigurable>())
            {
                doctor.Configure(_preferences);
            }

            var diagnoses = doctors
                .Select(doctor => doctor.Diagnose())
                .ToArray();

            foreach (var diagnosis in diagnoses.Where(d => d.severity == DiagnosisSeverity.Hint))
            {
                _logger.Info("👨‍⚕️  Symptoms: " + diagnosis.symptoms);
                _logger.Info("💊  Treatment: " + diagnosis.treatment);
            }

            foreach (var diagnosis in diagnoses.Where(d => d.severity == DiagnosisSeverity.Warning))
            {
                _logger.Warn("👨‍⚕️  Symptoms: " + diagnosis.symptoms);
                _logger.Warn("💊  Treatment: " + diagnosis.treatment);
            }

            var errors = string.Join("\n", diagnoses
                .Where(d => d.severity == DiagnosisSeverity.Error)
                .Select(d => "👨‍⚕️  Symptoms: " + d.symptoms + "\n💊  Treatment: " + d.treatment)
                .ToArray());

            if (!string.IsNullOrEmpty(errors))
            {
                errors += "\nUse 'jenny fix' to apply treatments";
                throw new Exception(errors);
            }
        }
    }
}
