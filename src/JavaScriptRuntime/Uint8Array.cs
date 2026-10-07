using System;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Uint8Array")]
    public sealed class Uint8Array : TypedArrayBase
    {
        private const int ElementSize = 1;
        /// <summary>Realm-owned <c>Uint8Array.prototype</c> (issue #1824). The constructor surface is
        /// wired per realm from this slot instead of from a process-wide static ctor.</summary>
        internal static JsObject Prototype
            => GetPrototype(RuntimeIntrinsics.Current);

        internal static JsObject GetPrototype(RuntimeIntrinsics intrinsics)
            => intrinsics.GetOrCreate(
                RuntimeIntrinsicSlot.Uint8ArrayPrototype,
                static () => new JsObject(),
                static prototype => InitializePrototype(prototype));

        private static void InitializePrototype(JsObject prototype)
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            PrototypeChain.SetPrototype(prototype, GlobalThis.ObjectPrototypeValue);

            PropertyDescriptorStore.DefineOrUpdate(typeof(Uint8Array), "prototype", new JsPropertyDescriptor
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
            => fromBase64(value, null);

        public static Uint8Array fromBase64(object? value, object? options)
        {
            if (value is not string text)
            {
                throw new TypeError("Uint8Array.fromBase64 requires a string input");
            }

            var decodingOptions = GetBase64DecodingOptions(options);
            var decoded = new byte[(text.Length / 4) * 3 + 3];
            var result = DecodeBase64(text, decodingOptions.Alphabet, decodingOptions.LastChunkHandling, decoded);
            global::System.Array.Resize(ref decoded, result.Written);
            return new Uint8Array(new ArrayBuffer(decoded, cloneBuffer: false), 0, decoded.Length);
        }

        public static Uint8Array fromHex(object? source)
        {
            if (source is not string text)
            {
                throw new TypeError("Uint8Array.fromHex requires a string input");
            }

            if ((text.Length & 1) != 0)
            {
                throw new SyntaxError("Invalid hexadecimal input");
            }

            if (text.Length == 0)
            {
                return new Uint8Array();
            }

            var bytes = new byte[text.Length / 2];
            for (int i = 0; i < text.Length; i += 2)
            {
                var high = GetHexDigitValue(text[i]);
                var low = GetHexDigitValue(text[i + 1]);
                if (high < 0 || low < 0)
                {
                    throw new SyntaxError("Invalid hexadecimal input");
                }

                bytes[i / 2] = (byte)((high << 4) | low);
            }

            return new Uint8Array(new ArrayBuffer(bytes, cloneBuffer: false), 0, bytes.Length);
        }

        internal static void ConfigureIntrinsicSurface(object constructorValue)
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            // Static; the receiver is ignored (issue #1895).
            DefineBuiltinFunction(
                constructorValue,
                "fromBase64",
                (BuiltinFunction2)ConstructorFromBase64,
                1);
            DefineBuiltinFunction(
                constructorValue,
                "fromHex",
                (BuiltinFunction1)ConstructorFromHex,
                1);
            DefineBuiltinFunction(
                Prototype,
                "setFromBase64",
                (BuiltinFunction2)PrototypeSetFromBase64,
                1);
            DefineBuiltinFunction(
                Prototype,
                "setFromHex",
                (BuiltinFunction1)PrototypeSetFromHex,
                1);
            DefineBuiltinFunction(
                Prototype,
                "toBase64",
                (BuiltinFunction1)PrototypeToBase64,
                0);
            DefineBuiltinFunction(
                Prototype,
                "toHex",
                (BuiltinFunction0)PrototypeToHex,
                0);
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

        public JsObject setFromBase64(object? source)
            => setFromBase64(source, null);

        public JsObject setFromBase64(object? source, object? options)
        {
            if (source is not string text)
            {
                throw new TypeError("Uint8Array.prototype.setFromBase64 requires a string input");
            }

            var decodingOptions = GetBase64DecodingOptions(options);
            var length = GetCurrentLengthForIteration();
            var decoded = DecodeBase64(text, decodingOptions.Alphabet, decodingOptions.LastChunkHandling,
                BufferObject.RawBytes.AsSpan(ByteOffsetBytes, length));

            var result = new JsObject();
            result.SetNumber("read", decoded.Read);
            result.SetNumber("written", decoded.Written);
            return result;
        }

        public JsObject setFromHex(object? source)
        {
            if (source is not string text)
            {
                throw new TypeError("Uint8Array.prototype.setFromHex requires a string input");
            }

            var length = GetCurrentLengthForIteration();
            if ((text.Length & 1) != 0)
            {
                throw new SyntaxError("Invalid hexadecimal input");
            }

            var written = 0;
            var maxBytes = global::System.Math.Min(length, text.Length / 2);
            while (written < maxBytes)
            {
                var sourceIndex = written * 2;
                var high = GetHexDigitValue(text[sourceIndex]);
                var low = GetHexDigitValue(text[sourceIndex + 1]);
                if (high < 0 || low < 0)
                {
                    throw new SyntaxError("Invalid hexadecimal input");
                }

                BufferObject.RawBytes[ByteOffsetBytes + written] = (byte)((high << 4) | low);
                written++;
            }

            var result = new JsObject();
            result.SetNumber("read", written * 2);
            result.SetNumber("written", written);
            return result;
        }

        public string toBase64()
            => ToBase64Core(null);

        public string toBase64(object? options)
            => ToBase64Core(options);

        public string toHex()
        {
            var length = GetCurrentLengthForIteration();
            var hex = new char[checked(length * 2)];
            const string digits = "0123456789abcdef";

            for (var i = 0; i < length; i++)
            {
                var value = BufferObject.RawBytes[ByteOffsetBytes + i];
                hex[i * 2] = digits[value >> 4];
                hex[(i * 2) + 1] = digits[value & 0x0F];
            }

            return new string(hex);
        }

        protected override double ReadElementValue(int index)
            => BufferObject.RawBytes[ByteOffsetBytes + index];

        protected override void WriteElementValue(int index, double value)
            => BufferObject.RawBytes[ByteOffsetBytes + index] = TypeUtilities.ToUint8(value);

        protected override TypedArrayBase CreateSameType(ArrayBuffer buffer, int byteOffset, int length)
            => new Uint8Array(buffer, byteOffset, length);

        private static object? ConstructorFromBase64(object? thisArgument, object? source, object? options)
            => fromBase64(source, options);

        private static object? ConstructorFromHex(object? thisArgument, object? source)
            => fromHex(source);

        private static object? PrototypeSetFromBase64(object? thisArgument, object? source, object? options)
        {
            if (thisArgument is not Uint8Array array)
            {
                throw new TypeError("Uint8Array.prototype.setFromBase64 called on incompatible receiver");
            }

            return array.setFromBase64(source, options);
        }

        private static object? PrototypeSetFromHex(object? thisArgument, object? source)
        {
            if (thisArgument is not Uint8Array array)
            {
                throw new TypeError("Uint8Array.prototype.setFromHex called on incompatible receiver");
            }

            return array.setFromHex(source);
        }

        private static object? PrototypeToBase64(object? thisArgument, object? options)
        {
            if (thisArgument is not Uint8Array array)
            {
                throw new TypeError("Uint8Array.prototype.toBase64 called on incompatible receiver");
            }

            return array.ToBase64Core(options);
        }

        private static object? PrototypeToHex(object? thisArgument)
        {
            if (thisArgument is not Uint8Array array)
            {
                throw new TypeError("Uint8Array.prototype.toHex called on incompatible receiver");
            }

            return array.toHex();
        }

        private string ToBase64Core(object? options)
        {
            var alphabet = "base64";
            var omitPadding = false;

            if (options is not null)
            {
                if (options is JsNull || TypeUtilities.IsPrimitive(options))
                {
                    throw new TypeError("Uint8Array.prototype.toBase64 options must be an object");
                }

                var alphabetValue = ObjectRuntime.GetProperty(options, "alphabet");
                if (alphabetValue is not null)
                {
                    if (alphabetValue is not string requestedAlphabet
                        || requestedAlphabet is not ("base64" or "base64url"))
                    {
                        throw new TypeError("Uint8Array.prototype.toBase64 alphabet must be 'base64' or 'base64url'");
                    }

                    alphabet = requestedAlphabet;
                }

                var omitPaddingValue = ObjectRuntime.GetProperty(options, "omitPadding");
                if (omitPaddingValue is not null)
                {
                    omitPadding = Operators.IsTruthy(omitPaddingValue);
                }
            }

            _ = GetCurrentLengthForIteration();
            var encoded = System.Convert.ToBase64String(CopyRawBytes());
            if (alphabet == "base64url")
            {
                encoded = encoded.Replace('+', '-').Replace('/', '_');
            }

            return omitPadding ? encoded.TrimEnd('=') : encoded;
        }

        private static (string Alphabet, string LastChunkHandling) GetBase64DecodingOptions(object? options)
        {
            if (options is null)
            {
                return ("base64", "loose");
            }

            if (options is JsNull || TypeUtilities.IsPrimitive(options))
            {
                throw new TypeError("Base64 decoding options must be an object");
            }

            var alphabetValue = ObjectRuntime.GetProperty(options, "alphabet");
            var alphabet = alphabetValue is null ? "base64" : alphabetValue as string;
            if (alphabet is not ("base64" or "base64url"))
            {
                throw new TypeError("Base64 alphabet must be 'base64' or 'base64url'");
            }

            var handlingValue = ObjectRuntime.GetProperty(options, "lastChunkHandling");
            var handling = handlingValue is null ? "loose" : handlingValue as string;
            if (handling is not ("loose" or "strict" or "stop-before-partial"))
            {
                throw new TypeError("Invalid base64 lastChunkHandling");
            }

            return (alphabet, handling);
        }

        private static (int Read, int Written) DecodeBase64(
            string text, string alphabet, string lastChunkHandling, Span<byte> destination)
        {
            if (destination.Length == 0)
            {
                return (0, 0);
            }

            var read = 0;
            var written = 0;
            var index = 0;
            var chunkLength = 0;
            var bits = 0;
            while (true)
            {
                index = SkipAsciiWhitespace(text, index);
                if (index == text.Length)
                {
                    if (chunkLength != 0)
                    {
                        if (lastChunkHandling == "stop-before-partial")
                        {
                            return (read, written);
                        }

                        if (lastChunkHandling == "strict" || chunkLength == 1)
                        {
                            throw new SyntaxError("Invalid base64 input");
                        }

                        WriteBase64Chunk(destination, ref written, bits, chunkLength, false);
                    }

                    return (text.Length, written);
                }

                var character = text[index++];
                if (character == '=')
                {
                    if (chunkLength < 2)
                    {
                        throw new SyntaxError("Invalid base64 padding");
                    }

                    index = SkipAsciiWhitespace(text, index);
                    if (chunkLength == 2)
                    {
                        if (index == text.Length && lastChunkHandling == "stop-before-partial")
                        {
                            return (read, written);
                        }

                        if (index == text.Length || text[index] != '=')
                        {
                            throw new SyntaxError("Invalid base64 padding");
                        }

                        index = SkipAsciiWhitespace(text, index + 1);
                    }

                    if (index != text.Length)
                    {
                        throw new SyntaxError("Invalid base64 padding");
                    }

                    WriteBase64Chunk(destination, ref written, bits, chunkLength, lastChunkHandling == "strict");
                    return (text.Length, written);
                }

                var digit = character switch
                {
                    >= 'A' and <= 'Z' => character - 'A',
                    >= 'a' and <= 'z' => character - 'a' + 26,
                    >= '0' and <= '9' => character - '0' + 52,
                    '+' when alphabet == "base64" => 62,
                    '/' when alphabet == "base64" => 63,
                    '-' when alphabet == "base64url" => 62,
                    '_' when alphabet == "base64url" => 63,
                    _ => -1
                };
                if (digit < 0)
                {
                    throw new SyntaxError("Invalid base64 character");
                }

                var remaining = destination.Length - written;
                if ((remaining == 1 && chunkLength == 2) || (remaining == 2 && chunkLength == 3))
                {
                    return (read, written);
                }

                bits = (bits << 6) | digit;
                if (++chunkLength == 4)
                {
                    WriteBase64Chunk(destination, ref written, bits, chunkLength, false);
                    chunkLength = 0;
                    bits = 0;
                    read = index;
                    if (written == destination.Length)
                    {
                        return (read, written);
                    }
                }
            }
        }

        private static void WriteBase64Chunk(Span<byte> destination, ref int written, int bits, int length, bool strict)
        {
            var unusedBits = (4 - length) * 2;
            if (strict && (bits & ((1 << unusedBits) - 1)) != 0)
            {
                throw new SyntaxError("Invalid base64 overflow bits");
            }

            bits <<= (4 - length) * 6;
            destination[written++] = (byte)(bits >> 16);
            if (length >= 3)
            {
                destination[written++] = (byte)(bits >> 8);
            }
            if (length == 4)
            {
                destination[written++] = (byte)bits;
            }
        }

        private static int SkipAsciiWhitespace(string text, int index)
        {
            while (index < text.Length && text[index] is '\t' or '\n' or '\f' or '\r' or ' ')
            {
                index++;
            }

            return index;
        }

        private static void DefineBuiltinFunction(
            object target,
            string name,
            Delegate function,
            double length)
        {
            Function.InitializeFunctionInstance(
                function,
                length,
                name,
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(function));
            PropertyDescriptorStore.DefineOrUpdate(function, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = null
            });
            PropertyDescriptorStore.DefineOrUpdate(target, name, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = function
            });
        }

        private void InitializeIntrinsicSurface()
            => PrototypeChain.InitializePrototype(this, Prototype);

        private static int GetHexDigitValue(char value)
            => value switch
            {
                >= '0' and <= '9' => value - '0',
                >= 'a' and <= 'f' => value - 'a' + 10,
                >= 'A' and <= 'F' => value - 'A' + 10,
                _ => -1
            };
    }
}
