using DesperateDevs.Unity.Editor;
using UnityEditor;

namespace Samples.DesperateDevs.Unity.Editor.Editor
{
    public class SampleMinifiedPreferencesWindow : PreferencesWindow
    {
        [MenuItem("Samples/DesperateDevs.Unity.Editor/Create SampleMinifiedPreferencesWindow")]
        public static void OpenPreferences()
        {
            var window = GetWindow<SampleMinifiedPreferencesWindow>(true, nameof(SampleMinifiedPreferencesWindow));
            window.Initialize(
                "Sample.properties",
                "Sample.userproperties",
                true,
                false,
                typeof(SamplePreferencesDrawer).FullName);

            window.Show();
        }
    }
}
