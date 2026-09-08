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

    private static readonly string[] CallbackMethodNames = ["map", "filter", "every", "some", "find", "findIndex"];

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

            foreach (var name in ZeroArgMethodNames.Concat(VariadicMethodNames).Concat(CallbackMethodNames))
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

            foreach (var name in CallbackMethodNames)
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(typedArrayPrototype, name));
                Assert.IsType<BuiltinFunction2>(adapter.Target);
                Assert.False(CallableOperations.IsConstructor(adapter));
                Assert.Equal(1d, ObjectRuntime.GetItem(adapter, "length"));
            }
        });
    }

    [Theory]
    [InlineData("map")]
    [InlineData("filter")]
    public void TypedArrayCallbackAdaptersPreserveReceiverArgumentsAndThisArg(string name)
    {
        WithRealm(() =>
        {
            var source = new Uint8Array(new object?[] { 1d, 2d });
            var thisArg = new JsObject();
            var calls = 0;
            var callback = new BuiltinDelegateFunctionAdapter((BuiltinFunction3)((receiver, value, index, array) =>
            {
                Assert.Same(thisArg, receiver);
                Assert.Same(source, array);
                Assert.Equal((double)calls++, index);
                return name == "map" ? (object)((double)value! + 10d) : (double)value! > 1d;
            }));
            var method = ObjectRuntime.GetItem(RuntimeIntrinsics.Current.TypedArrayPrototype, name);
            Assert.Same(method, ObjectRuntime.GetItem(source, name));

            var result = Assert.IsType<Uint8Array>(CallableOperations.Call2(method, source, callback, thisArg));
            Assert.Equal(2, calls);
            Assert.Equal(name == "map" ? 2d : 1d, result.length);
            Assert.Equal(name == "map" ? 11d : 2d, ObjectRuntime.GetItem(result, 0d));
        });
    }

    [Theory]
    [InlineData("every", true, 2, true)]
    [InlineData("every", false, 1, false)]
    [InlineData("some", true, 1, true)]
    [InlineData("some", false, 2, false)]
    public void TypedArrayPredicateAdaptersMatchDirectCalls(
        string name, bool callbackResult, int expectedCalls, bool expectedResult)
    {
        WithRealm(() =>
        {
            var source = new Uint8Array(new object?[] { 1d, 2d });
            var thisArg = new JsObject();
            var calls = 0;
            var callback = new BuiltinDelegateFunctionAdapter((BuiltinFunction3)((receiver, value, index, array) =>
            {
                Assert.Same(thisArg, receiver);
                Assert.Same(source, array);
                Assert.Equal((double)calls, index);
                Assert.Equal((double)++calls, value);
                return callbackResult;
            }));
            var method = ObjectRuntime.GetItem(RuntimeIntrinsics.Current.TypedArrayPrototype, name);
            Assert.Same(method, ObjectRuntime.GetItem(source, name));

            Assert.Equal(expectedResult, Assert.IsType<bool>(CallableOperations.Call2(method, source, callback, thisArg)));
            Assert.Equal(expectedCalls, calls);
            calls = 0;
            var args = new object?[] { callback, thisArg };
            Assert.Equal(expectedResult, name == "every" ? source.every(args) : source.some(args));
            Assert.Equal(expectedCalls, calls);
        });
    }

    [Theory]
    [InlineData("find", true, 2d)]
    [InlineData("find", false, null)]
    [InlineData("findIndex", true, 1d)]
    [InlineData("findIndex", false, -1d)]
    public void TypedArraySearchAdaptersMatchDirectCalls(string name, bool match, object? expectedResult)
    {
        WithRealm(() =>
        {
            var source = new Uint8Array(new object?[] { 1d, 2d, 3d });
            var thisArg = new JsObject();
            var calls = 0;
            var callback = new BuiltinDelegateFunctionAdapter((BuiltinFunction3)((receiver, value, index, array) =>
            {
                Assert.Same(thisArg, receiver);
                Assert.Same(source, array);
                Assert.Equal((double)calls, index);
                Assert.Equal((double)++calls, value);
                return match && (double)value! == 2d;
            }));
            var method = ObjectRuntime.GetItem(RuntimeIntrinsics.Current.TypedArrayPrototype, name);
            Assert.Same(method, ObjectRuntime.GetItem(source, name));

            Assert.Equal(expectedResult, CallableOperations.Call2(method, source, callback, thisArg));
            Assert.Equal(match ? 2 : 3, calls);
            calls = 0;
            var args = new object?[] { callback, thisArg };
            Assert.Equal(expectedResult, name == "find" ? source.find(args) : source.findIndex(args));
            Assert.Equal(match ? 2 : 3, calls);
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

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TypedArrayFromDirectAndAdapterCallsInterleaveReadsMappingAndConversion(bool direct)
    {
        WithRealm(() =>
        {
            var trace = new System.Collections.Generic.List<string>();
            var source = new JsObject();
            ObjectRuntime.SetItem(source, "length", 2d);
            for (var index = 0; index < 2; index++)
            {
                var currentIndex = index;
                PropertyDescriptorStore.DefineOrUpdate(source, index.ToString(), new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Accessor,
                    Get = new BuiltinDelegateFunctionAdapter((BuiltinFunction0)(_ =>
                    {
                        trace.Add($"get{currentIndex}");
                        return (double)currentIndex;
                    }))
                });
            }

            var mapper = new BuiltinDelegateFunctionAdapter((BuiltinFunction2)((_, value, index) =>
            {
                trace.Add($"map{index}");
                var mapped = new JsObject();
                ObjectRuntime.SetItem(mapped, "valueOf",
                    new BuiltinDelegateFunctionAdapter((BuiltinFunction0)(_ =>
                    {
                        trace.Add($"number{index}");
                        return value;
                    })));
                return mapped;
            }));
            var constructor = ObjectRuntime.GetItem(GlobalThis.globalThis, "Uint8Array");
            var from = ObjectRuntime.GetItem(constructor, "from");
            var result = direct
                ? Uint8Array.from(source, mapper)
                : Assert.IsType<Uint8Array>(CallableOperations.Call2(from, constructor, source, mapper));

            Assert.Equal(new[] { "get0", "map0", "number0", "get1", "map1", "number1" }, trace);
            Assert.Equal(2d, result.length);
            Assert.Equal(1d, ObjectRuntime.GetItem(result, 1d));

            trace.Clear();
            Assert.Throws<TypeError>(() =>
            {
                if (direct)
                {
                    Uint8Array.from(source, JsNull.Null);
                }
                else
                {
                    CallableOperations.Call2(from, constructor, source, JsNull.Null);
                }
            });
            Assert.Empty(trace);
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
    public void TypedArrayIntrinsicInitializationPreservesRealmIsolationAndIdentity()
    {
        var firstServices = RuntimeServices.BuildServiceProvider();
        var secondServices = RuntimeServices.BuildServiceProvider();
        var firstContext = RuntimeExecutionContext.GetOrCreate(firstServices);
        var secondContext = RuntimeExecutionContext.GetOrCreate(secondServices);

        object[] CaptureSurface()
        {
            var global = GlobalThis.globalThis;
            var prototype = JavaScriptRuntime.RuntimeIntrinsics.Current.TypedArrayPrototype;
            var constructor = ObjectRuntime.GetItem(prototype, "constructor")!;
            var values = ObjectRuntime.GetItem(prototype, "values")!;
            Assert.Same(prototype, ObjectRuntime.GetItem(constructor, "prototype"));
            Assert.Same(values, ObjectRuntime.GetItem(prototype, Symbol.iterator));

            var surface = new System.Collections.Generic.List<object>
            {
                prototype,
                constructor,
                values,
                ObjectRuntime.GetItem(constructor, "from")!,
                ObjectRuntime.GetItem(constructor, "of")!,
                GetAccessorGetter(prototype, "length")
            };
            foreach (var name in new[]
            {
                "Float64Array", "Float32Array", "Int32Array", "Int16Array", "Int8Array",
                "Uint32Array", "Uint16Array", "Uint8Array", "Uint8ClampedArray",
                "BigInt64Array", "BigUint64Array"
            })
            {
                var concreteConstructor = ObjectRuntime.GetItem(global, name)!;
                var concretePrototype = ObjectRuntime.GetItem(concreteConstructor, "prototype")!;
                Assert.Same(constructor, PrototypeChain.GetPrototypeOrNull(concreteConstructor));
                Assert.Same(prototype, PrototypeChain.GetPrototypeOrNull(concretePrototype));
                Assert.Same(concreteConstructor, ObjectRuntime.GetItem(concretePrototype, "constructor"));
                surface.Add(concreteConstructor);
                surface.Add(concretePrototype);
            }

            surface.Add(ObjectRuntime.GetItem(GlobalThis.Uint8Array, "fromHex")!);
            surface.Add(ObjectRuntime.GetItem(JavaScriptRuntime.Uint8Array.Prototype, "toHex")!);
            return surface.ToArray();
        }

        try
        {
            object[] firstSurface;
            using (firstContext.EnterAsRoot())
            {
                firstSurface = CaptureSurface();
                ObjectRuntime.SetProperty(firstSurface[0], "realmMutation", "first");
                ObjectRuntime.SetProperty(firstSurface[2], "realmMutation", "first");
                ObjectRuntime.SetProperty(JavaScriptRuntime.Uint8Array.Prototype, "realmMutation", "first");
            }

            using (secondContext.EnterAsRoot())
            {
                var secondSurface = CaptureSurface();
                Assert.Equal(firstSurface.Length, secondSurface.Length);
                for (var i = 0; i < firstSurface.Length; i++)
                {
                    Assert.NotSame(firstSurface[i], secondSurface[i]);
                }

                Assert.Null(ObjectRuntime.GetItem(secondSurface[0], "realmMutation"));
                Assert.Null(ObjectRuntime.GetItem(secondSurface[2], "realmMutation"));
                Assert.Null(ObjectRuntime.GetItem(JavaScriptRuntime.Uint8Array.Prototype, "realmMutation"));
            }

            using (firstContext.EnterAsRoot())
            {
                var reenteredSurface = CaptureSurface();
                for (var i = 0; i < firstSurface.Length; i++)
                {
                    Assert.Same(firstSurface[i], reenteredSurface[i]);
                }

                Assert.Equal("first", ObjectRuntime.GetItem(firstSurface[0], "realmMutation"));
                Assert.Equal("first", ObjectRuntime.GetItem(firstSurface[2], "realmMutation"));
                Assert.Equal("first", ObjectRuntime.GetItem(JavaScriptRuntime.Uint8Array.Prototype, "realmMutation"));
            }
        }
        finally
        {
            firstServices.OwningRealm!.Agent.Cluster.Dispose();
            secondServices.OwningRealm!.Agent.Cluster.Dispose();
        }
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
    public void DataViewSetterConvertsValueBeforeRejectingLargeValidIndex()
    {
        WithRealm(() =>
        {
            var view = new DataView(new ArrayBuffer(1d));
            var marker = new InvalidOperationException("value coercion marker");
            var value = new JsObject();
            ObjectRuntime.SetItem(
                value,
                "valueOf",
                new BuiltinDelegateFunctionAdapter((Func<object?>)(() => throw marker)));

            var exception = Assert.Throws<InvalidOperationException>(
                () => view.setInt8(2147483648d, value));

            Assert.Same(marker, exception);
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
