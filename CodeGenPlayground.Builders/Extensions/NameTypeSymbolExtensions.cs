using Microsoft.CodeAnalysis;

namespace CodeGenPlayground.Builders.Extensions;

internal static class NameTypeSymbolExtensions
{
    internal static INamedTypeSymbol GetBaseClassSymbol(this INamedTypeSymbol symbol, string namespaceName, string className)
    {
        var currentType = symbol.BaseType;
        while (currentType != null)
        {
            if (currentType.Name == className && currentType.GetFullNamespace() == namespaceName)
                return currentType;
            currentType = currentType.BaseType;
        }

        return null;
    }
}