using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs.CodeGeneration.CodeGenerator;
using DesperateDevs.Serialization;
using DesperateDevs;

namespace DesperateDevs.CodeGeneration.Unity.Plugins {

    public class WarnIfCompilationErrorsDoctor : IDoctor, IConfigurable {

        public string name { get { return "Warn If Compilation Errors"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return new Dictionary<string, string>(); } }

        readonly CodeGeneratorConfig _codeGeneratorConfig = new CodeGeneratorConfig();

        public void Configure(Preferences preferences) {
            _codeGeneratorConfig.Configure(preferences);
        }

        public Diagnosis Diagnose() {
            var isStandalone = AppDomain.CurrentDomain
                .GetAllTypes()
                .Any(type => type.FullName == "DesperateDevs.CodeGeneration.CodeGenerator.CLI.Program");

            if (isStandalone) {
                var typeName = typeof(WarnIfCompilationErrorsPreProcessor).FullName;
                if (_codeGeneratorConfig.preProcessors.Contains(typeName)) {
                    return new Diagnosis(
                        typeName + " uses Unity APIs but is used outside of Unity!",
                        "Remove " + typeName + " from CodeGenerator.PreProcessors",
                        DiagnosisSeverity.Error
                    );
                }
            }

            return Diagnosis.Healthy;
        }

        public bool Fix() {
            var preProcessorList = _codeGeneratorConfig.preProcessors.ToList();
            var removed = preProcessorList.Remove(typeof(WarnIfCompilationErrorsPreProcessor).FullName);
            if (removed) {
                _codeGeneratorConfig.preProcessors = preProcessorList.ToArray();

                return true;
            }

            return false;
        }
    }
}
