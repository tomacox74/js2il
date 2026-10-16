using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Runtime.Loader;
using JavaScriptRuntime;
using Jroc.IL;
using Jroc.IR;
using Jroc.Runtime;
using Jroc.Services;
using Jroc.Tests.Utilities;
using Jroc.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;

namespace Jroc.Tests;

[CollectionDefinition(nameof(BoxedPrimitiveTestsCollection), DisableParallelization = true)]
public sealed class BoxedPrimitiveTestsCollection;

[Collection(nameof(BoxedPrimitiveTestsCollection))]
public sealed class BoxedPrimitiveTests
{
    [Fact]
    public void SharedBoxesPreserveClrTypesAndDoNotAllocatePerSelection()
    {
        Assert.True(Assert.IsType<bool>(BoxedPrimitive.True));
        Assert.False(Assert.IsType<bool>(BoxedPrimitive.False));
        Assert.Equal(JsNull.Null, Assert.IsType<JsNull>(BoxedPrimitive.Null));
        Assert.NotSame(BoxedPrimitive.True, BoxedPrimitive.False);
        Assert.NotNull(BoxedPrimitive.Null);
        var results = new object[100_000];
        for (var i = 0; i < 10_000; i++)
            results[i] = BoxedPrimitive.Box((i & 1) == 0);

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < results.Length; i++)
            results[i] = BoxedPrimitive.Box((i & 1) == 0);
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.InRange(allocated, 0L, 1_024L);
        for (var i = 0; i < results.Length; i++)
            Assert.Same((i & 1) == 0 ? BoxedPrimitive.True : BoxedPrimitive.False, results[i]);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void AliasedVariableSlotBoxesCurrentValue(bool convertToObject, bool scheduled)
    {
        var body = new MethodBodyIR();
        body.VariableNames.Add("flag");
        body.VariableStorages.Add(new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var original = AddTemp(body, typeof(bool), variableSlot: 0);
        var reassigned = AddTemp(body, typeof(bool), variableSlot: 0);
        body.Instructions.Add(new LIRConstBoolean(true, original));
        body.Instructions.Add(new LIRConstBoolean(false, reassigned));
        var result = original;
        if (convertToObject)
        {
            result = AddTemp(body, typeof(object));
            body.Instructions.Add(new LIRConvertToObject(original, typeof(bool), result));
        }
        body.Instructions.Add(new LIRReturn(result));

        var compiled = CompileBody(body, scheduled);

        Assert.Same(BoxedPrimitive.False, compiled.Result);
        Assert.Contains("BoxedPrimitive::Box(bool)", compiled.Il);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void ConstantBoxesAvoidRedundantLoadsWithBothSchedulers(bool convertToObject, bool scheduled)
    {
        foreach (bool? value in new bool?[] { true, false, null })
        {
            var body = new MethodBodyIR();
            var type = value.HasValue ? typeof(bool) : typeof(JsNull);
            var constant = AddTemp(body, type);
            body.Instructions.Add(value.HasValue
                ? new LIRConstBoolean(value.Value, constant)
                : new LIRConstNull(constant));
            var result = constant;
            if (convertToObject)
            {
                result = AddTemp(body, typeof(object));
                body.Instructions.Add(new LIRConvertToObject(constant, type, result));
            }
            body.Instructions.Add(new LIRReturn(result));

            var compiled = CompileBody(body, scheduled);
            var fieldName = value.HasValue ? (value.Value ? "True" : "False") : "Null";

            Assert.Same(value.HasValue ? BoxedPrimitive.Box(value.Value) : BoxedPrimitive.Null, compiled.Result);
            Assert.Contains($"BoxedPrimitive::{fieldName}", compiled.Il);
            if (!scheduled)
            {
                Assert.DoesNotContain("ldc.i4", compiled.Il);
                Assert.DoesNotContain(": pop", compiled.Il);
            }
        }
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

    private static TempVariable AddTemp(MethodBodyIR body, Type type, int variableSlot = -1)
    {
        var temp = new TempVariable(body.Temps.Count);
        body.Temps.Add(temp);
        body.TempStorages.Add(new ValueStorage(
            type.IsValueType ? ValueStorageKind.UnboxedValue : ValueStorageKind.Reference, type));
        body.TempVariableSlots.Add(variableSlot);
        return temp;
    }

    private static (object Result, string Il) CompileBody(MethodBodyIR body, bool scheduled)
    {
        using var services = CompilerServices.BuildServiceProvider(new CompilerOptions
        {
            EmitPdb = false,
            LIRStackSchedulerMode = scheduled ? LIRStackSchedulerMode.GeneralRegions : LIRStackSchedulerMode.Identity
        });
        var metadata = services.GetRequiredService<MetadataBuilder>();
        metadata.AddModule(0, metadata.GetOrAddString("Boxing.dll"), metadata.GetOrAddGuid(Guid.NewGuid()), default, default);
        metadata.AddAssembly(metadata.GetOrAddString("Boxing"), new Version(1, 0), default, default, default, default);
        new TypeBuilder(metadata, "", "<Module>").AddTypeDefinition(TypeAttributes.NotPublic, default);
        var typeBuilder = new TypeBuilder(metadata, "", "Boxing");
        var compiler = ActivatorUtilities.CreateInstance<LIRToILCompiler>(services);
        var ilStream = new BlobBuilder();
        var method = compiler.TryCompile(new MethodDescriptor("Run", typeBuilder, [])
        {
            HasScopesParameter = false,
            HasNewTargetParameter = false
        }, body, new MethodBodyStreamEncoder(ilStream));
        Assert.False(method.IsNil);
        typeBuilder.AddTypeDefinition(TypeAttributes.Public, services.GetRequiredService<BaseClassLibraryReferences>().ObjectType);
        var pe = new BlobBuilder();
        new ManagedPEBuilder(
            new PEHeaderBuilder(imageCharacteristics: Characteristics.ExecutableImage | Characteristics.Dll),
            new MetadataRootBuilder(metadata), ilStream).Serialize(pe);
        var bytes = pe.ToArray();
        var il = AssemblyToText.ConvertToText(bytes, "Boxing");
        var context = new AssemblyLoadContext($"boxing-{Guid.NewGuid():N}", isCollectible: true);
        try
        {
            using var stream = new MemoryStream(bytes);
            var assembly = context.LoadFromStream(stream);
            var run = assembly.GetType("Boxing", throwOnError: true)!.GetMethod("Run")!.CreateDelegate<Func<object>>();
            return (run(), il);
        }
        finally
        {
            context.Unload();
        }
    }

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
