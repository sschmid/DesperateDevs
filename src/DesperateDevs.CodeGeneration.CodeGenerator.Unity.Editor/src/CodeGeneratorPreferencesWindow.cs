﻿using DesperateDevs.Serialization;
using DesperateDevs.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor
{
    public class CodeGeneratorPreferencesWindow : PreferencesWindow
    {
        [MenuItem(CodeGeneratorMenuItems.preferences, false, CodeGeneratorMenuItemPriorities.preferences)]
        public static void OpenPreferences()
        {
            var window = GetWindow<CodeGeneratorPreferencesWindow>(true, "Jenny");
            window.minSize = new Vector2(415f, 366f);
            window.Initialize(
                EditorPrefs.GetString(CodeGeneratorPreferencesDrawer.PROPERTIES_PATH_KEY, CodeGenerator.defaultPropertiesPath),
                Preferences.defaultUserPropertiesPath,
                "DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor.CodeGeneratorPreferencesDrawer"
            );

            window.Show();
        }
    }
}