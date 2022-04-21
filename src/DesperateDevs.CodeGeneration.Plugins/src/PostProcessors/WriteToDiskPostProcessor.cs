using System.Collections.Generic;
using System.IO;
using DesperateDevs.Serialization;

namespace DesperateDevs.CodeGeneration.Plugins
{
    public class WriteToDiskPostProcessor : IPostProcessor, IConfigurable
    {
        public string Name
        {
            get { return "Write to disk"; }
        }

        public int Order
        {
            get { return 100; }
        }

        public bool RunInDryMode
        {
            get { return false; }
        }

        public Dictionary<string, string> DefaultProperties
        {
            get { return _targetDirectoryConfig.DefaultProperties; }
        }

        readonly TargetDirectoryConfig _targetDirectoryConfig = new TargetDirectoryConfig();

        public void Configure(Preferences preferences)
        {
            _targetDirectoryConfig.Configure(preferences);
        }

        public CodeGenFile[] PostProcess(CodeGenFile[] files)
        {
            foreach (var file in files)
            {
                var fileName = _targetDirectoryConfig.targetDirectory + Path.DirectorySeparatorChar + file.FileName;
                var targetDir = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                File.WriteAllText(fileName, file.FileContent);
            }

            return files;
        }
    }
}
