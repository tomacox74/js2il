using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests.Array;

public sealed class BuiltinAdapterRuntimeTests
{
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
