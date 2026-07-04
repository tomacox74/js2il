namespace JavaScriptRuntime
{
    [IntrinsicObject("SharedArrayBuffer")]
    public sealed class SharedArrayBuffer : ArrayBuffer
    {
        public SharedArrayBuffer()
        {
        }

        public SharedArrayBuffer(object? length)
            : base(length)
        {
        }

        public SharedArrayBuffer(object? length, object? options)
            : base(length, options)
        {
        }
    }
}
