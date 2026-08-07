namespace Jroc.Tests;

public sealed class SharedTestCompilationTests
{
    [Fact]
    public void GetOrCompile_EvictsArtifactAfterPairedConsumers()
    {
        SharedTestCompilation.ClearCache();
        var compilationCount = 0;

        try
        {
            CompiledAssembly Compile(string _)
            {
                compilationCount++;
                return CreateCompiledAssembly(compilationCount);
            }

            var first = SharedTestCompilation.GetOrCompile(
                "SharedCache",
                "PairedConsumers",
                additionalScripts: null,
                Compile);

            Assert.Equal(1, compilationCount);
            Assert.True(
                SharedTestCompilation.IsCached(
                    "SharedCache",
                    "PairedConsumers",
                    additionalScripts: null));

            var second = SharedTestCompilation.GetOrCompile(
                "SharedCache",
                "PairedConsumers",
                additionalScripts: null,
                Compile);

            Assert.Same(first, second);
            Assert.Equal(1, compilationCount);
            Assert.False(
                SharedTestCompilation.IsCached(
                    "SharedCache",
                    "PairedConsumers",
                    additionalScripts: null));

            _ = SharedTestCompilation.GetOrCompile(
                "SharedCache",
                "PairedConsumers",
                additionalScripts: null,
                Compile);

            Assert.Equal(2, compilationCount);
        }
        finally
        {
            SharedTestCompilation.ClearCache();
        }
    }

    private static CompiledAssembly CreateCompiledAssembly(int compilationNumber)
    {
        var artifact = new JrocCompiledAssemblyArtifact(
            $"shared-cache-{compilationNumber}",
            [1],
            PdbBytes: null,
            ModuleIds: []);

        return new CompiledAssembly(
            artifact,
            "shared-cache.js",
            [],
            Path.GetTempPath());
    }
}
