using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace JavaScriptRuntime.Node
{
    [NodeModule("zlib")]
    public sealed class Zlib
    {
        public Buffer gzipSync(object? data)
            => gzipSync(data, null);

        public Buffer gzipSync(object? data, object? options)
        {
            var compressionLevel = ResolveGzipCompressionLevel(options, "gzipSync");
            var inputBytes = Crypto.CoerceBytes(data, null);
            return Buffer.FromBytes(CompressGzip(inputBytes, compressionLevel));
        }

        public Buffer gunzipSync(object? data)
            => gunzipSync(data, null);

        public Buffer gunzipSync(object? data, object? options)
        {
            ValidateNoDeferredOptions(options, "gunzipSync");
            var inputBytes = Crypto.CoerceBytes(data, null);
            return Buffer.FromBytes(DecompressGzip(inputBytes));
        }

        public Transform createGzip()
            => createGzip(null);

        public Transform createGzip(object? options)
        {
            var compressionLevel = ResolveGzipCompressionLevel(options, "createGzip");
            return new GzipTransform(compressionLevel);
        }

        public Transform createGunzip()
            => createGunzip(null);

        public Transform createGunzip(object? options)
        {
            ValidateNoDeferredOptions(options, "createGunzip");
            return new GunzipTransform();
        }

        private static byte[] CompressGzip(byte[] inputBytes, CompressionLevel compressionLevel)
        {
            using var output = new MemoryStream();
            using (var stream = new GZipStream(output, compressionLevel, leaveOpen: true))
            {
                if (inputBytes.Length > 0)
                {
                    stream.Write(inputBytes, 0, inputBytes.Length);
                }
            }

            return output.ToArray();
        }

        private static byte[] DecompressGzip(byte[] inputBytes)
        {
            try
            {
                using var input = new MemoryStream(inputBytes, writable: false);
                using var stream = new GZipStream(input, CompressionMode.Decompress, leaveOpen: false);
                using var output = new MemoryStream();
                stream.CopyTo(output);
                return output.ToArray();
            }
            catch (InvalidDataException ex)
            {
                throw new Error("Invalid gzip data.", ex);
            }
        }

        private static CompressionLevel ResolveGzipCompressionLevel(object? options, string methodName)
        {
            ValidateDeferredOptions(options, methodName, new HashSet<string>(StringComparer.Ordinal)
            {
                "level"
            });

            var levelValue = NodeNetworkingCommon.TryGetOption(options, "level");
            if (levelValue == null || levelValue is JsNull)
            {
                return CompressionLevel.Optimal;
            }

            double numericLevel;
            try
            {
                numericLevel = TypeUtilities.ToNumber(levelValue);
            }
            catch (Exception ex)
            {
                throw new TypeError("The \"level\" option must be an integer between 0 and 9.", ex);
            }

            if (double.IsNaN(numericLevel)
                || double.IsInfinity(numericLevel)
                || numericLevel != System.Math.Truncate(numericLevel)
                || numericLevel < 0
                || numericLevel > 9)
            {
                throw new RangeError("The \"level\" option must be an integer between 0 and 9.");
            }

            return (int)numericLevel switch
            {
                0 => CompressionLevel.NoCompression,
                <= 3 => CompressionLevel.Fastest,
                9 => CompressionLevel.SmallestSize,
                _ => CompressionLevel.Optimal
            };
        }

        private static void ValidateNoDeferredOptions(object? options, string methodName)
            => ValidateDeferredOptions(options, methodName, new HashSet<string>(StringComparer.Ordinal));

        private static void ValidateDeferredOptions(object? options, string methodName, ISet<string> supportedOptionNames)
        {
            if (options == null || options is JsNull)
            {
                return;
            }

            if (options is string || options.GetType().IsValueType)
            {
                throw new TypeError($"zlib.{methodName} options must be an object when provided.");
            }

            foreach (var key in EnumerateEnumerableOptionKeys(options))
            {
                if (supportedOptionNames.Contains(key))
                {
                    continue;
                }

                var optionValue = NodeNetworkingCommon.TryGetOption(options, key);
                if (optionValue == null || optionValue is JsNull)
                {
                    continue;
                }

                throw new Error($"zlib.{methodName} does not yet support the \"{key}\" option.");
            }
        }

        private static IEnumerable<string> EnumerateEnumerableOptionKeys(object options)
        {
            var keys = global::JavaScriptRuntime.Object.GetEnumerableKeys(options);
            for (var i = 0; i < keys.Count; i++)
            {
                var key = keys[i];
                if (key == null || key is JsNull)
                {
                    continue;
                }

                yield return DotNet2JSConversions.ToString(key);
            }
        }

        private abstract class BufferedZlibTransform : Transform
        {
            private readonly MemoryStream _input = new();
            private bool _completed;

            protected override void InvokeWrite(object? chunk)
            {
                if (_completed || chunk == null || chunk is JsNull)
                {
                    return;
                }

                var bytes = Crypto.CoerceBytes(chunk, null);
                if (bytes.Length > 0)
                {
                    _input.Write(bytes, 0, bytes.Length);
                }
            }

            public override void end()
            {
                CompleteAndEnd(null, null);
            }

            public override void end(object? chunk)
            {
                CompleteAndEnd(chunk, null);
            }

            public override void end(object? chunk, object? callback)
            {
                CompleteAndEnd(chunk, callback);
            }

            private void CompleteAndEnd(object? chunk, object? callback)
            {
                if (callback is Delegate del)
                {
                    once("finish", del);
                }

                if (_completed)
                {
                    base.end();
                    return;
                }

                if (chunk != null && chunk is not JsNull)
                {
                    InvokeWrite(chunk);
                }

                _completed = true;

                try
                {
                    var outputBytes = TransformFinalChunk(_input.ToArray());
                    if (outputBytes.Length > 0)
                    {
                        push(Buffer.FromBytes(outputBytes));
                    }
                }
                catch (Exception ex)
                {
                    destroy(ex as Error ?? new Error(ex.Message, ex));
                    return;
                }
                finally
                {
                    _input.Dispose();
                }

                base.end();
            }

            protected abstract byte[] TransformFinalChunk(byte[] inputBytes);
        }

        private sealed class GzipTransform : BufferedZlibTransform
        {
            private readonly CompressionLevel _compressionLevel;

            internal GzipTransform(CompressionLevel compressionLevel)
            {
                _compressionLevel = compressionLevel;
            }

            protected override byte[] TransformFinalChunk(byte[] inputBytes)
                => CompressGzip(inputBytes, _compressionLevel);
        }

        private sealed class GunzipTransform : BufferedZlibTransform
        {
            protected override byte[] TransformFinalChunk(byte[] inputBytes)
                => DecompressGzip(inputBytes);
        }
    }
}
