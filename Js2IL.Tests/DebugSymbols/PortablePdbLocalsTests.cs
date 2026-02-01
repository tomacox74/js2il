using Js2IL.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;
using Xunit;

namespace Js2IL.Tests.DebugSymbols;

public class PortablePdbLocalsTests
{
    [Fact]
    public void CompilingWithEmitPdb_EmitsLocalVariableNames_ForUncapturedLocals()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "PortablePdbLocals", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputPath);

        var jsPath = Path.Combine(outputPath, "locals.js");
        var dllPath = Path.Combine(outputPath, "locals.dll");
        var pdbPath = Path.Combine(outputPath, "locals.pdb");

        // x is not captured here, so it should be stored in a local slot.
        var js = "\"use strict\";\n" +
                 "function test() {\n" +
                 "  var x = 5;\n" +
                 "  console.log(x);\n" +
                 "}\n" +
                 "test();\n";

        var mockFs = new MockFileSystem();
        mockFs.AddFile(jsPath, js);

        var options = new CompilerOptions
        {
            OutputDirectory = outputPath,
            EmitPdb = true
        };

        var logger = new TestLogger();
        var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFs, logger);
        var compiler = serviceProvider.GetRequiredService<Compiler>();

        Assert.True(compiler.Compile(jsPath), $"Compilation failed. Errors: {logger.Errors}\nWarnings: {logger.Warnings}");
        Assert.True(File.Exists(dllPath), $"Expected DLL at '{dllPath}'.");
        Assert.True(File.Exists(pdbPath), $"Expected PDB at '{pdbPath}'.");

        using var pdbStream = File.OpenRead(pdbPath);
        using var provider = MetadataReaderProvider.FromPortablePdbStream(pdbStream);
        var pdbReader = provider.GetMetadataReader();

        bool foundX = false;
        foreach (var scopeHandle in pdbReader.LocalScopes)
        {
            var scope = pdbReader.GetLocalScope(scopeHandle);
            foreach (var localHandle in scope.GetLocalVariables())
            {
                var local = pdbReader.GetLocalVariable(localHandle);
                var name = pdbReader.GetString(local.Name);
                if (string.Equals(name, "x", StringComparison.Ordinal))
                {
                    foundX = true;
                    break;
                }
            }

            if (foundX)
            {
                break;
            }
        }

        Assert.True(foundX, "Expected Portable PDB to include a LocalVariable named 'x' so the debugger can display it.");
    }
}
