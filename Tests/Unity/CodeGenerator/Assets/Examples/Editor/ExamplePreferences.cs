using DesperateDevs.Unity.Editor;
using UnityEditor;
using UnityEngine;

public static class ExamplePreferences {

    [MenuItem("Example/Preferences... #%e", false, 0)]
    public static void OpenPreferences() {
        var prefrences = EditorLayout.GetWindow<PreferencesWindow>("Example", new Vector2(415f, 564));
        prefrences.preferencesName = "Example";
        prefrences.Show();
    }
}
