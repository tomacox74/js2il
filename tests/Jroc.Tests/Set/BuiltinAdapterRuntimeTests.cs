using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests.Set;

public sealed class BuiltinAdapterRuntimeTests
{
    private static readonly string[] PrototypeMethodNames =
    [
        "add",
        "has",
        "delete",
        "clear",
        "entries",
        "forEach",
        "keys",
        "values",
        "difference",
        "intersection",
        "isDisjointFrom",
        "isSubsetOf",
        "isSupersetOf",
        "symmetricDifference",
        "union",
        Symbol.iterator.DebugId
    ];

    [Fact]
    public void SetPrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var methodName in PrototypeMethodNames)
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.Set.Prototype, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }

            Assert.True(
                PropertyDescriptorStore.TryGetOwn(JavaScriptRuntime.Set.Prototype, "size", out var sizeDescriptor));
            var sizeGetter = Assert.IsType<BuiltinDelegateFunctionAdapter>(sizeDescriptor.Get);
            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(sizeGetter.Target));
            Assert.False(sizeGetter.RequiresInvocationContext);
        });
    }

    [Fact]
    public void SetPrototypeKeysValuesAndIteratorShareTheirCanonicalAdapter()
    {
        WithRealm(() =>
        {
            var values = ObjectRuntime.GetItem(JavaScriptRuntime.Set.Prototype, "values");
            Assert.Same(values, ObjectRuntime.GetItem(JavaScriptRuntime.Set.Prototype, "keys"));
            Assert.Same(values, ObjectRuntime.GetItem(JavaScriptRuntime.Set.Prototype, Symbol.iterator.DebugId));
        });
    }

    [Fact]
    public void SetPrototypeAdapterCanBeCalledFromAnotherRealm()
    {
        var firstRealmHas = WithRealm(
            () => ObjectRuntime.GetItem(JavaScriptRuntime.Set.Prototype, "has")!);

        var result = WithRealm(() =>
        {
            var secondRealmHas =
                ObjectRuntime.GetItem(JavaScriptRuntime.Set.Prototype, "has");
            Assert.NotSame(firstRealmHas, secondRealmHas);

            var set = new JavaScriptRuntime.Set();
            set.add("value");
            return CallableOperations.Call1(firstRealmHas, set, "value");
        });

        Assert.Equal(true, result);
    }

    [Fact]
    public void SetPrototypeMethodsRejectIncompatibleReceivers()
    {
        WithRealm(() =>
        {
            var has = ObjectRuntime.GetItem(JavaScriptRuntime.Set.Prototype, "has");
            Assert.Throws<TypeError>(
                () => CallableOperations.Call1(has, "not a set", "value"));
        });
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
