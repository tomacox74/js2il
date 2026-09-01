using System.Linq;
using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests.TypedArray;

/// <summary>
/// Focused coverage for issue #1895: TypedArray prototype methods/getters, ArrayBuffer/
/// SharedArrayBuffer/DataView accessors, and Uint8Array base64/hex methods must be wired
/// through the explicit-receiver ABI (<see cref="BuiltinFunctionDelegates"/>) instead of
/// ambient <c>RuntimeServices.GetCurrentThis()</c> reads.
/// </summary>
public sealed class BuiltinAdapterRuntimeTests
{
    private static readonly string[] ZeroArgMethodNames =
    [
        "toString",
        "toLocaleString",
        "toReversed"
    ];

    private static readonly string[] VariadicMethodNames =
    [
        "sort",
        "toSorted",
        "with",
        "copyWithin",
        "findLast",
        "findLastIndex",
        "reduceRight"
    ];

    private static readonly string[] GetterNames =
    [
        "length",
        "buffer",
        "byteOffset",
        "byteLength"
    ];

    [Fact]
    public void TypedArrayPrototypeMembersUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            var typedArrayPrototype = JavaScriptRuntime.RuntimeIntrinsics.Current.TypedArrayPrototype;

            foreach (var name in GetterNames)
            {
                Assert.True(PropertyDescriptorStore.TryGetOwn(typedArrayPrototype, name, out var descriptor), name);
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(descriptor.Get);
                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), name);
                Assert.False(adapter.RequiresInvocationContext, name);
            }

            foreach (var name in ZeroArgMethodNames.Concat(VariadicMethodNames))
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(typedArrayPrototype, name));
                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), name);
                Assert.False(adapter.RequiresInvocationContext, name);
            }

            foreach (var name in VariadicMethodNames)
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(typedArrayPrototype, name));
                Assert.IsType<BuiltinFunctionVariadic>(adapter.Target);
            }
        });
    }

    [Fact]
    public void TypedArrayPrototypeMembersPreserveFunctionalBehavior()
    {
        WithRealm(() =>
        {
            var array = new Uint8Array(new object?[] { 3d, 1d, 2d });

            var lengthGetter = GetAccessorGetter(
                JavaScriptRuntime.RuntimeIntrinsics.Current.TypedArrayPrototype,
                "length");
            var bufferGetter = GetAccessorGetter(
                JavaScriptRuntime.RuntimeIntrinsics.Current.TypedArrayPrototype,
                "buffer");
            var sort = ObjectRuntime.GetItem(
                JavaScriptRuntime.RuntimeIntrinsics.Current.TypedArrayPrototype,
                "sort");

            Assert.Equal(3d, CallableOperations.Call0(lengthGetter, array));
            Assert.IsType<ArrayBuffer>(CallableOperations.Call0(bufferGetter, array));

            var sorted = Assert.IsType<Uint8Array>(CallableOperations.Call0(sort, array));
            Assert.Same(array, sorted);
            Assert.Equal(1d, ObjectRuntime.GetItem(sorted, 0d));
            Assert.Equal(2d, ObjectRuntime.GetItem(sorted, 1d));
            Assert.Equal(3d, ObjectRuntime.GetItem(sorted, 2d));
        });
    }

    [Fact]
    public void TypedArrayPrototypeMembersRejectIncompatibleReceivers()
    {
        WithRealm(() =>
        {
            var lengthGetter = GetAccessorGetter(
                JavaScriptRuntime.RuntimeIntrinsics.Current.TypedArrayPrototype,
                "length");

            var ex = Assert.Throws<TypeError>(() => CallableOperations.Call0(lengthGetter, "not a typed array"));
            Assert.Contains("TypedArray.prototype.length", ex.Message);
        });
    }

    [Fact]
    public void ArrayBufferAccessorsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var name in new[] { "byteLength", "maxByteLength", "resizable" })
            {
                Assert.True(
                    PropertyDescriptorStore.TryGetOwn(JavaScriptRuntime.ArrayBuffer.Prototype, name, out var descriptor),
                    name);
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(descriptor.Get);
                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), name);
                Assert.False(adapter.RequiresInvocationContext, name);
            }

            var buffer = new ArrayBuffer(8d);
            var byteLengthGetter = GetAccessorGetter(JavaScriptRuntime.ArrayBuffer.Prototype, "byteLength");
            Assert.Equal(8d, CallableOperations.Call0(byteLengthGetter, buffer));

            var ex = Assert.Throws<TypeError>(() => CallableOperations.Call0(byteLengthGetter, "nope"));
            Assert.Contains("ArrayBuffer.prototype.byteLength", ex.Message);
        });
    }

    [Fact]
    public void ArrayBufferSliceUsesIntrinsicConstructorWhenGlobalBindingChanges()
    {
        WithRealm(() =>
        {
            var buffer = new ArrayBuffer(4d);
            ObjectRuntime.SetItem(buffer, "constructor", null);
            ObjectRuntime.SetItem(GlobalThis.globalThis, "ArrayBuffer", new JsObject());

            var result = buffer.slice(1d, 3d);

            Assert.Equal(2d, result.byteLength);
        });
    }

    [Fact]
    public void ArrayBufferSliceObservesSourceResizeDuringSpeciesConstruction()
    {
        WithRealm(() =>
        {
            var source = new ArrayBuffer(4d, new JsObject
            {
                ["maxByteLength"] = 4d
            });
            source.RawBytes[0] = 1;
            source.RawBytes[1] = 2;
            source.RawBytes[2] = 3;
            source.RawBytes[3] = 4;

            Func<object[], object?[], object?> species = (_, args) =>
            {
                source.resize(0d);
                return new ArrayBuffer(args[0]);
            };
            JavaScriptRuntime.Function.InitializeFunctionInstance(species, 1d, "Species");
            JavaScriptRuntime.Function.MarkConstructible(species);

            var constructor = new JsObject();
            ObjectRuntime.SetItem(constructor, Symbol.species, species);
            ObjectRuntime.SetItem(source, "constructor", constructor);

            var result = source.slice(0d, 4d);

            Assert.Equal(0d, source.byteLength);
            Assert.Equal(new byte[4], result.RawBytes);
        });
    }

    [Fact]
    public void SharedArrayBufferAccessorsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var name in new[] { "byteLength", "maxByteLength", "growable" })
            {
                Assert.True(
                    PropertyDescriptorStore.TryGetOwn(
                        JavaScriptRuntime.SharedArrayBuffer.SharedPrototype, name, out var descriptor),
                    name);
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(descriptor.Get);
                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), name);
                Assert.False(adapter.RequiresInvocationContext, name);
            }

            var buffer = new SharedArrayBuffer(4d);
            var byteLengthGetter = GetAccessorGetter(
                JavaScriptRuntime.SharedArrayBuffer.SharedPrototype, "byteLength");
            Assert.Equal(4d, CallableOperations.Call0(byteLengthGetter, buffer));
        });
    }

    [Fact]
    public void DataViewAccessorsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var name in new[] { "buffer", "byteLength", "byteOffset" })
            {
                Assert.True(
                    PropertyDescriptorStore.TryGetOwn(JavaScriptRuntime.DataView.Prototype, name, out var descriptor),
                    name);
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(descriptor.Get);
                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), name);
                Assert.False(adapter.RequiresInvocationContext, name);
            }

            var buffer = new ArrayBuffer(8d);
            var view = new DataView(buffer);
            var byteLengthGetter = GetAccessorGetter(JavaScriptRuntime.DataView.Prototype, "byteLength");
            Assert.Equal(8d, CallableOperations.Call0(byteLengthGetter, view));

            var ex = Assert.Throws<TypeError>(() => CallableOperations.Call0(byteLengthGetter, buffer));
            Assert.Contains("DataView.prototype.byteLength", ex.Message);
        });
    }


    [Fact]
    public void Uint8ArrayBase64AndHexMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            var uint8ArrayConstructor = ObjectRuntime.GetItem(
                JavaScriptRuntime.RuntimeExecutionContext.Current!.GetOrCreateGlobalObject(),
                "Uint8Array");

            var fromHex = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(uint8ArrayConstructor, "fromHex"));
            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(fromHex.Target));
            Assert.False(fromHex.RequiresInvocationContext);

            foreach (var name in new[] { "setFromBase64", "setFromHex", "toBase64", "toHex" })
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.Uint8Array.Prototype, name));
                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), name);
                Assert.False(adapter.RequiresInvocationContext, name);
            }

            var fromHexResult = Assert.IsType<Uint8Array>(CallableOperations.Call1(fromHex, null, "68656c6c6f"));
            Assert.Equal(5d, fromHexResult.length);

            var toHex = ObjectRuntime.GetItem(JavaScriptRuntime.Uint8Array.Prototype, "toHex");
            Assert.Equal("68656c6c6f", CallableOperations.Call0(toHex, fromHexResult));

            var toBase64 = ObjectRuntime.GetItem(JavaScriptRuntime.Uint8Array.Prototype, "toBase64");
            var base64 = Assert.IsType<string>(CallableOperations.Call0(toBase64, fromHexResult));
            Assert.Equal(System.Convert.ToBase64String(new byte[] { 0x68, 0x65, 0x6c, 0x6c, 0x6f }), base64);

            var setFromHex = ObjectRuntime.GetItem(JavaScriptRuntime.Uint8Array.Prototype, "setFromHex");
            var target = new Uint8Array(5d);
            var setResult = CallableOperations.Call1(setFromHex, target, "68656c6c6f");
            Assert.NotNull(setResult);

            var ex = Assert.Throws<TypeError>(() => CallableOperations.Call1(setFromHex, "not an array", "68"));
            Assert.Contains("setFromHex", ex.Message);
        });
    }

    private static object GetAccessorGetter(object target, string name)
    {
        Assert.True(PropertyDescriptorStore.TryGetOwn(target, name, out var descriptor), name);
        Assert.NotNull(descriptor.Get);
        return descriptor.Get!;
    }

    private static T WithRealm<T>(Func<T> body)
    {
        var context = RuntimeExecutionContext.GetOrCreate(
            RuntimeServices.BuildServiceProvider());
        using var scope = context.EnterAsRoot();
        context.GetOrCreateGlobalObject();
        return body();
    }

    private static void WithRealm(Action body)
        => WithRealm(
            () =>
            {
                body();
                return true;
            });
}
