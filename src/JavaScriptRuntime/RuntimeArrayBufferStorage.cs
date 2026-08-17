namespace JavaScriptRuntime;

internal class RuntimeArrayBufferStorage
{
    private byte[] _bytes;

    internal RuntimeArrayBufferStorage(byte[] bytes)
    {
        _bytes = bytes;
    }

    /// <summary>
    /// Reads use a plain field read because this sits on the typed-array element access
    /// hot path. Resizable ArrayBuffer storage is agent-confined, while shared storage
    /// keeps the same byte array until agent-cluster teardown. The release write remains
    /// volatile for teardown publication; ordinary readers do not rely on it for
    /// synchronization.
    /// </summary>
    internal byte[] Bytes
    {
        get => _bytes;
        set => Volatile.Write(ref _bytes, value);
    }
}

internal sealed class RuntimeSharedArrayBufferBackingStore : RuntimeArrayBufferStorage
{
    private int _released;

    internal RuntimeSharedArrayBufferBackingStore(
        long id,
        RuntimeAgentClusterSharedServices? owner,
        byte[] bytes)
        : base(bytes)
    {
        Id = id;
        Owner = owner;
    }

    internal long Id { get; }

    internal RuntimeAgentClusterSharedServices? Owner { get; }

    internal bool IsReleased => Volatile.Read(ref _released) != 0;

    internal void Release()
    {
        if (Interlocked.Exchange(ref _released, 1) == 0)
        {
            Bytes = System.Array.Empty<byte>();
        }
    }
}
