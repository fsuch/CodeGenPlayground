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
            // {
            //     if (node is ClassDeclarationSyntax classDeclarationSyntax)
            //     {
            //         context.Seman
            //         var isBuilder = classDeclarationSyntax.BaseList;
            //         return false;
            //     }
            //     return false;
            // },
            transform: (generatorSyntaxContext, _) =>
            {
                var classDeclarationSyntax = (ClassDeclarationSyntax)generatorSyntaxContext.Node;
                var classSymbol = generatorSyntaxContext.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax)!;

                var baseBuilderSymbol = classSymbol.GetBuilderBaseClassSymbol();
                if (baseBuilderSymbol == null)
                    return null;
                
                var isPartial = classSymbol.DeclaringSyntaxReferences
                    .Any(reference =>
                    {
                        var syntaxNode = reference.GetSyntax();
                        if (syntaxNode is not ClassDeclarationSyntax)
                            return false;

                        return classDeclarationSyntax.Modifiers.Any(modifier =>
                            modifier.IsKind(SyntaxKind.PartialKeyword));
                    });

                if (!isPartial)
                    return null;


                var builderDefinition = new BuilderDefinition
                {
                    BuilderName = classSymbol.Name,
                    BuilderNamespace = classSymbol.GetFullNamespace()
                };  
                return builderDefinition;
                // var a = context.CompilationProvider.Select((c, _) =>
                // {
                //     return new BuilderDefinition();
                // });
                return new BuilderDefinition();
            }
        ).Where(x => x != null);
        
        context.RegisterSourceOutput(valueProvider,
            static (sourceProductionContext, builderDefinition) =>
            {
                              var  source = 
                  $$"""
                    namespace {{builderDefinition.BuilderNamespace}} {
                         public partial class {{builderDefinition.BuilderName}} {
                             public string Foo() => "This is a test";
                         }
                     }
                    """;
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