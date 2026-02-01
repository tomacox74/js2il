using Js2IL.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Xunit;

namespace Js2IL.Tests.DebugSymbols;

public class PortablePdbSmokeTests
{
    [Fact]
    public void CompilingWithEmitPdb_ProducesReadablePdb_AndCodeViewEntry()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "PortablePdbSmoke", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputPath);

        var jsPath = Path.Combine(outputPath, "smoke.js");
        var dllPath = Path.Combine(outputPath, "smoke.dll");
        var pdbPath = Path.Combine(outputPath, "smoke.pdb");

        var mockFs = new MockFileSystem();
        mockFs.AddFile(jsPath, "\"use strict\";\nconsole.log('hi');\n");

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

        // Verify PE has a CodeView debug directory entry.
        using (var peStream = File.OpenRead(dllPath))
        using (var peReader = new PEReader(peStream))
        {
            var debugDirectory = peReader.ReadDebugDirectory();
            Assert.Contains(debugDirectory, e => e.Type == DebugDirectoryEntryType.CodeView);

            var codeViewEntry = debugDirectory.First(e => e.Type == DebugDirectoryEntryType.CodeView);
            var codeView = peReader.ReadCodeViewDebugDirectoryData(codeViewEntry);
            Assert.EndsWith(".pdb", codeView.Path, StringComparison.OrdinalIgnoreCase);
        }

        // Verify Portable PDB is readable and has method debug information rows.
        using (var pdbStream = File.OpenRead(pdbPath))
        using (var provider = MetadataReaderProvider.FromPortablePdbStream(pdbStream))
        {
            var pdbReader = provider.GetMetadataReader();

            Assert.True(pdbReader.Documents.Any(), "Expected at least one document row in the PDB.");
            Assert.True(pdbReader.MethodDebugInformation.Any(), "Expected MethodDebugInformation rows in the PDB.");

            bool foundSequencePoints = false;
            foreach (var mdiHandle in pdbReader.MethodDebugInformation)
            {
                var mdi = pdbReader.GetMethodDebugInformation(mdiHandle);
                if (!mdi.Document.IsNil)
                {
                    // At minimum we expect a document to be associated when emitting sequence points.
                    foundSequencePoints = true;
                    break;
                }

                if (!mdi.SequencePointsBlob.IsNil)
                {
                    foundSequencePoints = true;
                    break;
                }
            }

            Assert.True(foundSequencePoints, "Expected at least one method to have sequence points.");
        }
    }
}
