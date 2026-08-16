using JavaScriptRuntime;
using JsObjectConstructor = JavaScriptRuntime.Object;
using JsIterator = JavaScriptRuntime.Iterator;

namespace Jroc.Tests;

public sealed class IteratorObjectRepresentationTests
{
    [Fact]
    public void IteratorHelpers_UseInlineJsObjectStorage_AndContinueAfterFreeze()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        var helper = CreateMappedHelper();
        var customPrototype = new JsObject();

        Assert.IsAssignableFrom<JsObject>(helper);
        Assert.Same(JsIterator.HelperPrototype, JsObjectConstructor.getPrototypeOf(helper));
        Assert.False(JsObjectConstructor.hasOwn(helper, "next"));
        Assert.False(JsObjectConstructor.hasOwn(helper, "return"));
        Assert.Same(
            ObjectRuntime.GetProperty(JsIterator.HelperPrototype, "next"),
            ObjectRuntime.GetProperty(helper, "next"));
        Assert.Same(
            ObjectRuntime.GetProperty(JsIterator.HelperPrototype, "return"),
            ObjectRuntime.GetProperty(helper, "return"));

        ObjectRuntime.SetProperty(helper, "custom", "helper");
        Assert.Equal("helper", ObjectRuntime.GetProperty(helper, "custom"));
        Assert.True(JsObjectConstructor.hasOwn(helper, "custom"));

        JsObjectConstructor.setPrototypeOf(helper, customPrototype);
        Assert.Same(customPrototype, JsObjectConstructor.getPrototypeOf(helper));
        JsObjectConstructor.setPrototypeOf(helper, JsIterator.HelperPrototype);

        JsObjectConstructor.freeze(helper);
        Assert.True(JsObjectConstructor.isFrozen(helper));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(helper, "custom", "changed"));

        var first = helper.Next();
        var second = helper.Next();
        var exhausted = helper.Next();
        Assert.Equal(1d, first.value);
        Assert.Equal(2d, second.value);
        Assert.True(exhausted.done);
        Assert.True(helper.Next().done);
    }

    [Fact]
    public void IteratorResults_UseOwnJsObjectValueAndDoneProperties()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        var result = IteratorResult.Create("value", done: false);

        Assert.IsAssignableFrom<JsObject>(result);
        Assert.Same(GlobalThis.ObjectPrototypeValue, JsObjectConstructor.getPrototypeOf(result));
        Assert.True(JsObjectConstructor.hasOwn(result, "value"));
        Assert.True(JsObjectConstructor.hasOwn(result, "done"));
        Assert.Equal("value", ObjectRuntime.GetProperty(result, "value"));
        Assert.False(Assert.IsType<bool>(ObjectRuntime.GetProperty(result, "done")));

        var valueDescriptor = Assert.IsAssignableFrom<JsObject>(
            JsObjectConstructor.getOwnPropertyDescriptor(result, "value"));
        var doneDescriptor = Assert.IsAssignableFrom<JsObject>(
            JsObjectConstructor.getOwnPropertyDescriptor(result, "done"));
        Assert.True(Assert.IsType<bool>(ObjectRuntime.GetProperty(valueDescriptor, "enumerable")));
        Assert.True(Assert.IsType<bool>(ObjectRuntime.GetProperty(valueDescriptor, "writable")));
        Assert.True(Assert.IsType<bool>(ObjectRuntime.GetProperty(valueDescriptor, "configurable")));
        Assert.True(Assert.IsType<bool>(ObjectRuntime.GetProperty(doneDescriptor, "enumerable")));
        Assert.True(Assert.IsType<bool>(ObjectRuntime.GetProperty(doneDescriptor, "writable")));
        Assert.True(Assert.IsType<bool>(ObjectRuntime.GetProperty(doneDescriptor, "configurable")));

        ObjectRuntime.SetProperty(result, "value", "updated");
        ObjectRuntime.SetProperty(result, "done", true);
        Assert.Equal("updated", ObjectRuntime.IteratorResultValue(result));
        Assert.True(ObjectRuntime.IteratorResultDone(result));
        Assert.Equal("value", ((IIteratorResult)result).value);
        Assert.False(((IIteratorResult)result).done);
    }

    [Fact]
    public void IteratorHelperPrototypeStorage_IsIsolatedAcrossRuntimes()
    {
        object firstPrototype;
        var firstServices = RuntimeServices.BuildServiceProvider();
        using (RuntimeExecutionContext.GetOrCreate(firstServices).Enter())
        {
            var helper = CreateMappedHelper();
            firstPrototype = JsIterator.HelperPrototype;
            ObjectRuntime.SetProperty(firstPrototype, "isolationMarker", "first");

            Assert.Same(firstPrototype, JsObjectConstructor.getPrototypeOf(helper));
            Assert.Equal("first", ObjectRuntime.GetProperty(helper, "isolationMarker"));
        }

        var secondServices = RuntimeServices.BuildServiceProvider();
        using (RuntimeExecutionContext.GetOrCreate(secondServices).Enter())
        {
            var helper = CreateMappedHelper();
            var secondPrototype = JsIterator.HelperPrototype;

            Assert.NotSame(firstPrototype, secondPrototype);
            Assert.Same(secondPrototype, JsObjectConstructor.getPrototypeOf(helper));
            Assert.Null(ObjectRuntime.GetProperty(secondPrototype, "isolationMarker"));
            Assert.Null(ObjectRuntime.GetProperty(helper, "isolationMarker"));
        }
    }

    private static IJavaScriptIterator CreateMappedHelper()
    {
        _ = GlobalThis.globalThis;
        var source = JsIterator.From(new JavaScriptRuntime.Array(new object?[] { 1d, 2d }));
        var map = ObjectRuntime.GetProperty(source, "map");
        return Assert.IsAssignableFrom<IJavaScriptIterator>(
            CallableOperations.Call1(
                map,
                source,
                BuiltinDelegateFunctionAdapter.FromDelegate(
                    (Func<object[], object?[]?, object?>)(static (_, args) => args![0]))));
    }
}
