using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DesperateDevs.Extensions;

namespace DesperateDevs.Serialization
{
    // Properties File Format
    // https://docs.oracle.com/cd/E23095_01/Platform.93/ATGProgGuide/html/s0204propertiesfileformat01.html
    public class Properties
    {
        const string VariablePattern = @"\${(.+?)}";

        public IEnumerable<string> Keys => _dict.Keys;
        public IEnumerable<string> Values => _dict.Values;
        public int Count => _dict.Count;

        public string this[string key]
        {
            get => Regex.Replace(_dict[key], VariablePattern,
                match => _dict.TryGetValue(match.Groups[1].Value, out var reference)
                    ? reference
                    : match.Value);
            set => _dict[key] = UnescapedSpecialCharacters(value);
        }

        readonly Dictionary<string, string> _dict;
        readonly bool _doubleQuotedValues;

        public Properties(bool doubleQuotedValues = false) : this(string.Empty, doubleQuotedValues) { }

        public Properties(string properties, bool doubleQuotedValues = false)
        {
            properties = properties.ToUnixLineEndings();
            _doubleQuotedValues = doubleQuotedValues;
            _dict = new Dictionary<string, string>();
            var lines = GetLinesWithProperties(properties);
            AddProperties(MergeMultilineValues(lines));
        }

        void AddProperties(IEnumerable<string> lines)
        {
            var delimiter = new[] {'='};
            foreach (var line in lines)
            {
                var property = line.Split(delimiter, 2);
                if (property.Length != 2)
                    throw new InvalidPropertyException(property[0]);

                var value = property[1].Trim();
                this[property[0].TrimEnd()] = _doubleQuotedValues
                    ? value.Substring(1, value.Length - 2)
                    : value;
            }
        }

        public void AddProperties(Dictionary<string, string> properties, bool overwriteExisting)
        {
            foreach (var kvp in properties)
                if (overwriteExisting || !HasKey(kvp.Key))
                    this[kvp.Key] = kvp.Value;
        }

        public void RemoveProperty(string key) => _dict.Remove(key);
        public bool HasKey(string key) => _dict.ContainsKey(key);

        public Dictionary<string, string> ToDictionary() => new Dictionary<string, string>(_dict);

        public string ToMinifiedString()
        {
            var sb = new StringBuilder();
            foreach (var kvp in _dict)
            {
                var value = EscapedSpecialCharacters(kvp.Value);
                sb.AppendLine(kvp.Key + "=" + (_doubleQuotedValues ? $"\"{value}\"" : value));
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var kvp in _dict)
            {
                var values = EscapedSpecialCharacters(kvp.Value)
                    .FromCSV(false)
                    .Select(entry => entry.PadLeft(kvp.Key.Length + 3 + entry.Length));

                var value = string.Join(", \\\n", values).TrimStart();
                sb.AppendLine(kvp.Key + " = " + (_doubleQuotedValues ? $"\"{value}\"" : value));
            }

            return sb.ToString();
        }

        static IEnumerable<string> GetLinesWithProperties(string properties) => properties
            .Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.TrimStart(' '))
            .Where(line => !line.StartsWith("#", StringComparison.Ordinal));

        static IEnumerable<string> MergeMultilineValues(IEnumerable<string> lines)
        {
            var currentProperty = string.Empty;
            var values = new List<string>();
            foreach (var line in lines)
            {
                currentProperty += line;
                if (currentProperty.EndsWith("\\", StringComparison.Ordinal))
                {
                    currentProperty = currentProperty[..^1];
                }
                else
                {
                    values.Add(currentProperty);
                    currentProperty = string.Empty;
                }
            }

            return values;
        }

        static string EscapedSpecialCharacters(string str) => str
            .Replace("\n", "\\n")
            .Replace("\t", "\\t");

        static string UnescapedSpecialCharacters(string str) => str
            .Replace("\\n", "\n")
            .Replace("\\t", "\t");
    }

    public class InvalidPropertyException : Exception
    {
        public readonly string Key;

        public InvalidPropertyException(string key) : base("Invalid property: " + key)
        {
            Key = key;
        }
    }
}
