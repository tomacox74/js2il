using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests.Runtime;

/// <summary>
/// Focused coverage for issue #1895: Boolean/BigInt/Number/Symbol prototype (and related static)
/// adapters must be wired through the explicit-receiver ABI (<see cref="BuiltinFunctionDelegates"/>)
/// instead of ambient <c>RuntimeServices.GetCurrentThis()</c> reads.
/// </summary>
public sealed class PrimitivePrototypeBuiltinAdapterRuntimeTests
{
    [Fact]
    public void BooleanPrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var methodName in new[] { "toString", "valueOf" })
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.GlobalThis.BooleanPrototypeValue, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }

            var toString = ObjectRuntime.GetItem(JavaScriptRuntime.GlobalThis.BooleanPrototypeValue, "toString");
            var valueOf = ObjectRuntime.GetItem(JavaScriptRuntime.GlobalThis.BooleanPrototypeValue, "valueOf");

            Assert.Equal("true", CallableOperations.Call0(toString, true));
            Assert.Equal(false, CallableOperations.Call0(valueOf, false));
        });
    }

    [Fact]
    public void BigIntStaticAndPrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            var bigIntFunction = JavaScriptRuntime.GlobalThis.BigInt;

            foreach (var methodName in new[] { "asIntN", "asUintN" })
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(bigIntFunction, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }

            foreach (var methodName in new[] { "toString", "toLocaleString", "valueOf" })
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.GlobalThis.BigIntPrototypeValue, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }

            var asUintN = ObjectRuntime.GetItem(bigIntFunction, "asUintN");
            var result = CallableOperations.Call2(asUintN, null, 8d, System.Numerics.BigInteger.MinusOne);
            Assert.Equal(new System.Numerics.BigInteger(255), result);

            var toString = ObjectRuntime.GetItem(JavaScriptRuntime.GlobalThis.BigIntPrototypeValue, "toString");
            Assert.Equal("42", CallableOperations.Call0(toString, new System.Numerics.BigInteger(42)));
        });
    }

    [Fact]
    public void NumberPrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var methodName in new[]
                     {
                         "toString", "valueOf", "toExponential", "toFixed", "toLocaleString", "toPrecision"
                     })
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.GlobalThis.NumberPrototypeValue, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }

            var toFixed = ObjectRuntime.GetItem(JavaScriptRuntime.GlobalThis.NumberPrototypeValue, "toFixed");
            Assert.Equal("3.14", CallableOperations.Call1(toFixed, 3.14159d, 2d));

            var valueOf = ObjectRuntime.GetItem(JavaScriptRuntime.GlobalThis.NumberPrototypeValue, "valueOf");
            Assert.Equal(7d, CallableOperations.Call0(valueOf, 7d));
        });
    }

    [Fact]
    public void SymbolCallAndPrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            var globalObject = RuntimeExecutionContext.Current!.GetOrCreateGlobalObject();
            var symbolAdapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(globalObject, "Symbol"));

            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(symbolAdapter.Target));
            Assert.False(symbolAdapter.RequiresInvocationContext);

            var created = CallableOperations.Call1(symbolAdapter, null, "d");
            var symbol = Assert.IsType<JavaScriptRuntime.Symbol>(created);
            Assert.Equal("d", symbol.Description);

            foreach (var methodName in new[] { "toString", "valueOf" })
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(JavaScriptRuntime.GlobalThis.SymbolPrototypeValue, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }

            Assert.True(
                PropertyDescriptorStore.TryGetOwn(
                    JavaScriptRuntime.GlobalThis.SymbolPrototypeValue,
                    JavaScriptRuntime.Symbol.toPrimitive.DebugId,
                    out var toPrimitiveDescriptor));
            var toPrimitiveAdapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(toPrimitiveDescriptor.Value);
            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(toPrimitiveAdapter.Target));
            Assert.False(toPrimitiveAdapter.RequiresInvocationContext);

            var toString = ObjectRuntime.GetItem(JavaScriptRuntime.GlobalThis.SymbolPrototypeValue, "toString");
            Assert.Equal("Symbol(d)", CallableOperations.Call0(toString, symbol));
        });
    }

    private static T WithRealm<T>(Func<T> body)
    {
        var context = RuntimeExecutionContext.GetOrCreate(
            RuntimeServices.BuildServiceProvider());
        using var scope = context.EnterAsRoot();
        context.GetOrCreateGlobalObject();
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
