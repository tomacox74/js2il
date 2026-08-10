using JavaScriptRuntime;

namespace Jroc.Tests.Function;

public sealed class FunctionRuntimeTests
{
    [Fact]
    public void CollectRestArguments_UsesTypedStartIndexAndReturnsArray()
    {
        var previousArguments = RuntimeServices.SetCurrentArguments(new object?[] { "first", 2d, true });
        try
        {
            var rest = RuntimeServices.CollectRestArguments(1d);

            Assert.Equal(2, rest.Count);
            Assert.Equal(2d, rest[0]);
            Assert.True(Assert.IsType<bool>(rest[1]));
        }
        finally
        {
            RuntimeServices.SetCurrentArguments(previousArguments);
        }
    }

    [Fact]
    public void Construct_UsesDescriptorFreeJsObjectStorage()
    {
        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            Func<object[], object?[]?, object?> constructor = static (_, args) =>
            {
                ObjectRuntime.SetProperty(RuntimeServices.GetCurrentThis()!, "value", args![0]);
                return 7d;
            };
            JavaScriptRuntime.Function.InitializeFunctionInstance(constructor, 1d, "Receiver", requiresInvocationContext: true);
            JavaScriptRuntime.Function.MarkConstructible(constructor);
            var constructorValue =
                BuiltinDelegateFunctionAdapter.FromDelegate(constructor);

            var instance = Assert.IsType<JsObject>(
                CallableOperations.Construct(
                    constructorValue,
                    new object?[] { 42d }));
            Assert.Equal(42d, instance["value"]);
            Assert.Equal(42d, ObjectRuntime.GetProperty(instance, "value"));
            Assert.False(instance.HasNonDataDescriptors);

            var prototype = Assert.IsType<JsObject>(
                ObjectRuntime.GetProperty(constructorValue, "prototype"));
            Assert.Same(prototype, JavaScriptRuntime.Object.getPrototypeOf(instance));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void OrdinaryFunctionPrototypes_AreJsObjects()
    {
        _ = GlobalThis.Function;

        Assert.IsType<JsObject>(JavaScriptRuntime.Function.Prototype);
        Assert.IsType<JsObject>(JavaScriptRuntime.Function.RestrictedPropertiesPrototype);
        Assert.Same(
            JavaScriptRuntime.Function.Prototype,
            JavaScriptRuntime.Object.getPrototypeOf(JavaScriptRuntime.Function.RestrictedPropertiesPrototype));

        Assert.True(PropertyDescriptorStore.TryGetOwn(JavaScriptRuntime.Function.Prototype, "caller", out var caller));
        Assert.True(PropertyDescriptorStore.TryGetOwn(JavaScriptRuntime.Function.Prototype, "arguments", out var arguments));
        Assert.Same(caller.Get, caller.Set);
        Assert.Same(caller.Get, arguments.Get);
        Assert.Same(arguments.Get, arguments.Set);
    }
}
