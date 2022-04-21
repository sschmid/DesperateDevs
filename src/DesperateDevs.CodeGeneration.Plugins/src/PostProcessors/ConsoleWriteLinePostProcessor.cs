using System;
using System.Linq;

namespace DesperateDevs.CodeGeneration.Plugins {

    public class ConsoleWriteLinePostProcessor : IPostProcessor {

        public string Name { get { return "Console.WriteLine generated files"; } }
        public int Order { get { return 200; } }
        public bool RunInDryMode { get { return true; } }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            Console.WriteLine(files.Aggregate(
                string.Empty,
                (acc, file) => acc + file.FileName + " - " + file.GeneratorName + "\n")
            );

            return files;
        }
    }
}
