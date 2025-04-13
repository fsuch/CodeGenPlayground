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
}