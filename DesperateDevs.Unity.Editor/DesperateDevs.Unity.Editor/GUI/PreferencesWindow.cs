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
        string _propertiesPath;
        string _userPropertiesPath;
        string[] _preferencesDrawerName;

        Preferences _preferences;
        IPreferencesDrawer[] _preferencesDrawers;
        Vector2 _scrollViewPosition;

        Exception _configException;

        public void Initialize(string propertiesPath, string userPropertiesPath, params string[] preferencesDrawerName)
        {
            _propertiesPath = propertiesPath;
            _userPropertiesPath = userPropertiesPath;
            _preferencesDrawerName = preferencesDrawerName;
        }

        void initialize()
        {
            try
            {
                _preferences = new Preferences(_propertiesPath, _userPropertiesPath);

                _preferencesDrawers = AppDomain.CurrentDomain
                    .GetInstancesOf<IPreferencesDrawer>()
                    .Where(drawer => _preferencesDrawerName.Contains(drawer.GetType().FullName))
                    .OrderBy(drawer => drawer.priority)
                    .ToArray();

                foreach (var drawer in _preferencesDrawers)
                {
                    drawer.Initialize(_preferences);
                }
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
