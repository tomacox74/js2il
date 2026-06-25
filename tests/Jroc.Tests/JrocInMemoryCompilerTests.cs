using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace Jroc.Tests;

public sealed class JrocInMemoryCompilerTests
{
    [Fact]
    public void Compile_WithSourceText_ReturnsArtifactWithoutWritingFiles()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "Jroc.Tests", "JrocInMemoryCompiler", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputPath);

        try
        {
            var entryPath = Path.Combine(outputPath, "memory-entry.js");
            var dllPath = Path.Combine(outputPath, "memory-entry.dll");

            var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(entryPath)
            {
                SourceText = "\"use strict\";\nexports.answer = 42;\n",
                EmitPdb = true
            });

            Assert.Equal("memory-entry", artifact.AssemblyName);
            Assert.Contains("memory-entry", artifact.ModuleIds, StringComparer.OrdinalIgnoreCase);
            Assert.NotEmpty(artifact.PeBytes);
            Assert.NotNull(artifact.PdbBytes);
            Assert.NotEmpty(artifact.PdbBytes!);
            Assert.False(File.Exists(dllPath), $"Did not expect '{dllPath}' to be written.");

            using var peStream = new MemoryStream(artifact.PeBytes, writable: false);
            using var peReader = new PEReader(peStream);
            var codeViewEntry = Assert.Single(peReader.ReadDebugDirectory(), entry => entry.Type == DebugDirectoryEntryType.CodeView);
            var codeView = peReader.ReadCodeViewDebugDirectoryData(codeViewEntry);
            Assert.Equal("memory-entry.pdb", codeView.Path);

            using var pdbStream = new MemoryStream(artifact.PdbBytes!, writable: false);
            using var provider = MetadataReaderProvider.FromPortablePdbStream(pdbStream);
            Assert.True(provider.GetMetadataReader().Documents.Any());
        }
        finally
        {
            try { Directory.Delete(outputPath, recursive: true); } catch { }
        }
    }

    [Fact]
    public void Compile_WithFileBackedInput_ReturnsArtifact()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "Jroc.Tests", "JrocInMemoryCompilerFileBacked", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputPath);

        try
        {
            var entryPath = Path.Combine(outputPath, "file-backed.js");
            File.WriteAllText(entryPath, "\"use strict\";\nmodule.exports = 123;\n");

            var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(entryPath));

            Assert.Equal("file-backed", artifact.AssemblyName);
            Assert.Contains("file-backed", artifact.ModuleIds, StringComparer.OrdinalIgnoreCase);
            Assert.NotEmpty(artifact.PeBytes);
            Assert.Null(artifact.PdbBytes);
        }
        finally
        {
            try { Directory.Delete(outputPath, recursive: true); } catch { }
        }
    }

    [Fact]
    public void Compile_WhenCompilationFails_ThrowsWithCapturedErrors()
    {
        var entryPath = Path.Combine(Path.GetTempPath(), "broken-memory-entry.js");

        var ex = Assert.Throws<InvalidOperationException>(() => JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(entryPath)
        {
            SourceText = "\"use strict\";\nmodule.exports = ;\n"
        }));

        Assert.Contains("Compilation failed.", ex.Message);
        Assert.Contains("Parse Errors:", ex.Message);
        Assert.Contains("Unexpected token", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
