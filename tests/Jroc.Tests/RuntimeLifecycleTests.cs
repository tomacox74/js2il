using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;
using JavaScriptRuntime.EngineCore;

namespace Jroc.Tests;

public sealed class RuntimeLifecycleTests
{
    [Fact]
    public void ExecuteEntersRootPumpsWorkAndDisposesOwnedGraph()
    {
        RuntimeAgentCluster cluster;
        RuntimeAgent agent;
        RuntimeRealm realm;
        var microtaskRan = false;

        using (var lifecycle = CreateLifecycle())
        {
            cluster = lifecycle.Cluster;
            agent = lifecycle.Agent;
            realm = lifecycle.Realm;

            lifecycle.Execute(
                services =>
                {
                    Assert.Same(
                        lifecycle.ExecutionContext,
                        RuntimeExecutionContext.Current);
                    Assert.Same(realm, RuntimeExecutionContext.Current!.Realm);
                    services.Resolve<IMicrotaskScheduler>()
                        .QueueMicrotask(() => microtaskRan = true);
                },
                waitForTimers: false);

            Assert.True(microtaskRan);
            Assert.Null(RuntimeExecutionContext.Current);
        }

        Assert.True(realm.IsDisposed);
        Assert.True(agent.IsDisposed);
        Assert.True(cluster.IsDisposed);
        Assert.True(realm.DisposalOrder < agent.DisposalOrder);
        Assert.True(agent.DisposalOrder < cluster.DisposalOrder);
    }

    [Fact]
    public void ExplicitClusterCanHostAReplacementAgentAfterLifecycleDisposal()
    {
        var cluster = new RuntimeAgentCluster();
        RuntimeAgent firstAgent;

        using (var lifecycle = CreateLifecycle(cluster: cluster))
        {
            firstAgent = lifecycle.Agent;
            Assert.Same(cluster, lifecycle.Cluster);
            Assert.Equal(1, cluster.AgentCount);
        }

        Assert.True(firstAgent.IsDisposed);
        Assert.False(cluster.IsDisposed);
        Assert.Equal(0, cluster.AgentCount);

        var replacement = cluster.CreateAgent();
        Assert.False(replacement.IsDisposed);
        cluster.Dispose();
    }

    [Fact]
    public void ConfigurationFailureUnwindsAgentButNotSuppliedCluster()
    {
        var cluster = new RuntimeAgentCluster();

        var exception = Assert.Throws<InvalidOperationException>(
            () => CreateLifecycle(
                cluster,
                _ => throw new InvalidOperationException("bootstrap failed")));

        Assert.Equal("bootstrap failed", exception.Message);
        Assert.Equal(0, cluster.AgentCount);
        Assert.False(cluster.IsDisposed);
        Assert.Null(RuntimeExecutionContext.Current);
        cluster.Dispose();
    }

    [Fact]
    public async Task HostedLifecycleSuppressesAndRestoresInheritedRuntimeState()
    {
        var parentServices = RuntimeServices.BuildServiceProvider();
        var parentContext = RuntimeExecutionContext.GetOrCreate(parentServices);
        var parentThis = new object();

        using (parentContext.EnterAsRoot())
        {
            RuntimeServices.SetCurrentThis(parentThis);
            Engine._serviceProviderOverride.Value = parentServices;

            await Task.Run(() =>
            {
                Assert.Same(parentContext, RuntimeExecutionContext.Current);
                Assert.Same(parentThis, RuntimeServices.GetCurrentThis());

                using (var lifecycle = CreateLifecycle(
                    suppressInheritedExecutionContext: true))
                {
                    lifecycle.Execute(
                        _ =>
                        {
                            Assert.NotSame(
                                parentContext,
                                RuntimeExecutionContext.Current);
                            Assert.Null(RuntimeServices.GetCurrentThis());
                            Assert.Null(Engine._serviceProviderOverride.Value);
                        },
                        waitForTimers: false);
                }

                Assert.Same(parentContext, RuntimeExecutionContext.Current);
                Assert.Same(parentThis, RuntimeServices.GetCurrentThis());
                Assert.Same(
                    parentServices,
                    Engine._serviceProviderOverride.Value);
            });

            Engine._serviceProviderOverride.Value = null;
            RuntimeServices.SetCurrentThis(null);
        }

        parentServices.Resolve<RuntimeAgentCluster>().Dispose();
    }

