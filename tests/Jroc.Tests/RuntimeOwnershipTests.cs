using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests;

public sealed class RuntimeOwnershipTests
{
    [Fact]
    public void CreateIsolatedRealm_BuildsExplicitOwnershipGraph()
    {
        var realm = RuntimeOwnershipFactory.CreateIsolatedRealm();
        var agent = realm.Agent;
        var cluster = agent.Cluster;

        Assert.Same(realm, realm.Services.OwningRealm);
        Assert.Same(realm, realm.Services.Resolve<RuntimeRealm>());
        Assert.Same(agent, realm.Services.Resolve<RuntimeAgent>());
        Assert.Same(cluster, realm.Services.Resolve<RuntimeAgentCluster>());
        Assert.Equal(1, agent.RealmCount);
        Assert.Equal(1, cluster.AgentCount);

        cluster.Dispose();
    }

    [Fact]
    public void RuntimeServices_RegistersOwnersForConstructorInjection()
    {
        var services = RuntimeServices.BuildServiceProvider();
        var consumer = services.Resolve<OwnerAwareService>();

        Assert.Same(services.OwningRealm, consumer.Realm);
        Assert.Same(consumer.Realm.Agent, consumer.Agent);
        Assert.Same(consumer.Agent.Cluster, consumer.Cluster);

        consumer.Cluster.Dispose();
    }

    [Fact]
    public void RuntimeServices_CreatesAnIndependentOwnershipGraphPerContainer()
    {
        var first = RuntimeServices.BuildServiceProvider();
        var second = RuntimeServices.BuildServiceProvider();

        Assert.NotSame(first.OwningRealm, second.OwningRealm);
        Assert.NotSame(
            first.Resolve<RuntimeAgent>(),
            second.Resolve<RuntimeAgent>());
        Assert.NotSame(
            first.Resolve<RuntimeAgentCluster>(),
            second.Resolve<RuntimeAgentCluster>());

        first.Resolve<RuntimeAgentCluster>().Dispose();
        second.Resolve<RuntimeAgentCluster>().Dispose();
    }

    [Fact]
    public void Agent_CanOwnMultipleRealms()
    {
        var first = RuntimeOwnershipFactory.CreateIsolatedRealm();
        var agent = first.Agent;
        var second = agent.CreateRealm();

        Assert.NotSame(first, second);
        Assert.Same(agent, second.Agent);
        Assert.Equal(2, agent.RealmCount);
        Assert.Same(second, second.Services.OwningRealm);

        agent.Cluster.Dispose();
    }

    [Fact]
    public void ClusterDisposal_DisposesChildrenInReverseOwnershipOrder()
    {
        var cluster = new RuntimeAgentCluster();
        var firstAgent = cluster.CreateAgent();
        var firstRealm = firstAgent.CreateRealm();
        var secondRealm = firstAgent.CreateRealm();
        var secondAgent = cluster.CreateAgent();
        var thirdRealm = secondAgent.CreateRealm();

        cluster.Dispose();

        Assert.True(firstRealm.IsDisposed);
        Assert.True(secondRealm.IsDisposed);
        Assert.True(thirdRealm.IsDisposed);
        Assert.True(firstAgent.IsDisposed);
        Assert.True(secondAgent.IsDisposed);
        Assert.True(cluster.IsDisposed);

        Assert.True(thirdRealm.DisposalOrder < secondAgent.DisposalOrder);
        Assert.True(secondAgent.DisposalOrder < secondRealm.DisposalOrder);
        Assert.True(secondRealm.DisposalOrder < firstRealm.DisposalOrder);
        Assert.True(firstRealm.DisposalOrder < firstAgent.DisposalOrder);
        Assert.True(firstAgent.DisposalOrder < cluster.DisposalOrder);
    }

    [Fact]
    public void Disposal_IsIdempotentAndDetachesChildren()
    {
        var cluster = new RuntimeAgentCluster();
        var agent = cluster.CreateAgent();
        var realm = agent.CreateRealm();

        realm.Dispose();
        realm.Dispose();

        Assert.True(realm.IsDisposed);
        Assert.Equal(0, agent.RealmCount);

        agent.Dispose();
        agent.Dispose();

        Assert.True(agent.IsDisposed);
        Assert.Equal(0, cluster.AgentCount);

        cluster.Dispose();
        cluster.Dispose();

        Assert.True(cluster.IsDisposed);
    }

    [Fact]
    public void DisposedOwnersAndTheirServiceContainersCannotBeReused()
    {
        var realm = RuntimeOwnershipFactory.CreateIsolatedRealm();
        var services = realm.Services;
        var agent = realm.Agent;
        var cluster = agent.Cluster;

        realm.Dispose();

        Assert.Throws<ObjectDisposedException>(() => services.Resolve<RuntimeRealm>());

        agent.Dispose();

        Assert.Throws<ObjectDisposedException>(() => agent.CreateRealm());

        cluster.Dispose();

        Assert.Throws<ObjectDisposedException>(() => cluster.CreateAgent());
    }

    [Fact]
    public void RealmOwnershipRegistrationsCannotBeReplacedRemovedOrCleared()
    {
        var realm = RuntimeOwnershipFactory.CreateIsolatedRealm();
        var services = realm.Services;

        Assert.Throws<InvalidOperationException>(
            () => services.Replace(new RuntimeAgentCluster()));
        Assert.Throws<InvalidOperationException>(
            () => services.Remove<RuntimeRealm>());

        services.Clear();

        Assert.Same(realm, services.Resolve<RuntimeRealm>());
        Assert.Same(realm.Agent, services.Resolve<RuntimeAgent>());
        Assert.Same(realm.Agent.Cluster, services.Resolve<RuntimeAgentCluster>());

        realm.Agent.Cluster.Dispose();
    }

    [Fact]
    public void ChildServiceContainerKeepsTheSameRealmOwner()
    {
        var realm = RuntimeOwnershipFactory.CreateIsolatedRealm();
        var child = realm.Services.CreateScope();

        Assert.Same(realm, child.OwningRealm);
        Assert.Same(realm, child.Resolve<RuntimeRealm>());

        realm.Dispose();

        Assert.Throws<ObjectDisposedException>(() => child.Resolve<RuntimeRealm>());

        realm.Agent.Cluster.Dispose();
    }

    [Fact]
    public void ServiceContainerCannotBeAttachedToASecondRealm()
    {
        var first = RuntimeOwnershipFactory.CreateIsolatedRealm();
        var second = RuntimeOwnershipFactory.CreateIsolatedRealm();

        Assert.Throws<InvalidOperationException>(
            () => first.Services.AttachOwningRealm(second));
        Assert.Same(first, first.Services.OwningRealm);

        first.Agent.Cluster.Dispose();
        second.Agent.Cluster.Dispose();
    }

    private sealed class OwnerAwareService
    {
        public OwnerAwareService(
            RuntimeAgentCluster cluster,
            RuntimeAgent agent,
            RuntimeRealm realm)
        {
            Cluster = cluster;
            Agent = agent;
            Realm = realm;
        }

        internal RuntimeAgentCluster Cluster { get; }

        internal RuntimeAgent Agent { get; }

        internal RuntimeRealm Realm { get; }
    }
}
