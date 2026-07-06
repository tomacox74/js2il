using JavaScriptRuntime;
using Jroc.Runtime;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace Jroc.Tests;

public sealed class CompilerRuntimeIntrinsicCatalogTests
{
    [Fact]
    public void CompileToArtifact_AllowsHostRegisteredGlobalReference()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "Jroc.Tests", "CompilerRuntimeIntrinsicCatalog", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputPath);

        try
        {
            var jsPath = Path.Combine(outputPath, "host-global.js");
            var mockFs = new MockFileSystem();
            mockFs.AddFile(jsPath, "\"use strict\";\nexports.value = assert;\n");

            var options = new CompilerOptions
            {
                OutputDirectory = outputPath,
                HostRuntimeIntrinsics = new HostRuntimeIntrinsicDescriptorsBuilder()
                    .AddGlobalValue("assert", new object())
                    .Build()
            };

            var logger = new TestLogger();
            var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFs, logger);
            var compiler = serviceProvider.GetRequiredService<Compiler>();

            var artifact = compiler.CompileToArtifact(jsPath);

            Assert.NotNull(artifact);
            Assert.DoesNotContain("assert is not defined", logger.Errors, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            try { Directory.Delete(outputPath, recursive: true); } catch { }
        }
    }

    [Fact]
    public void CompileAndLoadModule_AllowsScriptToInvokeCSharpHostAssert()
    {
        var callCount = 0;
        var hostIntrinsics = new HostRuntimeIntrinsicDescriptorsBuilder()
            .AddGlobalValue(
                "assert",
                (Action<object>)(message =>
                {
                    callCount++;
                    Assert.Equal("called from JavaScript", message);
                }))
            .Build();

        using var module = JrocInMemoryCompiler.CompileAndLoadModule(
            new JrocInMemoryCompileRequest(Path.Combine(Path.GetTempPath(), "host-assert-module.js"))
            {
                SourceText = "\"use strict\";\nassert(\"called from JavaScript\");\nexports.ok = true;\n",
                HostRuntimeIntrinsics = hostIntrinsics
            },
            options: new JsModuleLoadOptions
            {
                HostRuntimeIntrinsics = hostIntrinsics
            });

        dynamic exports = module.Exports;
        Assert.True((bool)exports.ok);
        Assert.Equal(1, callCount);
    }

    [Fact]
    public void CompileToArtifact_LowersHostRegisteredIntrinsicConstructor()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "Jroc.Tests", "CompilerRuntimeIntrinsicCatalog", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputPath);

        try
        {
            var jsPath = Path.Combine(outputPath, "host-intrinsic.js");
            var mockFs = new MockFileSystem();
            mockFs.AddFile(jsPath, "\"use strict\";\nexports.value = new HostThing();\n");

            var options = new CompilerOptions
            {
                OutputDirectory = outputPath,
                HostRuntimeIntrinsics = new HostRuntimeIntrinsicDescriptorsBuilder()
                    .AddIntrinsicObject("HostThing", typeof(HostThing))
                    .Build()
            };

            var logger = new TestLogger();
            var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFs, logger);
            var compiler = serviceProvider.GetRequiredService<Compiler>();

            var artifact = compiler.CompileToArtifact(jsPath);

            Assert.NotNull(artifact);
            AssertAssemblyReferencesType(artifact, nameof(HostThing));
            Assert.DoesNotContain("HostThing is not defined", logger.Errors, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            try { Directory.Delete(outputPath, recursive: true); } catch { }
        }
    }

    private static void AssertAssemblyReferencesType(JrocCompiledAssemblyArtifact artifact, string typeName)
    {
        using var peStream = new MemoryStream(artifact.PeBytes, writable: false);
        using var peReader = new PEReader(peStream);
        var metadataReader = peReader.GetMetadataReader();

        var referencesType = metadataReader.TypeReferences
            .Select(metadataReader.GetTypeReference)
            .Any(typeRef => string.Equals(metadataReader.GetString(typeRef.Name), typeName, StringComparison.Ordinal));

        Assert.True(referencesType, $"Expected compiled assembly to reference host intrinsic type '{typeName}'.");
    }
}

public sealed class HostThing
{
}
