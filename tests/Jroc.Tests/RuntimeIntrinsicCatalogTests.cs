using JavaScriptRuntime;

namespace Jroc.Tests;

public class RuntimeIntrinsicCatalogTests
{
    [Fact]
    public void DefaultCatalog_ResolvesBuiltInGlobalsIntrinsicObjectsAndNodeModules()
    {
        var catalog = new RuntimeIntrinsicCatalog();

        Assert.True(catalog.TryGetGlobalBinding("Array", out var arrayGlobal));
        Assert.NotNull(arrayGlobal);
        Assert.Equal("Array", arrayGlobal.Name);

        Assert.True(catalog.TryGetKnownGlobal("undefined", out var undefinedGlobal));
        Assert.NotNull(undefinedGlobal);
        Assert.Equal("undefined", undefinedGlobal.Name);

        Assert.True(catalog.TryGetIntrinsicObject("Array", out var arrayIntrinsic));
        Assert.NotNull(arrayIntrinsic);
        Assert.Equal(typeof(JavaScriptRuntime.Array), arrayIntrinsic.Type);
        Assert.Equal(IntrinsicCallKind.ArrayConstruct, arrayIntrinsic.CallKind);

        Assert.True(catalog.TryGetModuleBinding("node:fs", out var fsModule));
        Assert.NotNull(fsModule);
        Assert.Equal(typeof(JavaScriptRuntime.Node.FS), fsModule.ModuleType);
    }

    [Fact]
    public void Catalog_MergesHostProvidedEntries()
    {
        var descriptors = new HostRuntimeIntrinsicDescriptorsBuilder()
            .AddGlobalValue("assert", 1)
            .AddIntrinsicObject("HostThing", typeof(HostIntrinsicObject))
            .AddModuleType("host:module", typeof(HostModule))
            .AddKnownGlobal("$262", typeof(object), isConstant: true)
            .Build();

        var catalog = new RuntimeIntrinsicCatalog(descriptors);

        Assert.True(catalog.TryGetGlobalBinding("assert", out var assertGlobal));
        Assert.NotNull(assertGlobal);
        Assert.Equal(1, assertGlobal.CreateValue());

        Assert.True(catalog.TryGetIntrinsicObject("HostThing", out var hostIntrinsic));
        Assert.NotNull(hostIntrinsic);
        Assert.Equal(typeof(HostIntrinsicObject), hostIntrinsic.Type);

        Assert.True(catalog.TryGetModuleBinding("host:module", out var hostModule));
        Assert.NotNull(hostModule);
        Assert.Equal(typeof(HostModule), hostModule.ModuleType);

        Assert.True(catalog.TryGetKnownGlobal("$262", out var test262Global));
        Assert.NotNull(test262Global);
        Assert.Equal(typeof(object), test262Global.ValueType);
        Assert.True(test262Global.IsConstant);
    }

    [Fact]
    public void HostGlobalDuplicate_PreservesBuiltInByDefaultAndCanExplicitlyReplace()
    {
        var preserveDescriptors = new HostRuntimeIntrinsicDescriptorsBuilder()
            .AddGlobalValue("Array", "host array")
            .Build();

        var preserveCatalog = new RuntimeIntrinsicCatalog(preserveDescriptors);

        Assert.True(preserveCatalog.TryGetGlobalBinding("Array", out var preservedArray));
        Assert.NotNull(preservedArray);
        Assert.NotEqual("host array", preservedArray.CreateValue());

        var replaceDescriptors = new HostRuntimeIntrinsicDescriptorsBuilder()
            .AddGlobalValue("Array", "host array", overwritePolicy: RuntimeGlobalOverwritePolicy.ReplaceExisting)
            .Build();

        var replaceCatalog = new RuntimeIntrinsicCatalog(replaceDescriptors);

        Assert.True(replaceCatalog.TryGetGlobalBinding("Array", out var replacedArray));
        Assert.NotNull(replacedArray);
        Assert.Equal("host array", replacedArray.CreateValue());
    }

    [Fact]
    public void HostIntrinsicAndModuleDuplicates_AreRejected()
    {
        var duplicateIntrinsic = new HostRuntimeIntrinsicDescriptorsBuilder()
            .AddIntrinsicObject("Array", typeof(HostIntrinsicObject))
            .Build();

        Assert.Throws<InvalidOperationException>(() => new RuntimeIntrinsicCatalog(duplicateIntrinsic));

        var duplicateModule = new HostRuntimeIntrinsicDescriptorsBuilder()
            .AddModuleType("node:fs", typeof(HostModule))
            .Build();

        Assert.Throws<InvalidOperationException>(() => new RuntimeIntrinsicCatalog(duplicateModule));
    }

    private sealed class HostIntrinsicObject
    {
    }

    private sealed class HostModule
    {
    }
}
