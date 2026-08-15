using JavaScriptRuntime;
using JsObjectConstructor = JavaScriptRuntime.Object;

namespace Jroc.Tests;

public sealed class BooleanObjectRepresentationTests
{
    private static readonly IReadOnlyDictionary<string, string> DocumentedNonJsObjectIntrinsicExceptions =
        new Dictionary<string, string>
        {
            ["JavaScriptRuntime.AggregateError"] = "Must remain an Exception for CLR throw/catch mechanics.",
            ["JavaScriptRuntime.ArrayBuffer"] = "Planned shared-base migration with typed-array views.",
            ["JavaScriptRuntime.DataView"] = "Planned view-object migration with computed buffer metadata.",
            ["JavaScriptRuntime.Date"] = "Planned low-risk internal-slot wrapper migration.",
            ["JavaScriptRuntime.Error"] = "Must remain an Exception for CLR throw/catch mechanics.",
            ["JavaScriptRuntime.EvalError"] = "Must remain an Exception for CLR throw/catch mechanics.",
            ["JavaScriptRuntime.FinalizationRegistry"] = "Planned internal-slot wrapper migration.",
            ["JavaScriptRuntime.Float32Array"] = "Planned TypedArrayBase exotic-object migration.",
            ["JavaScriptRuntime.Float64Array"] = "Planned TypedArrayBase exotic-object migration.",
            ["JavaScriptRuntime.Int16Array"] = "Planned TypedArrayBase exotic-object migration.",
            ["JavaScriptRuntime.Int32Array"] = "Planned TypedArrayBase exotic-object migration.",
            ["JavaScriptRuntime.Int8Array"] = "Planned TypedArrayBase exotic-object migration.",
            ["JavaScriptRuntime.Map"] = "Planned collection and iterator family migration.",
            ["JavaScriptRuntime.Node.Buffer"] = "Planned indexed/view-object migration.",
            ["JavaScriptRuntime.Object"] = "Static intrinsic holder; constructed objects use JsObject.",
            ["JavaScriptRuntime.Promise"] = "Planned internal-slot wrapper migration.",
            ["JavaScriptRuntime.Proxy"] = "Requires a dedicated migration to preserve trap invariants.",
            ["JavaScriptRuntime.RangeError"] = "Must remain an Exception for CLR throw/catch mechanics.",
            ["JavaScriptRuntime.ReferenceError"] = "Must remain an Exception for CLR throw/catch mechanics.",
            ["JavaScriptRuntime.RegExp"] = "Planned low-risk internal-slot wrapper migration.",
            ["JavaScriptRuntime.Set"] = "Planned collection and iterator family migration.",
            ["JavaScriptRuntime.SharedArrayBuffer"] = "Planned shared-base migration with typed-array views.",
            ["JavaScriptRuntime.SuppressedError"] = "Must remain an Exception for CLR throw/catch mechanics.",
            ["JavaScriptRuntime.Symbol"] = "Primitive representation with dedicated identity semantics.",
            ["JavaScriptRuntime.SyntaxError"] = "Must remain an Exception for CLR throw/catch mechanics.",
            ["JavaScriptRuntime.TypeError"] = "Must remain an Exception for CLR throw/catch mechanics.",
            ["JavaScriptRuntime.Uint16Array"] = "Planned TypedArrayBase exotic-object migration.",
            ["JavaScriptRuntime.Uint32Array"] = "Planned TypedArrayBase exotic-object migration.",
            ["JavaScriptRuntime.Uint8Array"] = "Planned TypedArrayBase exotic-object migration.",
            ["JavaScriptRuntime.Uint8ClampedArray"] = "Planned TypedArrayBase exotic-object migration.",
            ["JavaScriptRuntime.URIError"] = "Must remain an Exception for CLR throw/catch mechanics.",
            ["JavaScriptRuntime.WeakMap"] = "Planned collection migration.",
            ["JavaScriptRuntime.WeakRef"] = "Planned internal-slot wrapper migration.",
            ["JavaScriptRuntime.WeakSet"] = "Planned collection migration."
        };

