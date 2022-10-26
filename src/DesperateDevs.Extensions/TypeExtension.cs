using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DesperateDevs.Extensions
{
    public static class TypeExtension
    {
        public static bool ImplementsInterface<T>(this Type type) =>
            !type.IsInterface && type.GetInterface(typeof(T).FullName) != null;

        /// Generates a simplified type string for the specified type that
        /// can be compiled. This is useful for code generation that will
        /// produce compilable source code.
        /// e.g. int instead of System.Int32
        /// e.g. System.Collections.Generic.Dictionary<int, string> instead of
        /// System.Collections.Generic.Dictionary`2[System.Int32,System.String]
        public static string ToCompilableString(this Type type)
        {
            if (BuiltInTypesToString.TryGetValue(type.FullName, out var fullName))
                return fullName;

            if (type.IsGenericType)
            {
                var genericArguments = type.GetGenericArguments()
                    .Select(argType => argType.ToCompilableString());
                return $"{type.FullName.Split('`')[0]}<{string.Join(", ", genericArguments)}>";
            }

            if (type.IsArray)
                return $"{type.GetElementType().ToCompilableString()}[{new string(',', type.GetArrayRank() - 1)}]";

            if (type.IsNested)
                return type.FullName.Replace('+', '.');

            return type.FullName;
        }

        /// Tries to find and create a type based on the specified type string.
        public static Type ToType(this string typeString)
        {
            var fullTypeName = GenerateTypeString(typeString);
            var type = Type.GetType(fullTypeName);
            if (type != null)
                return type;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(fullTypeName);
                if (type != null)
                    return type;
            }

            return null;
        }

        public static string ShortTypeName(this string fullTypeName)
        {
            var split = fullTypeName.Split('.');
            return split[split.Length - 1];
        }

        public static string RemoveDots(this string fullTypeName) =>
            fullTypeName.Replace(".", string.Empty);

        static string GenerateTypeString(string typeString)
        {
            if (BuiltInTypeStrings.TryGetValue(typeString, out var fullName))
            {
                typeString = fullName;
            }
            else
            {
                typeString = GenerateGenericArguments(typeString);
                typeString = GenerateArray(typeString);
            }

            return typeString;
        }

        static string GenerateGenericArguments(string typeString)
        {
            const string genericArgsPattern = @"<(?<arg>.*)>";
            var separator = new[] {", "};
            typeString = Regex.Replace(typeString, genericArgsPattern,
                match =>
                {
                    var ts = GenerateTypeString(match.Groups["arg"].Value);
                    var argsCount = ts.Split(separator, StringSplitOptions.None).Length;
                    return $"`{argsCount}[{ts}]";
                });

            return typeString;
        }

        static string GenerateArray(string typeString)
        {
            const string arrayPattern = @"(?<type>[^\[]*)(?<rank>\[,*\])";
            typeString = Regex.Replace(typeString, arrayPattern,
                match =>
                {
                    var type = GenerateTypeString(match.Groups["type"].Value);
                    var rank = match.Groups["rank"].Value;
                    return type + rank;
                });

            return typeString;
        }

        static readonly Dictionary<string, string> BuiltInTypesToString = new Dictionary<string, string>
        {
            {"System.Boolean", "bool"},
            {"System.Byte", "byte"},
            {"System.SByte", "sbyte"},
            {"System.Char", "char"},
            {"System.Decimal", "decimal"},
            {"System.Double", "double"},
            {"System.Single", "float"},
            {"System.Int32", "int"},
            {"System.UInt32", "uint"},
            {"System.Int64", "long"},
            {"System.UInt64", "ulong"},
            {"System.Object", "object"},
            {"System.Int16", "short"},
            {"System.UInt16", "ushort"},
            {"System.String", "string"},
            {"System.Void", "void"}
        };

        static readonly Dictionary<string, string> BuiltInTypeStrings = new Dictionary<string, string>
        {
            {"bool", "System.Boolean"},
            {"byte", "System.Byte"},
            {"sbyte", "System.SByte"},
            {"char", "System.Char"},
            {"decimal", "System.Decimal"},
            {"double", "System.Double"},
            {"float", "System.Single"},
            {"int", "System.Int32"},
            {"uint", "System.UInt32"},
            {"long", "System.Int64"},
            {"ulong", "System.UInt64"},
            {"object", "System.Object"},
            {"short", "System.Int16"},
            {"ushort", "System.UInt16"},
            {"string", "System.String"},
            {"void", "System.Void"}
        };
    }
}
