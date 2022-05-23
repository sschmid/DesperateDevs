using DesperateDevs.Serialization;
using DesperateDevs.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Samples.DesperateDevs.Unity.Editor.Editor
{
    public class SamplePreferencesDrawer : AbstractPreferencesDrawer
    {
        public override string Title => nameof(SamplePreferencesDrawer);
        Texture2D _headerTexture;

        bool _unfolded = true;
        string _objectFieldFolder = "Assets";
        string _objectFieldFile = "Sample.properties";
        string _searchString = string.Empty;

        public override void Initialize(Preferences preferences)
        {
            _headerTexture = EditorLayout.LoadTexture("l:Jenny-Header");
        }

        public override void DrawHeader(Preferences preferences)
        {
            EditorLayout.DrawTexture(_headerTexture);
        }

        protected override void OnDrawContent(Preferences preferences)
        {
            var path = EditorLayout.ObjectFieldOpenFolderPanel("Folder", _objectFieldFolder, "Assets");
            if (!string.IsNullOrEmpty(path)) _objectFieldFolder = path;

            path = EditorLayout.ObjectFieldOpenFilePanel("File", _objectFieldFile, "Assets", "properties");
            if (!string.IsNullOrEmpty(path)) _objectFieldFile = path;

            EditorGUILayout.Space();
            _unfolded = EditorLayout.Foldout(_unfolded, "Foldout");
            if (_unfolded)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorLayout.MiniButton("Mini Button");
                    EditorLayout.MiniButton("x");
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    EditorLayout.MiniButtonLeft("Left");
                    EditorLayout.MiniButtonMid("Middle");
                    EditorLayout.MiniButtonRight("Right");
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Search");
            _searchString = EditorLayout.SearchTextField(_searchString);
            if (EditorLayout.MatchesSearchString("Desperate", _searchString))
                EditorGUILayout.LabelField("Desperate");
            if (EditorLayout.MatchesSearchString("Devs", _searchString))
                EditorGUILayout.LabelField("Devs");
        }
    }
}
