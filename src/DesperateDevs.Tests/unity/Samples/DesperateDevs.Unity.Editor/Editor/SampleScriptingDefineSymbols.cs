using DesperateDevs.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Samples.DesperateDevs.Unity.Editor.Editor
{
    public class SampleScriptingDefineSymbolsWindow : EditorWindow
    {
        [MenuItem("Samples/DesperateDevs.Unity.Editor/Create SampleScriptingDefineSymbolsWindow")]
        public static void CreateWindow()
        {
            EditorWindow.CreateWindow<SampleScriptingDefineSymbolsWindow>(nameof(SampleScriptingDefineSymbolsWindow));
        }

        ScriptingDefineSymbols _symbols;

        void OnFocus() => _symbols = new ScriptingDefineSymbols();

        void OnGUI()
        {
            if (GUILayout.Button("Add SAMPLE")) _symbols.Add("SAMPLE", BuildTargetGroup.WebGL);
            if (GUILayout.Button("Remove SAMPLE")) _symbols.Remove("SAMPLE", BuildTargetGroup.WebGL);
            if (GUILayout.Button("AddForAll SAMPLE")) _symbols.AddForAll("SAMPLE");
            if (GUILayout.Button("RemoveForAll SAMPLE")) _symbols.RemoveForAll("SAMPLE");
        }
    }
}
