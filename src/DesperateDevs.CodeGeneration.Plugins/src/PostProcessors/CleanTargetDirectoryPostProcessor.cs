using System.Collections.Generic;
using System.IO;
using DesperateDevs.Logging;
using DesperateDevs.Serialization;

namespace DesperateDevs.CodeGeneration.Plugins {

    public class CleanTargetDirectoryPostProcessor : IPostProcessor, IConfigurable {

        public string name { get { return "Clean target directory"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return false; } }

        public Dictionary<string, string> defaultProperties { get { return _targetDirectoryConfig.defaultProperties; } }

        readonly Logger _logger = fabl.GetLogger(typeof(CleanTargetDirectoryPostProcessor));

        readonly TargetDirectoryConfig _targetDirectoryConfig = new TargetDirectoryConfig();

        public void Configure(Preferences preferences) {
            _targetDirectoryConfig.Configure(preferences);
        }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            cleanDir();
            return files;
        }

        void cleanDir() {
            if (Directory.Exists(_targetDirectoryConfig.targetDirectory)) {
                var files = new DirectoryInfo(_targetDirectoryConfig.targetDirectory).GetFiles("*.cs", SearchOption.AllDirectories);
                foreach (var file in files) {
                    try {
                        File.Delete(file.FullName);
                    } catch {
                        _logger.Error("Could not delete file " + file);
                    }
                }
            } else {
                Directory.CreateDirectory(_targetDirectoryConfig.targetDirectory);
            }
        }
    }
}