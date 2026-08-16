using System;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Uint8ClampedArray")]
    public sealed class Uint8ClampedArray : TypedArrayBase
    {
        private const int ElementSize = 1;
        /// <summary>Realm-owned <c>Uint8ClampedArray.prototype</c> (issue #1824). The constructor surface is
        /// wired per realm from this slot instead of from a process-wide static ctor.</summary>
        internal static JsObject Prototype
            => RuntimeIntrinsics.Current.GetOrCreate(
                RuntimeIntrinsicSlot.Uint8ClampedArrayPrototype,
                static () => new JsObject(),
                static prototype => InitializePrototype(prototype));

        private static void InitializePrototype(JsObject prototype)
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            PrototypeChain.SetPrototype(prototype, GlobalThis.ObjectPrototypeValue);

            PropertyDescriptorStore.DefineOrUpdate(typeof(Uint8ClampedArray), "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = prototype
            });

            PropertyDescriptorStore.DefineOrUpdate(prototype, "constructor", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = typeof(Uint8ClampedArray)
            });
        }

        public Uint8ClampedArray()
        {
            InitializeEmpty();
            InitializeIntrinsicSurface();
        }

        public Uint8ClampedArray(object? arg)
        {
            InitializeFromArgument(arg);
            InitializeIntrinsicSurface();
        }

        public Uint8ClampedArray(object? arg, object? byteOffset)
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

        public Uint8ClampedArray(object? arg, object? byteOffset, object? length)
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

        private Uint8ClampedArray(ArrayBuffer buffer, int byteOffset, int length)
        {
            InitializeFromExisting(buffer, byteOffset, length);
            InitializeIntrinsicSurface();
        }

        public static Uint8ClampedArray from(object? source)
            => FromSource(nameof(Uint8ClampedArray), source, null, null, static values => new Uint8ClampedArray(values));

        public static Uint8ClampedArray from(object? source, object? mapper)
            => FromSource(nameof(Uint8ClampedArray), source, mapper, null, static values => new Uint8ClampedArray(values));

        public static Uint8ClampedArray from(object? source, object? mapper, object? thisArg)
            => FromSource(nameof(Uint8ClampedArray), source, mapper, thisArg, static values => new Uint8ClampedArray(values));

        public static Uint8ClampedArray of(object[]? args)
            => new Uint8ClampedArray(args ?? global::System.Array.Empty<object?>());

        protected override int BytesPerElement => ElementSize;

        protected override string TypedArrayName => nameof(Uint8ClampedArray);

        public Uint8ClampedArray slice()
            => (Uint8ClampedArray)SliceCore(null, null);

        public Uint8ClampedArray slice(object? start)
            => (Uint8ClampedArray)SliceCore(start, null);

        public Uint8ClampedArray slice(object? start, object? end)
            => (Uint8ClampedArray)SliceCore(start, end);

        public Uint8ClampedArray subarray()
            => (Uint8ClampedArray)SubarrayCore(null, null);

        public Uint8ClampedArray subarray(object? start)
            => (Uint8ClampedArray)SubarrayCore(start, null);

        public Uint8ClampedArray subarray(object? start, object? end)
            => (Uint8ClampedArray)SubarrayCore(start, end);

        protected override double ReadElementValue(int index)
            => BufferObject.RawBytes[ByteOffsetBytes + index];

        protected override void WriteElementValue(int index, double value)
            => BufferObject.RawBytes[ByteOffsetBytes + index] = TypeUtilities.ToUint8Clamp(value);

        protected override TypedArrayBase CreateSameType(ArrayBuffer buffer, int byteOffset, int length)
            => new Uint8ClampedArray(buffer, byteOffset, length);

        private void InitializeIntrinsicSurface()
            => PrototypeChain.InitializePrototype(this, Prototype);
    }
}
