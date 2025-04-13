using Microsoft.CodeAnalysis;

namespace CodeGenPlayground.Builders.Extensions;

internal static class NamespaceSymbolExtensions
{
    internal static bool IsNullOrGlobalNamespace(this INamespaceSymbol symbol)
    {
        return symbol == null || symbol.IsGlobalNamespace;
    }

    internal static string[] GetFullNamespaceArray(this INamespaceSymbol symbol)
    {
        if (symbol.IsNullOrGlobalNamespace())
            return [];

        return [.. GetFullNamespaceArray(symbol.ContainingNamespace), symbol.Name];
    }
}