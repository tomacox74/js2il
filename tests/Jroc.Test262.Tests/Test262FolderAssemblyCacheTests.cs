using System.Runtime.CompilerServices;
using System.Reflection.Metadata;

namespace Jroc.Test262.Tests;

public sealed class Test262FolderAssemblyCacheTests
{
    [Fact]
    public void RegisteredFactsFromDifferentClasses_ShareOneCompilationAndLoad()
    {
        var folder = Path.Combine(ProjectDirectory(), "language", "arguments-object", "JavaScript");
        var firstPath = Path.Combine(folder, "10.6-12-2.js");
        var secondPath = Path.Combine(folder, "10.6-12-1.js");

        using var cache = new Test262FolderAssemblyCache();
        var assemblies = new Test262FolderAssemblyCache.FolderAssembly[8];
        Parallel.For(0, assemblies.Length, index =>
            assemblies[index] = cache.Get(index % 2 == 0 ? firstPath : secondPath));

        Assert.All(assemblies, assembly => Assert.Same(assemblies[0], assembly));
        var first = assemblies[0].GetEntry(firstPath);
        var second = assemblies[0].GetEntry(secondPath);
        Assert.NotEqual(first.ModuleId, second.ModuleId);
        Assert.Throws<InvalidOperationException>(() =>
            assemblies[0].GetEntry(Path.Combine(folder, "10.5-1gs.js")));
        Assert.Equal(1, cache.CompilationCount);
        Assert.Equal(1, cache.LoadCount);

        using var pdbStream = new MemoryStream(assemblies[0].Artifact.PdbBytes!);
        using var pdbProvider = MetadataReaderProvider.FromPortablePdbStream(pdbStream);
        var pdb = pdbProvider.GetMetadataReader();
        var documents = pdb.Documents.Select(handle => pdb.GetString(pdb.GetDocument(handle).Name)).ToArray();
        Assert.Contains(firstPath, documents);
        Assert.Contains(secondPath, documents);

        var resultSecond = Jroc.Tests.Test262SharedAssertHarness.ExecuteCompiledEntry(
            "10.6-12-1", second.Script, secondPath,
            assemblies[0].Loaded, assemblies[0].Artifact, second.ModuleId);
        var resultFirst = Jroc.Tests.Test262SharedAssertHarness.ExecuteCompiledEntry(
            "10.6-12-2", first.Script, firstPath,
            assemblies[0].Loaded, assemblies[0].Artifact, first.ModuleId);
        Jroc.Tests.Test262SharedAssertHarness.AssertNoOutput("10.6-12-1", resultSecond.Output);
        Jroc.Tests.Test262SharedAssertHarness.AssertNoOutput("10.6-12-2", resultFirst.Output);
    }

    [Fact]
    public void NestedJavaScriptFolder_HasItsOwnAssembly()
    {
        using var cache = new Test262FolderAssemblyCache();
        var root = Path.Combine(ProjectDirectory(), "language", "arguments-object");
        var first = cache.Get(Path.Combine(root, "JavaScript", "10.6-12-2.js"));
        var nested = cache.Get(Path.Combine(root, "mapped", "JavaScript", "mapped-arguments-nonwritable-nonconfigurable-1.js"));
        Assert.NotSame(first, nested);
        Assert.Equal(2, cache.CompilationCount);
        Assert.Equal(2, cache.LoadCount);
    }

    [Fact]
    public void CollidingEntryNames_KeepDistinctEntriesAndOriginalPdbPaths()
    {
        var folder = Path.Combine(ProjectDirectory(), "language", "expressions", "delete", "JavaScript");
        var firstPath = Path.Combine(folder, "11.4.1-4-a-3-s.js");
        var secondPath = Path.Combine(folder, "11.4.1-4.a-3-s.js");

        using var cache = new Test262FolderAssemblyCache();
        var assembly = cache.Get(firstPath);
        Assert.NotEqual(assembly.GetEntry(firstPath).ModuleId, assembly.GetEntry(secondPath).ModuleId);

        using var pdbStream = new MemoryStream(assembly.Artifact.PdbBytes!);
        using var pdbProvider = MetadataReaderProvider.FromPortablePdbStream(pdbStream);
        var pdb = pdbProvider.GetMetadataReader();
        var documents = pdb.Documents.Select(handle => pdb.GetString(pdb.GetDocument(handle).Name)).ToArray();
        Assert.Contains(firstPath, documents);
        Assert.Contains(secondPath, documents);
    }

    [Fact]
    public void CollidingDirectoryAndEntryNames_ShareOneAssembly()
    {
        var folder = Path.Combine(ProjectDirectory(), "built-ins", "Object", "prototype", "JavaScript");
        var firstPath = Path.Combine(folder, "__proto__", "get-abrupt.js");
        var secondPath = Path.Combine(folder, "proto.js");

        using var cache = new Test262FolderAssemblyCache();
        var assembly = cache.Get(firstPath);
        Assert.Same(assembly, cache.Get(secondPath));
        Assert.NotEqual(assembly.GetEntry(firstPath).ModuleId, assembly.GetEntry(secondPath).ModuleId);
        Assert.Equal(1, cache.CompilationCount);
        Assert.Equal(1, cache.LoadCount);
    }

    [Fact]
    public void Dispose_UnloadsTheCollectibleAssembly()
    {
        var reference = LoadAndDispose();
        for (var i = 0; i < 10 && reference.IsAlive; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        Assert.False(reference.IsAlive);
    }

    [Fact]
    public void InvalidRunnableFolder_ReportsSourceAndDoesNotRetryIndividualEntries()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Jroc.Test262.Tests", Guid.NewGuid().ToString("N"));
        var folder = Path.Combine(directory, "JavaScript");
        var sourcePath = Path.Combine(folder, "invalid.js");
        Directory.CreateDirectory(folder);
        try
        {
            File.WriteAllText(Path.Combine(directory, "ExecutionTests.cs"),
                """[Fact] public Task Invalid() => ExecutionTest("invalid");""");
            File.WriteAllText(sourcePath, "eval('1');");
            using var cache = new Test262FolderAssemblyCache();
            var error = Assert.Throws<InvalidOperationException>(() => cache.Get(sourcePath));
            Assert.Contains(folder, error.Message);
            Assert.Contains(sourcePath, error.Message);
            Assert.Contains("eval", error.Message);
            Assert.Same(error, Assert.Throws<InvalidOperationException>(() => cache.Get(sourcePath)));
            Assert.Equal(0, cache.LoadCount);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference LoadAndDispose()
    {
        using var cache = new Test262FolderAssemblyCache();
        var folder = Path.Combine(ProjectDirectory(), "language", "arguments-object", "mapped", "JavaScript");
        return cache.Get(Path.Combine(folder, "mapped-arguments-nonwritable-nonconfigurable-1.js"))
            .Loaded.LoadContextWeakReference;
    }

    private static string ProjectDirectory([CallerFilePath] string sourceFilePath = "")
        => Path.GetDirectoryName(sourceFilePath)!;
}
