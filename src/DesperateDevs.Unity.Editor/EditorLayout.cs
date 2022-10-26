﻿using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DesperateDevs.Unity.Editor
{
    public static class EditorLayout
    {
        public static bool DrawSectionHeaderToggle(string header, bool value) =>
            GUILayout.Toggle(value, header, Styles.SectionHeader);

        public static void BeginSectionContent() =>
            EditorGUILayout.BeginVertical(Styles.SectionContent);

        public static void EndSectionContent() =>
            EditorGUILayout.EndVertical();

        public static Rect BeginVerticalBox() =>
            EditorGUILayout.BeginVertical(GUI.skin.box);

        public static void EndVerticalBox() =>
            EditorGUILayout.EndVertical();

        public static Texture2D LoadTexture(string label)
        {
            var guid = AssetDatabase.FindAssets(label).FirstOrDefault();
            return guid != null
                ? AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guid))
                : null;
        }

        public static Rect DrawTexture(Texture2D texture)
        {
            if (texture != null)
            {
                var ratio = (float)texture.width / texture.height;
                var rect = GUILayoutUtility.GetAspectRect(ratio, GUILayout.ExpandWidth(true));
                GUI.DrawTexture(rect, texture, ScaleMode.ScaleAndCrop);
                return rect;
            }
            else
            {
                return new Rect();
            }
        }

        public static bool ObjectFieldButton(string label, string buttonText)
        {
            bool clicked;
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(146));
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
                return path.Replace($"{Directory.GetCurrentDirectory()}/", string.Empty);
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
                return path.Replace($"{Directory.GetCurrentDirectory()}/", string.Empty);
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
                searchString = GUILayout.TextField(searchString, Styles.ToolbarSearchTextField);
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
