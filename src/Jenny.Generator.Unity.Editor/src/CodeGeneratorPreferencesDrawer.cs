using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Unity.Editor;
using DesperateDevs.Extensions;
using UnityEditor;
using UnityEngine;

namespace Jenny.Generator.Unity.Editor
{
    public class CodeGeneratorPreferencesDrawer : AbstractPreferencesDrawer
    {
        public override string Title => "Jenny";

        string[] _availablePreProcessorTypes;
        string[] _availableDataProviderTypes;
        string[] _availableGeneratorTypes;
        string[] _availablePostProcessorTypes;

        string[] _availablePreProcessorNames;
        string[] _availableDataProviderNames;
        string[] _availableGeneratorNames;
        string[] _availablePostProcessorNames;

        Texture2D _headerTexture;
        ICodeGenerationPlugin[] _instances;

        CodeGeneratorConfig _codeGeneratorConfig;

        public static readonly string PropertiesPathKey = $"{typeof(CodeGeneratorPreferencesDrawer).Namespace}.PropertiesPath";
        public static readonly string UseExternalCodeGeneratorKey = $"{typeof(CodeGeneratorPreferencesDrawer).Namespace}.UseExternalCodeGenerator";

        bool _useExternalCodeGenerator;
        bool _doDryRun;

        public override void Initialize(Preferences preferences)
        {
            _headerTexture = EditorLayout.LoadTexture("l:Jenny-Header");
            _codeGeneratorConfig = preferences.CreateAndConfigure<CodeGeneratorConfig>();
            preferences.Properties.AddProperties(_codeGeneratorConfig.DefaultProperties, false);

            _instances = CodeGeneratorUtil.LoadFromPlugins(preferences);

            SetTypesAndNames<IPreProcessor>(_instances, out _availablePreProcessorTypes, out _availablePreProcessorNames);
            SetTypesAndNames<IDataProvider>(_instances, out _availableDataProviderTypes, out _availableDataProviderNames);
            SetTypesAndNames<ICodeGenerator>(_instances, out _availableGeneratorTypes, out _availableGeneratorNames);
            SetTypesAndNames<IPostProcessor>(_instances, out _availablePostProcessorTypes, out _availablePostProcessorNames);

            preferences.Properties.AddProperties(CodeGeneratorUtil.GetDefaultProperties(_instances, _codeGeneratorConfig), false);

            _useExternalCodeGenerator = EditorPrefs.GetBool(UseExternalCodeGeneratorKey);
            _doDryRun = EditorPrefs.GetBool(UnityCodeGenerator.DryRunKey, true);
        }

        public override void DrawHeader(Preferences preferences)
        {
            var rect = EditorLayout.DrawTexture(_headerTexture);
            var propertiesPath = Path.GetFileName(preferences.PropertiesPath);

            var buttonWidth = 60 + propertiesPath.Length * 5;
            const int buttonHeight = 15;
            const int padding = 4;
            var buttonRect = new Rect(
                rect.width - buttonWidth - padding,
                rect.y + rect.height - buttonHeight - padding,
                buttonWidth,
                buttonHeight
            );

            if (GUI.Button(buttonRect, "Edit " + propertiesPath, EditorStyles.miniButton))
            {
                EditorWindow.focusedWindow.Close();
                System.Diagnostics.Process.Start(preferences.PropertiesPath);
            }
        }

        protected override void OnDrawContent(Preferences preferences)
        {
            var path = EditorLayout.ObjectFieldOpenFilePanel(
                "Properties",
                preferences.PropertiesPath,
                preferences.PropertiesPath,
                "properties"
            );
            if (!string.IsNullOrEmpty(path))
            {
                EditorPrefs.SetString(PropertiesPathKey, path);
                EditorWindow.focusedWindow.Close();
                CodeGeneratorPreferencesWindow.OpenPreferences();
            }

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Auto Import Plugins");
                if (EditorLayout.MiniButton("Auto Import"))
                    AutoImport(preferences);
            }
            EditorGUILayout.EndHorizontal();

            _codeGeneratorConfig.PreProcessors = DrawMaskField("Pre Processors", _availablePreProcessorTypes, _availablePreProcessorNames, _codeGeneratorConfig.PreProcessors);
            _codeGeneratorConfig.DataProviders = DrawMaskField("Data Providers", _availableDataProviderTypes, _availableDataProviderNames, _codeGeneratorConfig.DataProviders);
            _codeGeneratorConfig.CodeGenerators = DrawMaskField("Code Generators", _availableGeneratorTypes, _availableGeneratorNames, _codeGeneratorConfig.CodeGenerators);
            _codeGeneratorConfig.PostProcessors = DrawMaskField("Post Processors", _availablePostProcessorTypes, _availablePostProcessorNames, _codeGeneratorConfig.PostProcessors);

            DrawConfigurables(preferences);

