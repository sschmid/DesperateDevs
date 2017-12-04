using System;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;
using UnityEditor;
using UnityEngine;

namespace DesperateDevs.Unity.Editor {

    public class PreferencesWindow : EditorWindow {

        Texture2D _headerTexture;
        Preferences _preferences;
        IPreferencesDrawer[] _preferencesDrawers;
        Vector2 _scrollViewPosition;

        Exception _configException;

        void OnEnable() {
            _headerTexture = EntitasEditorLayout.LoadTexture("l:EntitasHeader");
            _preferencesDrawers = AppDomain.CurrentDomain
                .GetInstancesOf<IPreferencesDrawer>()
                .OrderBy(drawer => drawer.priority)
                .ToArray();

            try {
                _preferences = Preferences.sharedInstance;
                _preferences.Refresh();

                foreach (var drawer in _preferencesDrawers) {
                    drawer.Initialize(_preferences);
                }

                _preferences.Save();
            } catch (Exception ex) {
                _configException = ex;
            }
        }

        void OnGUI() {
            _scrollViewPosition = EditorGUILayout.BeginScrollView(_scrollViewPosition);
            {
                drawPreferencesDrawers();
            }
            EditorGUILayout.EndScrollView();

            if (GUI.changed) {
                _preferences.Save();
            }
        }

        void drawPreferencesDrawers() {
            if (_configException == null) {
                for (int i = 0; i < _preferencesDrawers.Length; i++) {
                    try {
                        _preferencesDrawers[i].Draw(_preferences);
                    } catch (Exception ex) {
                        drawException(ex);
                    }

                    if (i < _preferencesDrawers.Length - 1) {
                        EditorGUILayout.Space();
                    }
                }
            } else {
                drawException(_configException);
            }
        }

        void drawException(Exception exception) {
            var style = new GUIStyle(GUI.skin.label);
            style.wordWrap = true;
            style.normal.textColor = Color.red;

            EditorGUILayout.LabelField(exception.Message, style);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Please make sure Entitas.properties is set up correctly.");
        }
    }
}
