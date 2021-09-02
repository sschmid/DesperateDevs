using System;
using System.Diagnostics;
using DesperateDevs.Serialization.CLI.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI
{
    public class GenerateCommand : AbstractPreferencesCommand
    {
        public override string trigger => "gen";
        public override string description => "Generate files based on properties file";
        public override string group => CommandGroups.CODE_GENERATION;
        public override string example => "gen";

        public GenerateCommand() : base(typeof(GenerateCommand).FullName)
        {
        }

        protected override void run()
        {
            _logger.Info("Generating using " + _preferences.propertiesPath);

            var watch = new Stopwatch();
            watch.Start();

            var codeGenerator = CodeGeneratorUtil.CodeGeneratorFromPreferences(_preferences);

            codeGenerator.OnProgress += (title, info, progress) =>
            {
                var p = (int)(progress * 100);
                _logger.Debug($"{title}: {info} ({p}%)");
            };

            var files = codeGenerator.Generate();

            watch.Stop();

            _logger.Info("[" + DateTime.Now.ToLongTimeString() + "]" + " Generated " + files.Length + " files in " + (watch.ElapsedMilliseconds / 1000f).ToString("0.0") + " seconds");
        }
    }
}
