using JavaScriptRuntime;
using JavaScriptRuntime.CommonJS;
using Xunit;

namespace Jroc.Tests.CommonJS;

public class ModuleObjectRepresentationTests
{
    [Fact]
    public void Module_DefaultExports_IsJsObject()
    {
        var module = new Module(
            "module-id",
            "module-id.js",
            parent: null,
            static _ => null);

        var exports = Assert.IsType<JsObject>(module.exports);

        ObjectRuntime.SetProperty(exports, "value", 42d);

        Assert.Equal(42d, ObjectRuntime.GetProperty(module.exports!, "value"));
    }

    [Fact]
    public void DynamicImport_CjsNamespace_IsJsObject_AndPreservesLiveAccessors()
    {
        var exports = new JsObject();
        ObjectRuntime.SetProperty(exports, "value", 1d);

        var namespaceObject = EsModuleInterop.ToDynamicImportResult(exports);
        var cachedNamespaceObject = EsModuleInterop.ToDynamicImportResult(exports);

        var jsNamespace = Assert.IsType<JsObject>(namespaceObject);
        Assert.Same(namespaceObject, cachedNamespaceObject);
        Assert.Same(exports, ObjectRuntime.GetProperty(jsNamespace, "default"));
        Assert.Same(exports, ObjectRuntime.GetProperty(jsNamespace, "module.exports"));
        Assert.Equal(1d, ObjectRuntime.GetProperty(jsNamespace, "value"));

        ObjectRuntime.SetProperty(exports, "value", 2d);

        Assert.Equal(2d, ObjectRuntime.GetProperty(jsNamespace, "value"));
        Assert.True(PropertyDescriptorStore.TryGetOwn(jsNamespace, "value", out var valueDescriptor));
        Assert.Equal(JsPropertyDescriptorKind.Accessor, valueDescriptor.Kind);
        Assert.True(PropertyDescriptorStore.TryGetOwn(exports, "__jroc_esm_namespace", out var cacheDescriptor));
        Assert.Same(jsNamespace, Assert.IsType<JsObject>(cacheDescriptor.Value));
        Assert.False(cacheDescriptor.Enumerable);
        Assert.False(cacheDescriptor.Configurable);
        Assert.False(cacheDescriptor.Writable);
    }

    [Fact]
    public void DynamicImport_PrimitiveNamespace_IsJsObject()
    {
        var namespaceObject = EsModuleInterop.ToDynamicImportResult(42d);

        var jsNamespace = Assert.IsType<JsObject>(namespaceObject);
        Assert.Equal(42d, ObjectRuntime.GetProperty(jsNamespace, "default"));
        Assert.Equal(42d, ObjectRuntime.GetProperty(jsNamespace, "module.exports"));
    }
}
