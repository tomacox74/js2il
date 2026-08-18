using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests.WeakMap;

public sealed class BuiltinAdapterRuntimeTests
{
    private static readonly string[] PrototypeMethodNames =
    [
        "delete",
        "get",
        "has",
        "set"
    ];

    [Fact]
    public void WeakMapPrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var methodName in PrototypeMethodNames)
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.WeakMap.Prototype, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }
        });
    }

    [Fact]
    public void WeakMapPrototypeAdapterCanBeCalledFromAnotherRealm()
    {
        var firstRealmHas = WithRealm(
            () => ObjectRuntime.GetItem(JavaScriptRuntime.WeakMap.Prototype, "has")!);

        var result = WithRealm(() =>
        {
            var secondRealmHas =
                ObjectRuntime.GetItem(JavaScriptRuntime.WeakMap.Prototype, "has");
            Assert.NotSame(firstRealmHas, secondRealmHas);

            var key = new object();
            var weakMap = new JavaScriptRuntime.WeakMap();
            weakMap.set(key, "value");
            return CallableOperations.Call1(firstRealmHas, weakMap, key);
        });

        Assert.Equal(true, result);
    }

    [Fact]
    public void WeakMapPrototypeMethodsRejectIncompatibleReceivers()
    {
        WithRealm(() =>
        {
            var get = ObjectRuntime.GetItem(JavaScriptRuntime.WeakMap.Prototype, "get");
            Assert.Throws<TypeError>(
                () => CallableOperations.Call1(get, "not a weak map", new object()));
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
