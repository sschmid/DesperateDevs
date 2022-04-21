using System;

namespace DesperateDevs.CodeGeneration.Plugins
{
    public class NewLinePostProcessor : IPostProcessor
    {
        public string Name => "Convert newlines";
        public int Order => 95;
        public bool RunInDryMode => true;

        public CodeGenFile[] PostProcess(CodeGenFile[] files)
        {
            foreach (var file in files)
                file.FileContent = file.FileContent.Replace("\n", Environment.NewLine);

            return files;
        }
    }
}
