namespace JavaScriptRuntime;

internal sealed class RuntimeAgentClusterSharedServices : IDisposable
{
    private int _disposed;

    internal RuntimeAgentClusterSharedServices(RuntimeAgentCluster cluster)
    {
        Cluster = cluster;
        Transport = new RuntimeMessageTransportService(cluster);
        Broadcasts = new RuntimeBroadcastChannelRegistry(cluster);
        Atomics = new RuntimeAtomicsSynchronizationDomain(cluster);
        SharedMemory = new RuntimeSharedMemoryService(this);
    }

    internal RuntimeAgentCluster Cluster { get; }

    internal RuntimeMessageTransportService Transport { get; }

    internal RuntimeBroadcastChannelRegistry Broadcasts { get; }

    internal RuntimeSharedMemoryService SharedMemory { get; }

    internal RuntimeAtomicsSynchronizationDomain Atomics { get; }

    internal void RemoveAgent(RuntimeAgent agent, bool isLastAgent)
    {
        if (Volatile.Read(ref _disposed) != 0)
        {
            return;
        }

        Transport.RemoveAgent(agent);
        Broadcasts.RemoveAgent(agent);
        Atomics.RemoveAgent(agent);
        if (isLastAgent)
        {
            SharedMemory.ReleaseAll();
            Atomics.Reset();
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        Transport.Dispose();
        Broadcasts.Dispose();
        Atomics.Dispose();
        SharedMemory.Dispose();
    }
}

internal sealed class RuntimeMessageTransportService : IDisposable
{
    private readonly object _gate = new();
    private readonly RuntimeAgentCluster _cluster;
    private readonly Dictionary<long, RuntimeMessagePortCore> _ports = new();
    private long _nextId;
    private bool _disposed;

    internal RuntimeMessageTransportService(RuntimeAgentCluster cluster)
    {
        _cluster = cluster;
    }

    internal (RuntimeMessagePortCore First, RuntimeMessagePortCore Second) CreateEntangledPair(
        RuntimeAgent firstOwner,
        RuntimeAgent secondOwner)
    {
        ArgumentNullException.ThrowIfNull(firstOwner);
        ArgumentNullException.ThrowIfNull(secondOwner);
        return _cluster.WhileAgentsActive(
            firstOwner,
            secondOwner,
            () =>
            {
                lock (_gate)
                {
                    ThrowIfDisposed();
                    var firstId = ++_nextId;
                    var secondId = ++_nextId;
                    var first = new RuntimeMessagePortCore(
                        this,
                        firstId,
                        firstOwner,
                        secondId);
                    var second = new RuntimeMessagePortCore(
                        this,
                        secondId,
                        secondOwner,
                        firstId);
                    _ports.Add(firstId, first);
                    _ports.Add(secondId, second);
                    return (first, second);
                }
            });
    }

    internal bool Post(long senderId, long receiverId, ReadOnlySpan<byte> payload)
    {
        RuntimeMessagePortCore? receiver;
        lock (_gate)
        {
            ThrowIfDisposed();
            if (!_ports.ContainsKey(senderId)
                || !_ports.TryGetValue(receiverId, out receiver))
            {
                return false;
            }
        }

        return receiver.Enqueue(payload);
    }

    internal void Close(long id)
    {
        RuntimeMessagePortCore? port;
        RuntimeMessagePortCore? peer = null;
        lock (_gate)
        {
            if (!_ports.Remove(id, out port))
            {
                return;
            }

            if (_ports.TryGetValue(port.PeerId, out peer))
            {
                peer.DetachPeer();
            }
        }

        port.CloseFromService();
    }

    internal void RemoveAgent(RuntimeAgent agent)
    {
        RuntimeMessagePortCore[] owned;
        lock (_gate)
        {
            owned = _ports.Values
                .Where(port => ReferenceEquals(port.Owner, agent))
                .ToArray();
        }

        foreach (var port in owned)
        {
            Close(port.Id);
        }
    }

    public void Dispose()
    {
        RuntimeMessagePortCore[] ports;
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            ports = _ports.Values.ToArray();
            _ports.Clear();
        }

        foreach (var port in ports)
        {
            port.CloseFromService();
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(RuntimeMessageTransportService));
        }
    }
}

internal sealed class RuntimeMessagePortCore : IDisposable
{
    private readonly object _gate = new();
    private readonly RuntimeMessageTransportService _transport;
    private readonly Queue<byte[]> _messages = new();
    private long _peerId;
    private bool _closed;

