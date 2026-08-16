using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using JavaScriptRuntime;
using AssemblyName = System.Reflection.AssemblyName;

namespace Jroc.Tests;

public sealed class RuntimeRealmValueCacheTests
{
    [Fact]
    public void TemplateObjectsPreserveIdentityWithinARealmButNotAcrossRealms()
    {
        var first = CreateContext();
        var second = CreateContext();
        object firstTemplate;
        object secondTemplate;

        using (first.EnterAsRoot())
        {
            firstTemplate = RuntimeServices.CreateTemplateObject(
                "module.js:1:1",
                ["cooked"],
                ["raw"]);
            Assert.Same(
                firstTemplate,
                RuntimeServices.CreateTemplateObject(
                    "module.js:1:1",
                    ["ignored"],
                    ["ignored"]));
        }

        using (second.EnterAsRoot())
        {
            secondTemplate = RuntimeServices.CreateTemplateObject(
                "module.js:1:1",
                ["cooked"],
                ["raw"]);
        }

        Assert.NotSame(firstTemplate, secondTemplate);
        first.Realm.Agent.Cluster.Dispose();
        second.Realm.Agent.Cluster.Dispose();
    }

    [Fact]
    public void ClassConstructorsPreserveIdentityWithinARealmButNotAcrossRealms()
    {
        var first = CreateContext();
        var second = CreateContext();
        var sharedScopes = new object[] { new JsObject() };
        JsClassConstructorObject firstConstructor;
        JsClassConstructorObject secondConstructor;

        using (first.EnterAsRoot())
        {
            firstConstructor = MaterializeConstructor(sharedScopes);
            Assert.Same(firstConstructor, MaterializeConstructor(sharedScopes));
        }

        using (second.EnterAsRoot())
        {
            secondConstructor = MaterializeConstructor(sharedScopes);
        }

        Assert.NotSame(firstConstructor, secondConstructor);
        first.Realm.Agent.Cluster.Dispose();
        second.Realm.Agent.Cluster.Dispose();
    }

    [Fact]
    public void LazyClassMetadataKeepsCapturedScopesIsolatedAcrossRealms()
    {
        var first = CreateContext();
        var second = CreateContext();
        var firstScope = new JsObject();
        var secondScope = new JsObject();

        using (first.EnterAsRoot())
        {
            RegisterLazyMethod(typeof(TestClassOwner), firstScope);
        }

        using (second.EnterAsRoot())
        {
            RegisterLazyMethod(typeof(TestClassOwner), secondScope);
        }

        Assert.True(
            first.Realm.ValueCaches.LazyClassMetadata.TryGetValue(
                typeof(TestClassOwner),
                out var firstSlot));
        Assert.True(
            second.Realm.ValueCaches.LazyClassMetadata.TryGetValue(
                typeof(TestClassOwner),
                out var secondSlot));
        Assert.NotSame(firstSlot, secondSlot);
        Assert.Same(firstScope, Assert.Single(firstSlot.Methods).Scopes[0]);
        Assert.Same(secondScope, Assert.Single(secondSlot.Methods).Scopes[0]);

        first.Realm.Agent.Cluster.Dispose();
        second.Realm.Agent.Cluster.Dispose();
    }

    [Fact]
    public void RealmDisposalClearsTemplatesConstructorsAndCapturedScopes()
    {
        var context = CreateContext();
        var capturedScope = new JsObject();

        using (context.EnterAsRoot())
        {
            _ = RuntimeServices.CreateTemplateObject("site", ["value"], ["value"]);
            _ = MaterializeConstructor([capturedScope]);
            _ = RuntimeServices.RegisterLazyClassMethodDataProperty(
                typeof(TestClassOwner),
                "method",
                nameof(TestClassOwner.Method),
                0d,
                "method",
                false,
                false,
                false,
                false,
                new object[] { capturedScope });
        }

        Assert.Single(context.Realm.ValueCaches.TemplateObjects);
        Assert.Single(context.Realm.ValueCaches.MaterializedClassConstructors);
        Assert.True(
            context.Realm.ValueCaches.LazyClassMetadata.TryGetValue(
                typeof(TestClassOwner),
                out _));

        context.Realm.Dispose();

        Assert.Empty(context.Realm.ValueCaches.TemplateObjects);
        Assert.Empty(context.Realm.ValueCaches.MaterializedClassConstructors);
        Assert.False(
            context.Realm.ValueCaches.LazyClassMetadata.TryGetValue(
                typeof(TestClassOwner),
                out _));
    }

    [Fact]
    public void RealmDisposalReleasesCachedValuesAndCollectibleTypes()
    {
        var references = CreateDisposedRealmWithCachedValues();

        for (var attempt = 0;
            attempt < 10 && references.Any(reference => reference.IsAlive);
            attempt++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        Assert.All(references, reference => Assert.False(reference.IsAlive));
    }

    private static RuntimeExecutionContext CreateContext()
    {
        var services = RuntimeServices.BuildServiceProvider();
        return RuntimeExecutionContext.GetOrCreate(services);
    }

    private static JsClassConstructorObject MaterializeConstructor(object[] scopes)
        => RuntimeServices.InitializeClassConstructorObject(
            new TestClassConstructor(),
            typeof(TestClassOwner),
            scopes,
            formalParameterCount: 0,
            freshIdentity: false);

    private static void RegisterLazyMethod(Type ownerType, object capturedScope)
        => _ = RuntimeServices.RegisterLazyClassMethodDataProperty(
            ownerType,
            "method",
            nameof(TestClassOwner.Method),
            0d,
            "method",
            false,
            false,
            false,
            false,
            new object[] { capturedScope });

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference[] CreateDisposedRealmWithCachedValues()
    {
        var assembly = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName($"CollectibleRealmValues_{Guid.NewGuid():N}"),
            AssemblyBuilderAccess.RunAndCollect);
        var module = assembly.DefineDynamicModule("main");
        var collectibleType = module.DefineType("Generated.Class").CreateType()!;
        var context = CreateContext();
        object template;
        JsClassConstructorObject constructor;
        var capturedScope = new JsObject();

        using (context.EnterAsRoot())
        {
            template = RuntimeServices.CreateTemplateObject(
                "collectible",
                ["value"],
                ["value"]);
            constructor = RuntimeServices.InitializeClassConstructorObject(
                new TestClassConstructor(),
                collectibleType,
                [capturedScope],
                formalParameterCount: 0,
                freshIdentity: false);
            _ = RuntimeServices.RegisterLazyClassMethodDataProperty(
                collectibleType,
                "method",
                "Method",
                0d,
                "method",
                false,
                false,
                false,
                false,
                new object[] { capturedScope });
        }

        var references = new[]
        {
            new WeakReference(template),
            new WeakReference(constructor),
            new WeakReference(capturedScope),
            new WeakReference(collectibleType),
            new WeakReference(assembly)
        };
        context.Realm.Dispose();
        return references;
    }

    private sealed class TestClassConstructor : JsClassConstructorObject
    {
    }

    private sealed class TestClassOwner
    {
        public object? Method() => null;
    }
}
