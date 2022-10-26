using UnityEditor;
using UnityEngine;

namespace DesperateDevs.Unity.Editor.Samples
{
    public class SampleScriptingDefineSymbolsWindow : EditorWindow
    {
        [MenuItem("Samples/DesperateDevs.Unity.Editor/Create SampleScriptingDefineSymbolsWindow")]
        public static void CreateWindow()
        {
            GetWindow<SampleScriptingDefineSymbolsWindow>(nameof(SampleScriptingDefineSymbolsWindow));
        }

        ScriptingDefineSymbols _symbols;

        void OnFocus() => _symbols = new ScriptingDefineSymbols();

        void OnGUI()
        {
            if (GUILayout.Button("Log build target groups"))
                foreach (var buildTargetGroup in ScriptingDefineSymbols.BuildTargetGroups)
                    Debug.Log(buildTargetGroup);

            if (GUILayout.Button("Add SAMPLE")) _symbols.Add("SAMPLE", BuildTargetGroup.WebGL);
            if (GUILayout.Button("Remove SAMPLE")) _symbols.Remove("SAMPLE", BuildTargetGroup.WebGL);
            if (GUILayout.Button("AddForAll SAMPLE")) _symbols.AddForAll("SAMPLE");
            if (GUILayout.Button("RemoveForAll SAMPLE")) _symbols.RemoveForAll("SAMPLE");
        }
    }
}
