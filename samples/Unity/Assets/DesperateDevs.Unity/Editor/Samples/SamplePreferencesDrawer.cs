using System.Globalization;
using DesperateDevs.Serialization;
using UnityEditor;
using UnityEngine;

namespace DesperateDevs.Unity.Editor.Samples
{
    public class SamplePreferencesDrawer : AbstractPreferencesDrawer
    {
        public override string Title => nameof(SamplePreferencesDrawer);

        bool _unfolded = true;
        string _objectFieldFolder = "Assets";
        string _objectFieldFile = "Sample.properties";
        string _searchString = string.Empty;

        public override void Initialize(Preferences preferences) { }

        public override void DrawHeader(Preferences preferences) { }

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
            _searchString = EditorLayout.SearchTextField(_searchString).ToLowerInvariant();
            if (EditorLayout.MatchesSearchString("desperate", _searchString))
                EditorGUILayout.LabelField("Desperate");
            if (EditorLayout.MatchesSearchString("devs", _searchString))
                EditorGUILayout.LabelField("Devs");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(preferences["key"]);
            if (GUILayout.Button("Save new random"))
            {
                preferences["random"] = Random.value.ToString(CultureInfo.InvariantCulture);
                preferences.Save();
            }
        }
    }
}