    internal RuntimeMessagePortCore(
        RuntimeMessageTransportService transport,
        long id,
        RuntimeAgent owner,
        long peerId)
    {
        _transport = transport;
        Id = id;
        Owner = owner;
        _peerId = peerId;
    }

    internal long Id { get; }

    internal RuntimeAgent Owner { get; }

    internal long PeerId => Volatile.Read(ref _peerId);

    internal bool Post(ReadOnlySpan<byte> payload)
    {
        var peerId = PeerId;
        return peerId != 0 && _transport.Post(Id, peerId, payload);
    }

    internal bool TryReceive(out byte[]? payload)
    {
        lock (_gate)
        {
            if (_messages.Count == 0)
            {
                payload = null;
                return false;
            }

            payload = _messages.Dequeue();
            return true;
        }
    }

    internal bool Enqueue(ReadOnlySpan<byte> payload)
    {
        lock (_gate)
        {
            if (_closed)
            {
                return false;
            }

            _messages.Enqueue(payload.ToArray());
            return true;
        }
    }

    internal void DetachPeer()
        => Interlocked.Exchange(ref _peerId, 0);

    internal void CloseFromService()
    {
        lock (_gate)
        {
            _closed = true;
            _messages.Clear();
            DetachPeer();
        }
    }

    public void Dispose()
        => _transport.Close(Id);
}

internal sealed class RuntimeBroadcastChannelRegistry : IDisposable
{
    private readonly object _gate = new();
    private readonly RuntimeAgentCluster _cluster;
    private readonly Dictionary<long, RuntimeBroadcastEndpointCore> _endpoints = new();
    private long _nextId;
    private bool _disposed;

    internal RuntimeBroadcastChannelRegistry(RuntimeAgentCluster cluster)
    {
        _cluster = cluster;
    }

    internal RuntimeBroadcastEndpointCore Register(RuntimeAgent owner, string name)
    {
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentNullException.ThrowIfNull(name);
        return _cluster.WhileAgentsActive(
            owner,
            second: null,
            () =>
            {
                lock (_gate)
                {
                    ThrowIfDisposed();
                    var id = ++_nextId;
                    var endpoint = new RuntimeBroadcastEndpointCore(
                        this,
                        id,
                        owner,
                        name);
                    _endpoints.Add(id, endpoint);
                    return endpoint;
                }
            });
    }

    internal int Post(long senderId, string name, ReadOnlySpan<byte> payload)
    {
        RuntimeBroadcastEndpointCore[] recipients;
        lock (_gate)
        {
            ThrowIfDisposed();
            if (!_endpoints.ContainsKey(senderId))
            {
                return 0;
            }

            recipients = _endpoints.Values
                .Where(endpoint => endpoint.Id != senderId
                    && string.Equals(endpoint.Name, name, StringComparison.Ordinal))
                .ToArray();
        }

        var delivered = 0;
        foreach (var recipient in recipients)
        {
            if (recipient.Enqueue(payload))
            {
                delivered++;
            }
        }

        return delivered;
    }

    internal void Close(long id)
    {
        RuntimeBroadcastEndpointCore? endpoint;
        lock (_gate)
        {
            if (!_endpoints.Remove(id, out endpoint))
            {
                return;
            }
        }

        endpoint.CloseFromService();
    }

    internal void RemoveAgent(RuntimeAgent agent)
    {
        RuntimeBroadcastEndpointCore[] owned;
        lock (_gate)
        {
            owned = _endpoints.Values
                .Where(endpoint => ReferenceEquals(endpoint.Owner, agent))
                .ToArray();
        }

        foreach (var endpoint in owned)
        {
            Close(endpoint.Id);
        }
    }

    public void Dispose()
    {
        RuntimeBroadcastEndpointCore[] endpoints;
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            endpoints = _endpoints.Values.ToArray();
            _endpoints.Clear();
        }

        foreach (var endpoint in endpoints)
        {
            endpoint.CloseFromService();
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(RuntimeBroadcastChannelRegistry));
        }
    }
}

internal sealed class RuntimeBroadcastEndpointCore : IDisposable
{
    private readonly object _gate = new();
    private readonly RuntimeBroadcastChannelRegistry _registry;
    private readonly Queue<byte[]> _messages = new();
    private bool _closed;

    internal RuntimeBroadcastEndpointCore(
        RuntimeBroadcastChannelRegistry registry,
        long id,
        RuntimeAgent owner,
        string name)
    {
        _registry = registry;
        Id = id;
        Owner = owner;
        Name = name;
    }

