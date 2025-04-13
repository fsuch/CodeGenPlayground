using System.Linq;
using Microsoft.CodeAnalysis;

namespace CodeGenPlayground.Builders.Extensions;

internal static class NameTypeSymbolExtensions
{
    internal static INamedTypeSymbol GetBuilderBaseClassSymbol(this INamedTypeSymbol symbol)
    {
        var currentType = symbol.BaseType;
        while (currentType != null)
        {
            if (currentType.Name == "BuilderBase" && currentType.GetFullNamespace() == "CodeGenPlayground.Builders")
                return currentType;
            currentType = currentType.BaseType;
        }

        return null;
    }


    internal static IPropertySymbol[] GetProperties(this INamedTypeSymbol symbol)
    {
        return symbol
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.DeclaredAccessibility == Accessibility.Public && !p.IsStatic)
            .ToArray();
    }
}