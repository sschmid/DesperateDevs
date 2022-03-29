using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

namespace DesperateDevs.Unity.Editor
{
    public class ScriptingDefineSymbols
    {
        public static BuildTargetGroup[] BuildTargetGroups => Enum.GetValues(typeof(BuildTargetGroup))
            .Cast<BuildTargetGroup>()
            .Where(buildTarget => buildTarget != BuildTargetGroup.Unknown)
            .Where(buildTarget => !Attribute.IsDefined(
                buildTarget.GetType().GetField(buildTarget.ToString()),
                typeof(ObsoleteAttribute)))
            .Distinct()
            .ToArray();

        public void Add(string defineSymbol, BuildTargetGroup buildTargetGroup)
        {
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                buildTargetGroup,
                Regex.Replace(symbols, $@"\b{defineSymbol}\b", string.Empty) + ";" + defineSymbol
            );
        }

        public void AddForAll(string defineSymbol)
        {
            foreach (var buildTargetGroup in BuildTargetGroups)
                Add(defineSymbol, buildTargetGroup);
        }

        public void Remove(string defineSymbol, BuildTargetGroup buildTargetGroup)
        {
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                buildTargetGroup,
                Regex.Replace(symbols, $@"\b{defineSymbol}\b", string.Empty)
            );
        }

        public void RemoveForAll(string defineSymbol)
        {
            foreach (var buildTargetGroup in BuildTargetGroups)
                Remove(defineSymbol, buildTargetGroup);
        }
    }
}
