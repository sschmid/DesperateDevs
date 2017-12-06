using System;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;
using UnityEditor;
using UnityEngine;

namespace DesperateDevs.Unity.Editor {

    public class PreferencesWindow : EditorWindow {

        public string preferencesName;

        Preferences _preferences;
        IPreferencesDrawer[] _preferencesDrawers;
        Vector2 _scrollViewPosition;

        Exception _configException;

        void initialize() {
            try {
                _preferences = Preferences.sharedInstance;

                var config = new PreferencesConfig(preferencesName);
                _preferences.properties.AddProperties(config.defaultProperties, false);
                config.Configure(_preferences);

                var allPreferencesDrawers = AppDomain.CurrentDomain
                    .GetInstancesOf<IPreferencesDrawer>()
                    .OrderBy(drawer => drawer.priority)
                    .ToArray();

                if (config.preferenceDrawers.Length == 0) {
                    config.preferenceDrawers = allPreferencesDrawers
                        .Select(drawer => drawer.GetType().FullName)
                        .ToArray();
                }

                var enabledPreferenceDrawers = config.preferenceDrawers;

                _preferencesDrawers = allPreferencesDrawers
                    .Where(drawer => enabledPreferenceDrawers.Contains(drawer.GetType().FullName))
                    .ToArray();

                foreach (var drawer in _preferencesDrawers) {
                    drawer.Initialize(_preferences);
                }

                _preferences.Save();
            } catch (Exception ex) {
                _preferencesDrawers = new IPreferencesDrawer[0];
                _configException = ex;
            }
        }

        void OnGUI() {
            if (_preferencesDrawers == null) {
                initialize();
            }

            drawHeader();
            _scrollViewPosition = EditorGUILayout.BeginScrollView(_scrollViewPosition);
            {
                drawContent();
            }
            EditorGUILayout.EndScrollView();

            if (GUI.changed) {
                _preferences.Save();
            }
        }

        void drawHeader() {
            for (int i = 0; i < _preferencesDrawers.Length; i++) {
                try {
                    _preferencesDrawers[i].DrawHeader(_preferences);
                } catch (Exception ex) {
                    drawException(ex);
                }
            }
        }

        void drawContent() {
            if (_configException == null) {
                for (int i = 0; i < _preferencesDrawers.Length; i++) {
                    try {
                        _preferencesDrawers[i].DrawContent(_preferences);
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

            if (Event.current.alt) {
                EditorGUILayout.LabelField(exception.ToString(), style);
            } else {
                EditorGUILayout.LabelField(exception.Message, style);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Please make sure the properties files are set up correctly.");
        }
    }
}
