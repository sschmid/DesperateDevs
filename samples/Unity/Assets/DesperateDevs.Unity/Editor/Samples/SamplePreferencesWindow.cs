using UnityEditor;

namespace DesperateDevs.Unity.Editor.Samples
{
    public class SamplePreferencesWindow : PreferencesWindow
    {
        [MenuItem("Samples/DesperateDevs.Unity.Editor/Create SamplePreferencesWindow")]
        public static void OpenPreferences()
        {
            var window = GetWindow<SamplePreferencesWindow>(true, nameof(SamplePreferencesWindow));
            window.Initialize(
                "Sample.properties",
                "Sample.userproperties",
                false,
                false,
                typeof(SamplePreferencesDrawer).FullName);

            window.Show();
        }
    }
}