    [Fact]
    public async Task ConcurrentHostedLifecyclesKeepObservableStateIsolated()
    {
        using var ready = new Barrier(2);
        var first = RunIsolatedLifecycle("first", ready);
        var second = RunIsolatedLifecycle("second", ready);

        var results = await Task.WhenAll(first, second);

        Assert.Equal("first", results[0].Value);
        Assert.Equal("second", results[1].Value);
        Assert.NotSame(results[0].Realm, results[1].Realm);
        Assert.NotSame(results[0].Global, results[1].Global);
        Assert.NotSame(results[0].Scheduler, results[1].Scheduler);
        Assert.All(results, result => Assert.True(result.ClusterDisposed));
        Assert.Null(RuntimeExecutionContext.Current);
    }

    [Fact]
    public void BorrowedServiceProviderIsConfiguredButNotDisposed()
    {
        var services = RuntimeServices.BuildServiceProvider();
        var realm = services.OwningRealm!;
        var agent = realm.Agent;

        using (var lifecycle = RuntimeLifecycle.Create(
            typeof(RuntimeLifecycleTests).Assembly,
            isHostedExecution: false,
            existingServices: services))
        {
            lifecycle.Execute(
                activeServices => Assert.Same(services, activeServices),
                waitForTimers: false);
        }

        Assert.False(realm.IsDisposed);
        Assert.False(agent.IsDisposed);
        agent.Cluster.Dispose();
    }

    [Fact]
    public void RepeatedCreateExecuteDisposeCyclesLeaveNoAmbientState()
    {
        for (var index = 0; index < 5; index++)
        {
            RuntimeAgentCluster cluster;
            using (var lifecycle = CreateLifecycle())
            {
                cluster = lifecycle.Cluster;
                lifecycle.Execute(
                    _ => Assert.NotNull(RuntimeExecutionContext.Current),
                    waitForTimers: false);
            }

            Assert.True(cluster.IsDisposed);
            Assert.Null(RuntimeExecutionContext.Current);
        }
    }

    private static RuntimeLifecycle CreateLifecycle(
        RuntimeAgentCluster? cluster = null,
        Action<ServiceContainer>? configureServices = null,
        bool suppressInheritedExecutionContext = false)
        => RuntimeLifecycle.Create(
            typeof(RuntimeLifecycleTests).Assembly,
            isHostedExecution: true,
            cluster: cluster,
            configureServices: configureServices,
            suppressInheritedExecutionContext: suppressInheritedExecutionContext);

    private static Task<IsolationResult> RunIsolatedLifecycle(
        string value,
        Barrier ready)
        => Task.Run(() =>
        {
            RuntimeRealm realm;
            object global;
            NodeSchedulerState scheduler;
            RuntimeAgentCluster cluster;

            using (var lifecycle = CreateLifecycle(
                suppressInheritedExecutionContext: true))
            {
                realm = lifecycle.Realm;
                cluster = lifecycle.Cluster;
                scheduler = lifecycle.Services.Resolve<NodeSchedulerState>();
                object? runtimeGlobal = null;

                lifecycle.Execute(
                    _ =>
                    {
                        runtimeGlobal = lifecycle.ExecutionContext.GetOrCreateGlobalObject();
                        var global = runtimeGlobal;
                        ObjectRuntime.SetItem(global, "lifecycleValue", value);
                        Assert.True(ready.SignalAndWait(TimeSpan.FromSeconds(5)));
                        Assert.Equal(
                            value,
                            ObjectRuntime.GetItem(global, "lifecycleValue"));
                    },
                    waitForTimers: false);
                global = runtimeGlobal
                    ?? throw new InvalidOperationException(
                        "The lifecycle did not create its global object.");
            }

            return new IsolationResult(
                realm,
                global,
                scheduler,
                ObjectRuntime.GetItem(global, "lifecycleValue"),
                cluster.IsDisposed);
        });

    private sealed record IsolationResult(
        RuntimeRealm Realm,
        object Global,
        NodeSchedulerState Scheduler,
        object? Value,
        bool ClusterDisposed);
}