            EditorGUILayout.Space();
            DrawGenerateButtons();
        }

        void AutoImport(Preferences preferences)
        {
            var propertiesPath = Path.GetFileName(preferences.PropertiesPath);
            if (EditorUtility.DisplayDialog("Jenny - Auto Import",
                    "Auto Import will automatically find and set all plugins for you. " +
                    "It will search in folders and sub folders specified in " + propertiesPath +
                    " under the key '" + CodeGeneratorConfig.SearchPathsKey + "'." +
                    "\n\nThis will overwrite your current plugin settings." +
                    "\n\nDo you want to continue?",
                    "Continue and Overwrite",
                    "Cancel"
                ))
            {
                var searchPaths = CodeGeneratorUtil.BuildSearchPaths(
                    _codeGeneratorConfig.SearchPaths,
                    new[] {"./Assets", "./Library/ScriptAssemblies"}
                );

                CodeGeneratorUtil.AutoImport(_codeGeneratorConfig, searchPaths);
                preferences.Save();

                Initialize(preferences);
                _codeGeneratorConfig.PreProcessors = _availablePreProcessorTypes;
                _codeGeneratorConfig.DataProviders = _availableDataProviderTypes;
                _codeGeneratorConfig.CodeGenerators = _availableGeneratorTypes;
                _codeGeneratorConfig.PostProcessors = _availablePostProcessorTypes;
            }
        }

        void DrawConfigurables(Preferences preferences)
        {
            var defaultProperties = CodeGeneratorUtil.GetDefaultProperties(_instances, _codeGeneratorConfig);
            preferences.Properties.AddProperties(defaultProperties, false);

            if (defaultProperties.Count != 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Plugins Configuration", EditorStyles.boldLabel);
            }

            foreach (var kvp in defaultProperties.OrderBy(kvp => kvp.Key))
                preferences[kvp.Key] = EditorGUILayout.TextField(kvp.Key.ShortTypeName().ToSpacedCamelCase(), preferences[kvp.Key]);
        }

        static void SetTypesAndNames<T>(ICodeGenerationPlugin[] instances, out string[] availableTypes, out string[] availableNames) where T : ICodeGenerationPlugin
        {
            var orderedInstances = CodeGeneratorUtil.GetOrderedInstancesOf<T>(instances);

            availableTypes = orderedInstances
                .Select(instance => instance.GetType().ToCompilableString())
                .ToArray();

            availableNames = orderedInstances
                .Select(instance => instance.Name)
                .ToArray();
        }

        static string[] DrawMaskField(string title, string[] types, string[] names, string[] input)
        {
            var mask = 0;

            for (var i = 0; i < types.Length; i++)
                if (input.Contains(types[i]))
                    mask += (1 << i);

            if (names.Length != 0)
            {
                var everything = (int)Math.Pow(2, types.Length) - 1;
                if (mask == everything)
                    mask = -1;

                mask = EditorGUILayout.MaskField(title, mask, names);
            }
            else
            {
                EditorGUILayout.LabelField(title, "No " + title + " available");
            }

            var selected = new List<string>();
            for (var i = 0; i < types.Length; i++)
            {
                var index = 1 << i;
                if ((index & mask) == index)
                    selected.Add(types[i]);
            }

            // Re-add unavailable types
            selected.AddRange(input.Where(type => !types.Contains(type)));

            return selected.ToArray();
        }

        void DrawGenerateButtons()
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUI.BeginChangeCheck();
                {
                    _useExternalCodeGenerator = EditorGUILayout.Toggle("Use Jenny Server", _useExternalCodeGenerator);
                }
                var useExternalCodeGeneratorChanged = EditorGUI.EndChangeCheck();
                if (useExternalCodeGeneratorChanged)
                    EditorPrefs.SetBool(UseExternalCodeGeneratorKey, _useExternalCodeGenerator);

                if (_useExternalCodeGenerator)
                {
                    _codeGeneratorConfig.Port = EditorGUILayout.IntField("Port", _codeGeneratorConfig.Port);
                    _codeGeneratorConfig.Host = EditorGUILayout.TextField("Host", _codeGeneratorConfig.Host);
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    {
                        _doDryRun = EditorGUILayout.Toggle("Safe Mode (Dry Run first)", _doDryRun);
                    }
                    var doDryRunChanged = EditorGUI.EndChangeCheck();
                    if (doDryRunChanged)
                        EditorPrefs.SetBool(UnityCodeGenerator.DryRunKey, _doDryRun);
                }

                var bgColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Generate", GUILayout.Height(32)))
                {
                    if (_useExternalCodeGenerator)
                        UnityCodeGenerator.GenerateExternal();
                    else
                        UnityCodeGenerator.Generate();
                }

                GUI.backgroundColor = bgColor;
            }
            EditorGUILayout.EndVertical();
        }
    }
}
