using System.Linq;
using CodeGenPlayground.Builders.Extensions;
using Microsoft.CodeAnalysis;

namespace CodeGenPlayground.Builders;

internal class BuilderDefinition
{
    public string BuilderName { get; set; }
    public string BuilderNamespace { get; set; }
    public PropertyDefinition[] Properties { get; set; }
    
    internal class PropertyDefinition
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public TypeDefinition Type { get; set; }

        internal static PropertyDefinition FromPropertySymbol(IPropertySymbol property)
        {
            return new PropertyDefinition
            {
                Name = property.Name,
                Namespace = property.GetFullNamespace(),
                Type = TypeDefinition.FromTypeSymbol(property.Type)
            };
        }
    }

    internal class TypeDefinition
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public bool IsNullable { get; set; }
        public TypeDefinition[] TypeArguments { get; set; }
        public string FullName => $"{Namespace}.{Name}";

        internal static TypeDefinition FromTypeSymbol(ITypeSymbol type)
        {
            return new TypeDefinition
            {
                Name = type.Name,
                Namespace = type.GetFullNamespace(),
                IsNullable = type.IsNullable(),
                TypeArguments = type.GetTypeArguments().Select(FromTypeSymbol).ToArray()
            };
        }
    }
}