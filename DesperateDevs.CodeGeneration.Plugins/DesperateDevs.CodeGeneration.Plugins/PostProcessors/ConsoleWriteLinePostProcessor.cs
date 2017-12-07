using System;
using System.Linq;

namespace DesperateDevs.CodeGeneration.Plugins {

    public class ConsoleWriteLinePostProcessor : IPostProcessor {

        public string name { get { return "Console.WriteLine generated files"; } }
        public int priority { get { return 200; } }
        public bool runInDryMode { get { return true; } }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            Console.WriteLine(files.Aggregate(
                string.Empty,
                (acc, file) => acc + file.fileName + " - " + file.generatorName + "\n")
            );

            return files;
        }
    }
}
