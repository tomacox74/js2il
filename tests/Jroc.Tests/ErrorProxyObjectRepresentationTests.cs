using JavaScriptRuntime;
using JsObjectConstructor = JavaScriptRuntime.Object;

namespace Jroc.Tests;

public sealed class ErrorProxyObjectRepresentationTests
{
    [Fact]
    public void Errors_RemainExceptionBackedWithRealmOwnedPrototypes()
    {
        var first = CreateErrorInNewRealm();
        var second = CreateErrorInNewRealm();

        Assert.IsAssignableFrom<Exception>(first.Error);
        Assert.IsNotAssignableFrom<JsObject>(first.Error);
        Assert.NotSame(first.Prototype, second.Prototype);

        Exception? caught = null;
        try
        {
            throw first.Error;
        }
        catch (Exception exception)
        {
            caught = exception;
        }

        Assert.Same(first.Error, caught);
    }

    [Fact]
    public void Proxies_RemainNonJsObjectAndDispatchOwnDescriptorsThroughTrapsOrTargets()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        var target = new JsObject();
        ObjectRuntime.SetProperty(target, "targetSlot", "target value");

        var handler = new JsObject();
        var descriptorTrapCalls = 0;
        ObjectRuntime.SetProperty(
            handler,
            "getOwnPropertyDescriptor",
            (Func<object[], object?[]?, object?>)((_, args) =>
            {
                descriptorTrapCalls++;
                Assert.NotNull(args);
                Assert.Same(target, args![0]);
                Assert.Equal("targetSlot", args[1]);
                return CreateDataDescriptor("trap value");
            }));

        var proxy = new JavaScriptRuntime.Proxy(target, handler);

        Assert.IsNotAssignableFrom<JsObject>(proxy);

        var trappedDescriptor = Assert.IsType<JsObject>(
            JsObjectConstructor.getOwnPropertyDescriptor(proxy, "targetSlot"));
        Assert.Equal("trap value", ObjectRuntime.GetProperty(trappedDescriptor, "value"));
        Assert.True(JsObjectConstructor.hasOwn(proxy, "targetSlot"));
        Assert.Equal(2, descriptorTrapCalls);
        Assert.Equal("target value", ObjectRuntime.GetProperty(target, "targetSlot"));

        var forwardingProxy = new JavaScriptRuntime.Proxy(target, new JsObject());
        var forwardedDescriptor = Assert.IsType<JsObject>(
            JsObjectConstructor.getOwnPropertyDescriptor(forwardingProxy, "targetSlot"));
        Assert.Equal("target value", ObjectRuntime.GetProperty(forwardedDescriptor, "value"));
    }

    private static (Error Error, object Prototype) CreateErrorInNewRealm()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).EnterAsRoot();
        var prototype = GlobalThis.ErrorPrototypeValue;
        var error = new Error("boom");

        Assert.Equal("boom", error.Message);
        Assert.Same(prototype, PrototypeChain.GetPrototypeOrNull(error));
        return (error, prototype);
    }

    private static JsObject CreateDataDescriptor(object? value)
        => new()
        {
            ["value"] = value,
            ["writable"] = true,
            ["enumerable"] = true,
            ["configurable"] = true
        };
}
