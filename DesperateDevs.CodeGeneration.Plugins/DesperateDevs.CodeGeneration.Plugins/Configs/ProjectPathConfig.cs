using System.Collections.Generic;
using DesperateDevs.Serialization;

namespace DesperateDevs.CodeGeneration.Plugins {

    public class ProjectPathConfig : AbstractConfigurableConfig {

        const string PROJECT_PATH_KEY = "DesperateDevs.CodeGeneration.Plugins.ProjectPath";

        public override Dictionary<string, string> defaultProperties {
            get {
                return new Dictionary<string, string> {
                    { PROJECT_PATH_KEY, "Assembly-CSharp.csproj" }
                };
            }
        }

        public string projectPath { get { return _preferences[PROJECT_PATH_KEY]; } }
    }
}
