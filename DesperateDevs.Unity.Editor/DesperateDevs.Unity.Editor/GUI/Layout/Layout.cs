using UnityEditor;
using UnityEngine;

namespace DesperateDevs.Unity.Editor {

    public static partial class EditorLayout {

        public static bool DrawSectionHeaderToggle(string header, bool value) {
            return GUILayout.Toggle(value, header, Styles.sectionHeader);
        }

        public static void BeginSectionContent() {
            EditorGUILayout.BeginVertical(Styles.sectionContent);
        }

        public static void EndSectionContent() {
            EditorGUILayout.EndVertical();
        }

        public static Rect BeginVerticalBox() {
            return EditorGUILayout.BeginVertical(GUI.skin.box);
        }

        public static void EndVerticalBox() {
            EditorGUILayout.EndVertical();
        }
    }
}
