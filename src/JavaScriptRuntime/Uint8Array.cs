using System;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Uint8Array")]
    public sealed class Uint8Array : TypedArrayBase
    {
        private const int ElementSize = 1;
        internal static readonly JsObject Prototype = CreatePrototype();

        static Uint8Array()
        {
            PrototypeChain.SetPrototype(Prototype, GlobalThis.ObjectPrototypeValue);

            PropertyDescriptorStore.DefineOrUpdate(typeof(Uint8Array), "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = Prototype
            });

            PropertyDescriptorStore.DefineOrUpdate(Prototype, "constructor", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = typeof(Uint8Array)
            });
        }

        public Uint8Array()
        {
            InitializeEmpty();
            InitializeIntrinsicSurface();
        }

        public Uint8Array(object? arg)
        {
            InitializeFromArgument(arg);
            InitializeIntrinsicSurface();
        }

        public Uint8Array(object? arg, object? byteOffset)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, null);
                InitializeIntrinsicSurface();
                return;
            }

            InitializeFromArgument(arg);
            InitializeIntrinsicSurface();
        }

        public Uint8Array(object? arg, object? byteOffset, object? length)
        {
            if (arg is ArrayBuffer arrayBuffer)
            {
                InitializeFromBuffer(arrayBuffer, byteOffset, length);
                InitializeIntrinsicSurface();
                return;
            }

            InitializeFromArgument(arg);
            InitializeIntrinsicSurface();
        }

        private Uint8Array(ArrayBuffer buffer, int byteOffset, int length)
        {
            InitializeFromExisting(buffer, byteOffset, length);
            InitializeIntrinsicSurface();
        }

        public static Uint8Array from(object? source)
            => FromSource(nameof(Uint8Array), source, null, null, static values => new Uint8Array(values));

        public static Uint8Array from(object? source, object? mapper)
            => FromSource(nameof(Uint8Array), source, mapper, null, static values => new Uint8Array(values));

        public static Uint8Array from(object? source, object? mapper, object? thisArg)
            => FromSource(nameof(Uint8Array), source, mapper, thisArg, static values => new Uint8Array(values));

        public static Uint8Array fromBase64(object? value)
        {
            try
            {
                var decoded = System.Convert.FromBase64String(DotNet2JSConversions.ToString(value));
                return new Uint8Array(new ArrayBuffer(decoded, cloneBuffer: false), 0, decoded.Length);
            }
            catch (FormatException ex)
            {
                throw new SyntaxError("Invalid base64 input", ex);
            }
        }

        public static Uint8Array of(object[]? args)
            => new Uint8Array(args ?? global::System.Array.Empty<object?>());

        protected override int BytesPerElement => ElementSize;

        protected override string TypedArrayName => nameof(Uint8Array);

        public Uint8Array slice()
            => (Uint8Array)SliceCore(null, null);

        public Uint8Array slice(object? start)
            => (Uint8Array)SliceCore(start, null);

        public Uint8Array slice(object? start, object? end)
            => (Uint8Array)SliceCore(start, end);

        public Uint8Array subarray()
            => (Uint8Array)SubarrayCore(null, null);

        public Uint8Array subarray(object? start)
            => (Uint8Array)SubarrayCore(start, null);

        public Uint8Array subarray(object? start, object? end)
            => (Uint8Array)SubarrayCore(start, end);

        protected override double ReadElementValue(int index)
            => BufferObject.RawBytes[ByteOffsetBytes + index];

        protected override void WriteElementValue(int index, double value)
            => BufferObject.RawBytes[ByteOffsetBytes + index] = ToUint8(value);

        protected override TypedArrayBase CreateSameType(ArrayBuffer buffer, int byteOffset, int length)
            => new Uint8Array(buffer, byteOffset, length);

        private static JsObject CreatePrototype()
            => new();

        private void InitializeIntrinsicSurface()
            => PrototypeChain.SetPrototype(this, Prototype);

        private static byte ToUint8(double value)
            => unchecked((byte)TypeUtilities.ToInt32(value));
    }
}
