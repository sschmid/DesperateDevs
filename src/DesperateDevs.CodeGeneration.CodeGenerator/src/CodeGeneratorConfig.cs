using System.Collections.Generic;
using DesperateDevs.Serialization;
using DesperateDevs;
using DesperateDevs.Extensions;

namespace DesperateDevs.CodeGeneration.CodeGenerator {

    public class CodeGeneratorConfig : AbstractConfigurableConfig {

        public const string SEARCH_PATHS_KEY = "Jenny.SearchPaths";
        public const string PLUGINS_PATHS_KEY = "Jenny.Plugins";

        public const string PRE_PROCESSORS_KEY = "Jenny.PreProcessors";
        public const string DATA_PROVIDERS_KEY = "Jenny.DataProviders";
        public const string CODE_GENERATORS_KEY = "Jenny.CodeGenerators";
        public const string POST_PROCESSORS_KEY = "Jenny.PostProcessors";

        public const string PORT_KEY = "Jenny.Server.Port";
        public const string HOST_KEY = "Jenny.Client.Host";

        public override Dictionary<string, string> defaultProperties {
            get {
                return new Dictionary<string, string> {
                    { SEARCH_PATHS_KEY, string.Empty },
                    { PLUGINS_PATHS_KEY, string.Empty },
                    { PRE_PROCESSORS_KEY, string.Empty },
                    { DATA_PROVIDERS_KEY, string.Empty },
                    { CODE_GENERATORS_KEY, string.Empty },
                    { POST_PROCESSORS_KEY, string.Empty },
                    { PORT_KEY, "3333" },
                    { HOST_KEY , "localhost" }
                };
            }
        }

        public string[] searchPaths {
            get { return _preferences[SEARCH_PATHS_KEY].ArrayFromCSV(); }
            set { _preferences[SEARCH_PATHS_KEY] = value.ToCSV(); }
        }

        public string[] plugins {
            get { return _preferences[PLUGINS_PATHS_KEY].ArrayFromCSV(); }
            set { _preferences[PLUGINS_PATHS_KEY] = value.ToCSV(); }
        }

        public string[] preProcessors {
            get { return _preferences[PRE_PROCESSORS_KEY].ArrayFromCSV(); }
            set { _preferences[PRE_PROCESSORS_KEY] = value.ToCSV(); }
        }

        public string[] dataProviders {
            get { return _preferences[DATA_PROVIDERS_KEY].ArrayFromCSV(); }
            set { _preferences[DATA_PROVIDERS_KEY] = value.ToCSV(); }
        }

        public string[] codeGenerators {
            get { return _preferences[CODE_GENERATORS_KEY].ArrayFromCSV(); }
            set { _preferences[CODE_GENERATORS_KEY] = value.ToCSV(); }
        }

        public string[] postProcessors {
            get { return _preferences[POST_PROCESSORS_KEY].ArrayFromCSV(); }
            set { _preferences[POST_PROCESSORS_KEY] = value.ToCSV(); }
        }

        public int port {
            get { return int.Parse(_preferences[PORT_KEY]); }
            set { _preferences[PORT_KEY] = value.ToString(); }
        }

        public string host {
            get { return _preferences[HOST_KEY]; }
            set { _preferences[HOST_KEY] = value; }
        }
    }
}