    internal long Id { get; }

    internal RuntimeAgent Owner { get; }

    internal string Name { get; }

    internal int Post(ReadOnlySpan<byte> payload)
        => _registry.Post(Id, Name, payload);

    internal bool TryReceive(out byte[]? payload)
    {
        lock (_gate)
        {
            if (_messages.Count == 0)
            {
                payload = null;
                return false;
            }

            payload = _messages.Dequeue();
            return true;
        }
    }

    internal bool Enqueue(ReadOnlySpan<byte> payload)
    {
        lock (_gate)
        {
            if (_closed)
            {
                return false;
            }

            _messages.Enqueue(payload.ToArray());
            return true;
        }
    }

    internal void CloseFromService()
    {
        lock (_gate)
        {
            _closed = true;
            _messages.Clear();
        }
    }

    public void Dispose()
        => _registry.Close(Id);
}

internal sealed class RuntimeSharedMemoryService : IDisposable
{
    private readonly object _gate = new();
    private readonly RuntimeAgentClusterSharedServices _owner;
    private readonly List<WeakReference<RuntimeSharedArrayBufferBackingStore>> _stores = [];
    private long _nextId;
    private bool _disposed;

    internal RuntimeSharedMemoryService(RuntimeAgentClusterSharedServices owner)
    {
        _owner = owner;
    }

    internal RuntimeSharedArrayBufferBackingStore Create(
        RuntimeAgent agent,
        int byteLength)
        => _owner.Cluster.WhileAgentsActive(
            agent,
            second: null,
            () => CreateCore(byteLength));

    private RuntimeSharedArrayBufferBackingStore CreateCore(int byteLength)
    {
        lock (_gate)
        {
            ThrowIfDisposed();
            var bytes = byteLength == 0
                ? System.Array.Empty<byte>()
                : new byte[byteLength];
            var store = new RuntimeSharedArrayBufferBackingStore(
                ++_nextId,
                _owner,
                bytes);
            _stores.Add(new WeakReference<RuntimeSharedArrayBufferBackingStore>(store));
            if (_stores.Count % 64 == 0)
            {
                _stores.RemoveAll(reference => !reference.TryGetTarget(out _));
            }

            return store;
        }
    }

    internal void ReleaseAll()
    {
        RuntimeSharedArrayBufferBackingStore[] stores;
        lock (_gate)
        {
            stores = _stores
                .Select(reference => reference.TryGetTarget(out var store) ? store : null)
                .Where(store => store != null)
                .Cast<RuntimeSharedArrayBufferBackingStore>()
                .ToArray();
            _stores.Clear();
        }

        foreach (var store in stores)
        {
            store.Release();
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
        }

        ReleaseAll();
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(RuntimeSharedMemoryService));
        }
    }
}

internal enum RuntimeAtomicsWaitResult
{
    NotEqual,
    Notified,
    TimedOut
}

internal sealed class RuntimeAtomicsSynchronizationDomain : IDisposable
{
    private readonly object _gate = new();
    private readonly RuntimeAgentCluster _cluster;
    private readonly Dictionary<WaitLocation, List<Waiter>> _waiters = new();
    private bool _disposed;

    internal RuntimeAtomicsSynchronizationDomain(RuntimeAgentCluster cluster)
    {
        _cluster = cluster;
    }

    internal int WaiterCount
    {
        get
        {
            lock (_gate)
            {
                return _waiters.Values.Sum(static waiters => waiters.Count);
            }
        }
    }

    internal RuntimeAtomicsWaitResult Wait(
        RuntimeAgent agent,
        RuntimeSharedArrayBufferBackingStore store,
        int byteOffset,
        int expectedValue,
        int timeoutMilliseconds)
    {
        Validate(agent, store);
        var location = new WaitLocation(store.Id, byteOffset);
        using var waiter = new Waiter(agent);

        var immediateResult = _cluster.WhileAgentsActive(
            agent,
            second: null,
            () =>
            {
                lock (_gate)
                {
                    ThrowIfDisposed();
                    if (ReadInt32(store, byteOffset) != expectedValue)
                    {
                        return RuntimeAtomicsWaitResult.NotEqual;
                    }

                    if (!_waiters.TryGetValue(location, out var locationWaiters))
                    {
                        locationWaiters = [];
                        _waiters.Add(location, locationWaiters);
                    }

                    locationWaiters.Add(waiter);
                }

                return (RuntimeAtomicsWaitResult?)null;
            });
        if (immediateResult.HasValue)
        {
            return immediateResult.Value;
        }

        RuntimeAtomicsWaitResult result;
        try
        {
            var signaled = waiter.Signal.Wait(timeoutMilliseconds, agent.ShutdownToken);
            result = signaled && !waiter.IsCancelled
                ? RuntimeAtomicsWaitResult.Notified
                : RuntimeAtomicsWaitResult.TimedOut;
        }
        catch (OperationCanceledException)
        {
            result = RuntimeAtomicsWaitResult.TimedOut;
        }
        finally
        {
            RemoveWaiter(location, waiter);
        }

        return result;
    }

