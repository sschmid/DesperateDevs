using System;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Serialization.Cli.Utils;
using DesperateDevs.Extensions;

namespace DesperateDevs.CodeGeneration.CodeGenerator.Cli
{
    public class DoctorCommand : AbstractPreferencesCommand
    {
        public override string Trigger => "doctor";
        public override string Description => "Check the config for potential problems";
        public override string Group => CommandGroups.PLUGINS;
        public override string Example => "doctor";

        public DoctorCommand() : base(typeof(DoctorCommand).FullName)
        {
        }

        protected override void Run()
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

            foreach (var diagnosis in diagnoses.Where(d => d.Severity == DiagnosisSeverity.Hint))
            {
                _logger.Info("👨‍⚕️  Symptoms: " + diagnosis.Symptoms);
                _logger.Info("💊  Treatment: " + diagnosis.Treatment);
            }

            foreach (var diagnosis in diagnoses.Where(d => d.Severity == DiagnosisSeverity.Warning))
            {
                _logger.Warn("👨‍⚕️  Symptoms: " + diagnosis.Symptoms);
                _logger.Warn("💊  Treatment: " + diagnosis.Treatment);
            }

            var errors = string.Join("\n", diagnoses
                .Where(d => d.Severity == DiagnosisSeverity.Error)
                .Select(d => "👨‍⚕️  Symptoms: " + d.Symptoms + "\n💊  Treatment: " + d.Treatment)
                .ToArray());

            if (!string.IsNullOrEmpty(errors))
            {
                errors += "\nUse 'jenny fix' to apply treatments";
                throw new Exception(errors);
            }
        }
    }
}
