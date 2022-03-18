using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DesperateDevs.Extensions;

namespace DesperateDevs.Serialization {

    public class Properties {

        public string[] keys { get { return _dict.Keys.ToArray(); } }
        public string[] values { get { return _dict.Values.ToArray(); } }

        public int count { get { return _dict.Count; } }

        const string placeholderPattern = @"\${(.+?)}";

        public bool doubleQuoteMode {
            get { return _isDoubleQuoteMode; }
            set {
                _isDoubleQuoteMode = value;
                foreach (var key in _dict.Keys.ToArray()) {
                    this[key] = this[key];
                }
            }
        }

        bool _isDoubleQuoteMode;

        public string this[string key] {
            get {
                var value = Regex.Replace(
                    _dict[key],
                    placeholderPattern,
                    match => {
                        var matchValue = match.Groups[1].Value;
                        return _dict.ContainsKey(matchValue)
                            ? _dict[matchValue]
                            : "${" + matchValue + "}";
                    });

                return _isDoubleQuoteMode
                    ? removeDoubleQuotes(value)
                    : value;
            }
            set {
                var unescaped = unescapedSpecialCharacters(value.Trim());
                _dict[key.Trim()] = _isDoubleQuoteMode
                    ? addDoubleQuotes(unescaped)
                    : unescaped;
            }
        }

        readonly Dictionary<string, string> _dict;

        public Properties() : this(string.Empty) {
        }

        public Properties(string properties) {
            properties = properties.ToUnixLineEndings();
            _dict = new Dictionary<string, string>();
            var lines = getLinesWithProperties(properties);
            addProperties(mergeMultilineValues(lines));
        }

        public Properties(Dictionary<string, string> properties) {
            _dict = new Dictionary<string, string>(properties);
        }

        public bool HasKey(string key) {
            return _dict.ContainsKey(key);
        }

        public void AddProperties(Dictionary<string, string> properties, bool overwriteExisting) {
            foreach (var kv in properties) {
                if (overwriteExisting || !HasKey(kv.Key)) {
                    this[kv.Key] = kv.Value;
                }
            }
        }

        public void RemoveProperty(string key) {
            _dict.Remove(key);
        }

        public Dictionary<string, string> ToDictionary() {
            return new Dictionary<string, string>(_dict);
        }

        void addProperties(string[] lines) {
            var keyValueDelimiter = new[] { '=' };
            var properties = lines.Select(
                line => line.Split(keyValueDelimiter, 2)
            );
            foreach (var property in properties) {
                if (property.Length != 2) {
                    throw new InvalidKeyPropertiesException(property[0]);
                }

                this[property[0]] = property[1];
            }
        }

        static string[] getLinesWithProperties(string properties) {
            var delimiter = new[] { '\n' };
            return properties
                .Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.TrimStart(' '))
                .Where(line => !line.StartsWith("#", StringComparison.Ordinal))
                .ToArray();
        }

        static string[] mergeMultilineValues(string[] lines) {
            var currentProperty = string.Empty;
            return lines.Aggregate(new List<string>(), (acc, line) => {
                currentProperty += line;
                if (currentProperty.EndsWith("\\", StringComparison.Ordinal)) {
                    currentProperty = currentProperty.Substring(
                        0, currentProperty.Length - 1
                    );
                } else {
                    acc.Add(currentProperty);
                    currentProperty = string.Empty;
                }

                return acc;
            }).ToArray();
        }

        static string escapedSpecialCharacters(string str) {
            return str
                .Replace("\n", "\\n")
                .Replace("\t", "\\t");
        }

        static string unescapedSpecialCharacters(string str) {
            return str
                .Replace("\\n", "\n")
                .Replace("\\t", "\t");
        }

        static string addDoubleQuotes(string str) {
            return isInDoubleQuotes(str)
                ? str
                : "\"" + str + "\"";
        }

        static string removeDoubleQuotes(string str) {
            return isInDoubleQuotes(str)
                ? str.Substring(1, str.Length - 2)
                : str;
        }

        static bool isInDoubleQuotes(string str) {
            return str.StartsWith("\"") && str.EndsWith("\"");
        }

        static string propertyPair(string key, string value, bool minified) {
            if (minified) {
                return key + "=" + value;
            }

            return key + " = " + value;
        }

        public override string ToString() {
            return _dict.Aggregate(string.Empty, (properties, kv) => {
                var contentValues = escapedSpecialCharacters(kv.Value)
                    .FromCSV()
                    .Select(value => value.PadLeft(kv.Key.Length + 3 + value.Length))
                    .ToArray();

                var content = string.Join(", \\\n", contentValues).TrimStart();

                return properties + propertyPair(kv.Key, content, false) + (contentValues.Length > 1 ? "\n\n" : "\n");
            });
        }

        public string ToMinifiedString() {
            return _dict.Aggregate(string.Empty, (properties, kv) => {
                var content = escapedSpecialCharacters(kv.Value);
                return properties + propertyPair(kv.Key, content, true) + "\n";
            });
        }
    }

    public class InvalidKeyPropertiesException : Exception {

        public readonly string key;

        public InvalidKeyPropertiesException(string key) : base("Invalid key: " + key) {
            this.key = key;
        }
    }
}
