using System;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;
using UnityEditor;
using UnityEngine;

namespace DesperateDevs.Unity.Editor
{
    public class PreferencesWindow : EditorWindow
    {
        string _preferencesId;
        string _propertiesPath;
        string _userPropertiesPath;

        Preferences _preferences;
        IPreferencesDrawer[] _preferencesDrawers;
        Vector2 _scrollViewPosition;

        Exception _configException;

        public void Initialize(string preferencesId, string propertiesPath, string userPropertiesPath)
        {
            _preferencesId = preferencesId;
            _propertiesPath = propertiesPath;
            _userPropertiesPath = userPropertiesPath;
        }

        void initialize()
        {
            try
            {
                _preferences = new Preferences(_propertiesPath, _userPropertiesPath);

                var config = new PreferencesConfig(_preferencesId);
                _preferences.properties.AddProperties(config.defaultProperties, false);
                config.Configure(_preferences);

                var allPreferencesDrawers = AppDomain.CurrentDomain
                    .GetInstancesOf<IPreferencesDrawer>()
                    .OrderBy(drawer => drawer.priority)
                    .ToArray();

                if (config.preferenceDrawers.Length == 0)
                {
                    config.preferenceDrawers = allPreferencesDrawers
                        .Select(drawer => drawer.GetType().FullName)
                        .ToArray();
                }

                var enabledPreferenceDrawers = config.preferenceDrawers;

                _preferencesDrawers = allPreferencesDrawers
                    .Where(drawer => enabledPreferenceDrawers.Contains(drawer.GetType().FullName))
                    .ToArray();

                foreach (var drawer in _preferencesDrawers)
                {
                    drawer.Initialize(_preferences);
                }

                _preferences.Save();
            }
            catch (Exception ex)
            {
                _preferencesDrawers = new IPreferencesDrawer[0];
                _configException = ex;
            }
        }

        void OnGUI()
        {
            if (_preferencesDrawers == null)
            {
                initialize();
            }

            drawHeader();
            _scrollViewPosition = EditorGUILayout.BeginScrollView(_scrollViewPosition);
            {
                drawContent();
            }
            EditorGUILayout.EndScrollView();

            if (GUI.changed)
            {
                _preferences.Save();
            }
        }

        void drawHeader()
        {
            foreach (var drawer in _preferencesDrawers)
            {
                try
                {
                    drawer.DrawHeader(_preferences);
                }
                catch (Exception ex)
                {
                    drawException(ex);
                }
            }
        }

        void drawContent()
        {
            if (_configException == null)
            {
                for (int i = 0; i < _preferencesDrawers.Length; i++)
                {
                    try
                    {
                        _preferencesDrawers[i].DrawContent(_preferences);
                    }
                    catch (Exception ex)
                    {
                        drawException(ex);
                    }

                    if (i < _preferencesDrawers.Length - 1)
                    {
                        EditorGUILayout.Space();
                    }
                }
            }
            else
            {
                drawException(_configException);
            }
        }

        static void drawException(Exception exception)
        {
            var style = new GUIStyle(GUI.skin.label);
            style.wordWrap = true;
            style.normal.textColor = Color.red;

            if (Event.current.alt)
            {
                EditorGUILayout.LabelField(exception.ToString(), style);
            }
            else
            {
                EditorGUILayout.LabelField(exception.Message, style);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Please make sure the properties files are set up correctly.");
        }
    }
}
