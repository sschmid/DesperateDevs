using System;
using System.Collections.Generic;
using System.IO;
using DesperateDevs.Serialization;

namespace DesperateDevs.CodeGeneration.Plugins {

    public class ValidateProjectPathPreProcessor : IPreProcessor, IConfigurable {

        public string name { get { return "Validate Project Path"; } }
        public int priority { get { return -10; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return _projectPathConfig.defaultProperties; } }

        readonly ProjectPathConfig _projectPathConfig = new ProjectPathConfig();

        Preferences _preferences;

        public void Configure(Preferences preferences) {
            _preferences = preferences;
            _projectPathConfig.Configure(preferences);
        }

        public void PreProcess() {
            if (!File.Exists(_projectPathConfig.projectPath)) {
                throw new Exception(
                    @"Could not find file '" + _projectPathConfig.projectPath + "\'\n" +
                    "Press \"Assets -> Open C# Project\" to create the project and make sure that \"Project Path\" is set to the created *.csproj."
                );
            }
        }
    }
}
