using JavaScriptRuntime;

namespace Jroc.Tests.Function;

public sealed class BuiltinAdapterRuntimeTests
{
    private static readonly string[] FixedArityPrototypeMethodNames = ["apply", "toString"];

    private static readonly string[] VariadicPrototypeMethodNames = ["call", "bind"];

    [Fact]
    public void FunctionPrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var methodName in FixedArityPrototypeMethodNames)
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.Function.Prototype, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
            }

            foreach (var methodName in VariadicPrototypeMethodNames)
            {
                var value = ObjectRuntime.GetItem(JavaScriptRuntime.Function.Prototype, methodName);
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(value);

                // call/bind forward an arbitrary number of trailing arguments and must
                // remain variadic rather than a fixed-arity BuiltinFunctionN.
                Assert.IsType<BuiltinFunctionVariadic>(adapter.Target);
                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
            }
        });
    }

    [Fact]
    public void FunctionPrototypeApplyThrowsOnNonFunctionReceiver()
    {
        WithRealm(() =>
        {
            var apply = ObjectRuntime.GetItem(JavaScriptRuntime.Function.Prototype, "apply");
            Assert.Throws<TypeError>(() => CallableOperations.Call0(apply, new JsObject()));
        });
    }

    [Fact]
    public void FunctionPrototypeCallForwardsAllTrailingArguments()
    {
        WithRealm(() =>
        {
            BuiltinFunction3 sum = static (thisArgument, a, b, c)
                => (double)(a ?? 0d) + (double)(b ?? 0d) + (double)(c ?? 0d);
            var sumFunction = BuiltinDelegateFunctionAdapter.FromDelegate(sum);
            var call = ObjectRuntime.GetItem(JavaScriptRuntime.Function.Prototype, "call");

            var result = CallableOperations.Call4(call, sumFunction, null, 1d, 2d, 3d);

            Assert.Equal(6d, result);
        });
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
