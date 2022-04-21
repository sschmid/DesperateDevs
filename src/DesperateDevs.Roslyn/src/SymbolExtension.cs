using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DesperateDevs.Roslyn
{
    public static class SymbolExtension
    {
        public static AttributeData GetAttribute<T>(this ISymbol type, bool inherit = false) =>
            type.GetAttributes<T>(inherit).SingleOrDefault();

        public static AttributeData[] GetAttributes<T>(this ISymbol type, bool inherit = false) => type.GetAttributes()
            .Where(attr => IsAttributeType<T>(attr, inherit))
            .ToArray();

        public static ISymbol[] GetPublicMembers(this ITypeSymbol type, bool includeBaseTypeMembers)
        {
            var members = type.GetMembers()
                .Where(IsPublicMember)
                .ToArray();

            if (includeBaseTypeMembers)
                if (type.BaseType != null && type.BaseType.ToDisplayString() != "object")
                    members = members.Concat(GetPublicMembers(type.BaseType, true)).ToArray();

            return members;
        }

        public static ITypeSymbol PublicMemberType(this ISymbol member) => (member is IFieldSymbol)
            ? ((IFieldSymbol)member).Type
            : ((IPropertySymbol)member).Type;

        public static string ToCompilableString(this ISymbol symbol) => symbol.ToDisplayString()
            .Replace("*", string.Empty);

        static bool IsAttributeType<T>(AttributeData attr, bool inherit) => inherit
            ? attr.AttributeClass.BaseType.ToCompilableString() == typeof(T).FullName
            : attr.AttributeClass.ToCompilableString() == typeof(T).FullName;

        static bool IsPublicMember(ISymbol symbol) =>
            (symbol is IFieldSymbol || IsAutoProperty(symbol))
            && !symbol.IsStatic
            && symbol.DeclaredAccessibility == Accessibility.Public
            && symbol.CanBeReferencedByName;

        static bool IsAutoProperty(ISymbol symbol)
        {
            if (symbol is IPropertySymbol property)
            {
                return property.SetMethod != null
                       && property.GetMethod != null
                       && !property.GetMethod.DeclaringSyntaxReferences.First()
                           .GetSyntax()
                           .DescendantNodes()
                           .Any(node => node is MethodDeclarationSyntax)
                       && !property.SetMethod.DeclaringSyntaxReferences.First()
                           .GetSyntax()
                           .DescendantNodes()
                           .Any(node => node is MethodDeclarationSyntax);
            }
            else
            {
                return false;
            }
        }
    }
}
