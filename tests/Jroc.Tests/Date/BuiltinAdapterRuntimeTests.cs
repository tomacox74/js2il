using System.Linq;
using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests.Date;

/// <summary>
/// Focused coverage for issue #1895: <c>Date</c> prototype getters/setters, the
/// <c>[Symbol.toPrimitive]</c> adapter, and the constructor's static <c>now</c>/<c>parse</c>/
/// <c>UTC</c> methods must be wired through the explicit-receiver ABI
/// (<see cref="BuiltinFunctionDelegates"/>) instead of ambient
/// <c>RuntimeServices.GetCurrentThis()</c> reads.
/// </summary>
public sealed class BuiltinAdapterRuntimeTests
{
    private static readonly string[] ZeroArgGetterNames =
    [
        "getDate", "getDay", "getFullYear", "getHours", "getMilliseconds", "getMinutes",
        "getMonth", "getSeconds", "getTime", "getTimezoneOffset",
        "getUTCDate", "getUTCDay", "getUTCFullYear", "getUTCHours", "getUTCMilliseconds",
        "getUTCMinutes", "getUTCMonth", "getUTCSeconds",
        "toDateString", "toISOString", "toLocaleDateString", "toLocaleString",
        "toLocaleTimeString", "toString", "toTimeString", "toUTCString", "valueOf"
    ];

    private static readonly string[] SetterNames =
    [
        "setDate", "setFullYear", "setHours", "setMilliseconds", "setMinutes", "setMonth",
        "setSeconds", "setTime", "setUTCDate", "setUTCFullYear", "setUTCHours",
        "setUTCMilliseconds", "setUTCMinutes", "setUTCMonth", "setUTCSeconds"
    ];

    [Fact]
    public void DatePrototypeMembersUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            var datePrototype = JavaScriptRuntime.GlobalThis.DatePrototypeValue;

            foreach (var name in ZeroArgGetterNames.Concat(SetterNames).Append("toJSON"))
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(datePrototype, name));
                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), name);
                Assert.False(adapter.RequiresInvocationContext, name);
            }

            Assert.True(
                PropertyDescriptorStore.TryGetOwn(
                    datePrototype,
                    JavaScriptRuntime.Symbol.toPrimitive.DebugId,
                    out var toPrimitiveDescriptor));
            var toPrimitiveAdapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(toPrimitiveDescriptor.Value);
            Assert.True(BuiltinFunctionDelegates.IsReceiverAware(toPrimitiveAdapter.Target));
            Assert.False(toPrimitiveAdapter.RequiresInvocationContext);
        });
    }

    [Fact]
    public void DatePrototypeGettersAndSettersPreserveFunctionalBehavior()
    {
        WithRealm(() =>
        {
            var datePrototype = JavaScriptRuntime.GlobalThis.DatePrototypeValue;
            var date = new JavaScriptRuntime.Date(0d);

            var getUTCFullYear = ObjectRuntime.GetItem(datePrototype, "getUTCFullYear");
            Assert.Equal(1970d, CallableOperations.Call0(getUTCFullYear, date));

            var setUTCFullYear = ObjectRuntime.GetItem(datePrototype, "setUTCFullYear");
            CallableOperations.Call3(setUTCFullYear, date, 2000d, 0d, 1d);
            Assert.Equal(2000d, CallableOperations.Call0(getUTCFullYear, date));

            var setUTCHours = ObjectRuntime.GetItem(datePrototype, "setUTCHours");
            CallableOperations.Call4(setUTCHours, date, 12d, 30d, 15d, 500d);

            var getUTCHours = ObjectRuntime.GetItem(datePrototype, "getUTCHours");
            var getUTCMinutes = ObjectRuntime.GetItem(datePrototype, "getUTCMinutes");
            Assert.Equal(12d, CallableOperations.Call0(getUTCHours, date));
            Assert.Equal(30d, CallableOperations.Call0(getUTCMinutes, date));

            var toISOString = ObjectRuntime.GetItem(datePrototype, "toISOString");
            Assert.Equal("2000-01-01T12:30:15.500Z", CallableOperations.Call0(toISOString, date));
        });
    }

    [Fact]
    public void DatePrototypeMembersRejectIncompatibleReceivers()
    {
        WithRealm(() =>
        {
            var datePrototype = JavaScriptRuntime.GlobalThis.DatePrototypeValue;
            var getTime = ObjectRuntime.GetItem(datePrototype, "getTime");

            var ex = Assert.Throws<TypeError>(() => CallableOperations.Call0(getTime, "not a date"));
            Assert.Contains("Date.prototype method called on incompatible receiver", ex.Message);
        });
    }

    [Fact]
    public void DateSymbolToPrimitiveUsesReceiverAwareAdapter()
    {
        WithRealm(() =>
        {
            var datePrototype = JavaScriptRuntime.GlobalThis.DatePrototypeValue;
            Assert.True(
                PropertyDescriptorStore.TryGetOwn(
                    datePrototype,
                    JavaScriptRuntime.Symbol.toPrimitive.DebugId,
                    out var descriptor));
            var toPrimitive = descriptor.Value!;

            var date = new JavaScriptRuntime.Date(0d);

            Assert.Equal(0d, CallableOperations.Call1(toPrimitive, date, "number"));
            Assert.IsType<string>(CallableOperations.Call1(toPrimitive, date, "string"));
            Assert.IsType<string>(CallableOperations.Call1(toPrimitive, date, "default"));
        });
    }

    [Fact]
    public void DateConstructorStaticsUseReceiverAwareAdaptersAndIgnoreReceiver()
    {
        WithRealm(() =>
        {
            foreach (var name in new[] { "now", "parse", "UTC" })
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(typeof(JavaScriptRuntime.Date), name));
                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), name);
                Assert.False(adapter.RequiresInvocationContext, name);
            }

            var utc = ObjectRuntime.GetItem(typeof(JavaScriptRuntime.Date), "UTC");
            var arguments = new object?[] { 2000d, 0d, 1d, 0d, 0d, 0d, 0d };

            var resultWithNullReceiver = CallableOperations.Call(utc, null, arguments);
            var resultWithArbitraryReceiver = CallableOperations.Call(utc, "ignored receiver", arguments);

            Assert.Equal(resultWithNullReceiver, resultWithArbitraryReceiver);
            Assert.Equal(946684800000d, resultWithNullReceiver);

            var parse = ObjectRuntime.GetItem(typeof(JavaScriptRuntime.Date), "parse");
            Assert.Equal(
                CallableOperations.Call1(parse, null, "1970-01-01T00:00:00.000Z"),
                CallableOperations.Call1(parse, "ignored", "1970-01-01T00:00:00.000Z"));
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
