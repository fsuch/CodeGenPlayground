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
                        Properties = modelProperties
                            .Select(BuilderDefinition.PropertyDefinition.FromPropertySymbol)
                            .ToArray()
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
                    var propertyType = GetPropertyType(property.Type);

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

                string GetPropertyType(BuilderDefinition.TypeDefinition type)
                {
                    if (type.IsArray)
                        return $"{GetPropertyType(type.TypeArguments.Single())}[]";

                    if (type.TypeArguments.Length > 0)
                        return $"{type.FullName}<{string.Join(", ", type.TypeArguments.Select(GetPropertyType))}>";

                    return type.FullName;
                }
            });
    }
}