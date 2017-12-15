using System;
using System.Linq;
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
                return new Diagnosis(
                    typeName + "uses Unity APIs but is used outside of Unity!",
                    "Please remove " + typeName + " when using the standalone code generator.",
                    DiagnosisSeverity.Error
                );
            }

            return Diagnosis.Healthy;
        }
    }
}
