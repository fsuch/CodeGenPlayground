using Microsoft.CodeAnalysis;

namespace CodeGenPlayground.Tests;

[Generator]
public class BuilderGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
            var compilationProvider = context.CompilationProvider;

        IncrementalValueProvider<(Platform Platform, OptimizationLevel OptimizationLevel, string? AssemblyName)> settings = context
            .CompilationProvider
            .Select((Compilation c, CancellationToken _) =>
            {
                var builderBaseSymbol = (INamedTypeSymbol)c.GetSymbolsWithName("BuilderBase").Single();
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
                    
                    
                    
                }
                
                // Grab the values from Compilation and CompilationOptions
                return (c.Options.Platform, c.Options.OptimizationLevel, c.AssemblyName);
            });
        
        // Generate the source from the captured values
        context.RegisterSourceOutput(settings, static (spc, opts) =>
        {
            var  source = 
                $$"""
                  // Platform: {{opts.Platform}}
                  // Configuration: {{opts.OptimizationLevel}}
                  // AssemblyName: {{opts.AssemblyName}}
                  """;

            spc.AddSource("Example.g.cs", source);
        });
    }
}