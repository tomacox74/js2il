using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using JavaScriptRuntime;
using JavaScriptRuntime.Modules.CommonJS;
using JavaScriptRuntime.Modules.ESM;
using JavaScriptRuntime.Modules.Shared;
using AssemblyName = System.Reflection.AssemblyName;

namespace Jroc.Tests;

public sealed class RuntimeModuleStateTests
{
    [Fact]
    public async Task ParallelRealmsKeepIdenticalModuleIdsIndependent()
    {
        var first = CreateContext();
        var second = CreateContext();
        using var ready = new Barrier(2);

        var firstTask = Task.Run(() =>
        {
            using var scope = first.EnterAsRoot();
            var meta = RuntimeServices.GetImportMeta("shared.js");
            RequireDelegate require = static _ => "first";
            RuntimeServices.RegisterModuleRequire("shared.js", require);
            ready.SignalAndWait();

            Assert.Same(require, RuntimeServices.GetRequireForModule("shared.js"));
            return meta;
        });
        var secondTask = Task.Run(() =>
        {
            using var scope = second.EnterAsRoot();
            var meta = RuntimeServices.GetImportMeta("shared.js");
            RequireDelegate require = static _ => "second";
            RuntimeServices.RegisterModuleRequire("shared.js", require);
            ready.SignalAndWait();

            Assert.Same(require, RuntimeServices.GetRequireForModule("shared.js"));
            return meta;
        });

        var metas = await Task.WhenAll(firstTask, secondTask);
        Assert.NotSame(metas[0], metas[1]);

        first.Realm.Dispose();

        using (second.Enter())
        {
            Assert.Equal(
                "second",
                RuntimeServices.GetRequireForModule("shared.js")!("ignored"));
            Assert.Same(metas[1], RuntimeServices.GetImportMeta("shared.js"));
        }
    }

    [Fact]
    public void EsmBindingsWithIdenticalIdsRemainRealmLocal()
    {
        var first = CreateContext();
        var second = CreateContext();
        var firstExports = new JsObject();
        var secondExports = new JsObject();

        using (first.Enter())
        {
            EsModuleLinker.MarkEsModule(firstExports);
            EsModuleLinker.RegisterLocalExport(firstExports, "shared.js", "value");
            EsModuleLinker.SetLocalExport("shared.js", "value", 1d);
        }

        using (second.Enter())
        {
            EsModuleLinker.MarkEsModule(secondExports);
            EsModuleLinker.RegisterLocalExport(secondExports, "shared.js", "value");
            EsModuleLinker.SetLocalExport("shared.js", "value", 2d);
        }

        Assert.Equal(1d, ObjectRuntime.GetProperty(firstExports, "value"));
        Assert.Equal(2d, ObjectRuntime.GetProperty(secondExports, "value"));
    }

    [Fact]
    public void CommonJsNamespaceCacheIsRealmLocal()
    {
        var first = CreateContext();
        var second = CreateContext();
        var exports = new JsObject();
        ObjectRuntime.SetProperty(exports, "value", 1d);
        JavaScriptRuntime.Object.preventExtensions(exports);
        object firstNamespace;
        object secondNamespace;

        using (first.Enter())
        {
            firstNamespace = EsModuleInterop.ToDynamicImportResult(exports);
            Assert.Same(
                firstNamespace,
                EsModuleInterop.ToDynamicImportResult(exports));
        }

        using (second.Enter())
        {
            secondNamespace = EsModuleInterop.ToDynamicImportResult(exports);
            Assert.Same(
                secondNamespace,
                EsModuleInterop.ToDynamicImportResult(exports));
        }

        Assert.NotSame(firstNamespace, secondNamespace);
    }

    [Fact]
    public void NestedFramesRestoreModuleLocationAndActiveRequireState()
    {
        var context = CreateContext();
        var outerModule = new Module(
            "outer",
            "/outer/main.js",
            parent: null,
            static _ => null);
        var innerModule = new Module(
            "inner",
            "/inner/main.js",
            parent: null,
            static _ => null);

        using (context.Enter())
        {
            context.SetCurrentParentModule(outerModule);
            ModuleContext.SetModuleContext("/outer", "/outer/main.js");

            using (context.Enter())
            {
                Assert.Same(outerModule, context.GetCurrentParentModule());
                AssertModuleLocation("/outer", "/outer/main.js");
                context.SetCurrentParentModule(innerModule);
                ModuleContext.SetModuleContext("/inner", "/inner/main.js");
            }

            Assert.Same(outerModule, context.GetCurrentParentModule());
            AssertModuleLocation("/outer", "/outer/main.js");
        }
    }

    [Fact]
    public void RealmDisposalReleasesTheModuleGraph()
    {
        var context = CreateContext();
        var exports = new JsObject();
        var commonJsExports = new JsObject();

        using (context.Enter())
        {
            context.Realm.ModuleState.ModulesAssembly = typeof(RuntimeModuleStateTests).Assembly;
            _ = RuntimeServices.GetImportMeta("shared.js");
            RuntimeServices.RegisterModuleRequire("shared.js", static _ => null);
            EsModuleLinker.MarkEsModule(exports);
            _ = EsModuleInterop.ToDynamicImportResult(commonJsExports);
        }

        context.Realm.Dispose();

        Assert.Null(context.Realm.ModuleState.ModulesAssembly);
        Assert.Empty(context.Realm.ModuleState.ImportMetaByUrl);
        Assert.Empty(context.Realm.ModuleState.RequireByModuleId);
        Assert.Empty(context.Realm.ModuleState.EsModuleBindings);
        Assert.False(
            context.Realm.ModuleState.EsModuleNamespaces.TryGetValue(
                exports,
                out _));
        Assert.False(
            context.Realm.ModuleState.CommonJsNamespaceCache.TryGetValue(
                commonJsExports,
                out _));
    }

    [Fact]
    public void RealmDisposalDoesNotRetainCollectibleModuleAssembly()
    {
        var assemblyReference = CreateDisposedRealmWithCollectibleAssembly();

        for (var attempt = 0; attempt < 10 && assemblyReference.IsAlive; attempt++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        Assert.False(assemblyReference.IsAlive);
    }

    private static RuntimeExecutionContext CreateContext()
    {
        var services = RuntimeServices.BuildServiceProvider();
        return RuntimeExecutionContext.GetOrCreate(services);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateDisposedRealmWithCollectibleAssembly()
    {
        var assembly = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName($"CollectibleModule_{Guid.NewGuid():N}"),
            AssemblyBuilderAccess.RunAndCollect);
        var module = assembly.DefineDynamicModule("main");
        _ = module.DefineType("Modules.Entry").CreateType();
        var reference = new WeakReference(assembly);
        var context = CreateContext();
        context.Realm.ModuleState.ModulesAssembly = assembly;
        context.Realm.Dispose();
        return reference;
    }

    private static void AssertModuleLocation(
        string expectedDirectory,
        string expectedFilename)
    {
        var module = ModuleContext.CreateModuleContext();
        Assert.Equal(expectedDirectory, module.__dirname);
        Assert.Equal(expectedFilename, module.__filename);
    }
}
