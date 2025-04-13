using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace CodeGenPlayground.Builders.Extensions;

internal static class TypeSymbolExtensions
{
    internal static bool IsNullable(this ITypeSymbol typeSymbol)
    {
        // Nullable reference types (e.g., string?)
        if (typeSymbol.IsReferenceType && typeSymbol.NullableAnnotation == NullableAnnotation.Annotated)
            return true;

        // Nullable value types (e.g., int?)
        if (typeSymbol is INamedTypeSymbol
            {
                IsGenericType: true, ConstructedFrom.SpecialType: SpecialType.System_Nullable_T
            })
            return true;

        return false;
    }

    internal static bool IsEnumerable(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol == null)
            return false;

        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
            return false;

        if (namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_String)
            return false;

        if (namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Collections_IEnumerable)
            return true;

        return typeSymbol.AllInterfaces.Any(i => i.IsEnumerable());
    }

    internal static ImmutableArray<ITypeSymbol> GetTypeArguments(this ITypeSymbol typeSymbol)
    {
        return typeSymbol is INamedTypeSymbol namedTypeSymbol ? namedTypeSymbol.TypeArguments : [];
    }

    internal static bool IsArray(this ITypeSymbol typeSymbol)
    {
        return typeSymbol is IArrayTypeSymbol;
    }

    internal static IPropertySymbol[] GetPublicPropertiesWithSetMethod(this ITypeSymbol symbol)
    {
        return symbol
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.DeclaredAccessibility == Accessibility.Public && !p.IsStatic && p.SetMethod != null)
            .ToArray();
    }

    internal static IMethodSymbol[] GetAllMethods(this ITypeSymbol symbol)
    {
        return symbol
            .GetMembers()
            .OfType<IMethodSymbol>()
            .Where(p => p.DeclaredAccessibility == Accessibility.Public && !p.IsStatic)
            .ToArray();
    }
}