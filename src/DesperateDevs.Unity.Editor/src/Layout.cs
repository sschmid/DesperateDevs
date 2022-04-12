﻿using UnityEditor;
using UnityEngine;

namespace DesperateDevs.Unity.Editor
{
    public static partial class EditorLayout
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
    }
}