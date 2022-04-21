using System.Linq;
using UnityEngine;

namespace DesperateDevs.CodeGeneration.Unity.Plugins
{
    public class DebugLogPostProcessor : IPostProcessor
    {
        public string Name
        {
            get { return "Debug.Log generated files"; }
        }

        public int Order
        {
            get { return 200; }
        }

        public bool RunInDryMode
        {
            get { return true; }
        }

        public CodeGenFile[] PostProcess(CodeGenFile[] files)
        {
            Debug.Log(files.Aggregate(
                string.Empty,
                (acc, file) => acc + file.FileName + " - " + file.GeneratorName + "\n")
            );

            return files;
        }
    }
}
