using System.Linq;
using UnityEngine;

namespace DesperateDevs.CodeGeneration.Unity.Plugins {

    public class DebugLogPostProcessor : IPostProcessor {

        public string name { get { return "Debug.Log generated files"; } }
        public bool isEnabledByDefault { get { return true; } }
        public int priority { get { return 200; } }
        public bool runInDryMode { get { return true; } }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            Debug.Log(files.Aggregate(
                string.Empty,
                (acc, file) => acc + file.fileName + " - " + file.generatorName + "\n")
            );

            return files;
        }
    }
}
