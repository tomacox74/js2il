using System.Runtime.CompilerServices;
using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Array;

[Collection(Jroc.Test262.Tests.built_ins.InMemoryExecutionTestsBase.CollectionName)]
public sealed class InMemoryExecutionTests
{
    [Fact]
    public void InMemoryExecution_UnloadsCollectibleContexts()
    {
        var weakReferences = RunRepresentativeArrayFixtures();

        for (var i = 0; weakReferences.Any(reference => reference.IsAlive) && i < 10; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        Assert.All(weakReferences, reference => Assert.False(reference.IsAlive));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static IReadOnlyList<WeakReference> RunRepresentativeArrayFixtures([CallerFilePath] string sourceFilePath = "")
    {
        var weakReferences = new List<WeakReference>();
        foreach (var testName in new[] { "constructor", "S15.4.5.2_A1_T1", "S15.4.5.2_A2_T1" })
        {
            var result = Test262SharedAssertHarness.CompileAndExecute(
                testName,
                "built_ins.Array",
                name => GetJavaScriptAndSourcePath(name),
                sourceFilePath);
            weakReferences.Add(result.LoadContextWeakReference);
        }

        return weakReferences;
    }

    private static (string Script, string? SourcePath) GetJavaScriptAndSourcePath(string testName)
    {
        var scriptPath = Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "built-ins",
            "Array",
            "JavaScript",
            testName + ".js");

        scriptPath = Path.GetFullPath(scriptPath);
        if (!File.Exists(scriptPath))
        {
            throw new FileNotFoundException($"JavaScript fixture not found at '{scriptPath}'.", scriptPath);
        }

        return (File.ReadAllText(scriptPath), scriptPath);
    }
}
