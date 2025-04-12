using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace CodeGenPlayground.Builders;

[Generator]
public class BuilderGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
            // var compilationProvider = context.CompilationProvider;

        // IncrementalValueProvider<(Platform Platform, OptimizationLevel OptimizationLevel, string AssemblyName)> settings = context
        //     .CompilationProvider
        //     .Select((c, _) =>
        //     {
        //         var builderBaseSymbol = (INamedTypeSymbol)c.GetSymbolsWithName("BuilderBase").SingleOrDefault();
        //         if (builderBaseSymbol == null)
        //             return (Platform.Arm, OptimizationLevel.Debug, "foo");  
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
        //         return (c.Options.Platform, c.Options.OptimizationLevel, c.AssemblyName);
        //     });
        
        // Generate the source from the captured values
//         context.RegisterSourceOutput(settings, static (spc, opts) =>
//         {
//             var  source = 
//                 $$"""
//                   // Platform: {{opts.Platform}}
//                   // Configuration: {{opts.OptimizationLevel}}
//                   // AssemblyName: {{opts.AssemblyName}}
//                   """;
//             System.IO.File.AppendAllText(@"D:\temp\sourcegen", $"Here at {DateTime.Now}");
//             spc.AddSource("Example.g.cs", source);
//         });

        context.RegisterSourceOutput(
            context.CompilationProvider,
            (sourceProductionContext, compilation) =>
            {
                var descriptor = new DiagnosticDescriptor(
                    "GEN001", "Generator Ran", "The generator executed successfully", "SourceGenerator",
                    DiagnosticSeverity.Error, true);
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