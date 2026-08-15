using JavaScriptRuntime;
using JsObjectConstructor = JavaScriptRuntime.Object;

namespace Jroc.Tests;

public sealed class WeakCollectionObjectRepresentationTests
{
    [Fact]
    public void WeakCollectionWrappers_UseInlineJsObjectStorage()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        var weakMap = new JavaScriptRuntime.WeakMap();
        var weakSet = new JavaScriptRuntime.WeakSet();
        var key = new JsObject();
        var customPrototype = new JsObject();

        Assert.IsAssignableFrom<JsObject>(weakMap);
        Assert.IsAssignableFrom<JsObject>(weakSet);
        Assert.Same(JavaScriptRuntime.WeakMap.Prototype, JsObjectConstructor.getPrototypeOf(weakMap));
        Assert.Same(JavaScriptRuntime.WeakSet.Prototype, JsObjectConstructor.getPrototypeOf(weakSet));

        ObjectRuntime.SetProperty(weakMap, "custom", 42d);
        ObjectRuntime.SetProperty(weakSet, "custom", "weak-set");
        Assert.Equal(42d, ObjectRuntime.GetProperty(weakMap, "custom"));
        Assert.Equal("weak-set", ObjectRuntime.GetProperty(weakSet, "custom"));

        JsObjectConstructor.setPrototypeOf(weakMap, customPrototype);
        Assert.Same(customPrototype, JsObjectConstructor.getPrototypeOf(weakMap));

        JsObjectConstructor.freeze(weakMap);
        JsObjectConstructor.freeze(weakSet);
        Assert.True(JsObjectConstructor.isFrozen(weakMap));
        Assert.True(JsObjectConstructor.isFrozen(weakSet));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(weakMap, "custom", 0d));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(weakSet, "custom", "changed"));

        Assert.Same(weakMap, weakMap.set(key, "value"));
        Assert.Equal("value", weakMap.get(key));
        Assert.Same(weakSet, weakSet.add(key));
        Assert.True(weakSet.has(key));
    }

    [Fact]
    public void WeakCollectionPrototypeDescriptorMutations_AreIsolatedAcrossRuntimes()
    {
        var mutationResult = InMemoryTestCompiler.CompileAndExecute(
            "mutate-weak-collection-prototype-descriptors",
            "WeakCollection.PrototypeIsolation",
            GetDescriptorIsolationScript);
        var readResult = InMemoryTestCompiler.CompileAndExecute(
            "read-weak-collection-prototype-descriptors",
            "WeakCollection.PrototypeIsolation",
            GetDescriptorIsolationScript);

        Assert.Equal($"weak-map{Environment.NewLine}weak-set{Environment.NewLine}", mutationResult.Output);
        Assert.Equal($"true{Environment.NewLine}true{Environment.NewLine}", readResult.Output);
    }

    private static (string Script, string? SourcePath) GetDescriptorIsolationScript(string testName)
        => testName switch
        {
            "mutate-weak-collection-prototype-descriptors" => ("""
                Object.defineProperty(WeakMap.prototype, "descriptorLeakCheck", {
                  value: "weak-map",
                  enumerable: true,
                  configurable: true,
                  writable: true
                });
                Object.defineProperty(WeakSet.prototype, "descriptorLeakCheck", {
                  value: "weak-set",
                  enumerable: true,
                  configurable: true,
                  writable: true
                });
                console.log(new WeakMap().descriptorLeakCheck);
                console.log(new WeakSet().descriptorLeakCheck);
                """, null),
            "read-weak-collection-prototype-descriptors" => ("""
                console.log(Object.getOwnPropertyDescriptor(
                  WeakMap.prototype,
                  "descriptorLeakCheck"
                ) === undefined);
                console.log(Object.getOwnPropertyDescriptor(
                  WeakSet.prototype,
                  "descriptorLeakCheck"
                ) === undefined);
                """, null),
            _ => throw new ArgumentOutOfRangeException(nameof(testName), testName, "Unknown descriptor isolation script.")
        };
}
