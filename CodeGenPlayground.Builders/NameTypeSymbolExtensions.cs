using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGenPlayground.Builders;

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

internal static class NameTypeSymbolExtensions
{
    internal static string GetFullNamespace(this INamedTypeSymbol symbol)
    {
        return string.Join(".", symbol.ContainingNamespace.GetFullNamespaceArray());
    }

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

    internal static bool IsPartialClass(this INamedTypeSymbol symbol)
    {
        return symbol.DeclaringSyntaxReferences
            .Any(reference =>
            {
                var syntaxNode = reference.GetSyntax();
                if (syntaxNode is not ClassDeclarationSyntax classDeclarationSyntax)
                    return false;

                return classDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword));
            });
    }
}