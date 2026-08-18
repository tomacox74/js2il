using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests.Object;

/// <summary>
/// Verifies the Object constructor statics and Object.prototype methods migrated to the
/// explicit-receiver BuiltinFunction0..5/BuiltinFunctionVariadic ABI (issue #1895) no longer
/// require ambient invocation state.
/// </summary>
public sealed class BuiltinAdapterRuntimeTests
{
    private static readonly string[] ConstructorStaticMethodNames =
    [
        "assign",
        "create",
        "defineProperties",
        "defineProperty",
        "entries",
        "freeze",
        "fromEntries",
        "getOwnPropertyDescriptor",
        "getOwnPropertyDescriptors",
        "getOwnPropertyNames",
        "getOwnPropertySymbols",
        "getPrototypeOf",
        "groupBy",
        "hasOwn",
        "is",
        "isExtensible",
        "isFrozen",
        "isSealed",
        "keys",
        "preventExtensions",
        "seal",
        "setPrototypeOf",
        "values"
    ];

    private static readonly string[] VariadicConstructorStaticMethodNames =
    [
        "assign"
    ];

    private static readonly string[] PrototypeMethodNames =
    [
        "hasOwnProperty",
        "isPrototypeOf",
        "propertyIsEnumerable",
        "toLocaleString",
        "toString",
        "valueOf",
        "__defineGetter__",
        "__defineSetter__",
        "__lookupGetter__",
        "__lookupSetter__"
    ];

    [Fact]
    public void ObjectConstructorStaticMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var methodName in ConstructorStaticMethodNames)
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(GlobalThis.Object, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }

            foreach (var methodName in VariadicConstructorStaticMethodNames)
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(GlobalThis.Object, methodName));
                Assert.IsType<BuiltinFunctionVariadic>(adapter.Target);
            }
        });
    }

    [Fact]
    public void ObjectPrototypeMethodsUseReceiverAwareAdapters()
    {
        WithRealm(() =>
        {
            foreach (var methodName in PrototypeMethodNames)
            {
                var adapter = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                    ObjectRuntime.GetItem(GlobalThis.ObjectPrototypeValue, methodName));

                Assert.True(BuiltinFunctionDelegates.IsReceiverAware(adapter.Target), methodName);
                Assert.False(adapter.RequiresInvocationContext, methodName);
            }
        });
    }

    [Fact]
    public void ObjectHasOwnReportsSpecCompliantLengthAfterMigration()
    {
        // hasOwn previously bypassed InitializeBuiltinStaticFunction; migration restored the
        // missing call alongside the ABI change (issue #1895).
        WithRealm(() =>
        {
            var hasOwn = ObjectRuntime.GetItem(GlobalThis.Object, "hasOwn");
            Assert.Equal(2d, ObjectRuntime.GetItem(hasOwn, "length"));
            Assert.Equal("hasOwn", ObjectRuntime.GetItem(hasOwn, "name"));
        });
    }

    [Fact]
    public void ObjectPrototypeMethodsAreCallableWithExplicitReceiver()
    {
        WithRealm(() =>
        {
            var target = CreateOrdinaryObject();
            ObjectRuntime.SetItem(target, "foo", 1d);

            var hasOwnProperty = ObjectRuntime.GetItem(GlobalThis.ObjectPrototypeValue, "hasOwnProperty");
            Assert.Equal(true, CallableOperations.Call1(hasOwnProperty, target, "foo"));

            var toString = ObjectRuntime.GetItem(GlobalThis.ObjectPrototypeValue, "toString");
            Assert.Equal("[object Object]", CallableOperations.Call0(toString, target));

            var valueOf = ObjectRuntime.GetItem(GlobalThis.ObjectPrototypeValue, "valueOf");
            Assert.Same(target, CallableOperations.Call0(valueOf, target));

            var defineGetter = ObjectRuntime.GetItem(GlobalThis.ObjectPrototypeValue, "__defineGetter__");
            var getter = BuiltinDelegateFunctionAdapter.FromDelegate((BuiltinFunction0)(_ => 42d));
            CallableOperations.Call2(
                defineGetter,
                target,
                "bar",
                getter);

            Assert.Equal(42d, ObjectRuntime.GetItem(target, "bar"));
        });
    }

    [Fact]
    public void ObjectConstructorStaticMethodsCommonAritiesAllocateNoInvocationState()
    {
        WithRealm(() =>
        {
            var getPrototypeOf = Assert.IsType<BuiltinDelegateFunctionAdapter>(
                ObjectRuntime.GetItem(GlobalThis.Object, "getPrototypeOf"));
            var source = CreateOrdinaryObject();

            for (var index = 0; index < 1_000; index++)
            {
                CallableOperations.Call1(getPrototypeOf, null, source);
            }

            var before = GC.GetAllocatedBytesForCurrentThread();
            object? result = null;
            for (var index = 0; index < 10_000; index++)
            {
                result = CallableOperations.Call1(getPrototypeOf, null, source);
            }
            var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.Same(GlobalThis.ObjectPrototypeValue, result);
            Assert.Equal(0, allocated);
        });
    }

    private static JsObject CreateOrdinaryObject()
    {
        var result = new JsObject();
        PrototypeChain.SetPrototype(result, GlobalThis.ObjectPrototypeValue);
        return result;
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
