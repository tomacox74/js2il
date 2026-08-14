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
            _agents.Clear();

            for (var index = agents.Length - 1; index >= 0; index--)
            {
                agents[index].Dispose();
            }

            DisposalOrder = NextDisposalOrder();
            _state = RuntimeOwnershipState.Disposed;
        }
    }

    internal long NextDisposalOrder()
        => Interlocked.Increment(ref _disposalSequence);

    internal void Detach(RuntimeAgent agent)
    {
        lock (_gate)
        {
            _agents.Remove(agent);
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
    }

    internal RuntimeAgentCluster Cluster { get; }

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

    public void Dispose()
    {
        lock (_gate)
        {
            if (_state == RuntimeOwnershipState.Disposed)
            {
                return;
            }

            _state = RuntimeOwnershipState.Disposing;
            var realms = _realms.ToArray();
            _realms.Clear();

            for (var index = realms.Length - 1; index >= 0; index--)
            {
                realms[index].Dispose();
            }

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
        Services = new ServiceContainer();
        Services.AttachOwningRealm(this);
    }

    internal RuntimeAgent Agent { get; }

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
