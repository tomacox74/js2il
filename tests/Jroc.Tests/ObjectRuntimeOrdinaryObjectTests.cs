using System.Collections.Generic;
using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class ObjectRuntimeOrdinaryObjectTests
{
    [Fact]
    public void CoreDispatch_PreservesJsObjectBehavior()
    {
        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            object target = new JsObject();
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
    public void ProxyFallback_PreservesJsObjectTargetSemantics()
    {
        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            object target = new JsObject();
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

    [Fact]
    public void DeletedIntrinsicProperty_IsMaskedAcrossCoreDispatch()
    {
        object target = new JsObject();
        ObjectRuntime.TrySetOwnValue(target, "hidden", 42d);
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

    [Fact]
    public void RuntimeCreatedOrdinaryRecords_AreJsObjects()
    {
        var moduleId = $"module-{Guid.NewGuid():N}.js";
        var importMeta = Assert.IsType<JsObject>(RuntimeServices.GetImportMeta(moduleId));
        var revocable = Assert.IsType<JsObject>(
            JavaScriptRuntime.Proxy.revocable(new JsObject(), new JsObject()));

        Assert.Same(importMeta, RuntimeServices.GetImportMeta(moduleId));
        Assert.IsType<string>(ObjectRuntime.GetProperty(importMeta, "url"));
        Assert.IsType<JavaScriptRuntime.Proxy>(ObjectRuntime.GetProperty(revocable, "proxy"));
        Assert.IsAssignableFrom<Delegate>(ObjectRuntime.GetProperty(revocable, "revoke"));
    }

    [Fact]
    public void ExternalDictionary_RemainsAHostObject()
    {
        var hostObject = new Dictionary<string, object?>();

        ObjectRuntime.SetProperty(hostObject, "value", 42d);

        Assert.Equal(42d, ObjectRuntime.GetProperty(hostObject, "value"));
        Assert.True(JavaScriptRuntime.Object.hasOwn(hostObject, "value"));
        Assert.Equal(42d, hostObject["value"]);
    }
}
