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
        public Preferences preferences { get { return _preferences; } }

        string _propertiesPath;
        string _userPropertiesPath;
        string[] _preferencesDrawerNames;

        Preferences _preferences;
        IPreferencesDrawer[] _preferencesDrawers;
        Vector2 _scrollViewPosition;

        Exception _configException;

        public void Initialize(string propertiesPath, string userPropertiesPath, params string[] preferencesDrawerNames)
        {
            _propertiesPath = propertiesPath;
            _userPropertiesPath = userPropertiesPath;
            _preferencesDrawerNames = preferencesDrawerNames;
        }

        void initialize()
        {
            try
            {
                _preferences = new Preferences(_propertiesPath, _userPropertiesPath);

                var availableDrawers = AppDomain.CurrentDomain
                    .GetNonAbstractTypes<IPreferencesDrawer>();

                _preferencesDrawers = _preferencesDrawerNames
                    .Select(drawerName => availableDrawers.SingleOrDefault(type => type.FullName == drawerName))
                    .Where(type => type != null)
                    .Select(type => (IPreferencesDrawer)Activator.CreateInstance(type))
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
