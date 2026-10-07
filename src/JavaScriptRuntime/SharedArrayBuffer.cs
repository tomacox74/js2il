namespace JavaScriptRuntime
{
    [IntrinsicObject("SharedArrayBuffer")]
    public sealed class SharedArrayBuffer : ArrayBuffer
    {
        private static readonly BuiltinFunction1 _sharedArrayBufferPrototypeGrowValue = static (thisArgument, newLength) =>
        {
            if (thisArgument is not JavaScriptRuntime.SharedArrayBuffer buffer)
            {
                throw new TypeError("SharedArrayBuffer.prototype.grow called on incompatible receiver");
            }

            return buffer.grow(newLength);
        };
        private static readonly BuiltinFunction2 _sharedArrayBufferPrototypeSliceValue = static (thisArgument, start, end) =>
        {
            if (thisArgument is not JavaScriptRuntime.SharedArrayBuffer buffer)
            {
                throw new TypeError("SharedArrayBuffer.prototype.slice called on incompatible receiver");
            }

            return buffer.slice(start, end);
        };

        internal new static void ConfigureIntrinsicSurface(object constructorValue, object objectPrototype)
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            GlobalThis.ConfigureConstructorPrototypeSurface(
                constructorValue,
                JavaScriptRuntime.SharedArrayBuffer.SharedPrototype, objectPrototype);
            PropertyDescriptorStore.DefineOrUpdate(constructorValue, "length", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = 1d
            });
            PropertyDescriptorStore.DefineOrUpdate(constructorValue, "name", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "SharedArrayBuffer"
            });
            DefineSharedArrayBufferAccessor("byteLength", static buffer => buffer.byteLength);
            DefineSharedArrayBufferAccessor("maxByteLength", static buffer => buffer.maxByteLength);
            DefineSharedArrayBufferAccessor("growable", static buffer => buffer.growable);
            GlobalThis.DefineBuiltinFunctionProperty(
                JavaScriptRuntime.SharedArrayBuffer.SharedPrototype,
                "grow",
                _sharedArrayBufferPrototypeGrowValue,
                1d);
            GlobalThis.DefineBuiltinFunctionProperty(
                JavaScriptRuntime.SharedArrayBuffer.SharedPrototype,
                "slice",
                _sharedArrayBufferPrototypeSliceValue,
                2d);
            GlobalThis.DefineIntrinsicToStringTagProperty(JavaScriptRuntime.SharedArrayBuffer.SharedPrototype, "SharedArrayBuffer");
        }

        private static void DefineSharedArrayBufferAccessor(
            string propertyName,
            Func<JavaScriptRuntime.SharedArrayBuffer, object?> read)
        {
            BuiltinFunction0 getter = thisArgument =>
            {
                if (thisArgument is not JavaScriptRuntime.SharedArrayBuffer buffer)
                {
                    throw new TypeError($"get SharedArrayBuffer.prototype.{propertyName} called on incompatible receiver");
                }
                return read(buffer);
            };
            JavaScriptRuntime.Function.InitializeFunctionInstance(
                getter,
                0d,
                $"get {propertyName}",
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(getter));
            PropertyDescriptorStore.DefineOrUpdate(JavaScriptRuntime.SharedArrayBuffer.SharedPrototype, propertyName, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Enumerable = false,
                Configurable = true,
                Get = getter
            });
        }

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
            : this(CreateBackingStore(length, options))
        {
        }

        private SharedArrayBuffer(RuntimeSharedArrayBufferBackingStore backingStore)
            : base(backingStore)
        {
            _backingStore = backingStore;
            InitializeIntrinsicSurface();
        }

        internal RuntimeSharedArrayBufferBackingStore BackingStore => _backingStore;

        public new double maxByteLength => _backingStore.MaxByteLength;

        public bool growable => _backingStore.IsGrowable;

        public object? grow(object? newLength)
        {
            var byteLength = CoerceByteLength(newLength);
            if (!growable)
            {
                throw new TypeError("SharedArrayBuffer is not growable");
            }

            lock (_backingStore)
            {
                var bytes = _backingStore.Bytes;
                if (byteLength < bytes.Length || byteLength > _backingStore.MaxByteLength)
                {
                    throw new RangeError("Invalid SharedArrayBuffer length");
                }

                if (byteLength != bytes.Length)
                {
                    var grown = new byte[byteLength];
                    System.Buffer.BlockCopy(bytes, 0, grown, 0, bytes.Length);
                    _backingStore.Bytes = grown;
                }
            }

            return null;
        }

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

        public new SharedArrayBuffer slice(object? start)
            => slice(start, null);

        public new SharedArrayBuffer slice(object? start, object? end)
        {
            var bytes = RawBytes;
            var startIndex = CoerceRelativeIndex(start, 0, bytes.Length);
            var endIndex = CoerceRelativeIndex(end, bytes.Length, bytes.Length);
            if (endIndex < startIndex)
            {
                endIndex = startIndex;
            }

            var length = endIndex - startIndex;
            var constructor = ResolveSpeciesConstructor(GlobalThis.SharedArrayBufferIntrinsicConstructor);
            var created = CallableOperations.Construct1(constructor, constructor, (double)length);
            if (created is not SharedArrayBuffer result)
            {
                throw new TypeError("SharedArrayBuffer species constructor must return a SharedArrayBuffer");
            }

            if (ReferenceEquals(result, this))
            {
                throw new TypeError("SharedArrayBuffer species constructor returned the source buffer");
            }

            if (result.byteLength < length)
            {
                throw new TypeError("SharedArrayBuffer species constructor returned a buffer that is too small");
            }

            if (length > 0)
            {
                var current = RawBytes;
                var available = System.Math.Max(current.Length - startIndex, 0);
                var copyLength = System.Math.Min(length, available);
                if (copyLength > 0)
                {
                    System.Buffer.BlockCopy(current, startIndex, result.RawBytes, 0, copyLength);
                }
            }
            return result;
        }

        private static RuntimeSharedArrayBufferBackingStore CreateBackingStore(object? length, object? options = null)
        {
            var byteLength = CoerceByteLength(length);
            int? maxByteLength = TryGetMaxByteLength(options, out var requestedMax)
                ? requestedMax
                : null;
            if (maxByteLength < byteLength)
            {
                throw new RangeError("Invalid SharedArrayBuffer maxByteLength");
            }

            var context = RuntimeExecutionContext.CurrentOrOverride;
            if (context != null)
            {
                return context.Agent.Cluster.SharedServices.SharedMemory.Create(
                    context.Agent,
                    byteLength,
                    maxByteLength);
            }

            var bytes = byteLength == 0
                ? System.Array.Empty<byte>()
                : new byte[byteLength];
            return new RuntimeSharedArrayBufferBackingStore(0, null, bytes, maxByteLength);
        }

        private void InitializeIntrinsicSurface()
        {
            PrototypeChain.SetPrototype(this, SharedPrototype);
        }
    }
}
