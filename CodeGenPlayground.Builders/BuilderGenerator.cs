using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CodeGenPlayground.Builders;

[Generator]
public class BuilderGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
            // var compilationProvider = context.CompilationProvider;

        IncrementalValuesProvider<BuilderDefinition> valueProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
            predicate: static (node, _) => node is ClassDeclarationSyntax,
            transform: (generatorSyntaxContext, _) =>
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
                        var propertyDefinition = new BuilderDefinition.PropertyDefinition
                        {
                            Name = property.Name,
                            Namespace = property.GetFullNamespace(),
                            Type = property.Type.Name,
                            TypeNamespace = property.Type.GetFullNamespace()
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
                    var propertyType = $"{property.TypeNamespace}.{property.Type}";
                    
                    source += $$"""
                              public {{builderDefinition.BuilderName}} With{{property.Name}}({{propertyType}} value)
                                  {
                                      Instance.{{property.Name}} = value;
                                      return this;
                                  }
                                  
                              """;
                }
                
            source += "    }";
              sourceProductionContext.AddSource($"{builderDefinition.BuilderName}.g.cs", source);
            });
            
        // IncrementalValueProvider<BuilderDefinition> settings = context
        //     .CompilationProvider
        //     .Select((c, _) =>
        //     {
        //         var builderBaseSymbol = (INamedTypeSymbol)c.GetSymbolsWithName("BuilderBase").SingleOrDefault();
        //         if (builderBaseSymbol == null)
        //             return null;  
        //                 
        //         foreach (var symbol in c.GetSymbolsWithName("CustomerBuilder"))
        //         {
        //             if (symbol is not INamedTypeSymbol namedTypeSymbol)
        //                 continue;
        //             
        //             if (namedTypeSymbol.IsAbstract)
        //                 continue;
        //
        //             var isBuilder = false;
        //             var currentType = namedTypeSymbol.BaseType;
        //             while (currentType != null)
        //             {
        //                 if (SymbolEqualityComparer.Default.Equals(currentType.OriginalDefinition, builderBaseSymbol))
        //                 {
        //                     isBuilder = true;
        //                 }
        //                 currentType = currentType.BaseType;
        //             }
        //
        //             var isPartial = namedTypeSymbol.DeclaringSyntaxReferences
        //                 .Any(reference =>
        //                 {
        //                     var syntaxNode = reference.GetSyntax();
        //                     if (syntaxNode is not Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax
        //                         classDeclarationSyntax)
        //                         return false;
        //                     return classDeclarationSyntax.Modifiers.Any(modifier =>
        //                         modifier.IsKind(SyntaxKind.PartialKeyword));
        //                 });
        //
        //             if (!isPartial)
        //                 continue;
        //         }
        //         
        //         // Grab the values from Compilation and CompilationOptions
        //         return new BuilderDefinition();
        //     });
        
//         // Generate the source from the captured values
//          context.RegisterSourceOutput(settings, (sourceProductionContext, opts) =>
//          {
//              var  source = 
//                  $$"""
//  
//                    """;
//              sourceProductionContext.AddSource("CustomerBuilder.g.cs", source);
//          });

        context.RegisterSourceOutput(
            context.CompilationProvider,
            (sourceProductionContext, compilation) =>
            {
                var descriptor = new DiagnosticDescriptor(
                    "GEN001", "Generator Ran", "The generator executed successfully", "SourceGenerator",
                    DiagnosticSeverity.Warning, true);
                sourceProductionContext.ReportDiagnostic(Diagnostic.Create(descriptor, Location.None));
            });

        context.RegisterSourceOutput(
            context.CompilationProvider,
            (sourceProductionContext, compilation) =>
            {
                sourceProductionContext.AddSource("GeneratorRan.g.cs", """
                                                                       // Generator ran!
                                                                       namespace GeneratorDebug {
                                                                           public static class Marker {
                                                                               public const string Message = "IIncrementalGenerator ran!";
                                                                           }
                                                                       }
                                                                       """);
            });
    }
}