using JavaScriptRuntime;
using JavaScriptRuntime.Modules.CommonJS;

namespace Jroc.Tests;

public sealed class RuntimeExecutionContextTests
{
    [Fact]
    public void Enter_NestsAndRestoresThePreviousRealm()
    {
        var firstServices = RuntimeServices.BuildServiceProvider();
        var secondServices = RuntimeServices.BuildServiceProvider();
        var first = RuntimeExecutionContext.GetOrCreate(firstServices);
        var second = RuntimeExecutionContext.GetOrCreate(secondServices);

        Assert.Null(RuntimeExecutionContext.Current);

        using (first.Enter())
        {
            Assert.Same(first, RuntimeExecutionContext.Current);
            Assert.Same(firstServices, GlobalThis.ServiceProvider);

            using (second.Enter())
            {
                Assert.Same(second, RuntimeExecutionContext.Current);
                Assert.Same(secondServices, GlobalThis.ServiceProvider);
            }

            Assert.Same(first, RuntimeExecutionContext.Current);
            Assert.Same(firstServices, GlobalThis.ServiceProvider);
        }

        Assert.Null(RuntimeExecutionContext.Current);
        Assert.Null(GlobalThis.ServiceProvider);
    }

    [Fact]
    public void Enter_RestoresAfterExceptionUnwinding()
    {
        var context = CreateContext();

        _ = Assert.Throws<InvalidOperationException>(
            new Action(() =>
            {
                using var scope = context.Enter();
                throw new InvalidOperationException("expected");
            }));

        Assert.Null(RuntimeExecutionContext.Current);
    }

    [Fact]
    public void Enter_RejectsOutOfOrderDisposal()
    {
        var first = CreateContext();
        var second = CreateContext();
        var firstScope = first.Enter();
        var secondScope = second.Enter();

        _ = Assert.Throws<InvalidOperationException>(firstScope.Dispose);
        Assert.Same(second, RuntimeExecutionContext.Current);

        secondScope.Dispose();
        firstScope.Dispose();

        Assert.Null(RuntimeExecutionContext.Current);
    }

    [Fact]
    public async Task Enter_FlowsAcrossAsyncContinuations()
    {
        var context = CreateContext();

        using (context.Enter())
        {
            await Task.Yield();
            Assert.Same(context, RuntimeExecutionContext.Current);
            Assert.Same(
                context,
                await Task.Run(() => RuntimeExecutionContext.Current));
        }

        Assert.Null(RuntimeExecutionContext.Current);
    }

    [Fact]
    public async Task ParallelAgentsKeepIndependentExecutionFrames()
    {
        var first = CreateContext();
        var second = CreateContext();
        using var ready = new Barrier(2);

        var firstTask = Task.Run(() =>
        {
            using var _ = first.EnterAsRoot();
            ready.SignalAndWait();
            Assert.Same(first, RuntimeExecutionContext.Current);
            Assert.Same(first.Services, GlobalThis.ServiceProvider);
        });
        var secondTask = Task.Run(() =>
        {
            using var _ = second.EnterAsRoot();
            ready.SignalAndWait();
            Assert.Same(second, RuntimeExecutionContext.Current);
            Assert.Same(second.Services, GlobalThis.ServiceProvider);
        });

        await Task.WhenAll(firstTask, secondTask);
        Assert.Null(RuntimeExecutionContext.Current);
    }

    [Fact]
    public void EnterAsRootSuppressesInheritedRuntimeAndInvocationState()
    {
        var parent = CreateContext();
        var child = CreateContext();
        var parentThis = new object();
        var parentArguments = new object?[] { "parent" };
        var parentNewTarget = new object();
        var parentCallee = new object();

        using (parent.Enter())
        {
            RuntimeServices.SetCurrentThis(parentThis);
            RuntimeServices.SetCurrentArguments(parentArguments);
            RuntimeServices.SetCurrentNewTarget(parentNewTarget);
            RuntimeServices.SetCurrentCallee(parentCallee);
            Engine._serviceProviderOverride.Value = parent.Services;

            using (child.EnterAsRoot())
            {
                Assert.Same(child, RuntimeExecutionContext.Current);
                Assert.Null(RuntimeServices.GetCurrentThis());
                Assert.Null(RuntimeServices.GetCurrentArguments());
                Assert.Null(RuntimeServices.GetCurrentNewTarget());
                Assert.Null(RuntimeServices.GetCurrentCallee());
                Assert.Null(Engine._serviceProviderOverride.Value);
            }

            Assert.Same(parent, RuntimeExecutionContext.Current);
            Assert.Same(parentThis, RuntimeServices.GetCurrentThis());
            Assert.Same(parentArguments, RuntimeServices.GetCurrentArguments());
            Assert.Same(parentNewTarget, RuntimeServices.GetCurrentNewTarget());
            Assert.Same(parentCallee, RuntimeServices.GetCurrentCallee());
            Assert.Same(parent.Services, Engine._serviceProviderOverride.Value);

            Engine._serviceProviderOverride.Value = null;
            RuntimeServices.SetCurrentThis(null);
            RuntimeServices.SetCurrentArguments(null);
            RuntimeServices.SetCurrentNewTarget(null);
            RuntimeServices.SetCurrentCallee(null);
        }
    }

    [Fact]
    public void ModuleLocationFollowsTheActiveFrame()
    {
        var first = CreateContext();
        var second = CreateContext();

        using (first.Enter())
        {
            ModuleContext.SetModuleContext("/first", "/first/main.js");
            AssertModuleLocation("/first", "/first/main.js");

            using (second.Enter())
            {
                ModuleContext.SetModuleContext("/second", "/second/main.js");
                AssertModuleLocation("/second", "/second/main.js");
            }

            AssertModuleLocation("/first", "/first/main.js");
        }
    }

    [Fact]
    public void EachContextKeepsItsGlobalObjectIdentity()
    {
        var first = CreateContext();
        var second = CreateContext();
        object firstGlobal;

        using (first.Enter())
        {
            firstGlobal = GlobalThis.globalThis;
        }

        using (second.Enter())
        {
            Assert.NotSame(firstGlobal, GlobalThis.globalThis);
        }

        using (first.Enter())
        {
            Assert.Same(firstGlobal, GlobalThis.globalThis);
        }
    }

    [Fact]
    public void DisposedRealmCannotBeEntered()
    {
        var context = CreateContext();
        context.Realm.Dispose();

        _ = Assert.Throws<ObjectDisposedException>(() => context.Enter());
    }

    private static RuntimeExecutionContext CreateContext()
    {
        var services = RuntimeServices.BuildServiceProvider();
        return RuntimeExecutionContext.GetOrCreate(services);
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