    internal int Notify(
        RuntimeSharedArrayBufferBackingStore store,
        int byteOffset,
        int count)
    {
        if (count <= 0)
        {
            return 0;
        }

        lock (_gate)
        {
            ThrowIfDisposed();
            ValidateStore(store);
            var location = new WaitLocation(store.Id, byteOffset);
            if (!_waiters.TryGetValue(location, out var waiters))
            {
                return 0;
            }

            var selected = waiters.Take(count).ToArray();
            waiters.RemoveRange(0, selected.Length);
            if (waiters.Count == 0)
            {
                _waiters.Remove(location);
            }

            foreach (var waiter in selected)
            {
                waiter.Signal.Set();
            }

            return selected.Length;
        }
    }

    internal void RemoveAgent(RuntimeAgent agent)
    {
        lock (_gate)
        {
            var removed = new List<Waiter>();
            foreach (var (location, waiters) in _waiters.ToArray())
            {
                removed.AddRange(
                    waiters.Where(waiter => ReferenceEquals(waiter.Agent, agent)));
                waiters.RemoveAll(
                    waiter => ReferenceEquals(waiter.Agent, agent));
                if (waiters.Count == 0)
                {
                    _waiters.Remove(location);
                }
            }

            foreach (var waiter in removed)
            {
                waiter.Cancel();
            }
        }
    }

    internal void Reset()
    {
        lock (_gate)
        {
            var waiters = _waiters.Values.SelectMany(static values => values).ToArray();
            _waiters.Clear();
            foreach (var waiter in waiters)
            {
                waiter.Cancel();
            }
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
        }

        Reset();
    }

    private void RemoveWaiter(WaitLocation location, Waiter waiter)
    {
        lock (_gate)
        {
            if (!_waiters.TryGetValue(location, out var waiters))
            {
                return;
            }

            waiters.Remove(waiter);
            if (waiters.Count == 0)
            {
                _waiters.Remove(location);
            }
        }
    }

    private void Validate(
        RuntimeAgent agent,
        RuntimeSharedArrayBufferBackingStore store)
    {
        ArgumentNullException.ThrowIfNull(agent);
        if (!ReferenceEquals(agent.Cluster, _cluster))
        {
            throw new InvalidOperationException(
                "Atomics waiters must belong to this agent cluster.");
        }

        ValidateStore(store);
    }

    private void ValidateStore(RuntimeSharedArrayBufferBackingStore store)
    {
        ArgumentNullException.ThrowIfNull(store);
        if (!ReferenceEquals(store.Owner?.Atomics, this) || store.IsReleased)
        {
            throw new InvalidOperationException(
                "The shared backing store does not belong to this synchronization domain.");
        }
    }

    private static int ReadInt32(
        RuntimeSharedArrayBufferBackingStore store,
        int byteOffset)
    {
        var bytes = store.Bytes;
        if (byteOffset < 0 || byteOffset > bytes.Length - sizeof(int))
        {
            throw new ArgumentOutOfRangeException(nameof(byteOffset));
        }

        var span = bytes.AsSpan(byteOffset, sizeof(int));
        return BitConverter.IsLittleEndian
            ? System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(span)
            : System.Buffers.Binary.BinaryPrimitives.ReadInt32BigEndian(span);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(RuntimeAtomicsSynchronizationDomain));
        }
    }

    private readonly record struct WaitLocation(long StoreId, int ByteOffset);

    private sealed class Waiter : IDisposable
    {
        internal Waiter(RuntimeAgent agent)
        {
            Agent = agent;
        }

        internal RuntimeAgent Agent { get; }

        internal ManualResetEventSlim Signal { get; } = new(false);

        internal bool IsCancelled => Volatile.Read(ref _cancelled) != 0;

        private int _cancelled;

        internal void Cancel()
        {
            Volatile.Write(ref _cancelled, 1);
            Signal.Set();
        }

        public void Dispose()
            => Signal.Dispose();
    }
}
