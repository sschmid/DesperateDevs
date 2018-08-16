﻿using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs.CodeGeneration.CodeGenerator;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace DesperateDevs.CodeGeneration.Unity.Plugins {

    public class DebugLogDoctor : IDoctor, IConfigurable {

        public string name { get { return "Debug.Log"; } }
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
                var typeName = typeof(DebugLogPostProcessor).FullName;
                if (_codeGeneratorConfig.postProcessors.Contains(typeName)) {
                    return new Diagnosis(
                        typeName + " uses Unity APIs but is used outside of Unity!",
                        "Remove " + typeName + " from CodeGenerator.PostProcessors",
                        DiagnosisSeverity.Error
                    );
                }
            }

            return Diagnosis.Healthy;
        }

        public bool Fix() {
            var postProcessorList = _codeGeneratorConfig.postProcessors.ToList();
            var removed = postProcessorList.Remove(typeof(DebugLogPostProcessor).FullName);
            if (removed) {
                _codeGeneratorConfig.postProcessors = postProcessorList.ToArray();

                return true;
            }

            return false;
        }
    }
}