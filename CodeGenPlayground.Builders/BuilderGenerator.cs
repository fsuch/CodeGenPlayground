using System.Linq;
using CodeGenPlayground.Builders.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGenPlayground.Builders;

[Generator]
public class BuilderGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var valueProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax,
                (generatorSyntaxContext, _) =>
                {
                    var classDeclarationSyntax = (ClassDeclarationSyntax)generatorSyntaxContext.Node;
                    var builderClass = generatorSyntaxContext.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax)!;

                    if (!builderClass.IsPartialClass())
                        return null;

                    var builderBaseClass = builderClass.GetBuilderBaseClassSymbol();
                    if (builderBaseClass == null)
                        return null;

                    var modelClass = (INamedTypeSymbol)builderBaseClass.TypeArguments[0];
                    var modelProperties = modelClass.GetProperties();


                    var builderDefinition = new BuilderDefinition
                    {
                        BuilderName = builderClass.Name,
                        BuilderNamespace = builderClass.GetFullNamespace(),
                        Properties = modelProperties.Select(property =>
                        {
                            var typeIsNullable = property.Type.IsNullable();
                            var propertyType = typeIsNullable ? property.Type.GetElementType() : property.Type;
                            var propertyDefinition = new BuilderDefinition.PropertyDefinition
                            {
                                Name = property.Name,
                                Namespace = property.GetFullNamespace(),
                                Type = propertyType.Name,
                                TypeNamespace = propertyType.GetFullNamespace(),
                                TypeIsNullable = typeIsNullable
                            };
                            return propertyDefinition;
                        }).ToArray()
                    };
                    return builderDefinition;
                }
            ).Where(x => x != null);

        context.RegisterSourceOutput(valueProvider,
            static (sourceProductionContext, builderDefinition) =>
            {
                var source =
                    $$"""
                      namespace {{builderDefinition.BuilderNamespace}};
                      public partial class {{builderDefinition.BuilderName}} {

                      """;
                foreach (var property in builderDefinition.Properties.OrderBy(property => property.Name))
                {
                    var propertyType = GetPropertyType(property);

                    source += $$"""
                                public {{builderDefinition.BuilderName}} With{{property.Name}}({{propertyType}} value)
                                    {
                                        Instance.{{property.Name}} = value;
                                        return this;
                                    }
                                    
                                """;
                }

                source += "}";
                sourceProductionContext.AddSource($"{builderDefinition.BuilderName}.g.cs", source);
                return;

                string GetPropertyType(BuilderDefinition.PropertyDefinition property)
                {
                    return $"{property.TypeNamespace}.{property.Type}{(property.TypeIsNullable ? "?" : "")}";
                }
            });
    }
}