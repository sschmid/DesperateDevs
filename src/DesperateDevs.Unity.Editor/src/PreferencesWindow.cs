using System;
using System.Linq;
using DesperateDevs.Serialization;
using UnityEditor;
using UnityEngine;

namespace DesperateDevs.Unity.Editor
{
    public class PreferencesWindow : EditorWindow
    {
        public Preferences Preferences => _preferences;

        string _propertiesPath;
        string _userPropertiesPath;
        string[] _preferencesDrawerNames;
        bool _minified;
        bool _doubleQuotedValues;

        Preferences _preferences;
        IPreferencesDrawer[] _preferencesDrawers;
        Vector2 _scrollViewPosition;

        Exception _configException;

        public void Initialize(
            string propertiesPath,
            string userPropertiesPath,
            bool minified,
            bool doubleQuotedValues,
            params string[] preferencesDrawerNames)
        {
            _propertiesPath = propertiesPath;
            _userPropertiesPath = userPropertiesPath;
            _preferencesDrawerNames = preferencesDrawerNames;
            _minified = minified;
            _doubleQuotedValues = doubleQuotedValues;
        }

        void Initialize()
        {
            try
            {
                _preferences = new Preferences(_propertiesPath, _userPropertiesPath, _doubleQuotedValues)
                {
                    Minified = _minified
                };
                var availableDrawers = TypeCache.GetTypesDerivedFrom<IPreferencesDrawer>();
                _preferencesDrawers = _preferencesDrawerNames
                    .Select(drawerName => availableDrawers.SingleOrDefault(type => type.FullName == drawerName))
                    .Where(type => type != null)
                    .Select(type => (IPreferencesDrawer)Activator.CreateInstance(type))
                    .ToArray();

                foreach (var drawer in _preferencesDrawers)
                    drawer.Initialize(_preferences);

                _preferences.Save();
            }
            catch (Exception exception)
            {
                _preferencesDrawers = Array.Empty<IPreferencesDrawer>();
                _configException = exception;
            }
        }

        void OnGUI()
        {
            if (_preferencesDrawers == null)
                Initialize();

            DrawHeader();
            _scrollViewPosition = EditorGUILayout.BeginScrollView(_scrollViewPosition);
            {
                DrawContent();
            }
            EditorGUILayout.EndScrollView();

            if (GUI.changed)
                _preferences.Save();
        }

        void DrawHeader()
        {
            foreach (var drawer in _preferencesDrawers)
            {
                try
                {
                    drawer.DrawHeader(_preferences);
                }
                catch (Exception exception)
                {
                    DrawException(exception);
                }
            }
        }

        void DrawContent()
        {
            if (_configException == null)
            {
                for (var i = 0; i < _preferencesDrawers.Length; i++)
                {
                    try
                    {
                        _preferencesDrawers[i].DrawContent(_preferences);
                    }
                    catch (Exception exception)
                    {
                        DrawException(exception);
                    }

                    if (i < _preferencesDrawers.Length - 1)
                    {
                        EditorGUILayout.Space();
                    }
                }
            }
            else
            {
                DrawException(_configException);
            }
        }

        static void DrawException(Exception exception)
        {
            EditorGUILayout.LabelField(Event.current.alt ? exception.ToString() : exception.Message, Styles.ErrorLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Please make sure the properties files are set up correctly.");
        }
    }
}
