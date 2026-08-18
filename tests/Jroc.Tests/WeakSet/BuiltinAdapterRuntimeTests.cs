using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests.WeakSet;

public sealed class BuiltinAdapterRuntimeTests
{
    private static readonly string[] PrototypeMethodNames =
    [
        "add",
        "delete",
        "has"
    ];

    [Fact]
    public void WeakSetPrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var methodName in PrototypeMethodNames)
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.WeakSet.Prototype, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }
        });
    }

    [Fact]
    public void WeakSetPrototypeAdapterCanBeCalledFromAnotherRealm()
    {
        var firstRealmHas = WithRealm(
            () => ObjectRuntime.GetItem(JavaScriptRuntime.WeakSet.Prototype, "has")!);

        var result = WithRealm(() =>
        {
            var secondRealmHas =
                ObjectRuntime.GetItem(JavaScriptRuntime.WeakSet.Prototype, "has");
            Assert.NotSame(firstRealmHas, secondRealmHas);

            var value = new object();
            var weakSet = new JavaScriptRuntime.WeakSet();
            weakSet.add(value);
            return CallableOperations.Call1(firstRealmHas, weakSet, value);
        });

        Assert.Equal(true, result);
    }

    [Fact]
    public void WeakSetPrototypeMethodsRejectIncompatibleReceivers()
    {
        WithRealm(() =>
        {
            var has = ObjectRuntime.GetItem(JavaScriptRuntime.WeakSet.Prototype, "has");
            Assert.Throws<TypeError>(
                () => CallableOperations.Call1(has, "not a weak set", new object()));
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
