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
        ICodeGenerationPlugin[] _instances;

        CodeGeneratorConfig _codeGeneratorConfig;

        public override void Initialize(Preferences preferences) {
            _preferences = preferences;
            _codeGeneratorConfig = preferences.CreateAndConfigure<CodeGeneratorConfig>();
            preferences.properties.AddProperties(_codeGeneratorConfig.defaultProperties, false);

            _instances = CodeGeneratorUtil.LoadFromPlugins(preferences);

            setTypesAndNames<IPreProcessor>(_instances, out _availablePreProcessorTypes, out _availablePreProcessorNames);
            setTypesAndNames<IDataProvider>(_instances, out _availableDataProviderTypes, out _availableDataProviderNames);
            setTypesAndNames<ICodeGenerator>(_instances, out _availableGeneratorTypes, out _availableGeneratorNames);
            setTypesAndNames<IPostProcessor>(_instances, out _availablePostProcessorTypes, out _availablePostProcessorNames);

            _preferences.properties.AddProperties(CodeGeneratorUtil.GetDefaultProperties(_instances, _codeGeneratorConfig), false);
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
                " under the key 'Jenny.SearchPaths'." +
                "\n\nThis will overwrite your current plugin settings." +
                "\n\nDo you want to continue?",
                "Continue and Overwrite",
                "Cancel"
            )) {

                var searchPaths = _codeGeneratorConfig
                    .searchPaths
                    .Concat(new[] { "./Assets", "./Library/ScriptAssemblies" })
                    .Where(Directory.Exists)
                    .ToArray();

                CodeGeneratorUtil.AutoImport(_codeGeneratorConfig, searchPaths);
                _preferences.Save();

                Initialize(_preferences);
                _codeGeneratorConfig.preProcessors = _availablePreProcessorTypes;
                _codeGeneratorConfig.dataProviders = _availableDataProviderTypes;
                _codeGeneratorConfig.codeGenerators = _availableGeneratorTypes;
                _codeGeneratorConfig.postProcessors = _availablePostProcessorTypes;
            }
        }

        void drawConfigurables() {
            var defaultProperties = CodeGeneratorUtil.GetDefaultProperties(_instances, _codeGeneratorConfig);
            _preferences.properties.AddProperties(defaultProperties, false);

            foreach (var kv in defaultProperties.OrderBy(kv => kv.Key)) {
                _preferences[kv.Key] = EditorGUILayout.TextField(kv.Key.ShortTypeName().ToSpacedCamelCase(), _preferences[kv.Key]);
            }
        }

        static void setTypesAndNames<T>(ICodeGenerationPlugin[] instances, out string[] availableTypes, out string[] availableNames) where T : ICodeGenerationPlugin {
            var orderedInstances = CodeGeneratorUtil.GetOrderedInstancesOf<T>(instances);

            availableTypes = orderedInstances
                .Select(instance => instance.GetType().ToCompilableString())
                .ToArray();

            availableNames = orderedInstances
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

            if (names.Length != 0) {
                var everything = (int)Math.Pow(2, types.Length) - 1;
                if (mask == everything) {
                    mask = -1;
                }

                mask = EditorGUILayout.MaskField(title, mask, names);
            } else {
                EditorGUILayout.LabelField(title, "No " + title + " available");
            }

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
