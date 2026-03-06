using Js2IL.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Xunit;

namespace Js2IL.Tests.Import;

public sealed class ImportRewriteErrorTests
{
    [Fact]
    public void Import_WriteViaDestructuringAssignment_ShouldFailCompilation()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => Compile("""
import { x } from "./lib.mjs";
({ x } = { x: 1 });
console.log(x);
""", libSource: "export let x = 0;\n"));

        Assert.Contains("Cannot assign to import binding 'x'", ex.Message);
    }

    [Fact]
    public void Import_DeleteIdentifier_ShouldFailCompilation()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => Compile("""
import { x } from "./lib.mjs";
delete x;
console.log(x);
""", libSource: "export let x = 1;\n"));

        Assert.Contains("Cannot delete import binding 'x'", ex.Message);
    }

    private static void Compile(string entrySource, string libSource)
    {
        var root = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "ImportRewriteErrors", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);

        var entryPath = Path.Combine(root, "main.mjs");
        var libPath = Path.Combine(root, "lib.mjs");

        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile(entryPath, entrySource, null);
        mockFileSystem.AddFile(libPath, libSource, null);

        var options = new CompilerOptions
        {
            OutputDirectory = root,
            EmitPdb = false
        };

        var testLogger = new TestLogger();
        var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFileSystem, testLogger);
        var compiler = serviceProvider.GetRequiredService<Compiler>();

        if (!compiler.Compile(entryPath))
        {
            var details = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(testLogger.Errors))
            {
                details.AppendLine("Errors:");
                details.AppendLine(testLogger.Errors);
            }
            if (!string.IsNullOrWhiteSpace(testLogger.Warnings))
            {
                details.AppendLine("Warnings:");
                details.AppendLine(testLogger.Warnings);
            }

            throw new InvalidOperationException("Compilation failed.\n" + details);
        }

        throw new InvalidOperationException("Compilation unexpectedly succeeded.");
    }
}
