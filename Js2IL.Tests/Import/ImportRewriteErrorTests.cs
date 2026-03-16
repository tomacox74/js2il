using Js2IL.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
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

    [Fact]
    public void Import_MissingNamedExport_ShouldFailLinkPhase()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => Compile("""
"use strict";
import { missing } from "./lib.mjs";
console.log(missing);
""", libSource: """
"use strict";
export const present = 1;
"""));

        Assert.Contains("does not export 'missing'", ex.Message);
    }

    [Fact]
    public void Import_AmbiguousStarReExport_ShouldFailLinkPhase()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => Compile(
            """
"use strict";
import { shared } from "./lib.mjs";
console.log(shared);
""",
            libSource: """
"use strict";
export * from "./a.mjs";
export * from "./b.mjs";
""",
            additionalFiles: new Dictionary<string, string>
            {
                ["a.mjs"] = "\"use strict\";\nexport const shared = 'a';\n",
                ["b.mjs"] = "\"use strict\";\nexport const shared = 'b';\n"
            }));

        Assert.Contains("exports 'shared' ambiguously", ex.Message);
    }

    private static void Compile(string entrySource, string libSource, Dictionary<string, string>? additionalFiles = null)
    {
        var root = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "ImportRewriteErrors", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);

        var entryPath = Path.Combine(root, "main.mjs");
        var libPath = Path.Combine(root, "lib.mjs");

        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile(entryPath, entrySource, null);
        mockFileSystem.AddFile(libPath, libSource, null);
        if (additionalFiles != null)
        {
            foreach (var (relativePath, source) in additionalFiles)
            {
                var absolutePath = Path.Combine(root, relativePath);
                var absoluteDirectory = Path.GetDirectoryName(absolutePath);
                if (!string.IsNullOrWhiteSpace(absoluteDirectory))
                {
                    Directory.CreateDirectory(absoluteDirectory);
                }

                mockFileSystem.AddFile(absolutePath, source, null);
            }
        }

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
