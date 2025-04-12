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

public class BuilderDefinition
{
    public string PropertyName;
}


[Generator]
public class BuilderGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
            // var compilationProvider = context.CompilationProvider;

        IncrementalValuesProvider<BuilderDefinition> valueProvider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) =>
            {
                if (node is ClassDeclarationSyntax classDeclarationSyntax)
                {
                    return false;
                }
                return false;
            },
            transform: (context, _) =>
            {
                return new BuilderDefinition();
            }
        );
        
        context.RegisterSourceOutput(valueProvider,
            static (spc, enumToGenerate) =>
            {
                
            });
            
        IncrementalValueProvider<BuilderDefinition> settings = context
            .CompilationProvider
            .Select((c, _) =>
            {
                var builderBaseSymbol = (INamedTypeSymbol)c.GetSymbolsWithName("BuilderBase").SingleOrDefault();
                if (builderBaseSymbol == null)
                    return null;  
                        
                foreach (var symbol in c.GetSymbolsWithName("CustomerBuilder"))
                {
                    if (symbol is not INamedTypeSymbol namedTypeSymbol)
                        continue;
                    
                    if (namedTypeSymbol.IsAbstract)
                        continue;
        
                    var isBuilder = false;
                    var currentType = namedTypeSymbol.BaseType;
                    while (currentType != null)
                    {
                        if (SymbolEqualityComparer.Default.Equals(currentType.OriginalDefinition, builderBaseSymbol))
                        {
                            isBuilder = true;
                        }
                        currentType = currentType.BaseType;
                    }
        
                    var isPartial = namedTypeSymbol.DeclaringSyntaxReferences
                        .Any(reference =>
                        {
                            var syntaxNode = reference.GetSyntax();
                            if (syntaxNode is not Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax
                                classDeclarationSyntax)
                                return false;
                            return classDeclarationSyntax.Modifiers.Any(modifier =>
                                modifier.IsKind(SyntaxKind.PartialKeyword));
                        });
        
                    if (!isPartial)
                        continue;
                }
                
                // Grab the values from Compilation and CompilationOptions
                return new BuilderDefinition();
            });
        
        // Generate the source from the captured values
         context.RegisterSourceOutput(settings, (sourceProductionContext, opts) =>
         {
             var  source = 
                 $$"""
                   namespace CodeGenPlayground {
                       public partial class CustomerBuilder {
                           public string Foo() => "This is a test";
                       }
                   }
                   """;
             sourceProductionContext.AddSource("CustomerBuilder.g.cs", source);
         });

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