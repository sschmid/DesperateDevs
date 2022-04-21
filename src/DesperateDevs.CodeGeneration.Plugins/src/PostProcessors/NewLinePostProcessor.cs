using System;

namespace DesperateDevs.CodeGeneration.Plugins {

    public class NewLinePostProcessor : IPostProcessor {

        public string Name { get { return "Convert newlines"; } }
        public int Order { get { return 95; } }
        public bool RunInDryMode { get { return true; } }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            foreach (var file in files) {
                file.FileContent = file.FileContent.Replace("\n", Environment.NewLine);
            }

            return files;
        }
    }
}
