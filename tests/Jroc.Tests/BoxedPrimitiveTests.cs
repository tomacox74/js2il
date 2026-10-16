using System.Runtime.Loader;
using JavaScriptRuntime;
using Jroc.Runtime;
using Jroc.Tests.Utilities;

namespace Jroc.Tests;

[CollectionDefinition(nameof(BoxedPrimitiveTestsCollection), DisableParallelization = true)]
public sealed class BoxedPrimitiveTestsCollection;

[Collection(nameof(BoxedPrimitiveTestsCollection))]
public sealed class BoxedPrimitiveTests
{
    [Fact]
    public void SharedBoxesPreserveClrTypesAndAllocateNothingPerSelection()
    {
        Assert.True(Assert.IsType<bool>(BoxedPrimitive.True));
        Assert.False(Assert.IsType<bool>(BoxedPrimitive.False));
        Assert.Equal(JsNull.Null, Assert.IsType<JsNull>(BoxedPrimitive.Null));
        Assert.NotSame(BoxedPrimitive.True, BoxedPrimitive.False);
        Assert.NotNull(BoxedPrimitive.Null);
        object? result = null;
        for (var i = 0; i < 10_000; i++)
            result = BoxedPrimitive.Box((i & 1) == 0);

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 100_000; i++)
            result = BoxedPrimitive.Box((i & 1) == 0);
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Same(BoxedPrimitive.False, result);
        Assert.Equal(0, allocated);
        Assert.Same(BoxedPrimitive.True, BoxedPrimitive.Box(true));
    }

    [Fact]
    public async Task CompiledValuesShareOnlyPrimitiveBoxesAcrossConcurrentRealms()
    {
        var artifact = Compile("""
            function identity(value) { return value; }
            function comparison(value) { return value === 1; }
            function selectNull(value) { return value ? null : 1; }
            function shortCircuit(value) {
                return typeof value === "function" && typeof value.call === "function";
            }
            function capture() {
                let flag = false, empty = null;
                return function(value) { flag = value; empty = null; return [flag, empty]; };
            }
            const captured = capture()(true);
            const values = [true, false, null, identity(true), identity(null),
                comparison(1), comparison(2), captured[0], captured[1], undefined,
                shortCircuit(identity), shortCircuit(0)];
            if (typeof values[0] !== "boolean" || typeof values[2] !== "object" ||
                !values[0] || values[1] || values[2] || values[2] === undefined ||
                values[2] != undefined || !Object.is(values[2], null) ||
                !Object.is(values[0], true) || Object.is(values[0], values[1]) ||
                Number(values[0]) !== 1 || Number(values[1]) !== 0 ||
                Number(values[2]) !== 0 || String(values[2]) !== "null" ||
                selectNull(true) !== null || selectNull(false) !== 1) {
                throw new Error("Primitive semantics changed");
            }
            if (Object.getPrototypeOf(true) !== Boolean.prototype ||
                Object(true) === Object(true) ||
                Boolean.prototype.sharedPrimitiveMarker !== undefined) {
                throw new Error("Shared primitive retained wrapper or realm state");
            }
            Boolean.prototype.sharedPrimitiveMarker = {};
            if ((true).sharedPrimitiveMarker !== Boolean.prototype.sharedPrimitiveMarker) {
                throw new Error("Primitive lookup used another realm");
            }
            module.exports = { values, wrapper: new Boolean(true), marker: (true).sharedPrimitiveMarker };
            """);
        var il = AssemblyToText.ConvertToText(artifact.PeBytes, artifact.AssemblyName);
        Assert.Contains("BoxedPrimitive::True", il);
        Assert.Contains("BoxedPrimitive::False", il);
        Assert.Contains("BoxedPrimitive::Null", il);
        Assert.Contains("BoxedPrimitive::Box(bool)", il);
        Assert.DoesNotContain("box [System.Runtime]System.Boolean", il);
        Assert.DoesNotContain("box [JavaScriptRuntime]JavaScriptRuntime.JsNull", il);

        var results = await Task.WhenAll(Enumerable.Range(0, 4).Select(_ => Task.Run(() =>
        {
            object? wrapper = null;
            object? marker = null;
            WithExports(artifact, exports =>
            {
                var values = Assert.IsType<JavaScriptRuntime.Array>(exports["values"]);
                object?[] expected = [BoxedPrimitive.True, BoxedPrimitive.False, BoxedPrimitive.Null,
                    BoxedPrimitive.True, BoxedPrimitive.Null, BoxedPrimitive.True,
                    BoxedPrimitive.False, BoxedPrimitive.True, BoxedPrimitive.Null, null,
                    BoxedPrimitive.True, BoxedPrimitive.False];
                Assert.Equal(expected.Length, values.Count);
                for (var index = 0; index < expected.Length; index++)
                    Assert.Same(expected[index], values[index]);
                wrapper = Assert.IsType<JavaScriptRuntime.Boolean>(exports["wrapper"]);
                marker = Assert.IsType<JsObject>(exports["marker"]);
            });
            return (Wrapper: wrapper, Marker: marker);
        })));
        for (var first = 0; first < results.Length; first++)
            for (var second = first + 1; second < results.Length; second++)
            {
                Assert.NotSame(results[first].Wrapper, results[second].Wrapper);
                Assert.NotSame(results[first].Marker, results[second].Marker);
            }
    }

    [Fact]
    public void CapturedBooleanAndNullWritesDoNotAllocatePerWrite()
    {
        var host = new HostRuntimeIntrinsicDescriptorsBuilder()
            .AddGlobalValue("__allocated", (Func<double>)(() => GC.GetTotalAllocatedBytes(true)))
            .Build();
        var artifact = Compile("""
            function create() {
                let flag = false, empty = null;
                function read() { return [flag, empty]; }
                function write(count) {
                    for (let index = 0; index < count; index++) {
                        flag = (index & 1) === 0;
                        empty = null;
                    }
                }
                return { write, read };
            }
            const state = create();
            state.write(10000);
            const before = __allocated();
            state.write(100000);
            const allocated = __allocated() - before;
            module.exports = { allocated, values: state.read() };
            """, host);
        WithExports(artifact, exports =>
        {
            var allocated = Assert.IsType<double>(exports["allocated"]);
            Assert.InRange(allocated, 0d, 50_000d);
            var values = Assert.IsType<JavaScriptRuntime.Array>(exports["values"]);
            Assert.Same(BoxedPrimitive.False, values[0]);
            Assert.Same(BoxedPrimitive.Null, values[1]);
        }, host);
    }

    private static JrocCompiledAssemblyArtifact Compile(string source, HostRuntimeIntrinsicDescriptors? host = null)
        => JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(
            Path.Combine(Path.GetTempPath(), "shared-primitive-boxes.js"))
        {
            SourceText = source,
            EmitPdb = false,
            HostRuntimeIntrinsics = host ?? HostRuntimeIntrinsicDescriptors.Empty
        });

    private static void WithExports(
        JrocCompiledAssemblyArtifact artifact,
        Action<JsObject> inspect,
        HostRuntimeIntrinsicDescriptors? host = null)
    {
        var context = new AssemblyLoadContext($"primitive-boxes-{Guid.NewGuid():N}", isCollectible: true);
        try
        {
            using var pe = new MemoryStream(artifact.PeBytes);
            var assembly = context.LoadFromStream(pe);
            using var exports = JsEngine.LoadDynamicModule(assembly, artifact.ModuleIds[0],
                new JsModuleLoadOptions { HostRuntimeIntrinsics = host ?? HostRuntimeIntrinsicDescriptors.Empty });
            inspect(Assert.IsType<JsObject>(exports.UnwrapExports()));
        }
        finally
        {
            context.Unload();
        }
    }
}
