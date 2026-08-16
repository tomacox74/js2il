using JavaScriptRuntime.DependencyInjection;

namespace JavaScriptRuntime;

internal enum RuntimeOwnershipState
{
    Active,
    Disposing,
    Disposed
}

internal sealed class RuntimeAgentCluster : IDisposable
{
    private readonly object _gate = new();
    private readonly List<RuntimeAgent> _agents = [];
    private RuntimeOwnershipState _state;
    private long _disposalSequence;

    internal RuntimeAgentCluster()
    {
        SharedServices = new RuntimeAgentClusterSharedServices(this);
    }

    internal RuntimeAgentClusterSharedServices SharedServices { get; }

    internal bool IsDisposed
    {
        get
        {
            lock (_gate)
            {
                return _state != RuntimeOwnershipState.Active;
            }
        }
    }

    internal int AgentCount
    {
        get
        {
            lock (_gate)
            {
                return _agents.Count;
            }
        }
    }

    internal long DisposalOrder { get; private set; }

    internal RuntimeAgent CreateAgent()
    {
        lock (_gate)
        {
            ThrowIfDisposed();

            var agent = new RuntimeAgent(this);
            _agents.Add(agent);
            return agent;
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_state == RuntimeOwnershipState.Disposed)
            {
                return;
            }

            _state = RuntimeOwnershipState.Disposing;
            var agents = _agents.ToArray();

            for (var index = agents.Length - 1; index >= 0; index--)
            {
                agents[index].Dispose();
            }

            SharedServices.Dispose();
            DisposalOrder = NextDisposalOrder();
            _state = RuntimeOwnershipState.Disposed;
        }
    }

    internal long NextDisposalOrder()
        => Interlocked.Increment(ref _disposalSequence);

    internal T WhileAgentsActive<T>(
        RuntimeAgent first,
        RuntimeAgent? second,
        Func<T> action)
    {
        lock (_gate)
        {
            ThrowIfDisposed();
            if (!ReferenceEquals(first.Cluster, this)
                || (second != null && !ReferenceEquals(second.Cluster, this)))
            {
                throw new InvalidOperationException(
                    "Agent-cluster services cannot be used by an agent from another cluster.");
            }

            if (!_agents.Contains(first)
                || (second != null && !_agents.Contains(second)))
            {
                throw new ObjectDisposedException(nameof(RuntimeAgent));
            }

            return action();
        }
    }

    internal void Detach(RuntimeAgent agent)
    {
        lock (_gate)
        {
            if (_agents.Remove(agent))
            {
                SharedServices.RemoveAgent(agent, _agents.Count == 0);
            }
        }
    }

    private void ThrowIfDisposed()
    {
        if (_state != RuntimeOwnershipState.Active)
        {
            throw new ObjectDisposedException(nameof(RuntimeAgentCluster));
        }
    }
}

internal sealed class RuntimeAgent : IDisposable
{
    private readonly object _gate = new();
    private readonly List<RuntimeRealm> _realms = [];
    private RuntimeOwnershipState _state;

    internal RuntimeAgent(RuntimeAgentCluster cluster)
    {
        Cluster = cluster;
        Scheduling = new RuntimeAgentSchedulingState();
        SymbolRegistry = new RuntimeAgentSymbolRegistry();
    }

    internal RuntimeAgentCluster Cluster { get; }

    internal RuntimeAgentSchedulingState Scheduling { get; }

    internal RuntimeAgentSymbolRegistry SymbolRegistry { get; }

    internal CancellationToken ShutdownToken => Scheduling.ShutdownToken;

    internal bool IsDisposed
    {
        get
        {
            lock (_gate)
            {
                return _state != RuntimeOwnershipState.Active;
            }
        }
    }

    internal int RealmCount
    {
        get
        {
            lock (_gate)
            {
                return _realms.Count;
            }
        }
    }

    internal long DisposalOrder { get; private set; }

    internal RuntimeRealm CreateRealm()
    {
        lock (_gate)
        {
            ThrowIfDisposed();

            var realm = new RuntimeRealm(this);
            _realms.Add(realm);
            return realm;
        }
    }

    internal void EnqueueFromExternalThread(Action callback)
        => Scheduling.EnqueueFromExternalThread(callback);

    internal void RequestShutdown()
    {
        lock (_gate)
        {
            if (_state == RuntimeOwnershipState.Disposed)
            {
                return;
            }

            _state = RuntimeOwnershipState.Disposing;
        }

        Scheduling.RequestShutdown();
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_state == RuntimeOwnershipState.Disposed)
            {
                return;
            }

            _state = RuntimeOwnershipState.Disposing;
            Scheduling.RequestShutdown();
            var realms = _realms.ToArray();
            _realms.Clear();

            for (var index = realms.Length - 1; index >= 0; index--)
            {
                realms[index].Dispose();
            }

            Scheduling.Dispose();
            SymbolRegistry.Dispose();
            DisposalOrder = Cluster.NextDisposalOrder();
            _state = RuntimeOwnershipState.Disposed;
        }

        Cluster.Detach(this);
    }

    internal void Detach(RuntimeRealm realm)
    {
        lock (_gate)
        {
            _realms.Remove(realm);
        }
    }

    private void ThrowIfDisposed()
    {
        if (_state != RuntimeOwnershipState.Active)
        {
            throw new ObjectDisposedException(nameof(RuntimeAgent));
        }
    }
}

internal sealed class RuntimeRealm : IDisposable
{
    private readonly object _gate = new();
    private RuntimeOwnershipState _state;

    internal RuntimeRealm(RuntimeAgent agent)
    {
        Agent = agent;
        ModuleState = new Modules.RuntimeModuleState();
        ValueCaches = new RuntimeRealmValueCacheState();
        Intrinsics = new RuntimeIntrinsics();
        Services = new ServiceContainer();
        Services.AttachOwningRealm(this);
        Services.Register<EngineCore.ITickSource, EngineCore.TickSource>();
        Services.Register<
            EngineCore.IWaitHandle,
            EngineCore.WaitHandle>();
    }

    internal RuntimeAgent Agent { get; }

    internal Modules.RuntimeModuleState ModuleState { get; }

    internal RuntimeRealmValueCacheState ValueCaches { get; }

    /// <summary>
    /// The realm's well-known intrinsic object graph (ECMA-262 Realm Record
    /// [[Intrinsics]]). See <see cref="RuntimeIntrinsics"/> for scope/limitations.
    /// </summary>
    internal RuntimeIntrinsics Intrinsics { get; }

    internal ServiceContainer Services { get; }

    internal bool IsDisposed
    {
        get
        {
            lock (_gate)
            {
                return _state != RuntimeOwnershipState.Active;
            }
        }
    }

    internal long DisposalOrder { get; private set; }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_state == RuntimeOwnershipState.Disposed)
            {
                return;
            }

            _state = RuntimeOwnershipState.Disposing;
            ModuleState.Dispose();
            ValueCaches.Dispose();
            Intrinsics.Dispose();
            DisposalOrder = Agent.Cluster.NextDisposalOrder();
            _state = RuntimeOwnershipState.Disposed;
        }

        Agent.Detach(this);
    }
}

internal static class RuntimeOwnershipFactory
{
    internal static RuntimeRealm CreateIsolatedRealm()
    {
        var cluster = new RuntimeAgentCluster();

        try
        {
            return cluster.CreateAgent().CreateRealm();
        }
        catch
        {
            cluster.Dispose();
            throw;
        }
    }
}
