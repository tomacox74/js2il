using JavaScriptRuntime;
using JsObjectConstructor = JavaScriptRuntime.Object;

namespace Jroc.Tests;

public sealed class MapObjectRepresentationTests
{
    [Fact]
    public void MapAndIterators_UseInlineJsObjectStorage()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        var map = new JavaScriptRuntime.Map();
        var iterator = map.entries();
        var customPrototype = new JsObject();

        Assert.IsAssignableFrom<JsObject>(map);
        Assert.IsAssignableFrom<JsObject>(iterator);
        Assert.Same(JavaScriptRuntime.Map.Prototype, JsObjectConstructor.getPrototypeOf(map));
        Assert.Same(JavaScriptRuntime.Map.IteratorPrototype, JsObjectConstructor.getPrototypeOf(iterator));

        ObjectRuntime.SetProperty(map, "custom", 42d);
        Assert.Equal(42d, ObjectRuntime.GetProperty(map, "custom"));
        Assert.True(JsObjectConstructor.hasOwn(map, "custom"));

        ObjectRuntime.SetProperty(iterator, "custom", "iterator");
        Assert.Equal("iterator", ObjectRuntime.GetProperty(iterator, "custom"));
        Assert.True(JsObjectConstructor.hasOwn(iterator, "custom"));

        JsObjectConstructor.setPrototypeOf(map, customPrototype);
        Assert.Same(customPrototype, JsObjectConstructor.getPrototypeOf(map));

        JsObjectConstructor.freeze(map);
        Assert.True(JsObjectConstructor.isFrozen(map));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(map, "custom", 0d));
        Assert.Same(map, map.set("key", "value"));
        Assert.Equal("value", map.get("key"));

        JsObjectConstructor.freeze(iterator);
        Assert.True(JsObjectConstructor.isFrozen(iterator));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(iterator, "custom", "changed"));
    }

    [Fact]
    public void MapPrototypeDescriptorMutations_AreIsolatedAcrossRuntimes()
    {
        var mutationResult = InMemoryTestCompiler.CompileAndExecute(
            "mutate-map-prototype-descriptors",
            "Map.PrototypeIsolation",
            GetDescriptorIsolationScript);
        var readResult = InMemoryTestCompiler.CompileAndExecute(
            "read-map-prototype-descriptors",
            "Map.PrototypeIsolation",
            GetDescriptorIsolationScript);

        Assert.Equal($"runtime-one{Environment.NewLine}", mutationResult.Output);
        Assert.Equal($"true{Environment.NewLine}", readResult.Output);
    }

    private static (string Script, string? SourcePath) GetDescriptorIsolationScript(string testName)
        => testName switch
        {
            "mutate-map-prototype-descriptors" => ("""
                Object.defineProperty(Map.prototype, "descriptorLeakCheck", {
                  value: "runtime-one",
                  enumerable: true,
                  configurable: true,
                  writable: true
                });
                console.log(new Map().descriptorLeakCheck);
                """, null),
            "read-map-prototype-descriptors" => ("""
                console.log(Object.getOwnPropertyDescriptor(
                  Map.prototype,
                  "descriptorLeakCheck"
                ) === undefined);
                """, null),
            _ => throw new ArgumentOutOfRangeException(nameof(testName), testName, "Unknown descriptor isolation script.")
        };
}