    [Fact]
    public void BooleanWrappers_UseInlineJsObjectStorage()
    {
        var services = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = services;
            var wrapper = new JavaScriptRuntime.Boolean(true);
            var customPrototype = new JsObject();

            Assert.IsAssignableFrom<JsObject>(wrapper);
            Assert.Same(GlobalThis.BooleanPrototypeValue, JsObjectConstructor.getPrototypeOf(wrapper));

            ObjectRuntime.SetProperty(wrapper, "custom", 42d);
            Assert.Equal(42d, ObjectRuntime.GetProperty(wrapper, "custom"));
            Assert.True(JsObjectConstructor.hasOwn(wrapper, "custom"));

            JsObjectConstructor.setPrototypeOf(wrapper, customPrototype);
            Assert.Same(customPrototype, JsObjectConstructor.getPrototypeOf(wrapper));

            JsObjectConstructor.freeze(wrapper);
            Assert.True(JsObjectConstructor.isFrozen(wrapper));
            Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(wrapper, "custom", 0d));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void BooleanWrappers_DoNotAllocatePrototypeFallbackStorage()
    {
        const int objectCount = 10_000;
        var prototype = new JsObject();

        _ = MeasureJsObjectAllocations(1, prototype);
        _ = MeasureBooleanWrapperAllocations(1);

        var jsObjectAllocations = MeasureJsObjectAllocations(objectCount, prototype);
        var booleanWrapperAllocations = MeasureBooleanWrapperAllocations(objectCount);

        Assert.InRange(booleanWrapperAllocations, 0, jsObjectAllocations + objectCount * 24L);
    }

    [Fact]
    public void BooleanPrototypeDescriptorMutations_AreIsolatedAcrossRuntimes()
    {
        var mutationResult = InMemoryTestCompiler.CompileAndExecute(
            "mutate-boolean-prototype-descriptors",
            "Boolean.PrototypeIsolation",
            GetDescriptorIsolationScript);
        var readResult = InMemoryTestCompiler.CompileAndExecute(
            "read-boolean-prototype-descriptors",
            "Boolean.PrototypeIsolation",
            GetDescriptorIsolationScript);

        Assert.Equal($"runtime-one{Environment.NewLine}", mutationResult.Output);
        Assert.Equal($"true{Environment.NewLine}", readResult.Output);
    }

    [Fact]
    public void NonStaticIntrinsicRepresentations_AreDocumentedByJsObjectMigrationInventory()
    {
        var nonJsObjectIntrinsics = typeof(JsObject).Assembly
            .GetTypes()
            .Where(type => type.GetCustomAttributes(typeof(IntrinsicObjectAttribute), inherit: false).Length > 0)
            .Where(type => !type.IsAbstract || !type.IsSealed)
            .Where(type => !typeof(JsObject).IsAssignableFrom(type))
            .Select(type => type.FullName)
            .OrderBy(name => name)
            .ToArray();

        Assert.Equal(
            DocumentedNonJsObjectIntrinsicExceptions.Keys.OrderBy(name => name),
            nonJsObjectIntrinsics);
        Assert.All(
            DocumentedNonJsObjectIntrinsicExceptions.Values,
            reason => Assert.False(string.IsNullOrWhiteSpace(reason)));
    }

    private static long MeasureJsObjectAllocations(int count, object prototype)
    {
        var objects = new JsObject[count];
        var before = GC.GetAllocatedBytesForCurrentThread();

        for (var index = 0; index < objects.Length; index++)
        {
            var value = new JsObject();
            PrototypeChain.InitializePrototype(value, prototype);
            objects[index] = value;
        }

        GC.KeepAlive(objects);
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }

    private static long MeasureBooleanWrapperAllocations(int count)
    {
        var wrappers = new JavaScriptRuntime.Boolean[count];
        var before = GC.GetAllocatedBytesForCurrentThread();

        for (var index = 0; index < wrappers.Length; index++)
        {
            wrappers[index] = new JavaScriptRuntime.Boolean(true);
        }

        GC.KeepAlive(wrappers);
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }

    private static (string Script, string? SourcePath) GetDescriptorIsolationScript(string testName)
        => testName switch
        {
            "mutate-boolean-prototype-descriptors" => ("""
                Object.defineProperty(Boolean.prototype, "descriptorLeakCheck", {
                  value: "runtime-one",
                  enumerable: true,
                  configurable: true,
                  writable: true
                });
                console.log(new Boolean(false).descriptorLeakCheck);
                """, null),
            "read-boolean-prototype-descriptors" => ("""
                console.log(Object.getOwnPropertyDescriptor(
                  Boolean.prototype,
                  "descriptorLeakCheck"
                ) === undefined);
                """, null),
            _ => throw new ArgumentOutOfRangeException(nameof(testName), testName, "Unknown descriptor isolation script.")
        };
}
