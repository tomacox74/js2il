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
            if (algorithm is not string text)
            {
                throw new TypeError("The \"algorithm\" argument must be of type string");
            }

            return new Hash(ResolveHashAlgorithm(text));
        }

        public Hmac createHmac(object? algorithm, object? key)
        {
            if (algorithm is not string text)
            {
                throw new TypeError("The \"algorithm\" argument must be of type string");
            }

            return new Hmac(ResolveHashAlgorithm(text), CoerceHmacKeyBytes(key));
        }

        public Buffer randomBytes(object? size)
        {
            var length = CoerceSize(size, "size");
            var buffer = Buffer.allocUnsafe(length);
            FillBuffer(buffer);
            return buffer;
        }

        public object getRandomValues(object? target)
            => _webcrypto.getRandomValues(target);

        public Buffer pbkdf2Sync(object? password, object? salt, object? iterations, object? keylen, object? digest)
        {
            if (digest is not string digestText)
            {
                throw new TypeError("The \"digest\" argument must be of type string");
            }

            var passwordBytes = CoercePbkdf2InputBytes(password, "password");
            var saltBytes = CoercePbkdf2InputBytes(salt, "salt");
            var iterationCount = CoercePbkdf2Iterations(iterations);
            var outputLength = CoercePbkdf2KeyLength(keylen);
            var algorithm = ResolvePbkdf2DigestAlgorithm(digestText);
            var derived = new byte[outputLength];
            Rfc2898DeriveBytes.Pbkdf2(passwordBytes, saltBytes, derived, iterationCount, algorithm);
            return Buffer.FromBytes(derived);
        }

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
                case DataView dataView:
                    return CopyDataViewBytes(dataView);
                case string text:
                    return Buffer.from(text, encoding).ToByteArray();
                default:
                    return Buffer.from(DotNet2JSConversions.ToString(value), encoding).ToByteArray();
            }
        }

        internal static byte[] CoerceHmacKeyBytes(object? value)
        {
            switch (value)
            {
                case Buffer buffer:
                    return buffer.ToByteArray();
                case byte[] bytes:
                    return (byte[])bytes.Clone();
                case ArrayBuffer arrayBuffer:
                    return (byte[])arrayBuffer.RawBytes.Clone();
                case TypedArrayBase typedArray:
                    return CopyTypedArrayBytes(typedArray);
                case DataView dataView:
                    return CopyDataViewBytes(dataView);
                case string text:
                    return Buffer.from(text).ToByteArray();
                default:
                    throw CreateInvalidArgumentTypeError(
                        "key",
                        "of type string or an instance of ArrayBuffer, Buffer, TypedArray, or DataView",
                        value);
            }
        }

        internal static byte[] CoerceHmacDataBytes(object? value, object? encoding)
        {
            switch (value)
            {
                case Buffer buffer:
                    return buffer.ToByteArray();
                case byte[] bytes:
                    return (byte[])bytes.Clone();
                case TypedArrayBase typedArray:
                    return CopyTypedArrayBytes(typedArray);
                case DataView dataView:
                    return CopyDataViewBytes(dataView);
                case string text:
                    return Buffer.from(text, encoding).ToByteArray();
                default:
                    throw CreateInvalidArgumentTypeError(
                        "data",
                        "of type string or an instance of Buffer, TypedArray, or DataView",
                        value);
            }
        }

        internal static byte[] CoerceBufferSourceBytes(object? value, string argumentName)
        {
            switch (value)
            {
                case Buffer buffer:
                    return buffer.ToByteArray();
                case ArrayBuffer arrayBuffer:
                    return (byte[])arrayBuffer.RawBytes.Clone();
                case TypedArrayBase typedArray:
                    return CopyTypedArrayBytes(typedArray);
                case DataView dataView:
                    return CopyDataViewBytes(dataView);
                default:
                    throw new TypeError($"The \"{argumentName}\" argument must be an instance of ArrayBuffer, Buffer, TypedArray, or DataView");
            }
        }

        internal static byte[] CoercePbkdf2InputBytes(object? value, string argumentName)
        {
            switch (value)
            {
                case Buffer buffer:
                    return buffer.ToByteArray();
                case byte[] bytes:
                    return (byte[])bytes.Clone();
                case ArrayBuffer arrayBuffer:
                    return (byte[])arrayBuffer.RawBytes.Clone();
                case TypedArrayBase typedArray:
                    return CopyTypedArrayBytes(typedArray);
                case DataView dataView:
                    return CopyDataViewBytes(dataView);
                case string text:
                    return Buffer.from(text).ToByteArray();
                default:
                    throw CreateInvalidArgumentTypeError(
                        argumentName,
                        "of type string or an instance of ArrayBuffer, Buffer, TypedArray, or DataView",
                        value);
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

        internal static HashAlgorithmName ResolveWebCryptoDigestAlgorithm(object? algorithm)
            => ResolveHashAlgorithm(
                GetAlgorithmName(algorithm, "algorithm"),
                allowMd5: false,
                static () => new NotSupportedError("Unrecognized algorithm name"));

        internal static HmacImportParameters ResolveHmacImportParameters(object? algorithm)
        {
            if (algorithm is null || algorithm is JsNull)
            {
                throw new TypeError("The \"algorithm\" argument must be a string or object with a string \"name\" property");
            }

            var algorithmName = GetAlgorithmName(algorithm, "algorithm");
            if (!string.Equals(NormalizeAlgorithmName(algorithmName), "hmac", StringComparison.Ordinal))
            {
                throw new NotSupportedError("Unrecognized algorithm name");
            }

            var hashValue = JavaScriptRuntime.ObjectRuntime.GetProperty(algorithm, "hash");
            if (hashValue is null || hashValue is JsNull)
            {
                throw new TypeError("The HMAC algorithm requires a hash property");
            }

            var hashName = GetAlgorithmName(hashValue, "algorithm.hash");
            int? requestedLengthBits = null;
            if (algorithm is not string)
            {
                var lengthValue = JavaScriptRuntime.ObjectRuntime.GetProperty(algorithm, "length");
                if (lengthValue != null)
                {
                    requestedLengthBits = ParseRequestedHmacKeyLength(lengthValue);
                }
            }

            return new HmacImportParameters(
                ResolveHashAlgorithm(hashName, allowMd5: false, static () => new NotSupportedError("Unrecognized algorithm name")),
                CanonicalizeWebCryptoHashName(hashName),
                requestedLengthBits);
        }

        internal static void EnsureHmacAlgorithm(object? algorithm)
        {
            var algorithmName = GetAlgorithmName(algorithm, "algorithm");
            if (!string.Equals(NormalizeAlgorithmName(algorithmName), "hmac", StringComparison.Ordinal))
            {
                throw new NotSupportedError("Unrecognized algorithm name");
            }
        }

        internal static void ValidateImportedHmacKeyLength(byte[] keyBytes, int? requestedLengthBits)
        {
            var actualLengthBits = checked(keyBytes.Length * 8);
            if (actualLengthBits == 0)
            {
                throw new DataError("Zero-length key is not supported");
            }

            if (requestedLengthBits.HasValue && requestedLengthBits.Value != actualLengthBits)
            {
                throw new DataError("Invalid key length");
            }
        }

        private static HashAlgorithmName ResolvePbkdf2DigestAlgorithm(string digest)
            => ResolveHashAlgorithm(
                digest,
                allowMd5: false,
                () => new TypeError($"Invalid digest: {digest}"));

        private static HashAlgorithmName ResolveHashAlgorithm(string algorithm)
            => ResolveHashAlgorithm(algorithm, allowMd5: true, static () => new Error("Digest method not supported"));

        private static HashAlgorithmName ResolveHashAlgorithm(string algorithm, bool allowMd5, Func<Error> unsupportedErrorFactory)
        {
            var normalized = NormalizeAlgorithmName(algorithm);

            return normalized switch
            {
                "md5" when allowMd5 => HashAlgorithmName.MD5,
                "sha1" => HashAlgorithmName.SHA1,
                "sha256" => HashAlgorithmName.SHA256,
                "sha384" => HashAlgorithmName.SHA384,
                "sha512" => HashAlgorithmName.SHA512,
                _ => throw unsupportedErrorFactory(),
            };
        }

        private static int CoerceSize(object? size, string argumentName)
        {
            if (size == null || size is JsNull || !IsNumberValue(size))
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

        private static int CoercePbkdf2Iterations(object? iterations)
        {
            if (iterations == null || iterations is JsNull || !IsNumberValue(iterations))
            {
                throw new TypeError("The \"iterations\" argument must be of type number");
            }

            var number = TypeUtilities.ToNumber(iterations);
            if (double.IsNaN(number)
                || double.IsInfinity(number)
                || number < 1
                || global::System.Math.Truncate(number) != number
                || number > int.MaxValue)
            {
                throw new RangeError($"The value of \"iterations\" is out of range. It must be >= 1 && <= {int.MaxValue}.");
            }

            return (int)number;
        }

        private static int CoercePbkdf2KeyLength(object? keylen)
        {
            if (keylen == null || keylen is JsNull || !IsNumberValue(keylen))
            {
                throw new TypeError("The \"keylen\" argument must be of type number");
            }

            var number = TypeUtilities.ToNumber(keylen);
            if (double.IsNaN(number)
                || double.IsInfinity(number)
                || number < 0
                || global::System.Math.Truncate(number) != number
                || number > int.MaxValue)
            {
                throw new RangeError($"The value of \"keylen\" is out of range. It must be >= 0 && <= {int.MaxValue}.");
            }

            return (int)number;
        }

        private static bool IsNumberValue(object value)
        {
            return value is double
                || value is float
                || value is decimal
                || value is int
                || value is long
                || value is short
                || value is byte
                || value is sbyte
                || value is uint
                || value is ulong
                || value is ushort;
        }

        private static int ParseRequestedHmacKeyLength(object? value)
        {
            if (value == null)
            {
                throw new DataError("Invalid key length");
            }

            var number = TypeUtilities.ToNumber(value);
            if (double.IsNaN(number)
                || double.IsInfinity(number)
                || number < 0
                || global::System.Math.Truncate(number) != number
                || number > int.MaxValue)
            {
                throw new DataError("Invalid key length");
            }

            if (number == 0)
            {
                throw new DataError("Zero-length key is not supported");
            }

            return (int)number;
        }

        private static string NormalizeAlgorithmName(string algorithm)
            => algorithm
                .Trim()
                .Replace("-", string.Empty, StringComparison.Ordinal)
                .ToLowerInvariant();

        private static string GetAlgorithmName(object? algorithm, string argumentName)
        {
            if (algorithm is string text)
            {
                return text;
            }

            if (algorithm != null && algorithm is not JsNull)
            {
                var name = JavaScriptRuntime.ObjectRuntime.GetProperty(algorithm, "name");
                if (name is string nameText)
                {
                    return nameText;
                }
            }

            throw new TypeError($"The \"{argumentName}\" argument must be a string or object with a string \"name\" property");
        }

        private static string CanonicalizeWebCryptoHashName(string algorithm)
        {
            return NormalizeAlgorithmName(algorithm) switch
            {
                "sha1" => "SHA-1",
                "sha256" => "SHA-256",
                "sha384" => "SHA-384",
                "sha512" => "SHA-512",
                _ => throw new NotSupportedError("Unrecognized algorithm name"),
            };
        }

        private static TypeError CreateInvalidArgumentTypeError(string argumentName, string expectedDescription, object? value)
            => new($"The \"{argumentName}\" argument must be {expectedDescription}. {FormatReceivedValue(value)}");

        private static string FormatReceivedValue(object? value)
        {
            if (value is null || value is JsNull)
            {
                return "Received null";
            }

            var jsType = TypeUtilities.Typeof(value);
            return jsType switch
            {
                "number" or "boolean" or "bigint" or "string"
                    => $"Received type {jsType} ({DotNet2JSConversions.ToString(value)})",
                _ => $"Received type {jsType}",
            };
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

        private static byte[] CopyDataViewBytes(DataView dataView)
        {
            var byteOffset = (int)dataView.byteOffset;
            var byteLength = (int)dataView.byteLength;
            if (byteLength == 0)
            {
                return global::System.Array.Empty<byte>();
            }

            var copy = new byte[byteLength];
            global::System.Buffer.BlockCopy(dataView.buffer.RawBytes, byteOffset, copy, 0, byteLength);
            return copy;
        }

        private static void FillBuffer(Buffer buffer)
        {
            RandomNumberGenerator.Fill(buffer.AsWritableSpan());
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

    public sealed class Hmac
    {
        private IncrementalHash? _incrementalHash;

        internal Hmac(HashAlgorithmName algorithmName, byte[] key)
        {
            _incrementalHash = IncrementalHash.CreateHMAC(algorithmName, key);
        }

        public Hmac update(object? data)
            => update(data, null);

        public Hmac update(object? data, object? inputEncoding)
        {
            EnsureNotDigested();
            _incrementalHash!.AppendData(Crypto.CoerceHmacDataBytes(data, inputEncoding));
            return this;
        }

        public object digest()
            => digest(null);

        public object digest(object? outputEncoding)
        {
            EnsureNotDigested();

            var hmac = _incrementalHash!;
            var bytes = hmac.GetHashAndReset();
            hmac.Dispose();
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
        private readonly SubtleCrypto _subtle = new();

        public object subtle => _subtle;

        public object getRandomValues(object? target)
        {
            Crypto.FillRandomValues(target);
            return target!;
        }
    }

    public sealed class SubtleCrypto
    {
        public object digest(object? algorithm, object? data)
        {
            try
            {
                var algorithmName = Crypto.ResolveWebCryptoDigestAlgorithm(algorithm);
                var bytes = Crypto.CoerceBufferSourceBytes(data, "data");
                using var hash = IncrementalHash.CreateHash(algorithmName);
                hash.AppendData(bytes);
                return Promise.resolve(new ArrayBuffer(hash.GetHashAndReset(), cloneBuffer: false))!;
            }
            catch (Error ex)
            {
                return Promise.reject(ex)!;
            }
        }

        public object importKey(object? format, object? keyData, object? algorithm, object? extractable, object? keyUsages)
        {
            try
            {
                if (format is not string formatText)
                {
                    throw new TypeError("The \"format\" argument must be of type string");
                }

                if (!string.Equals(formatText, "raw", StringComparison.Ordinal))
                {
                    throw new NotSupportedError("Only raw secret keys are currently supported");
                }

                var importParameters = Crypto.ResolveHmacImportParameters(algorithm);
                var keyBytes = Crypto.CoerceBufferSourceBytes(keyData, "keyData");
                Crypto.ValidateImportedHmacKeyLength(keyBytes, importParameters.RequestedLengthBits);
                var usages = ParseKeyUsages(keyUsages);
                var key = new CryptoKey(
                    keyBytes,
                    importParameters.HashAlgorithmName,
                    importParameters.HashName,
                    TypeUtilities.ToBoolean(extractable),
                    usages);
                return Promise.resolve(key)!;
            }
            catch (Error ex)
            {
                return Promise.reject(ex)!;
            }
        }

        public object sign(object? algorithm, object? key, object? data)
        {
            try
            {
                Crypto.EnsureHmacAlgorithm(algorithm);
                var cryptoKey = RequireHmacKey(key, "sign");
                var bytes = Crypto.CoerceBufferSourceBytes(data, "data");
                using var hmac = IncrementalHash.CreateHMAC(cryptoKey.HashAlgorithmName, cryptoKey.KeyBytes);
                hmac.AppendData(bytes);
                return Promise.resolve(new ArrayBuffer(hmac.GetHashAndReset(), cloneBuffer: false))!;
            }
            catch (Error ex)
            {
                return Promise.reject(ex)!;
            }
        }

        public object verify(object? algorithm, object? key, object? signature, object? data)
        {
            try
            {
                Crypto.EnsureHmacAlgorithm(algorithm);
                var cryptoKey = RequireHmacKey(key, "verify");
                var signatureBytes = Crypto.CoerceBufferSourceBytes(signature, "signature");
                var bytes = Crypto.CoerceBufferSourceBytes(data, "data");
                using var hmac = IncrementalHash.CreateHMAC(cryptoKey.HashAlgorithmName, cryptoKey.KeyBytes);
                hmac.AppendData(bytes);
                var expected = hmac.GetHashAndReset();
                return Promise.resolve(CryptographicOperations.FixedTimeEquals(signatureBytes, expected))!;
            }
            catch (Error ex)
            {
                return Promise.reject(ex)!;
            }
        }

        private static CryptoKey RequireHmacKey(object? key, string requiredUsage)
        {
            if (key is not CryptoKey cryptoKey)
            {
                throw new TypeError("The \"key\" argument must be a CryptoKey");
            }

            if (!cryptoKey.SupportsUsage(requiredUsage))
            {
                throw new InvalidAccessError("Key does not support the requested operation");
            }

            return cryptoKey;
        }

        private static string[] ParseKeyUsages(object? keyUsages)
        {
            if (keyUsages is null || keyUsages is JsNull)
            {
                throw new TypeError("Failed to execute 'importKey' on 'SubtleCrypto': 5th argument can not be converted to sequence.");
            }

            if (keyUsages is not System.Collections.IEnumerable enumerable || keyUsages is string)
            {
                throw new TypeError("Failed to execute 'importKey' on 'SubtleCrypto': 5th argument can not be converted to sequence.");
            }

            var usages = new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
            var hasAny = false;
            var index = 0;
            foreach (var usage in enumerable)
            {
                hasAny = true;
                var usageText = usage switch
                {
                    null => "undefined",
                    JsNull => "null",
                    _ => DotNet2JSConversions.ToString(usage),
                };

                switch (usageText)
                {
                    case "sign":
                    case "verify":
                        usages.Add(usageText);
                        break;
                    case "encrypt":
                    case "decrypt":
                    case "deriveKey":
                    case "deriveBits":
                    case "wrapKey":
                    case "unwrapKey":
                        throw new SyntaxError("Unsupported key usage for an HMAC key");
                    default:
                        throw new TypeError($"Failed to execute 'importKey' on 'SubtleCrypto': 5th argument, index {index} value '{usageText}' is not a valid enum value of type KeyUsage.");
                }

                index++;
            }

            if (!hasAny)
            {
                throw new SyntaxError("Usages cannot be empty when importing a secret key.");
            }

            return usages.ToArray();
        }
    }

    public sealed class CryptoKey
    {
        private readonly HmacKeyAlgorithm _algorithm;
        private readonly JavaScriptRuntime.Array _usages;

        internal CryptoKey(byte[] keyBytes, HashAlgorithmName hashAlgorithmName, string hashName, bool extractable, string[] usages)
        {
            KeyBytes = keyBytes;
            HashAlgorithmName = hashAlgorithmName;
            _algorithm = new HmacKeyAlgorithm(hashName, keyBytes.Length * 8);
            _usages = new JavaScriptRuntime.Array(usages);
            this.extractable = extractable;
        }

        internal byte[] KeyBytes { get; }

        internal HashAlgorithmName HashAlgorithmName { get; }

        public string type => "secret";

        public object algorithm => _algorithm;

        public bool extractable { get; }

        public object usages => _usages;

        internal bool SupportsUsage(string usage)
        {
            foreach (var item in _usages)
            {
                if (item is string usageText && string.Equals(usageText, usage, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public sealed class HmacKeyAlgorithm
    {
        private readonly HashAlgorithmDescriptor _hash;

        internal HmacKeyAlgorithm(string hashName, int lengthBits)
        {
            _hash = new HashAlgorithmDescriptor(hashName);
            length = lengthBits;
        }

        public string name => "HMAC";

        public object hash => _hash;

        public double length { get; }
    }

    public sealed class HashAlgorithmDescriptor
    {
        internal HashAlgorithmDescriptor(string hashName)
        {
            name = hashName;
        }

        public string name { get; }
    }

    internal readonly struct HmacImportParameters
    {
        internal HmacImportParameters(HashAlgorithmName hashAlgorithmName, string hashName, int? requestedLengthBits)
        {
            HashAlgorithmName = hashAlgorithmName;
            HashName = hashName;
            RequestedLengthBits = requestedLengthBits;
        }

        internal HashAlgorithmName HashAlgorithmName { get; }

        internal string HashName { get; }

        internal int? RequestedLengthBits { get; }
    }

    internal sealed class NotSupportedError : Error
    {
        internal NotSupportedError(string? message) : base(message)
        {
            Name = "NotSupportedError";
        }
    }

    internal sealed class InvalidAccessError : Error
    {
        internal InvalidAccessError(string? message) : base(message)
        {
            Name = "InvalidAccessError";
        }
    }

    internal sealed class DataError : Error
    {
        internal DataError(string? message) : base(message)
        {
            Name = "DataError";
        }
    }
}
