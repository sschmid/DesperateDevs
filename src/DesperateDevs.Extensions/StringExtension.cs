using System;
using System.Collections.Generic;
using System.Linq;

namespace DesperateDevs.Extensions
{
    public static class StringExtension
    {
        public static string ToUpperFirst(this string str) => string.IsNullOrEmpty(str)
            ? str
            : char.ToUpper(str[0]) + str.Substring(1);

        public static string ToLowerFirst(this string str) => string.IsNullOrEmpty(str)
            ? str
            : char.ToLower(str[0]) + str.Substring(1);

        public static string ToUnixLineEndings(this string str) => str
            .Replace("\r\n", "\n")
            .Replace("\r", "\n");

        public static string ToUnixPath(this string str) => str.Replace("\\", "/");

        public static string ToCSV(this IEnumerable<string> values, bool minified, bool removeEmptyEntries) =>
            string.Join(minified ? "," : ", ", removeEmptyEntries
                ? values
                    .Where(value => !string.IsNullOrEmpty(value))
                    .Select(value => value.Trim())
                : values
                    .Select(value => value.Trim()));

        public static IEnumerable<string> FromCSV(this string values, bool removeEmptyEntries) => values
            .Split(new[] {','}, removeEmptyEntries
                ? StringSplitOptions.RemoveEmptyEntries
                : StringSplitOptions.None)
            .Select(value => value.Trim());

        public static string ToSpacedCamelCase(this string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var sb = new System.Text.StringBuilder(text.Length * 2);
                sb.Append(char.ToUpper(text[0]));
                for (var i = 1; i < text.Length; i++)
                {
                    if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                        sb.Append(' ');

                    sb.Append(text[i]);
                }

                return sb.ToString();
            }
            else
            {
                return text;
            }
        }

        public static string MakePathRelativeTo(this string path, string currentDirectory)
        {
            currentDirectory = CreateUri(currentDirectory);
            path = CreateUri(path);
            if (path.StartsWith(currentDirectory))
            {
                path = path.Replace(currentDirectory, string.Empty);
                if (path.StartsWith("/"))
                    path = path.Substring(1);
            }

            return path;
        }

        public static string CreateUri(this string path)
        {
            var uri = new Uri(path);
            return Uri.UnescapeDataString(uri.AbsolutePath + uri.Fragment);
        }
    }
}
