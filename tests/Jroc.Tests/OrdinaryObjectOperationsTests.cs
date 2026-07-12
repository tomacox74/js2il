using System.Dynamic;
using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class OrdinaryObjectOperationsTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CoreDispatch_PreservesJsObjectAndTransitionalExpandoBehavior(bool useExpando)
    {
        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            object target = useExpando ? new ExpandoObject() : new JsObject();
            var prototype = new JsObject();

            ObjectRuntime.SetProperty(target, "second", 2d);
            ObjectRuntime.SetProperty(target, "1", "index");
            ObjectRuntime.SetProperty(target, "first", 1d);
            ObjectRuntime.SetProperty(prototype, "inherited", 3d);
            JavaScriptRuntime.Object.setPrototypeOf(target, prototype);

            Assert.Equal(2d, ObjectRuntime.GetProperty(target, "second"));
            Assert.Equal(3d, ObjectRuntime.GetProperty(target, "inherited"));
            Assert.True(JavaScriptRuntime.Object.hasOwn(target, "first"));
            Assert.True(Operators.In("inherited", target));
            Assert.Equal(
                new object?[] { "1", "second", "first" },
                Assert.IsType<JavaScriptRuntime.Array>(JavaScriptRuntime.Object.getOwnPropertyNames(target)).ToArray());

            Assert.True(ObjectRuntime.DeleteProperty(target, "second"));
            Assert.False(JavaScriptRuntime.Object.hasOwn(target, "second"));

            JavaScriptRuntime.Object.freeze(target);
            Assert.True(JavaScriptRuntime.Object.isFrozen(target));
            Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(target, "first", 4d));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void ProxyFallback_PreservesTransitionalExpandoTargetSemantics()
    {
        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            object target = new ExpandoObject();
            var proxy = new JavaScriptRuntime.Proxy(target, new JsObject());

            ObjectRuntime.SetProperty(proxy, "value", 42d);

            Assert.Equal(42d, ObjectRuntime.GetProperty(proxy, "value"));
            Assert.True(Operators.In("value", proxy));
            Assert.True(ObjectRuntime.DeleteProperty(proxy, "value"));
            Assert.False(Operators.In("value", proxy));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DeletedIntrinsicProperty_IsMaskedAcrossCoreDispatch(bool useExpando)
    {
        object target = useExpando ? new ExpandoObject() : new JsObject();
        OrdinaryObjectOperations.TrySetOwnValue(target, "hidden", 42d);
        using (PropertyDescriptorStore.BeginIntrinsicInitialization())
        {
            PropertyDescriptorStore.DefineOrUpdate(target, "hidden", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Value = 42d,
                Writable = true,
                Enumerable = true,
                Configurable = true
            });
        }

        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;

            Assert.True(ObjectRuntime.DeleteProperty(target, "hidden"));
            Assert.Null(ObjectRuntime.GetProperty(target, "hidden"));
            Assert.Null(JavaScriptRuntime.Object.getOwnPropertyDescriptor(target, "hidden"));
            Assert.False(JavaScriptRuntime.Object.hasOwn(target, "hidden"));
            Assert.False(Operators.In("hidden", target));
            Assert.Empty(Assert.IsType<JavaScriptRuntime.Array>(
                JavaScriptRuntime.Object.getOwnPropertyNames(target)));
            Assert.True(JavaScriptRuntime.Object.CreateForInIterator(target).Next().done);
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }
}
