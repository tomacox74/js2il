namespace JavaScriptRuntime
{
    [IntrinsicObject("SharedArrayBuffer")]
    public sealed class SharedArrayBuffer : ArrayBuffer
    {
        internal static object SharedPrototype
            => RuntimeIntrinsics.Current.SharedArrayBufferPrototype;

        public SharedArrayBuffer()
        {
            InitializeIntrinsicSurface();
        }

        public SharedArrayBuffer(object? length)
            : base(length)
        {
            InitializeIntrinsicSurface();
        }

        public SharedArrayBuffer(object? length, object? options)
            : base(length, options, supportsResizing: false)
        {
            InitializeIntrinsicSurface();
        }

        private void InitializeIntrinsicSurface()
        {
            PrototypeChain.SetPrototype(this, SharedPrototype);
        }
    }
}
