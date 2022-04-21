using System.Collections.Generic;
using System.Text.RegularExpressions;
using DesperateDevs.Extensions;

namespace DesperateDevs.CodeGeneration
{
    public class CodeGeneratorData : Dictionary<string, object>
    {
        const string VariablePattern = @"\${(.+?)}";

        public CodeGeneratorData() { }

        public CodeGeneratorData(CodeGeneratorData data) : base(data) { }

        public string ReplacePlaceholders(string template) => Regex.Replace(template, VariablePattern,
            match =>
            {
                var split = match.Groups[1].Value.Split(':');
                var key = split[0];
                if (TryGetValue(key, out var value))
                {
                    if (split.Length == 1)
                    {
                        return value.ToString();
                    }
                    else
                    {
                        switch (split[1])
                        {
                            case "lower": return value.ToString().ToLower();
                            case "upper": return value.ToString().ToUpper();
                            case "lowerFirst": return value.ToString().LowercaseFirst();
                            case "upperFirst": return value.ToString().UppercaseFirst();
                            case "foreach": return ForEach((IEnumerable<object>)value, split[2]);
                            default: return value.ToString();
                        }
                    }
                }
                else
                {
                    return match.Value;
                }
            });

        string ForEach(IEnumerable<object> values, string template)
        {
            var result = string.Empty;
            foreach (var value in values)
                result += template
                    .Replace("$item", value.ToString())
                    .Replace("\\n", "\n");

            return result;
        }
    }
}
