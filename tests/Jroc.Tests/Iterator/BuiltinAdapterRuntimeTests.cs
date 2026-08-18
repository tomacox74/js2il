using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests.Iterator;

public sealed class BuiltinAdapterRuntimeTests
{
    private static readonly string[] FixedArityPrototypeMethodNames =
    [
        "next",
        "return",
        "every",
        "filter",
        "find",
        "flatMap",
        "forEach",
        "map",
        "some",
        "toArray",
        Symbol.iterator.DebugId
    ];

    private static readonly string[] VariadicPrototypeMethodNames =
    [
        "drop",
        "reduce",
        "take"
    ];

    [Fact]
    public void IteratorPrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var methodName in FixedArityPrototypeMethodNames)
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.Iterator.Prototype, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }

            foreach (var methodName in VariadicPrototypeMethodNames)
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.Iterator.Prototype, methodName));

                // Count-sensitive methods (arg-count changes semantics, e.g. reduce's
                // "has initial value" and drop/take's "requires a limit") must remain
                // variadic rather than a fixed-arity BuiltinFunctionN.
                Assert.IsType<BuiltinFunctionVariadic>(adapter.Target);
                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }
        });
    }

    [Fact]
    public void IteratorHelperPrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var methodName in new[] { "next", "return", Symbol.iterator.DebugId })
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.Iterator.HelperPrototype, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }
        });
    }

    [Fact]
    public void IteratorPrototypeNextThrowsOnIncompatibleReceiver()
    {
        WithRealm(() =>
        {
            var next = ObjectRuntime.GetItem(JavaScriptRuntime.Iterator.Prototype, "next");
            Assert.Throws<TypeError>(() => CallableOperations.Call0(next, new JsObject()));
        });
    }

    [Fact]
    public void IteratorPrototypeAdapterCanBeCalledFromAnotherRealm()
    {
        var firstRealmNext = WithRealm(
            () => ObjectRuntime.GetItem(JavaScriptRuntime.Iterator.Prototype, "next")!);

        var result = WithRealm(() =>
        {
            var secondRealmNext =
                ObjectRuntime.GetItem(JavaScriptRuntime.Iterator.Prototype, "next");
            Assert.NotSame(firstRealmNext, secondRealmNext);

            var receiver = JavaScriptRuntime.Iterator.From(
                new JavaScriptRuntime.Array(new object[] { 42d }));
            return CallableOperations.Call0(firstRealmNext, receiver);
        });

        var resultObject = Assert.IsType<IteratorResultObject>(result);
        Assert.Equal(42d, ObjectRuntime.GetItem(resultObject, "value"));
        Assert.Equal(false, ObjectRuntime.GetItem(resultObject, "done"));
    }

    private static T WithRealm<T>(Func<T> body)
    {
        var context = RuntimeExecutionContext.GetOrCreate(
            RuntimeServices.BuildServiceProvider());
        using var scope = context.EnterAsRoot();
        _ = GlobalThis.globalThis;
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
