using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;
using System.Reflection;

namespace Jroc.Tests.Array;

public sealed class BuiltinAdapterRuntimeTests
{
    [Theory]
    [InlineData("every", "adapter")]
    [InlineData("every", "args")]
    [InlineData("some", "adapter")]
    [InlineData("some", "args")]
    [InlineData("some", "fixed")]
    public void PredicateDirectAndAdapterCallsPreserveSparseIterationAndValidateEmptyCallbacks(string method, string mode)
    {
        WithRealm(() =>
        {
            var adapter = ObjectRuntime.GetItem(JavaScriptRuntime.Array.Prototype, method);
            bool Invoke(JavaScriptRuntime.Array array, object[] arguments)
            {
                if (mode == "fixed")
                {
                    return arguments.Length > 1
                        ? array.some(arguments[0], arguments[1])
                        : array.some(arguments.Length > 0 ? arguments[0] : null);
                }

                return mode == "args"
                    ? method == "every" ? array.every(arguments) : array.some(arguments)
                    : Assert.IsType<bool>(JavaScriptRuntime.Function.Call(adapter, array, arguments));
            }

            var source = new JavaScriptRuntime.Array { length = 4d };
            source[1] = 7d;
            source[3] = 9d;
            var prototype = new JsObject { ["2"] = 8d };
            PrototypeChain.SetPrototype(prototype, JavaScriptRuntime.Array.Prototype);
            PrototypeChain.SetPrototype(source, prototype);

            var thisArg = new JsObject();
            var calls = 0;
            var callback = new BuiltinDelegateFunctionAdapter((BuiltinFunction3)((receiver, value, index, array) =>
            {
                Assert.Same(thisArg, receiver);
                Assert.Same(source, array);
                Assert.Equal((double)calls + 1d, index);
                Assert.Equal((double)calls + 7d, value);
                calls++;
                source.length = 3d;
                source[4] = 99d;
                return method == "every";
            }));

            Assert.Equal(method == "every", Invoke(source, new object[] { callback, thisArg }));
            Assert.Equal(2, calls);
            Assert.Equal(method == "every", Invoke(new JavaScriptRuntime.Array(), new object[] { callback, thisArg }));
            Assert.Equal(2, calls);
            Assert.Throws<JavaScriptRuntime.TypeError>(() => Invoke(new JavaScriptRuntime.Array(), System.Array.Empty<object>()));
            Assert.Throws<JavaScriptRuntime.TypeError>(() => Invoke(new JavaScriptRuntime.Array(), new object[] { JsNull.Null }));
        });
    }

