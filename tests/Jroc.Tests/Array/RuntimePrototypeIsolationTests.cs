using System.Runtime.ExceptionServices;
using JavaScriptRuntime;
using Xunit;

namespace Jroc.Tests.Array;

public class RuntimePrototypeIsolationTests
{
    [Fact]
    public void InMemoryScripts_DoNotShareArrayPrototypeDescriptorMutations()
    {
        var mutationResult = InMemoryTestCompiler.CompileAndExecute(
            "mutate-array-prototype-descriptors",
            "Array.PrototypeIsolation",
            GetDescriptorIsolationScript);
        var readResult = InMemoryTestCompiler.CompileAndExecute(
            "read-array-prototype-descriptors",
            "Array.PrototypeIsolation",
            GetDescriptorIsolationScript);

        Assert.Equal(
            string.Join(
                Environment.NewLine,
                "123",
                "true",
                string.Empty),
            mutationResult.Output);
        Assert.Equal(
            string.Join(
                Environment.NewLine,
                "true",
                "false",
                "true",
                "true",
                string.Empty),
            readResult.Output);
    }

    [Fact]
    public void ArrayPrototype_UsesThreadLocalOverrides()
    {
        var results = new string?[2];
        var exceptions = new ExceptionDispatchInfo?[2];
        using var barrier = new Barrier(2);

        var left = CreatePrototypeMutationThread("left", 0);
        var right = CreatePrototypeMutationThread("right", 1);

        left.Start();
        right.Start();
        left.Join();
        right.Join();

        exceptions[0]?.Throw();
        exceptions[1]?.Throw();

        Assert.Equal("left", results[0]);
        Assert.Equal("right", results[1]);

        Thread CreatePrototypeMutationThread(string marker, int resultIndex)
            => new(() =>
            {
                try
                {
                    JavaScriptRuntime.Array.ResetPrototypeForTests();
                    var prototype = ObjectRuntime.GetItem(GlobalThis.Array, "prototype")
                        ?? throw new InvalidOperationException("Array.prototype was not configured.");
                    ObjectRuntime.SetProperty(prototype, "threadMarker", marker);

                    var array = new JavaScriptRuntime.Array();
                    barrier.SignalAndWait();

                    results[resultIndex] = DotNet2JSConversions.ToString(ObjectRuntime.GetItem(array, "threadMarker"));
                }
                catch (Exception ex)
                {
                    exceptions[resultIndex] = ExceptionDispatchInfo.Capture(ex);
                }
                finally
                {
                    JavaScriptRuntime.Array.ResetPrototypeForTests();
                }
            });
    }

    private static (string Script, string? SourcePath) GetDescriptorIsolationScript(string testName)
        => testName switch
        {
            "mutate-array-prototype-descriptors" => ("""
                Object.defineProperty(Array.prototype, "descriptorLeakCheck", {
                  value: 123,
                  enumerable: true,
                  configurable: true,
                  writable: true
                });
                Object.defineProperty(Array.prototype, "push", {
                  value: Array.prototype.push,
                  enumerable: true,
                  configurable: false,
                  writable: false
                });

                console.log(Object.getOwnPropertyDescriptor(Array.prototype, "descriptorLeakCheck").value);
                console.log(Object.getOwnPropertyDescriptor(Array.prototype, "push").enumerable);
                """, null),
            "read-array-prototype-descriptors" => ("""
                var leakedDescriptor = Object.getOwnPropertyDescriptor(Array.prototype, "descriptorLeakCheck");
                var pushDescriptor = Object.getOwnPropertyDescriptor(Array.prototype, "push");

                console.log(leakedDescriptor === undefined);
                console.log(pushDescriptor.enumerable);
                console.log(pushDescriptor.configurable);
                console.log(pushDescriptor.writable);
                """, null),
            _ => throw new ArgumentOutOfRangeException(nameof(testName), testName, "Unknown descriptor isolation script.")
        };
}
