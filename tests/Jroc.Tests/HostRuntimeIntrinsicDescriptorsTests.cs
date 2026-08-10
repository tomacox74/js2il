using JavaScriptRuntime;

namespace Jroc.Tests;

public class HostRuntimeIntrinsicDescriptorsTests
{
    [Fact]
    public void GlobalBindingDefaults_PreserveBuiltInsAndUseNonEnumerableWritableDataPropertyShape()
    {
        var descriptor = RuntimeGlobalBindingDescriptor.ForValue("assert", new object());

        Assert.Equal("assert", descriptor.Name);
        Assert.Equal(RuntimeGlobalOverwritePolicy.PreserveExisting, descriptor.OverwritePolicy);
        Assert.False(descriptor.PropertyAttributes.Enumerable);
        Assert.True(descriptor.PropertyAttributes.Configurable);
        Assert.True(descriptor.PropertyAttributes.Writable);
        Assert.False(descriptor.UsesFactory);
    }

    [Fact]
    public void GlobalBindingFactory_CreatesValueOnDemand()
    {
        var count = 0;
        var descriptor = RuntimeGlobalBindingDescriptor.ForFactory("assert", () => ++count);

        Assert.True(descriptor.UsesFactory);
        Assert.Equal(1, descriptor.CreateValue());
        Assert.Equal(2, descriptor.CreateValue());
    }

    [Fact]
    public void GlobalBindingValue_NormalizesDelegateOnce()
    {
        Func<object?, object?> callback = static value => value;
        var descriptor =
            RuntimeGlobalBindingDescriptor.ForValue("callback", callback);

        var adapter =
            Assert.IsType<BuiltinDelegateFunctionAdapter>(descriptor.Value);
        Assert.Same(callback, adapter.Target);
        Assert.Same(adapter, descriptor.CreateValue());
        Assert.True(CallableOperations.IsCallable(adapter));
        Assert.False(CallableOperations.IsCallable(callback));
    }

    [Fact]
    public void GlobalBindingFactory_NormalizesDelegateWithStableIdentity()
    {
        var descriptor = RuntimeGlobalBindingDescriptor.ForFactory(
            "callback",
            static () => (Func<object?, object?>)Echo);

        var first =
            Assert.IsType<BuiltinDelegateFunctionAdapter>(
                descriptor.CreateValue());
        var second =
            Assert.IsType<BuiltinDelegateFunctionAdapter>(
                descriptor.CreateValue());

        Assert.Same(first, second);
        Assert.Equal(
            "value",
            CallableOperations.Call1(first, null, "value"));
    }

    [Fact]
    public void ModuleFactory_NormalizesDelegateExports()
    {
        var descriptor = RuntimeModuleBindingDescriptor.ForFactory(
            "host:callback",
            static () => (Func<object?, object?>)Echo);

        var callback =
            Assert.IsType<BuiltinDelegateFunctionAdapter>(
                descriptor.CreateModule());

        Assert.Equal(
            "value",
            CallableOperations.Call1(callback, null, "value"));
    }

    [Fact]
    public void BuilderBuild_ReturnsFrozenSnapshot()
    {
        var builder = new HostRuntimeIntrinsicDescriptorsBuilder();

        var first = builder
            .AddGlobalValue("assert", 1)
            .AddIntrinsicObject("Test262Error", typeof(Error), IntrinsicCallKind.BuiltInError)
            .AddModuleType("test262:helper", typeof(object))
            .AddKnownGlobal("$262", typeof(object), isConstant: true)
            .Build();

        builder.AddGlobalValue("verifyProperty", 2);
        var second = builder.Build();

        Assert.Single(first.GlobalBindings);
        Assert.Single(first.IntrinsicObjects);
        Assert.Single(first.ModuleBindings);
        Assert.Single(first.KnownGlobals);
        Assert.Equal(2, second.GlobalBindings.Count);
    }

    [Fact]
    public void Descriptors_RejectMissingNamesAndRequiredValues()
    {
        Assert.Throws<ArgumentException>(() => RuntimeGlobalBindingDescriptor.ForValue("", 1));
        Assert.Throws<ArgumentNullException>(() => RuntimeGlobalBindingDescriptor.ForFactory("assert", null!));
        Assert.Throws<ArgumentException>(() => new RuntimeIntrinsicObjectDescriptor(" ", typeof(object)));
        Assert.Throws<ArgumentNullException>(() => new RuntimeIntrinsicObjectDescriptor("Test262Error", null!));
        Assert.Throws<ArgumentException>(() => RuntimeModuleBindingDescriptor.ForType("", typeof(object)));
        Assert.Throws<ArgumentNullException>(() => RuntimeModuleBindingDescriptor.ForFactory("helper", null!));
        Assert.Throws<ArgumentException>(() => new RuntimeKnownGlobalDescriptor(""));
    }

    private static object? Echo(object? value) => value;
}
