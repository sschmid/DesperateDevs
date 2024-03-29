using UnityEditor;

namespace DesperateDevs.Unity.Editor.Samples
{
    public class SampleMinifiedDoubleQuotedPreferencesWindow : PreferencesWindow
    {
        [MenuItem("Samples/DesperateDevs.Unity.Editor/Create SampleMinifiedDoubleQuotedPreferencesWindow")]
        public static void OpenPreferences()
        {
            var window = GetWindow<SampleMinifiedDoubleQuotedPreferencesWindow>(true, nameof(SampleMinifiedDoubleQuotedPreferencesWindow));
            window.Initialize(
                "Sample.properties",
                "Sample.userproperties",
                true,
                true,
                typeof(SamplePreferencesDrawer).FullName);

            window.Show();
        }
    }
}
