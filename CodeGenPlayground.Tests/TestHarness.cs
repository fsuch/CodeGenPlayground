using CodeGenPlayground.Builders;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;

namespace CodeGenPlayground.Tests;

public class TestHarness
{
    [Fact]
    public async Task Run()
    {
        var generator = new BuilderGenerator();

        var workspace = MSBuildWorkspace.Create();
        var project = await workspace.OpenProjectAsync(@"D:\repos\CodeGenPlayground\CodeGenPlayground\CodeGenPlayground.csproj");
        var compilation = await project.GetCompilationAsync();

        var driver = CSharpGeneratorDriver.Create(generator)
            .RunGeneratorsAndUpdateCompilation(compilation!, out _, out var _);

        // Verify the generated code
        var result = driver.GetRunResult();
    }
}