using System;
using DesperateDevs.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor
{
    public static class CodeGeneratorPreferencesWindow
    {
        [MenuItem(CodeGeneratorMenuItems.preferences, false, CodeGeneratorMenuItemPriorities.preferences)]
        [MenuItem("Tools/Jenny/Preferences... #%j", false, 1)]
        public static void OpenPreferences()
        {
            var propertiesPath = EditorPrefs.GetString(CodeGeneratorPreferencesDrawer.PROPERTIES_PATH_KEY, "Jenny.properties");
            var window = EditorLayout.GetWindow<PreferencesWindow>("Jenny", new Vector2(415f, 600));
            window.Initialize("Jenny", propertiesPath, Environment.UserName + ".userproperties");
            window.Show();
        }
    }
}
