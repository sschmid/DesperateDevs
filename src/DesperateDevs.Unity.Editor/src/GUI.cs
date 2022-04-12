using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DesperateDevs.Unity.Editor
{
    public static partial class EditorLayout
    {
        public static bool ObjectFieldButton(string label, string buttonText)
        {
            var clicked = false;
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(146));
                if (buttonText.Length > 24)
                    buttonText = "..." + buttonText.Substring(buttonText.Length - 24);

                clicked = GUILayout.Button(buttonText, EditorStyles.objectField);
            }
            EditorGUILayout.EndHorizontal();

            return clicked;
        }

        public static string ObjectFieldOpenFolderPanel(string label, string buttonText, string defaultPath)
        {
            if (ObjectFieldButton(label, buttonText))
            {
                var path = defaultPath ?? "Assets/";
                if (!Directory.Exists(path))
                    path = "Assets/";

                path = EditorUtility.OpenFolderPanel(label, path, string.Empty);
                return path.Replace(Directory.GetCurrentDirectory() + "/", string.Empty);
            }
            else
            {
                return null;
            }
        }

        public static string ObjectFieldOpenFilePanel(string label, string buttonText, string defaultPath, string extension)
        {
            if (ObjectFieldButton(label, buttonText))
            {
                var path = defaultPath ?? "Assets/";
                if (!File.Exists(path))
                    path = "Assets/";

                path = EditorUtility.OpenFilePanel(label, path, extension);
                return path.Replace(Directory.GetCurrentDirectory() + "/", string.Empty);
            }
            else
            {
                return null;
            }
        }

        public static bool MiniButton(string c) => MiniButton(c, EditorStyles.miniButton);
        public static bool MiniButtonLeft(string c) => MiniButton(c, EditorStyles.miniButtonLeft);
        public static bool MiniButtonMid(string c) => MiniButton(c, EditorStyles.miniButtonMid);
        public static bool MiniButtonRight(string c) => MiniButton(c, EditorStyles.miniButtonRight);

        static bool MiniButton(string c, GUIStyle style)
        {
            var options = c.Length == 1
                ? new[] {GUILayout.Width(19)}
                : Array.Empty<GUILayoutOption>();

            var clicked = GUILayout.Button(c, style, options);
            if (clicked)
                GUI.FocusControl(null);

            return clicked;
        }

        const int DefaultFoldoutMargin = 11;

        public static bool Foldout(bool foldout, string content, int leftMargin = DefaultFoldoutMargin) =>
            Foldout(foldout, content, EditorStyles.foldout, leftMargin);

        public static bool Foldout(bool foldout, string content, GUIStyle style, int leftMargin = DefaultFoldoutMargin)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(leftMargin);
                foldout = EditorGUILayout.Foldout(foldout, content, style);
            }
            EditorGUILayout.EndHorizontal();
            return foldout;
        }

        public static string SearchTextField(string searchString)
        {
            var guiChanged = GUI.changed;
            GUILayout.BeginHorizontal();
            {
                searchString = GUILayout.TextField(searchString, EditorStyles.toolbarSearchField);
                if (GUILayout.Button(string.Empty, Styles.ToolbarSearchCancelButtonStyle))
                    searchString = string.Empty;
            }
            GUILayout.EndHorizontal();
            GUI.changed = guiChanged;

            return searchString;
        }

        public static bool MatchesSearchString(string str, string search)
        {
            var searches = search.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            return searches.Length == 0 || searches.Any(str.Contains);
        }
    }
}
