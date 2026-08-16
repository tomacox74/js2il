using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class RuntimeAgentClusterSharedServicesTests
{
    [Fact]
    public void SymbolRegistryIsSharedByRealmsInOneAgentAndIsolatedBetweenAgents()
    {
        var cluster = new RuntimeAgentCluster();
        var firstAgent = cluster.CreateAgent();
        var firstRealm = CreateConfiguredRealm(firstAgent);
        var secondRealm = CreateConfiguredRealm(firstAgent);
        var otherAgent = cluster.CreateAgent();
        var otherRealm = CreateConfiguredRealm(otherAgent);
        var firstContext = RuntimeExecutionContext.GetOrCreate(firstRealm.Services);
        var secondContext = RuntimeExecutionContext.GetOrCreate(secondRealm.Services);
        var otherContext = RuntimeExecutionContext.GetOrCreate(otherRealm.Services);

        Symbol first;
        using (firstContext.EnterAsRoot())
        {
            first = Assert.IsType<Symbol>(Symbol.@for("shared"));
            Assert.Same(first, Symbol.@for("shared"));
            Assert.Equal("shared", Symbol.keyFor(first));
        }

        using (secondContext.EnterAsRoot())
        {
            Assert.Same(first, Symbol.@for("shared"));
            Assert.Equal("shared", Symbol.keyFor(first));
        }

        using (otherContext.EnterAsRoot())
        {
            Assert.NotSame(first, Symbol.@for("shared"));
            Assert.Null(Symbol.keyFor(first));
        }

        cluster.Dispose();
    }

    [Fact]
    public void MessageTransportQueuesOpaqueCopiesAcrossAgentsInOneCluster()
    {
        var cluster = new RuntimeAgentCluster();
        var firstAgent = cluster.CreateAgent();
        var secondAgent = cluster.CreateAgent();
        var (first, second) = cluster.SharedServices.Transport.CreateEntangledPair(
            firstAgent,
            secondAgent);
        var payload = new byte[] { 1, 2, 3 };

        Assert.True(first.Post(payload));
        payload[0] = 9;
        Assert.True(second.TryReceive(out var received));
        Assert.Equal([1, 2, 3], received);
        Assert.False(first.TryReceive(out _));

        secondAgent.Dispose();

        Assert.False(first.Post([4]));
        cluster.Dispose();
    }

    [Fact]
    public void MessageTransportRejectsAnAgentFromAnotherCluster()
    {
        var firstCluster = new RuntimeAgentCluster();
        var secondCluster = new RuntimeAgentCluster();
        var firstAgent = firstCluster.CreateAgent();
        var secondAgent = secondCluster.CreateAgent();

        Assert.Throws<InvalidOperationException>(
            () => firstCluster.SharedServices.Transport.CreateEntangledPair(
                firstAgent,
                secondAgent));

        firstCluster.Dispose();
        secondCluster.Dispose();
    }

    [Fact]
    public void BroadcastRegistryFansOutByNameAndUnregistersDisposedAgents()
    {
        var cluster = new RuntimeAgentCluster();
        var firstAgent = cluster.CreateAgent();
        var secondAgent = cluster.CreateAgent();
        var first = cluster.SharedServices.Broadcasts.Register(firstAgent, "events");
        var second = cluster.SharedServices.Broadcasts.Register(secondAgent, "events");
        var otherName = cluster.SharedServices.Broadcasts.Register(secondAgent, "other");

        Assert.Equal(1, first.Post([1, 2]));
        Assert.True(second.TryReceive(out var message));
        Assert.Equal([1, 2], message);
        Assert.False(first.TryReceive(out _));
        Assert.False(otherName.TryReceive(out _));

        secondAgent.Dispose();

        Assert.Equal(0, first.Post([3]));
        cluster.Dispose();
    }

    [Fact]
    public void SharedBufferWrappersShareBackingOnlyWithinTheirAgentCluster()
    {
        var cluster = new RuntimeAgentCluster();
        var firstAgent = cluster.CreateAgent();
        var firstRealm = CreateConfiguredRealm(firstAgent);
        var secondAgent = cluster.CreateAgent();
        var secondRealm = CreateConfiguredRealm(secondAgent);
        var firstContext = RuntimeExecutionContext.GetOrCreate(firstRealm.Services);
        var secondContext = RuntimeExecutionContext.GetOrCreate(secondRealm.Services);
        SharedArrayBuffer first;

        using (firstContext.EnterAsRoot())
        {
            first = new SharedArrayBuffer(4d);
            first.RawBytes[0] = 42;
        }

        using (secondContext.EnterAsRoot())
        {
            var second = first.CreateWrapperForCurrentRealm();
            Assert.NotSame(first, second);
            Assert.Equal(42, second.RawBytes[0]);
            Assert.NotSame(
                PrototypeChain.GetPrototypeOrNull(first),
                PrototypeChain.GetPrototypeOrNull(second));
        }

        var otherCluster = new RuntimeAgentCluster();
        var otherRealm = CreateConfiguredRealm(otherCluster.CreateAgent());
        var otherContext = RuntimeExecutionContext.GetOrCreate(otherRealm.Services);
        using (otherContext.EnterAsRoot())
        {
            Assert.Throws<InvalidOperationException>(
                first.CreateWrapperForCurrentRealm);
        }

        firstAgent.Dispose();
        Assert.Equal(4d, first.byteLength);
        secondAgent.Dispose();
        Assert.Equal(0d, first.byteLength);

        cluster.Dispose();
        otherCluster.Dispose();
    }

    [Fact]
    public void DisposingAnAgentRemovesAndReleasesItsAtomicsWaiters()
    {
        var cluster = new RuntimeAgentCluster();
        var agent = cluster.CreateAgent();
        var realm = CreateConfiguredRealm(agent);
        var context = RuntimeExecutionContext.GetOrCreate(realm.Services);
        RuntimeSharedArrayBufferBackingStore store;
        using (context.EnterAsRoot())
        {
            store = new SharedArrayBuffer(4d).BackingStore;
        }

        RuntimeAtomicsWaitResult? waitResult = null;
        Exception? failure = null;
        var waiterThread = new Thread(() =>
        {
            try
            {
                using var scope = context.EnterAsRoot();
                waitResult = cluster.SharedServices.Atomics.Wait(
                    agent,
                    store,
                    0,
                    expectedValue: 0,
                    timeoutMilliseconds: System.Threading.Timeout.Infinite);
            }
            catch (Exception exception)
            {
                failure = exception;
            }
        });
        waiterThread.Start();
        Assert.True(SpinWait.SpinUntil(
            () => cluster.SharedServices.Atomics.WaiterCount == 1,
            TimeSpan.FromSeconds(5)));

        agent.Dispose();
        waiterThread.Join();

        Assert.Null(failure);
        Assert.Equal(RuntimeAtomicsWaitResult.TimedOut, waitResult);
        Assert.Equal(0, cluster.SharedServices.Atomics.WaiterCount);
        Assert.True(store.IsReleased);
        cluster.Dispose();
    }

    [Fact]
    public void AtomicsDomainNotifiesAcrossAgentsOnlyForTheSharedBackingStoreLocation()
    {
        var cluster = new RuntimeAgentCluster();
        var firstAgent = cluster.CreateAgent();
        var firstRealm = CreateConfiguredRealm(firstAgent);
        var secondAgent = cluster.CreateAgent();
        var context = RuntimeExecutionContext.GetOrCreate(firstRealm.Services);
        RuntimeSharedArrayBufferBackingStore store;
        using (context.EnterAsRoot())
        {
            store = new SharedArrayBuffer(8d).BackingStore;
        }

        RuntimeAtomicsWaitResult? waitResult = null;
        var waiterThread = new Thread(
            () => waitResult = cluster.SharedServices.Atomics.Wait(
                firstAgent,
                store,
                4,
                expectedValue: 0,
                timeoutMilliseconds: System.Threading.Timeout.Infinite));
        waiterThread.Start();
        Assert.True(SpinWait.SpinUntil(
            () => cluster.SharedServices.Atomics.WaiterCount == 1,
            TimeSpan.FromSeconds(5)));

        Assert.Equal(
            0,
            cluster.SharedServices.Atomics.Notify(store, byteOffset: 0, count: 1));
        Assert.Equal(
            1,
            cluster.SharedServices.Atomics.Notify(store, byteOffset: 4, count: 1));
        waiterThread.Join();

        Assert.Equal(RuntimeAtomicsWaitResult.Notified, waitResult);
        Assert.Equal(0, cluster.SharedServices.Atomics.WaiterCount);
        secondAgent.Dispose();
        cluster.Dispose();
    }

    [Fact]
    public void AgentAndClusterSharedServicesAreReservedRealmServices()
    {
        var cluster = new RuntimeAgentCluster();
        var agent = cluster.CreateAgent();
        var first = agent.CreateRealm();
        var second = agent.CreateRealm();

        Assert.Same(
            agent.SymbolRegistry,
            first.Services.Resolve<RuntimeAgentSymbolRegistry>());
        Assert.Same(
            cluster.SharedServices.Transport,
            second.Services.Resolve<RuntimeMessageTransportService>());
        Assert.Same(
            first.Services.Resolve<RuntimeSharedMemoryService>(),
            second.Services.Resolve<RuntimeSharedMemoryService>());
        Assert.Throws<InvalidOperationException>(
            () => first.Services.Remove<RuntimeAtomicsSynchronizationDomain>());

        first.Services.Clear();

        Assert.Same(
            cluster.SharedServices.Broadcasts,
            first.Services.Resolve<RuntimeBroadcastChannelRegistry>());
        cluster.Dispose();
    }

    private static RuntimeRealm CreateConfiguredRealm(RuntimeAgent agent)
    {
        var realm = agent.CreateRealm();
        realm.Services.RegisterInstance<IPropertyDescriptorStore>(
            new PropertyDescriptorStore());
        return realm;
    }
}
