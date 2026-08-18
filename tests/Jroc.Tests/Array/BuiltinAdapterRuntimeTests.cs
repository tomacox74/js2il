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
