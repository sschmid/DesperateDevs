using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DesperateDevs.Roslyn {

    public static class SymbolExtension {

        public static AttributeData GetAttribute<T>(this ISymbol type, bool inherit = false) {
            return type.GetAttributes<T>(inherit).SingleOrDefault();
        }

        public static AttributeData[] GetAttributes<T>(this ISymbol type, bool inherit = false) {
            return type.GetAttributes()
                .Where(attr => isAttributeType<T>(attr, inherit))
                .ToArray();
        }

        public static ISymbol[] GetPublicMembers(this ITypeSymbol type, bool includeBaseTypeMembers) {
            var members = type.GetMembers()
                .Where(isPublicMember)
                .ToArray();

            if (includeBaseTypeMembers) {
                if (type.BaseType != null && type.BaseType.ToDisplayString() != "object") {
                    members = members.Concat(GetPublicMembers(type.BaseType, true)).ToArray();
                }
            }

            return members;
        }

        public static ITypeSymbol PublicMemberType(this ISymbol member) {
            return (member is IFieldSymbol)
                ? ((IFieldSymbol)member).Type
                : ((IPropertySymbol)member).Type;
        }

        public static string ToCompilableString(this ISymbol symbol) {
            return symbol.ToDisplayString()
                .Replace("*", string.Empty);
        }

        static bool isAttributeType<T>(AttributeData attr, bool inherit) {
            return inherit
                ? attr.AttributeClass.BaseType.ToCompilableString() == typeof(T).FullName
                : attr.AttributeClass.ToCompilableString() == typeof(T).FullName;
        }

        static bool isPublicMember(ISymbol symbol) {
            return (symbol is IFieldSymbol || IsAutoProperty(symbol))
                   && !symbol.IsStatic
                   && symbol.DeclaredAccessibility == Accessibility.Public
                   && symbol.CanBeReferencedByName;
        }

        static bool IsAutoProperty(ISymbol symbol) {
            var property = symbol as IPropertySymbol;
            if (property != null) {
                return property.SetMethod != null && property.GetMethod != null
                       && !property.GetMethod.DeclaringSyntaxReferences.First()
                           .GetSyntax()
                           .DescendantNodes()
                           .Any(node => node is MethodDeclarationSyntax)
                       && !property.SetMethod.DeclaringSyntaxReferences.First()
                           .GetSyntax()
                           .DescendantNodes()
                           .Any(node => node is MethodDeclarationSyntax);
            }

            return false;
        }
    }
}
