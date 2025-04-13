using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharpExtensions;

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

                return classDeclarationSyntax.Modifiers.Any(modifier => CSharpExtensions.IsKind((SyntaxToken)modifier, SyntaxKind.PartialKeyword));
            });
    }
}