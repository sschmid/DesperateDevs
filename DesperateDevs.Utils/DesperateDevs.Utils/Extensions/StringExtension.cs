using System;
using System.Linq;

namespace DesperateDevs.Utils {

    public static class StringExtension {

        public static string UppercaseFirst(this string str) {
            if (string.IsNullOrEmpty(str)) {
                return str;
            }

            return char.ToUpper(str[0]) + str.Substring(1);
        }

        public static string LowercaseFirst(this string str) {
            if (string.IsNullOrEmpty(str)) {
                return str;
            }

            return char.ToLower(str[0]) + str.Substring(1);
        }

        public static string ToUnixLineEndings(this string str) {
            return str.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        public static string ToUnixPath(this string str) {
            return str.Replace("\\", "/");
        }

        public static string ToCSV(this string[] values) {
            return string.Join(", ", values
                .Where(value => !string.IsNullOrEmpty(value))
                .Select(value => value.Trim())
                .ToArray()
            );
        }

        public static string[] ArrayFromCSV(this string values) {
            return values
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim())
                .ToArray();
        }

        public static string ToSpacedCamelCase(this string text) {
            var sb = new System.Text.StringBuilder(text.Length * 2);
            sb.Append(char.ToUpper(text[0]));
            for (int i = 1; i < text.Length; i++) {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ') {
                    sb.Append(' ');
                }

                sb.Append(text[i]);
            }

            return sb.ToString();
        }

        public static string MakePathRelativeTo(this string path, string currentDirectory) {
            currentDirectory = createUri(currentDirectory);
            path = createUri(path);
            if (path.StartsWith(currentDirectory)) {
                path = path.Replace(currentDirectory, string.Empty);
                if (path.StartsWith("/")) {
                    path = path.Substring(1);
                }
            }

            return path;
        }

        static string createUri(string path) {
            var uri = new Uri(path);
            return Uri.UnescapeDataString(uri.AbsolutePath + uri.Fragment);
        }
    }
}
