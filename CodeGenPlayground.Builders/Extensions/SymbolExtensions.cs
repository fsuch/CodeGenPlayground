using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGenPlayground.Builders.Extensions;

internal static class SymbolExtensions
{
    internal static string GetFullNamespace(this ISymbol symbol)
    {
        return string.Join(".", symbol.ContainingNamespace.GetFullNamespaceArray());
    }

    internal static bool IsPartialClass(this ISymbol symbol)
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


    internal static ITypeSymbol GetElementType(this ISymbol symbol)
    {
        if (symbol is not INamedTypeSymbol namedTypeSymbol)
            return null;

        return namedTypeSymbol.TypeArguments.Length == 0 ? null : namedTypeSymbol.TypeArguments[0];
    }
}