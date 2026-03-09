using System;
using System.Security.Cryptography;

namespace JavaScriptRuntime.Node
{
    [NodeModule("crypto")]
    public sealed class Crypto
    {
        private readonly WebCryptoBridge _webcrypto = new();

        public object webcrypto => _webcrypto;

        public Hash createHash(object? algorithm)
        {
            if (algorithm == null || algorithm is JsNull)
            {
                throw new TypeError("The \"algorithm\" argument must be of type string");
            }

            return new Hash(ResolveHashAlgorithm(algorithm));
        }

        public Buffer randomBytes(object? size)
        {
            var length = CoerceSize(size, "size");
            if (length == 0)
            {
                return Buffer.FromBytes(System.Array.Empty<byte>());
            }

            var bytes = GC.AllocateUninitializedArray<byte>(length);
            RandomNumberGenerator.Fill(bytes);
            return Buffer.FromBytes(bytes);
        }

        public object getRandomValues(object? target)
            => _webcrypto.getRandomValues(target);

        internal static byte[] CoerceBytes(object? value, object? encoding)
        {
            switch (value)
            {
                case null:
                case JsNull:
                    return System.Array.Empty<byte>();
                case Buffer buffer:
                    return buffer.ToByteArray();
                case byte[] bytes:
                    return (byte[])bytes.Clone();
                case ArrayBuffer arrayBuffer:
                    return (byte[])arrayBuffer.RawBytes.Clone();
                case TypedArrayBase typedArray:
                    return CopyTypedArrayBytes(typedArray);
                case string text:
                    return Buffer.from(text, encoding).ToByteArray();
                default:
                    return Buffer.from(DotNet2JSConversions.ToString(value), encoding).ToByteArray();
            }
        }

        internal static void FillRandomValues(object? target)
        {
            switch (target)
            {
                case Buffer buffer:
                    FillBuffer(buffer);
                    return;
                case Uint8Array uint8Array:
                    FillTypedArray(uint8Array);
                    return;
                case Int32Array int32Array:
                    FillTypedArray(int32Array);
                    return;
                default:
                    throw new TypeError("The \"typedArray\" argument must be an instance of Buffer, Uint8Array, or Int32Array");
            }
        }

        private static HashAlgorithmName ResolveHashAlgorithm(object? algorithm)
        {
            var normalized = DotNet2JSConversions.ToString(algorithm)
                .Trim()
                .Replace("-", string.Empty, StringComparison.Ordinal)
                .ToLowerInvariant();

            return normalized switch
            {
                "md5" => HashAlgorithmName.MD5,
                "sha1" => HashAlgorithmName.SHA1,
                "sha256" => HashAlgorithmName.SHA256,
                "sha384" => HashAlgorithmName.SHA384,
                "sha512" => HashAlgorithmName.SHA512,
                _ => throw new Error("Digest method not supported"),
            };
        }

        private static int CoerceSize(object? size, string argumentName)
        {
            if (size == null || size is JsNull)
            {
                throw new TypeError($"The \"{argumentName}\" argument must be of type number");
            }

            var number = TypeUtilities.ToNumber(size);
            if (double.IsNaN(number)
                || double.IsInfinity(number)
                || number < 0
                || global::System.Math.Truncate(number) != number
                || number > int.MaxValue)
            {
                throw new RangeError($"The value of \"{argumentName}\" is out of range. It must be an integer between 0 and {int.MaxValue}.");
            }

            return (int)number;
        }

        private static byte[] CopyTypedArrayBytes(TypedArrayBase typedArray)
        {
            var byteOffset = (int)typedArray.byteOffset;
            var byteLength = (int)typedArray.byteLength;
            if (byteLength == 0)
            {
                return System.Array.Empty<byte>();
            }

            var copy = new byte[byteLength];
            global::System.Buffer.BlockCopy(typedArray.buffer.RawBytes, byteOffset, copy, 0, byteLength);
            return copy;
        }

        private static void FillBuffer(Buffer buffer)
        {
            var bytes = buffer.length;
            for (int i = 0; i < bytes; i++)
            {
                buffer[(double)i] = RandomNumberGenerator.GetInt32(0, 256);
            }
        }

        private static void FillTypedArray(TypedArrayBase typedArray)
        {
            var byteOffset = (int)typedArray.byteOffset;
            var byteLength = (int)typedArray.byteLength;
            if (byteLength == 0)
            {
                return;
            }

            RandomNumberGenerator.Fill(typedArray.buffer.RawBytes.AsSpan(byteOffset, byteLength));
        }
    }

    public sealed class Hash
    {
        private IncrementalHash? _incrementalHash;

        internal Hash(HashAlgorithmName algorithmName)
        {
            _incrementalHash = IncrementalHash.CreateHash(algorithmName);
        }

        public Hash update(object? data)
            => update(data, null);

        public Hash update(object? data, object? inputEncoding)
        {
            EnsureNotDigested();
            _incrementalHash!.AppendData(Crypto.CoerceBytes(data, inputEncoding));
            return this;
        }

        public object digest()
            => digest(null);

        public object digest(object? outputEncoding)
        {
            EnsureNotDigested();

            var hash = _incrementalHash!;
            var bytes = hash.GetHashAndReset();
            hash.Dispose();
            _incrementalHash = null;

            if (outputEncoding == null || outputEncoding is JsNull)
            {
                return Buffer.FromBytes(bytes);
            }

            return Buffer.FromBytes(bytes).toString(outputEncoding);
        }

        private void EnsureNotDigested()
        {
            if (_incrementalHash == null)
            {
                throw new Error("Digest already called");
            }
        }
    }

    public sealed class WebCryptoBridge
    {
        public object getRandomValues(object? target)
        {
            Crypto.FillRandomValues(target);
            return target!;
        }
    }
}
