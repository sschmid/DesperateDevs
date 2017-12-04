using UnityEditor;
using UnityEngine;

namespace DesperateDevs.Unity.Editor {

    public static partial class EditorLayout {

        public static void ShowWindow<T>(string title, Vector2 size) where T : EditorWindow {
            var window = EditorWindow.GetWindow<T>(true, title);
            window.minSize = window.maxSize = size;
            window.Show();
        }

        public static Texture2D LoadTexture(string label) {
            var assets = AssetDatabase.FindAssets(label);
            if (assets.Length > 0) {
                var guid = assets[0];
                if (guid != null) {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                }
            }

            return null;
        }

        public static Rect DrawTexture(Texture2D texture) {
            if (texture != null) {
                var ratio = (float)texture.width / (float)texture.height;
                var rect = GUILayoutUtility.GetAspectRect(ratio, GUILayout.ExpandWidth(true));
                GUI.DrawTexture(rect, texture, ScaleMode.ScaleAndCrop);
                return rect;
            }

            return new Rect();
        }
    }
}
