using Jroc.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace Jroc.Tests;

public sealed class CompiledAssemblyArtifactTests
{
    [Fact]
    public void CompileToArtifact_WithEmitPdb_ProducesReusableBytes_WithoutWritingOutputFiles()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "Jroc.Tests", "CompiledAssemblyArtifact", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputPath);

        try
        {
            var jsPath = Path.Combine(outputPath, "artifact.js");
            var dllPath = Path.Combine(outputPath, "artifact.dll");
            var pdbPath = Path.Combine(outputPath, "artifact.pdb");
            var runtimeConfigPath = Path.Combine(outputPath, "artifact.runtimeconfig.json");

            var mockFs = new MockFileSystem();
            mockFs.AddFile(jsPath, "\"use strict\";\nexports.answer = 42;\n");

            var options = new CompilerOptions
            {
                OutputDirectory = outputPath,
                EmitPdb = true
            };

            var logger = new TestLogger();
            var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFs, logger);
            var compiler = serviceProvider.GetRequiredService<Compiler>();

            var artifact = compiler.CompileToArtifact(jsPath);

            Assert.NotNull(artifact);
            Assert.Equal("artifact", artifact.AssemblyName);
            Assert.Contains("artifact", artifact.ModuleIds, StringComparer.OrdinalIgnoreCase);
            Assert.NotEmpty(artifact.PeBytes);
            Assert.NotNull(artifact.PdbBytes);
            Assert.NotEmpty(artifact.PdbBytes!);
            Assert.False(File.Exists(dllPath), $"Did not expect '{dllPath}' to be written during artifact compilation.");
            Assert.False(File.Exists(pdbPath), $"Did not expect '{pdbPath}' to be written during artifact compilation.");
            Assert.False(File.Exists(runtimeConfigPath), $"Did not expect '{runtimeConfigPath}' to be written during artifact compilation.");

            using var peStream = new MemoryStream(artifact.PeBytes, writable: false);
            using var peReader = new PEReader(peStream);
            var debugDirectory = peReader.ReadDebugDirectory();
            var codeViewEntry = Assert.Single(debugDirectory, entry => entry.Type == DebugDirectoryEntryType.CodeView);
            var codeView = peReader.ReadCodeViewDebugDirectoryData(codeViewEntry);
            Assert.Equal("artifact.pdb", codeView.Path);

            using var pdbStream = new MemoryStream(artifact.PdbBytes!, writable: false);
            using var provider = MetadataReaderProvider.FromPortablePdbStream(pdbStream);
            var pdbReader = provider.GetMetadataReader();
            Assert.True(pdbReader.Documents.Any(), "Expected at least one document row in the in-memory PDB.");
            Assert.True(pdbReader.MethodDebugInformation.Any(), "Expected method debug information in the in-memory PDB.");
        }
        finally
        {
            try { Directory.Delete(outputPath, recursive: true); } catch { }
        }
    }

    [Fact]
    public void Materialize_WritesLaunchableAssemblyAndSymbols()
    {
        var root = Path.Combine(Path.GetTempPath(), "Jroc.Tests", "CompiledAssemblyArtifact", Guid.NewGuid().ToString("N"));
        var outputPath = Path.Combine(root, "output");

        try
        {
            var entryPath = Path.Combine(root, "materialize.js");
            var mockFs = new MockFileSystem();
            mockFs.AddFile(entryPath, "\"use strict\";\nconsole.log('materialized');\n");

            var artifact = JrocInMemoryCompiler.Compile(
                new JrocInMemoryCompileRequest(entryPath)
                {
                    FileSystem = mockFs,
                    EmitPdb = true
                });

            var materialized = artifact.Materialize(outputPath);

            Assert.Equal(Path.Combine(outputPath, "materialize.dll"), materialized.AssemblyPath);
            Assert.True(File.Exists(materialized.AssemblyPath));
            Assert.True(File.Exists(materialized.PdbPath!));
            Assert.True(File.Exists(materialized.RuntimeConfigPath));
            Assert.Equal(artifact.PeBytes, File.ReadAllBytes(materialized.AssemblyPath));
            Assert.Equal(artifact.PdbBytes, File.ReadAllBytes(materialized.PdbPath!));
        }
        finally
        {
            try { Directory.Delete(root, recursive: true); } catch { }
        }
    }

    [Fact]
    public void TestCompiler_ExplicitArtifactMaterialization_WritesOutput()
    {
        var root = Path.Combine(Path.GetTempPath(), "Jroc.Tests", "CompiledAssemblyArtifact", Guid.NewGuid().ToString("N"));
        var outputPath = Path.Combine(root, "output");

        try
        {
            var compiled = TestCompiler.Compile(
                testName: "test-compiler-materialize",
                testCategory: "CompiledAssemblyArtifact",
                outputDirectory: outputPath,
                getJavaScriptAndSourcePath: _ => ("\"use strict\";\nconsole.log('materialized');\n", null),
                additionalScripts: null,
                writeArtifacts: true);

            Assert.NotNull(compiled.MaterializedArtifact);
            Assert.NotNull(compiled.AssemblyPath);
            Assert.NotNull(compiled.PdbPath);
            Assert.True(File.Exists(compiled.AssemblyPath));
            Assert.True(File.Exists(compiled.PdbPath));
        }
        finally
        {
            try { Directory.Delete(root, recursive: true); } catch { }
        }
    }
}
