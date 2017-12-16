using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Unity.Editor;
using DesperateDevs.Utils;
using UnityEditor;
using UnityEngine;

namespace DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor {

    public class CodeGeneratorPreferencesDrawer : AbstractPreferencesDrawer {

        public override int priority { get { return 10; } }
        public override string title { get { return "Jenny"; } }

        string[] _availablePreProcessorTypes;
        string[] _availableDataProviderTypes;
        string[] _availableGeneratorTypes;
        string[] _availablePostProcessorTypes;

        string[] _availablePreProcessorNames;
        string[] _availableDataProviderNames;
        string[] _availableGeneratorNames;
        string[] _availablePostProcessorNames;

        Preferences _preferences;
        Type[] _types;

        CodeGeneratorConfig _codeGeneratorConfig;

        public override void Initialize(Preferences preferences) {
            _preferences = preferences;
            _codeGeneratorConfig = preferences.CreateAndConfigure<CodeGeneratorConfig>();
            preferences.properties.AddProperties(_codeGeneratorConfig.defaultProperties, false);

            _types = CodeGeneratorUtil.LoadTypesFromPlugins(preferences);

            setTypesAndNames<IPreProcessor>(_types, out _availablePreProcessorTypes, out _availablePreProcessorNames);
            setTypesAndNames<IDataProvider>(_types, out _availableDataProviderTypes, out _availableDataProviderNames);
            setTypesAndNames<ICodeGenerator>(_types, out _availableGeneratorTypes, out _availableGeneratorNames);
            setTypesAndNames<IPostProcessor>(_types, out _availablePostProcessorTypes, out _availablePostProcessorNames);

            _preferences.properties.AddProperties(CodeGeneratorUtil.GetDefaultProperties(_types, _codeGeneratorConfig), false);
        }

        public override void DrawHeader(Preferences preferences) {
        }

        protected override void drawContent(Preferences preferences) {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Auto Import Plugins");
                if (EditorLayout.MiniButton("Auto Import")) {
                    autoImport();
                }
            }
            EditorGUILayout.EndHorizontal();

            _codeGeneratorConfig.preProcessors = drawMaskField("Pre Processors", _availablePreProcessorTypes, _availablePreProcessorNames, _codeGeneratorConfig.preProcessors);
            _codeGeneratorConfig.dataProviders = drawMaskField("Data Providers", _availableDataProviderTypes, _availableDataProviderNames, _codeGeneratorConfig.dataProviders);
            _codeGeneratorConfig.codeGenerators = drawMaskField("Code Generators", _availableGeneratorTypes, _availableGeneratorNames, _codeGeneratorConfig.codeGenerators);
            _codeGeneratorConfig.postProcessors = drawMaskField("Post Processors", _availablePostProcessorTypes, _availablePostProcessorNames, _codeGeneratorConfig.postProcessors);

            EditorGUILayout.Space();
            drawConfigurables();

            drawGenerateButtons();
        }

        void autoImport() {
            var propertiesPath = Path.GetFileName(_preferences.propertiesPath);
            if (EditorUtility.DisplayDialog("Jenny - Auto Import",
                "Auto Import will automatically find and set all plugins for you. " +
                "It will search in folders and sub folders specified in " + propertiesPath +
                " under the key 'CodeGenerator.SearchPaths'." +
                "\n\nThis will overwrite your current plugin preferences." +
                "\n\nDo you want to continue?",
                "Continue and Overwrite",
                "Cancel"
            )) {
                var plugins = _codeGeneratorConfig.searchPaths
                    .Concat(new[] { "Assets" })
                    .SelectMany(path => Directory.Exists(path)
                        ? Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories)
                        : new string[0])
                    .Where(path => path.EndsWith(".plugins.dll", StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                _codeGeneratorConfig.searchPaths = _codeGeneratorConfig.searchPaths
                    .Concat(plugins.Select(Path.GetDirectoryName))
                    .Distinct()
                    .ToArray();

                _codeGeneratorConfig.plugins = plugins
                    .Select(Path.GetFileNameWithoutExtension)
                    .Distinct()
                    .ToArray();

                _preferences.Save();

                Initialize(_preferences);
                _codeGeneratorConfig.preProcessors = _availablePreProcessorTypes;
                _codeGeneratorConfig.dataProviders = _availableDataProviderTypes;
                _codeGeneratorConfig.codeGenerators = _availableGeneratorTypes;
                _codeGeneratorConfig.postProcessors = _availablePostProcessorTypes;
            }
        }

        void drawConfigurables() {
            var defaultProperties = CodeGeneratorUtil.GetDefaultProperties(_types, _codeGeneratorConfig);
            _preferences.properties.AddProperties(defaultProperties, false);

            foreach (var kv in defaultProperties.OrderBy(kv => kv.Key)) {
                _preferences[kv.Key] = EditorGUILayout.TextField(kv.Key.ShortTypeName().ToSpacedCamelCase(), _preferences[kv.Key]);
            }
        }

        static void setTypesAndNames<T>(Type[] types, out string[] availableTypes, out string[] availableNames) where T : ICodeGeneratorBase {
            IEnumerable<T> instances = CodeGeneratorUtil.GetOrderedInstancesOf<T>(types);

            availableTypes = instances
                .Select(instance => instance.GetType().ToCompilableString())
                .ToArray();

            availableNames = instances
                .Select(instance => instance.name)
                .ToArray();
        }

        static string[] drawMaskField(string title, string[] types, string[] names, string[] input) {
            var mask = 0;

            for (int i = 0; i < types.Length; i++) {
                if (input.Contains(types[i])) {
                    mask += (1 << i);
                }
            }

            mask = EditorGUILayout.MaskField(title, mask, names);

            var selected = new List<string>();
            for (int i = 0; i < types.Length; i++) {
                var index = 1 << i;
                if ((index & mask) == index) {
                    selected.Add(types[i]);
                }
            }

            // Re-add unavailable types
            selected.AddRange(input.Where(type => !types.Contains(type)));

            return selected.ToArray();
        }

        void drawGenerateButtons() {
            EditorGUILayout.BeginHorizontal();
            {
                var bgColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Generate", GUILayout.Height(32))) {
                    UnityCodeGenerator.Generate();
                }

                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button("External Generate", GUILayout.Width(128), GUILayout.Height(32))) {
                    UnityCodeGenerator.GenerateExternal();
                }

                GUI.backgroundColor = bgColor;

            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
