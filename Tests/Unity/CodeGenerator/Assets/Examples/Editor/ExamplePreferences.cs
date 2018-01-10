using DesperateDevs.Serialization;
using DesperateDevs.Unity.Editor;
using UnityEditor;
using UnityEngine;

public static class ExamplePreferences {

    [MenuItem("Example/Preferences... #%e", false, 0)]
    public static void OpenPreferences() {
        Preferences.sharedInstance = null;
        var window = EditorLayout.GetWindow<PreferencesWindow>("Example", new Vector2(415f, 564));
        window.preferencesName = "Example";
        window.Show();
    }
}

