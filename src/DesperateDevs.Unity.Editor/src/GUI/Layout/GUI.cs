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
            EditorGUILayout.LabelField(label, GUILayout.Width(146));
            if (buttonText.Length > 24)
            {
                buttonText = "..." + buttonText.Substring(buttonText.Length - 24);
            }

            clicked = GUILayout.Button(buttonText, EditorStyles.objectField);
            EditorGUILayout.EndHorizontal();

            return clicked;
        }

        public static string ObjectFieldOpenFolderPanel(string label, string buttonText, string defaultPath)
        {
            if (ObjectFieldButton(label, buttonText))
            {
                var path = defaultPath ?? "Assets/";
                if (!Directory.Exists(path))
                {
                    path = "Assets/";
                }

                path = EditorUtility.OpenFolderPanel(label, path, string.Empty);
                return path.Replace(Directory.GetCurrentDirectory() + "/", string.Empty);
            }

            return null;
        }

        public static string ObjectFieldOpenFilePanel(string label, string buttonText, string defaultPath, string extension)
        {
            if (ObjectFieldButton(label, buttonText))
            {
                var path = defaultPath ?? "Assets/";
                if (!File.Exists(path))
                {
                    path = "Assets/";
                }

                path = EditorUtility.OpenFilePanel(label, path, extension);
                return path.Replace(Directory.GetCurrentDirectory() + "/", string.Empty);
            }

            return null;
        }

        public static bool MiniButton(string c)
        {
            return MiniButton(c, EditorStyles.miniButton);
        }

        public static bool MiniButtonLeft(string c)
        {
            return MiniButton(c, EditorStyles.miniButtonLeft);
        }

        public static bool MiniButtonMid(string c)
        {
            return MiniButton(c, EditorStyles.miniButtonMid);
        }

        public static bool MiniButtonRight(string c)
        {
            return MiniButton(c, EditorStyles.miniButtonRight);
        }

        static bool MiniButton(string c, GUIStyle style)
        {
            var options = c.Length == 1
                ? new[] {GUILayout.Width(19)}
                : new GUILayoutOption[0];

            var clicked = GUILayout.Button(c, style, options);
            if (clicked)
            {
                GUI.FocusControl(null);
            }

            return clicked;
        }

        const int DefaultFoldoutMargin = 11;

        public static bool Foldout(bool foldout, string content, int leftMargin = DefaultFoldoutMargin)
        {
            return Foldout(foldout, content, EditorStyles.foldout, leftMargin);
        }

        public static bool Foldout(bool foldout, string content, GUIStyle style, int leftMargin = DefaultFoldoutMargin)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(leftMargin);
            foldout = EditorGUILayout.Foldout(foldout, content, style);
            EditorGUILayout.EndHorizontal();
            return foldout;
        }

        public static string SearchTextField(string searchString)
        {
            var guiChanged = GUI.changed;
            GUILayout.BeginHorizontal();
            searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"));
            if (GUILayout.Button(string.Empty, GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
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
