using System.Linq;
using CodeGenPlayground.Builders.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGenPlayground.Builders;

[Generator]
public class BuilderGenerator : IIncrementalGenerator
{
    private const string BuilderClassSuffix = "Builder";
    private const string BuilderNamespace = "CodeGenPlayground.Builders";
    private const string BuilderClassName = "BuilderBase";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var valueProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax classDeclarationSyntax && classDeclarationSyntax.Identifier.Text.EndsWith(BuilderClassSuffix),
                (generatorSyntaxContext, _) =>
                {
                    var classDeclarationSyntax = (ClassDeclarationSyntax)generatorSyntaxContext.Node;
                    var builderClass = generatorSyntaxContext.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax)!;

                    if (!builderClass.IsPartialClass())
                        return null;

                    var builderBaseClass = builderClass.GetBaseClassSymbol(BuilderNamespace, BuilderClassName);
                    if (builderBaseClass == null)
                        return null;

                    var modelClass = (INamedTypeSymbol)builderBaseClass.TypeArguments[0];
                    var modelProperties = modelClass.GetPublicPropertiesWithSetMethod();

                    var builderDefinition = new BuilderDefinition
                    {
                        BuilderName = builderClass.Name,
                        BuilderNamespace = builderClass.GetFullNamespace(),
                        Properties = modelProperties
                            .Select(BuilderDefinition.PropertyDefinition.FromPropertySymbol)
                            .ToArray(),
                        Methods = builderClass.GetAllMethods()
                            .Select(BuilderDefinition.MethodDefinition.FromMethodSymbol).ToArray()
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
                    var methodName = $"With{property.Name}";
                    if (MethodWithDefinitionAlreadyExists(methodName, builderDefinition, property))
                        continue;

                    var propertyType = GetPropertyType(property.Type);

                    source += $$"""
                                public {{builderDefinition.BuilderName}} {{methodName}}({{propertyType}} value)
                                    {
                                        Instance.{{property.Name}} = value;
                                        return this;
                                    }
                                    
                                """;
                }

                source += "}";
                sourceProductionContext.AddSource($"{builderDefinition.BuilderName}.g.cs", source);
                return;

                static string GetPropertyType(BuilderDefinition.TypeDefinition type)
                {
                    if (type.IsArray)
                        return $"{GetPropertyType(type.TypeArguments.Single())}[]";

                    if (type.TypeArguments.Length > 0)
                        return $"{type.FullName}<{string.Join(", ", type.TypeArguments.Select(GetPropertyType))}>";

                    return type.FullName;
                }

                static bool MethodWithDefinitionAlreadyExists(string methodName, BuilderDefinition builderDefinition,
                    BuilderDefinition.PropertyDefinition property)
                {
                    return builderDefinition.Methods.Where(method => method.Name == methodName).Any(methodDefinition =>
                        methodDefinition.Parameters.Length == 1 &&
                        methodDefinition.Parameters[0].FullName == property.Type.FullName);
                }
            });
    }
}