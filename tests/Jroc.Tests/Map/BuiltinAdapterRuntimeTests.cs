using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests.Map;

public sealed class BuiltinAdapterRuntimeTests
{
    private static readonly string[] PrototypeMethodNames =
    [
        "set",
        "get",
        "has",
        "delete",
        "clear",
        "keys",
        "values",
        "entries",
        "forEach",
        "getOrInsert",
        "getOrInsertComputed",
        Symbol.iterator.DebugId
    ];

    [Fact]
    public void MapPrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var methodName in PrototypeMethodNames)
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.Map.Prototype, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }

            Assert.True(
                PropertyDescriptorStore.TryGetOwn(JavaScriptRuntime.Map.Prototype, "size", out var sizeDescriptor));
            var sizeGetter = Assert.IsType<BuiltinDelegateFunctionAdapter>(sizeDescriptor.Get);
            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(sizeGetter.Target));
            Assert.False(sizeGetter.RequiresInvocationContext);
        });
    }

    [Fact]
    public void MapPrototypeEntriesAndIteratorShareTheirCanonicalAdapter()
    {
        WithRealm(() =>
        {
            Assert.Same(
                ObjectRuntime.GetItem(JavaScriptRuntime.Map.Prototype, "entries"),
                ObjectRuntime.GetItem(JavaScriptRuntime.Map.Prototype, Symbol.iterator.DebugId));
        });
    }

    [Fact]
    public void MapPrototypeAdapterCanBeCalledFromAnotherRealm()
    {
        var firstRealmHas = WithRealm(
            () => ObjectRuntime.GetItem(JavaScriptRuntime.Map.Prototype, "has")!);

        var result = WithRealm(() =>
        {
            var secondRealmHas =
                ObjectRuntime.GetItem(JavaScriptRuntime.Map.Prototype, "has");
            Assert.NotSame(firstRealmHas, secondRealmHas);

            var map = new JavaScriptRuntime.Map();
            map.set("key", "value");
            return CallableOperations.Call1(firstRealmHas, map, "key");
        });

        Assert.Equal(true, result);
    }

    [Fact]
    public void MapPrototypeMethodsRejectIncompatibleReceivers()
    {
        WithRealm(() =>
        {
            var get = ObjectRuntime.GetItem(JavaScriptRuntime.Map.Prototype, "get");
            Assert.Throws<TypeError>(
                () => CallableOperations.Call1(get, "not a map", "key"));
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
