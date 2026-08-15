using JavaScriptRuntime;
using JsObjectConstructor = JavaScriptRuntime.Object;

namespace Jroc.Tests;

public sealed class SetObjectRepresentationTests
{
    [Fact]
    public void SetAndIterators_UseInlineJsObjectStorage()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        var set = new JavaScriptRuntime.Set();
        var iterator = set.values();
        var customPrototype = new JsObject();

        Assert.IsAssignableFrom<JsObject>(set);
        Assert.IsAssignableFrom<JsObject>(iterator);
        Assert.Same(JavaScriptRuntime.Set.Prototype, JsObjectConstructor.getPrototypeOf(set));
        Assert.Same(JavaScriptRuntime.Set.IteratorPrototype, JsObjectConstructor.getPrototypeOf(iterator));

        ObjectRuntime.SetProperty(set, "custom", 42d);
        Assert.Equal(42d, ObjectRuntime.GetProperty(set, "custom"));
        Assert.True(JsObjectConstructor.hasOwn(set, "custom"));

        ObjectRuntime.SetProperty(iterator, "custom", "iterator");
        Assert.Equal("iterator", ObjectRuntime.GetProperty(iterator, "custom"));
        Assert.True(JsObjectConstructor.hasOwn(iterator, "custom"));

        JsObjectConstructor.setPrototypeOf(set, customPrototype);
        Assert.Same(customPrototype, JsObjectConstructor.getPrototypeOf(set));

        JsObjectConstructor.freeze(set);
        Assert.True(JsObjectConstructor.isFrozen(set));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(set, "custom", 0d));
        Assert.Same(set, set.add("value"));
        Assert.True((bool)set.has("value"));

        JsObjectConstructor.freeze(iterator);
        Assert.True(JsObjectConstructor.isFrozen(iterator));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(iterator, "custom", "changed"));
    }

    [Fact]
    public void SetPrototypeDescriptorMutations_AreIsolatedAcrossRuntimes()
    {
        var mutationResult = InMemoryTestCompiler.CompileAndExecute(
            "mutate-set-prototype-descriptors",
            "Set.PrototypeIsolation",
            GetDescriptorIsolationScript);
        var readResult = InMemoryTestCompiler.CompileAndExecute(
            "read-set-prototype-descriptors",
            "Set.PrototypeIsolation",
            GetDescriptorIsolationScript);

        Assert.Equal($"runtime-one{Environment.NewLine}", mutationResult.Output);
        Assert.Equal($"true{Environment.NewLine}", readResult.Output);
    }

    private static (string Script, string? SourcePath) GetDescriptorIsolationScript(string testName)
        => testName switch
        {
            "mutate-set-prototype-descriptors" => ("""
                Object.defineProperty(Set.prototype, "descriptorLeakCheck", {
                  value: "runtime-one",
                  enumerable: true,
                  configurable: true,
                  writable: true
                });
                console.log(new Set().descriptorLeakCheck);
                """, null),
            "read-set-prototype-descriptors" => ("""
                console.log(Object.getOwnPropertyDescriptor(
                  Set.prototype,
                  "descriptorLeakCheck"
                ) === undefined);
                """, null),
            _ => throw new ArgumentOutOfRangeException(nameof(testName), testName, "Unknown descriptor isolation script.")
        };
}
