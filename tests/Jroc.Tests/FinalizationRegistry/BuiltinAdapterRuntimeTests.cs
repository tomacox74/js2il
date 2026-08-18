using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests.FinalizationRegistry;

public sealed class BuiltinAdapterRuntimeTests
{
    private static readonly string[] PrototypeMethodNames =
    [
        "register",
        "unregister"
    ];

    [Fact]
    public void FinalizationRegistryPrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var methodName in PrototypeMethodNames)
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.FinalizationRegistry.Prototype, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }
        });
    }

    [Fact]
    public void FinalizationRegistryPrototypeAdapterCanBeCalledFromAnotherRealm()
    {
        var firstRealmRegister = WithRealm(
            () => ObjectRuntime.GetItem(JavaScriptRuntime.FinalizationRegistry.Prototype, "register")!);

        WithRealm(() =>
        {
            var secondRealmRegister =
                ObjectRuntime.GetItem(JavaScriptRuntime.FinalizationRegistry.Prototype, "register");
            Assert.NotSame(firstRealmRegister, secondRealmRegister);

            BuiltinFunction1 noopCleanupCallback = static (_, _) => null;
            var registry = new JavaScriptRuntime.FinalizationRegistry(
                new BuiltinDelegateFunctionAdapter(noopCleanupCallback));
            var target = new object();
            CallableOperations.Call2(firstRealmRegister, registry, target, "held-value");
        });
    }

    [Fact]
    public void FinalizationRegistryPrototypeMethodsRejectIncompatibleReceivers()
    {
        WithRealm(() =>
        {
            var unregister =
                ObjectRuntime.GetItem(JavaScriptRuntime.FinalizationRegistry.Prototype, "unregister");
            Assert.Throws<TypeError>(
                () => CallableOperations.Call1(unregister, "not a registry", new object()));
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
