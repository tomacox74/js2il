using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class AbortBuiltinAdapterRuntimeTests
{
    [Fact]
    public void AbortControllerPrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            var abortAdapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(AbortController.Prototype, "abort"));
            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(abortAdapter.Target));
            Assert.False(abortAdapter.RequiresInvocationContext);

            var signalAdapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                GetAccessorGetter(AbortController.Prototype, "signal"));
            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(signalAdapter.Target));
            Assert.False(signalAdapter.RequiresInvocationContext);
        });
    }

    [Fact]
    public void AbortSignalPrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var methodName in new[] { "addEventListener", "removeEventListener" })
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(AbortSignal.Prototype, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }

            foreach (var accessorName in new[] { "aborted", "reason" })
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    GetAccessorGetter(AbortSignal.Prototype, accessorName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), accessorName);
                Assert.False(adapter.RequiresInvocationContext, accessorName);
            }
        });
    }

    [Fact]
    public void AbortControllerPrototypeAbortThrowsOnIncompatibleReceiver()
    {
        WithRealm(() =>
        {
            var abort = ObjectRuntime.GetItem(AbortController.Prototype, "abort");
            Assert.Throws<TypeError>(() => CallableOperations.Call0(abort, new JsObject()));
        });
    }

    [Fact]
    public void AbortSignalPrototypeAddEventListenerThrowsOnIncompatibleReceiver()
    {
        WithRealm(() =>
        {
            var addEventListener = ObjectRuntime.GetItem(AbortSignal.Prototype, "addEventListener");
            Assert.Throws<TypeError>(() => CallableOperations.Call0(addEventListener, new JsObject()));
        });
    }

    private static object? GetAccessorGetter(JsObject prototype, string accessorName)
    {
        var lookup = prototype.GetOwnPropertyDescriptor(accessorName, out var descriptor);
        Assert.Equal(PropertyDescriptorLookup.Found, lookup);
        Assert.Equal(JsPropertyDescriptorKind.Accessor, descriptor.Kind);
        return descriptor.Get;
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
