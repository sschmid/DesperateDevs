using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DesperateDevs.Unity.Editor
{
    public static partial class EditorLayout
    {
        public static Texture2D LoadTexture(string label)
        {
            var guid = AssetDatabase.FindAssets(label).FirstOrDefault();
            if (guid != null)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }
            else
            {
                return null;
            }
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

            return new Rect();
        }
    }
}
