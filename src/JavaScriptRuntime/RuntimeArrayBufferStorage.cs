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
    /// hot path. Writes publish with <see cref="Volatile.Write"/> so a replaced buffer is
    /// visible to other agents; JavaScript requires <c>Atomics</c> for synchronized
    /// access to shared memory, so non-atomic reads need no acquire barrier here.
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
