namespace Jroc.Tests;

public sealed class SharedTestCompilationTests
{
    [Fact]
    public void GetOrCompile_EvictsArtifactAfterPairedConsumers()
    {
        var compilationCount = 0;
        var testName = $"PairedConsumers_{Guid.NewGuid():N}";

        CompiledAssembly Compile(string _)
        {
            compilationCount++;
            return CreateCompiledAssembly(compilationCount);
        }

        var first = SharedTestCompilation.GetOrCompile(
            "SharedCache",
            testName,
            additionalScripts: null,
            Compile);

        Assert.Equal(1, compilationCount);
        Assert.True(
            SharedTestCompilation.IsCached(
                "SharedCache",
                testName,
                additionalScripts: null));

        var second = SharedTestCompilation.GetOrCompile(
            "SharedCache",
            testName,
            additionalScripts: null,
            Compile);

        Assert.Same(first, second);
        Assert.Equal(1, compilationCount);
        Assert.False(
            SharedTestCompilation.IsCached(
                "SharedCache",
                testName,
                additionalScripts: null));

        _ = SharedTestCompilation.GetOrCompile(
            "SharedCache",
            testName,
            additionalScripts: null,
            Compile);

        Assert.Equal(2, compilationCount);

        _ = SharedTestCompilation.GetOrCompile(
            "SharedCache",
            testName,
            additionalScripts: null,
            Compile);

        Assert.False(
            SharedTestCompilation.IsCached(
                "SharedCache",
                testName,
                additionalScripts: null));
    }

    [Fact]
    public async Task GetOrCompile_ParallelPairedConsumers_ShareAndEvictArtifact()
    {
        const string category = "SharedCacheParallel";
        var testName = $"PairedConsumers_{Guid.NewGuid():N}";
        using var compilationStarted = new ManualResetEventSlim();
        using var releaseCompilation = new ManualResetEventSlim();
        var compilationCount = 0;

        CompiledAssembly Compile(string _)
        {
            Interlocked.Increment(ref compilationCount);
            compilationStarted.Set();
            Assert.True(releaseCompilation.Wait(TimeSpan.FromSeconds(10)), "Compilation was not released.");
            return CreateCompiledAssembly(compilationCount);
        }

        var first = Task.Run(() => SharedTestCompilation.GetOrCompile(
            category,
            testName,
            additionalScripts: null,
            Compile));

        Assert.True(compilationStarted.Wait(TimeSpan.FromSeconds(10)), "Compilation did not start.");

        var second = Task.Run(() => SharedTestCompilation.GetOrCompile(
            category,
            testName,
            additionalScripts: null,
            Compile));

        releaseCompilation.Set();
        var artifacts = await Task.WhenAll(first, second);

        Assert.Same(artifacts[0], artifacts[1]);
        Assert.Equal(1, compilationCount);
        Assert.False(
            SharedTestCompilation.IsCached(
                category,
                testName,
                additionalScripts: null));
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
