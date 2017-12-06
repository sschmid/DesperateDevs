using System;
using System.Linq;
using DesperateDevs.Unity.Editor;
using DesperateDevs.Utils;
using UnityEditor;
using UnityEngine;

public static class ExamplePreferences {

    [MenuItem("Example/Preferences...", false, 0)]
    public static void OpenPreferences() {
        var prefrences = EditorLayout.ShowWindow<PreferencesWindow>(
            "Example",
            new Vector2(415f, 564));

        prefrences.preferencesDrawers = AppDomain.CurrentDomain
            .GetInstancesOf<IPreferencesDrawer>()
            .OrderBy(drawer => drawer.priority)
            .ToArray();
    }
}
