using System;
using System.Linq;
using DesperateDevs.CodeGeneration.CodeGenerator;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;
using UnityEngine;

namespace DesperateDevs.CodeGeneration.Unity.Plugins {

    public class DebugLogPostProcessor : IPostProcessor, IDoctor {

        public string name { get { return "Debug.Log generated files"; } }
        public int priority { get { return 200; } }
        public bool runInDryMode { get { return true; } }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            Debug.Log(files.Aggregate(
                string.Empty,
                (acc, file) => acc + file.fileName + " - " + file.generatorName + "\n")
            );

            return files;
        }

        public Diagnosis Diagnose() {
            var isStandalone = AppDomain.CurrentDomain
                .GetAllTypes()
                .Any(type => type.FullName == "DesperateDevs.CodeGeneration.CodeGenerator.CLI.Program");

            if (isStandalone) {
                var typeName = typeof(DebugLogPostProcessor).FullName;
                var preferences = Preferences.sharedInstance;
                var config = preferences.CreateAndConfigure<CodeGeneratorConfig>();
                if (config.postProcessors.Contains(typeName)) {
                    return new Diagnosis(
                        typeName + " uses Unity APIs but is used outside of Unity!",
                        "Remove " + typeName + " from CodeGenerator.DataProviders",
                        DiagnosisSeverity.Error
                    );
                }
            }

            return Diagnosis.Healthy;
        }

        public bool Fix() {
            var preferences = Preferences.sharedInstance;
            var config = preferences.CreateAndConfigure<CodeGeneratorConfig>();
            var postProcessorList = config.postProcessors.ToList();
            var removed = postProcessorList.Remove(typeof(DebugLogPostProcessor).FullName);
            if (removed) {
                config.postProcessors = postProcessorList.ToArray();
                preferences.Save();

                return true;
            }

            return false;
        }
    }
}
