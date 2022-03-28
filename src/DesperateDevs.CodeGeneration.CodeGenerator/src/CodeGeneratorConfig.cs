using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Extensions;

namespace DesperateDevs.CodeGeneration.CodeGenerator
{
    public class CodeGeneratorConfig : AbstractConfigurableConfig
    {
        public const string SEARCH_PATHS_KEY = "Jenny.SearchPaths";
        public const string PLUGINS_PATHS_KEY = "Jenny.Plugins";

        public const string PRE_PROCESSORS_KEY = "Jenny.PreProcessors";
        public const string DATA_PROVIDERS_KEY = "Jenny.DataProviders";
        public const string CODE_GENERATORS_KEY = "Jenny.CodeGenerators";
        public const string POST_PROCESSORS_KEY = "Jenny.PostProcessors";

        public const string PORT_KEY = "Jenny.Server.Port";
        public const string HOST_KEY = "Jenny.Client.Host";

        public override Dictionary<string, string> DefaultProperties =>
            new Dictionary<string, string>
            {
                {SEARCH_PATHS_KEY, string.Empty},
                {PLUGINS_PATHS_KEY, string.Empty},
                {PRE_PROCESSORS_KEY, string.Empty},
                {DATA_PROVIDERS_KEY, string.Empty},
                {CODE_GENERATORS_KEY, string.Empty},
                {POST_PROCESSORS_KEY, string.Empty},
                {PORT_KEY, "3333"},
                {HOST_KEY, "localhost"}
            };

        readonly bool _minified;
        readonly bool _removeEmptyEntries;

        public CodeGeneratorConfig() : this(false, true) { }

        public CodeGeneratorConfig(bool minified, bool removeEmptyEntries)
        {
            _minified = minified;
            _removeEmptyEntries = removeEmptyEntries;
        }

        public string[] searchPaths
        {
            get => _preferences[SEARCH_PATHS_KEY].FromCSV(_removeEmptyEntries).ToArray();
            set => _preferences[SEARCH_PATHS_KEY] = value.ToCSV(_minified, _removeEmptyEntries);
        }

        public string[] plugins
        {
            get => _preferences[PLUGINS_PATHS_KEY].FromCSV(_removeEmptyEntries).ToArray();
            set => _preferences[PLUGINS_PATHS_KEY] = value.ToCSV(_minified, _removeEmptyEntries);
        }

        public string[] preProcessors
        {
            get => _preferences[PRE_PROCESSORS_KEY].FromCSV(_removeEmptyEntries).ToArray();
            set => _preferences[PRE_PROCESSORS_KEY] = value.ToCSV(_minified, _removeEmptyEntries);
        }

        public string[] dataProviders
        {
            get => _preferences[DATA_PROVIDERS_KEY].FromCSV(_removeEmptyEntries).ToArray();
            set => _preferences[DATA_PROVIDERS_KEY] = value.ToCSV(_minified, _removeEmptyEntries);
        }

        public string[] codeGenerators
        {
            get => _preferences[CODE_GENERATORS_KEY].FromCSV(_removeEmptyEntries).ToArray();
            set => _preferences[CODE_GENERATORS_KEY] = value.ToCSV(_minified, _removeEmptyEntries);
        }

        public string[] postProcessors
        {
            get => _preferences[POST_PROCESSORS_KEY].FromCSV(_removeEmptyEntries).ToArray();
            set => _preferences[POST_PROCESSORS_KEY] = value.ToCSV(_minified, _removeEmptyEntries);
        }

        public int port
        {
            get => int.Parse(_preferences[PORT_KEY]);
            set => _preferences[PORT_KEY] = value.ToString();
        }

        public string host
        {
            get => _preferences[HOST_KEY];
            set => _preferences[HOST_KEY] = value;
        }
    }
}
