namespace JavaScriptRuntime
{
    [IntrinsicObject("SharedArrayBuffer")]
    public sealed class SharedArrayBuffer : ArrayBuffer
    {
        private readonly RuntimeSharedArrayBufferBackingStore _backingStore;

        internal static object SharedPrototype
            => RuntimeIntrinsics.Current.SharedArrayBufferPrototype;

        public SharedArrayBuffer()
            : this(CreateBackingStore(null))
        {
        }

        public SharedArrayBuffer(object? length)
            : this(CreateBackingStore(length))
        {
        }

        public SharedArrayBuffer(object? length, object? options)
            : this(CreateBackingStore(length))
        {
        }

        private SharedArrayBuffer(RuntimeSharedArrayBufferBackingStore backingStore)
            : base(backingStore)
        {
            _backingStore = backingStore;
            InitializeIntrinsicSurface();
        }

        internal RuntimeSharedArrayBufferBackingStore BackingStore => _backingStore;

        internal SharedArrayBuffer CreateWrapperForCurrentRealm()
        {
            var currentServices = RuntimeExecutionContext.CurrentOrOverride?
                .Agent.Cluster.SharedServices;
            if (_backingStore.Owner != null
                && !ReferenceEquals(_backingStore.Owner, currentServices))
            {
                throw new InvalidOperationException(
                    "SharedArrayBuffer backing stores cannot cross agent clusters.");
            }

            return new SharedArrayBuffer(_backingStore);
        }

        private static RuntimeSharedArrayBufferBackingStore CreateBackingStore(object? length)
        {
            var byteLength = CoerceByteLength(length);
            var context = RuntimeExecutionContext.CurrentOrOverride;
            if (context != null)
            {
                return context.Agent.Cluster.SharedServices.SharedMemory.Create(
                    context.Agent,
                    byteLength);
            }

            var bytes = byteLength == 0
                ? System.Array.Empty<byte>()
                : new byte[byteLength];
            return new RuntimeSharedArrayBufferBackingStore(0, null, bytes);
        }

        private void InitializeIntrinsicSurface()
        {
            PrototypeChain.SetPrototype(this, SharedPrototype);
        }
    }
}
