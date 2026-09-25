namespace JavaScriptRuntime;

internal class RuntimeArrayBufferStorage
{
    private byte[] _bytes;

    internal RuntimeArrayBufferStorage(byte[] bytes)
    {
        _bytes = bytes;
    }

    /// <summary>
    /// Growable shared storage publishes replacement arrays to other agents.
    /// </summary>
    internal byte[] Bytes
    {
        get => Volatile.Read(ref _bytes);
        set => Volatile.Write(ref _bytes, value);
    }
}

internal sealed class RuntimeSharedArrayBufferBackingStore : RuntimeArrayBufferStorage
{
    private int _released;

    internal RuntimeSharedArrayBufferBackingStore(
        long id,
        RuntimeAgentClusterSharedServices? owner,
        byte[] bytes,
        int? maxByteLength = null)
        : base(bytes)
    {
        Id = id;
        Owner = owner;
        MaxByteLength = maxByteLength ?? bytes.Length;
        IsGrowable = maxByteLength.HasValue;
    }

    internal long Id { get; }

    internal RuntimeAgentClusterSharedServices? Owner { get; }

    internal int MaxByteLength { get; }

    internal bool IsGrowable { get; }

    internal bool IsReleased => Volatile.Read(ref _released) != 0;

    internal void Release()
    {
        if (Interlocked.Exchange(ref _released, 1) == 0)
        {
            Bytes = System.Array.Empty<byte>();
        }
    }
}