    [Theory]
    [InlineData("filter", false)]
    [InlineData("filter", true)]
    [InlineData("map", false)]
    [InlineData("map", true)]
    public void MapAndFilterDirectAndAdapterCallsPreserveSpeciesObjectAndSparseSemantics(string method, bool direct)
    {
        WithRealm(() =>
        {
            var source = new JavaScriptRuntime.Array { length = 3d };
            source[1] = 7d;
            var result = new JsObject();
            var prototype = new JsObject();
            var setterCalls = 0;
            var resultIndex = method == "map" ? "1" : "0";
            PropertyDescriptorStore.DefineOrUpdate(prototype, resultIndex, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Configurable = true,
                Set = new BuiltinDelegateFunctionAdapter((BuiltinFunction1)((_, _) =>
                {
                    setterCalls++;
                    return null;
                }))
            });
            PrototypeChain.SetPrototype(result, prototype);

            var trace = new List<string>();
            Func<object[], object?[]?, object?> species = (_, arguments) =>
            {
                trace.Add("construct");
                Assert.Equal(method == "map" ? 3d : 0d, Assert.Single(arguments!));
                return result;
            };
            JavaScriptRuntime.Function.InitializeFunctionInstance(species, 1d, "Species");
            JavaScriptRuntime.Function.MarkConstructible(species);
            ObjectRuntime.SetItem(source, "constructor", new JsObject
            {
                [Symbol.species.DebugId] = BuiltinDelegateFunctionAdapter.FromDelegate(species)
            });

            var thisArg = new JsObject();
            var callback = new BuiltinDelegateFunctionAdapter((BuiltinFunction3)((receiver, value, index, array) =>
            {
                trace.Add("callback");
                Assert.Same(thisArg, receiver);
                Assert.Same(source, array);
                Assert.Equal(1d, index);
                Assert.Equal(7d, value);
                source[3] = 99d;
                return method == "map" ? 14d : true;
            }));
            var adapter = ObjectRuntime.GetItem(JavaScriptRuntime.Array.Prototype, method);
            var actual = direct
                ? method == "map"
                    ? source.map(new object[] { callback, thisArg })
                    : source.filter(new object[] { callback, thisArg })
                : CallableOperations.Call2(adapter, source, callback, thisArg);

            Assert.Same(result, actual);
            Assert.Equal(new[] { "construct", "callback" }, trace);
            Assert.Equal(method == "map" ? 14d : 7d, ObjectRuntime.GetItem(result, resultIndex));
            Assert.True(JavaScriptRuntime.Object.hasOwn(result, resultIndex));
            Assert.False(JavaScriptRuntime.Object.hasOwn(result, method == "map" ? "0" : "1"));
            Assert.False(JavaScriptRuntime.Object.hasOwn(result, "length"));
            Assert.Equal(0, setterCalls);
        });
    }

    [Fact]
    public void ArraySpeciesGetterHasItsOwnReceiverAwareAdapter()
    {
        WithRealm(() =>
        {
            _ = GlobalThis.globalThis;
            Assert.True(PropertyDescriptorStore.TryGetOwn(GlobalThis.Array, Symbol.species.DebugId, out var arraySpecies));
            Assert.True(PropertyDescriptorStore.TryGetOwn(GlobalThis.ArrayBuffer, Symbol.species.DebugId, out var bufferSpecies));
            Assert.NotNull(arraySpecies.Get);
            Assert.NotNull(bufferSpecies.Get);
            var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                BuiltinDelegateFunctionAdapter.NormalizeJavaScriptObject(arraySpecies.Get));

            Assert.NotSame(adapter, BuiltinDelegateFunctionAdapter.NormalizeJavaScriptObject(bufferSpecies.Get));
            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target));
            Assert.False(adapter.RequiresInvocationContext);
        });
    }

    private static readonly string[] FixedArityPrototypeMethodNames =
    [
        "join",
        "toString",
        "indexOf",
        "every",
        "some",
        "filter",
        "map",
        "find",
        "findIndex",
        "includes",
        "findLast",
        "findLastIndex",
        "flat",
        "at",
        "toSorted",
        "with",
        "entries",
        "keys",
        "values",
        Symbol.iterator.DebugId
    ];

    private static readonly string[] VariadicPrototypeMethodNames =
    [
        "push",
        "reduce",
        "reduceRight"
    ];

    [Fact]
    public void ArrayPrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var methodName in FixedArityPrototypeMethodNames)
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.Array.Prototype, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }

            foreach (var methodName in VariadicPrototypeMethodNames)
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.Array.Prototype, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
                Assert.IsType<BuiltinFunctionVariadic>(adapter.Target);
            }
        });
    }

    [Fact]
    public void ArrayPrototypeValuesSharesItsCanonicalAdapterWithSymbolIterator()
    {
        WithRealm(() =>
        {
            Assert.Same(
                ObjectRuntime.GetItem(JavaScriptRuntime.Array.Prototype, "values"),
                ObjectRuntime.GetItem(JavaScriptRuntime.Array.Prototype, Symbol.iterator.DebugId));
        });
    }

    [Fact]
    public void ArrayPrototypeCommonAritiesAllocateNoInvocationState()
    {
        WithRealm(() =>
        {
            var receiver = new JavaScriptRuntime.Array { 1d, 2d, 3d };
            object argument = 1d;
            var at = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(JavaScriptRuntime.Array.Prototype, "at"));

            for (var index = 0; index < 1_000; index++)
            {
                InvokeAt(index % 2);
            }

            var before = GC.GetAllocatedBytesForCurrentThread();
            object? result = null;
            for (var index = 0; index < 10_000; index++)
            {
                for (var arity = 0; arity <= 1; arity++)
                {
                    result = InvokeAt(arity);
                }
            }
            var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.Equal(2d, result);
            Assert.Equal(0, allocated);

            object? InvokeAt(int arity)
                => arity switch
                {
                    0 => CallableOperations.Call0(at, receiver),
                    1 => CallableOperations.Call1(at, receiver, argument),
                    _ => throw new ArgumentOutOfRangeException(nameof(arity))
                };
        });
    }

    [Fact]
    public void ArrayPrototypeAdapterCanBeCalledFromAnotherRealm()
    {
        var firstRealmAt = WithRealm(
            () => ObjectRuntime.GetItem(JavaScriptRuntime.Array.Prototype, "at")!);

        var result = WithRealm(() =>
        {
            var secondRealmAt =
                ObjectRuntime.GetItem(JavaScriptRuntime.Array.Prototype, "at");
            Assert.NotSame(firstRealmAt, secondRealmAt);

            var receiver = new JavaScriptRuntime.Array { 1d, 2d, 3d };
            return CallableOperations.Call1(
                firstRealmAt,
                receiver,
                -1d);
        });

        Assert.Equal(3d, result);
    }

    [Fact]
    public void CrossRealmArrayDisablesDenseGrowthFastPath()
    {
        var receiver = WithRealm(
            () => new JavaScriptRuntime.Array { "tail" });
        var fastPath = typeof(JavaScriptRuntime.Array).GetMethod(
            "CanUseDenseGrowthFastPath",
            BindingFlags.Instance | BindingFlags.NonPublic);

        WithRealm(() =>
        {
            Assert.NotNull(fastPath);
            Assert.False((bool)fastPath.Invoke(receiver, null)!);
        });
    }

    [Fact]
    public void SpliceRejectsRoundedLengthAboveMaximumSafeInteger()
    {
        WithRealm(() =>
        {
            const double maximumSafeInteger = 9007199254740991d;
            var receiver = new JsObject
            {
                ["length"] = maximumSafeInteger
            };
            var splice = ObjectRuntime.GetItem(
                JavaScriptRuntime.Array.Prototype,
                "splice");

            Assert.Throws<TypeError>(
                () => CallableOperations.Call(
                    splice,
                    receiver,
                    new object?[]
                    {
                        maximumSafeInteger - 1d,
                        1d,
                        "first",
                        "second"
                    }));
        });
    }

    [Fact]
    public void ArrayMutatorsThrowWhenProxySetTrapReturnsFalse()
    {
        WithRealm(() =>
        {
            AssertSetTrapFailure(
                "reverse",
                CreateArrayLike(2d, (0d, "right"), (1d, "left")));
            AssertSetTrapFailure("shift", CreateArrayLike(0d));
            AssertSetTrapFailure(
                "sort",
                CreateArrayLike(2d, (0d, 2d), (1d, 1d)));
            AssertSetTrapFailure("splice", CreateArrayLike(0d));
            AssertSetTrapFailure("unshift", CreateArrayLike(0d));
        });
    }

    [Fact]
    public void ArrayMutatorsThrowWhenProxyDeleteTrapReturnsFalse()
    {
        WithRealm(() =>
        {
            AssertDeleteTrapFailure(
                "reverse",
                CreateArrayLike(2d, (1d, "upper")));
            AssertDeleteTrapFailure(
                "shift",
                CreateArrayLike(2d, (0d, "first")));
            AssertDeleteTrapFailure(
                "sort",
                CreateArrayLike(2d, (0d, "only")));
            AssertDeleteTrapFailure(
                "splice",
                CreateArrayLike(1d),
                0d,
                1d);
            AssertDeleteTrapFailure(
                "unshift",
                CreateArrayLike(1d),
                "new");
        });
    }

    private static JsObject CreateArrayLike(
        double length,
        params (double Index, object? Value)[] elements)
    {
        var target = new JsObject
        {
            ["length"] = length
        };
        foreach (var (index, value) in elements)
        {
            ObjectRuntime.SetItem(target, index, value);
        }

        return target;
    }

    private static void AssertSetTrapFailure(
        string methodName,
        JsObject target,
        params object?[] arguments)
    {
        var handler = new JsObject
        {
            ["set"] = (BuiltinFunction4)((_, _, _, _, _) => false)
        };
        var proxy = new JavaScriptRuntime.Proxy(target, handler);
        var method = ObjectRuntime.GetItem(
            JavaScriptRuntime.Array.Prototype,
            methodName);

        Assert.Throws<TypeError>(
            () => CallableOperations.Call(method, proxy, arguments));
    }

    private static void AssertDeleteTrapFailure(
        string methodName,
        JsObject target,
        params object?[] arguments)
    {
        var handler = new JsObject
        {
            ["deleteProperty"] = (BuiltinFunction2)((_, _, _) => false)
        };
        var proxy = new JavaScriptRuntime.Proxy(target, handler);
        var method = ObjectRuntime.GetItem(
            JavaScriptRuntime.Array.Prototype,
            methodName);

        Assert.Throws<TypeError>(
            () => CallableOperations.Call(method, proxy, arguments));
    }

    private static T WithRealm<T>(Func<T> body)
    {
        var context = RuntimeExecutionContext.GetOrCreate(
            RuntimeServices.BuildServiceProvider());
        using var scope = context.EnterAsRoot();
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
