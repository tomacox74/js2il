using System;
using System.Collections.Generic;

namespace JavaScriptRuntime.Node
{
    public class Readable : EventEmitter
    {
        private readonly Queue<object?> _buffer = new();
        private bool _ended;
        private bool _endEmitted;
        private bool _manuallyPaused;
        private bool _destroyed;
        private bool _closeEmitted;
        private int _pipePauseCount;
        private string? _encoding;
        private Utf8ChunkDecoder? _textDecoder;

        public bool readable => !_ended && !_destroyed;

        public bool destroyed => _destroyed;

        public string? readableEncoding => _encoding;

        public bool readableObjectMode { get; }

        public Readable(object? options = null)
        {
            readableObjectMode = ResolveObjectMode(options, "readableObjectMode");
        }

        public bool push(object? chunk)
        {
            if (_destroyed)
            {
                return false;
            }

            if (_ended)
            {
                throw new Error("Cannot push after EOF");
            }

            if (chunk == null || chunk is JsNull)
            {
                if (_encoding != null)
                {
                    QueueOrEmitChunk(_textDecoder?.Flush());
                }

                _ended = true;
                FlushBuffered();
                EmitEndIfReady();
                return false;
            }

            QueueOrEmitChunk(TransformChunk(chunk));
            FlushBuffered();
            return !IsPausedForDelivery();
        }

        public object? read()
        {
            if (_buffer.Count > 0)
            {
                var value = _buffer.Dequeue();
                EmitEndIfReady();
                return value;
            }

            if (_ended)
            {
                EmitEndIfReady();
            }

            return null;
        }

        public object? read(object? size)
        {
            return read();
        }

        public Readable pause()
        {
            if (_destroyed)
            {
                return this;
            }

            if (!_manuallyPaused)
            {
                _manuallyPaused = true;
                emit("pause");
            }

            return this;
        }

        public Readable resume()
        {
            if (_destroyed)
            {
                return this;
            }

            var wasPaused = _manuallyPaused;
            _manuallyPaused = false;
            if (wasPaused)
            {
                emit("resume");
            }

            FlushBuffered();
            return this;
        }

        public Readable setEncoding(object? encoding)
        {
            if (!FsEncodingOptions.TryGetTextEncoding(encoding, out _))
            {
                var requestedEncoding = DotNet2JSConversions.ToString(encoding);
                throw new NotSupportedException($"Readable.setEncoding only supports utf8 in the current runtime (received '{requestedEncoding}').");
            }

            var normalizedEncoding = NormalizeEncodingName(encoding);
            if (_encoding != null && !string.Equals(_encoding, normalizedEncoding, StringComparison.OrdinalIgnoreCase))
            {
                throw new NotSupportedException("Changing a readable stream encoding after it has been set is not supported.");
            }

            if (_encoding != null)
            {
                return this;
            }

            _encoding = normalizedEncoding;
            _textDecoder = new Utf8ChunkDecoder();
            ReprocessBufferedChunks();
            FlushBuffered();
            return this;
        }

        public virtual void destroy()
        {
            destroy(null);
        }

        public virtual void destroy(object? error)
        {
            if (_destroyed)
            {
                return;
            }

            _destroyed = true;
            _ended = true;
            _endEmitted = true;
            _buffer.Clear();
            _textDecoder = null;

            NodeNetworkingCommon.ScheduleOnEventLoop(null, () =>
            {
                if (error != null && error is not JsNull)
                {
                    emit("error", error);
                }

                EmitClose();
            });
        }

        public Writable pipe(object? destination)
        {
            if (destination is not Writable writable)
            {
                throw new TypeError("Pipe destination must be a Writable stream");
            }

            bool paused = false;
            bool drainAttached = false;
            Func<object[], object?[], object?>? onData = null;
            Func<object[], object?[], object?>? onEnd = null;
            Func<object[], object?[], object?>? onError = null;
            Func<object[], object?[], object?>? onDrain = null;

            void PausePipeFlow()
            {
                if (paused)
                {
                    return;
                }

                paused = true;
                _pipePauseCount++;
            }

            void ResumePipeFlow()
            {
                if (!paused)
                {
                    FlushBuffered();
                    return;
                }

                paused = false;
                if (_pipePauseCount > 0)
                {
                    _pipePauseCount--;
                }

                FlushBuffered();
            }

            void Cleanup()
            {
                if (onData != null)
                {
                    off("data", onData);
                }

                if (onEnd != null)
                {
                    off("end", onEnd);
                }

                if (onError != null)
                {
                    off("error", onError);
                }

                if (drainAttached && onDrain != null)
                {
                    drainAttached = false;
                    writable.off("drain", onDrain);
                }

                if (paused)
                {
                    paused = false;
                    if (_pipePauseCount > 0)
                    {
                        _pipePauseCount--;
                    }
                }
            }

            onDrain = (_, _) =>
            {
                if (drainAttached)
                {
                    drainAttached = false;
                    writable.off("drain", onDrain);
                }

                ResumePipeFlow();
                return null;
            };

            onData = (_, args) =>
            {
                if (args.Length == 0)
                {
                    return null;
                }

                var canContinue = writable.write(args[0]);
                if (!canContinue)
                {
                    PausePipeFlow();
                    if (!drainAttached)
                    {
                        drainAttached = true;
                        writable.on("drain", onDrain);
                    }
                }

                return null;
            };

            onEnd = (_, _) =>
            {
                Cleanup();
                writable.end();
                return null;
            };

            onError = (_, args) =>
            {
                var err = args.Length > 0 ? args[0] : new Error("Readable stream error");
                Cleanup();
                writable.destroy(err);
                return null;
            };

            on("data", onData);
            on("end", onEnd);
            on("error", onError);

            FlushBuffered();
            return writable;
        }

        protected void _read()
        {
        }

        private void EmitClose()
        {
            if (_closeEmitted)
            {
                return;
            }

            _closeEmitted = true;
            emit("close");
        }

        private void EmitEndIfReady()
        {
            if (_destroyed)
            {
                return;
            }

            if (_ended && !_endEmitted && _buffer.Count == 0)
            {
                _endEmitted = true;
                emit("end");
            }
        }

        private void FlushBuffered()
        {
            while (!IsPausedForDelivery() && listenerCount("data") > 0 && _buffer.Count > 0)
            {
                emit("data", _buffer.Dequeue());
            }

            EmitEndIfReady();
        }

        private bool IsPausedForDelivery()
            => _manuallyPaused || _pipePauseCount > 0;

        private static string NormalizeEncodingName(object? encoding)
        {
            var value = DotNet2JSConversions.ToString(encoding);
            if (string.Equals(value, "utf-8", StringComparison.OrdinalIgnoreCase))
            {
                return "utf8";
            }

            return value.ToLowerInvariant();
        }

        private void QueueOrEmitChunk(object? chunk)
        {
            if (chunk == null || chunk is JsNull)
            {
                return;
            }

            if (chunk is string text && text.Length == 0)
            {
                return;
            }

            if (IsPausedForDelivery() || listenerCount("data") == 0)
            {
                _buffer.Enqueue(chunk);
                return;
            }

            emit("data", chunk);
        }

        private void ReprocessBufferedChunks()
        {
            if (_buffer.Count == 0)
            {
                return;
            }

            var buffered = _buffer.ToArray();
            _buffer.Clear();

            foreach (var chunk in buffered)
            {
                QueueOrEmitChunk(TransformChunk(chunk));
            }
        }

        private object? TransformChunk(object? chunk)
        {
            if (_encoding == null)
            {
                return chunk;
            }

            if (chunk is string text)
            {
                return text;
            }

            if (chunk is Buffer buffer)
            {
                var bufferBytes = buffer.ToByteArray();
                return _textDecoder?.Decode(bufferBytes, bufferBytes.Length, flush: false);
            }

            if (chunk is byte[] rawBytes)
            {
                return _textDecoder?.Decode(rawBytes, rawBytes.Length, flush: false);
            }

            return NodeNetworkingCommon.CoerceToText(chunk);
        }

        private static bool ResolveObjectMode(object? options, string specificOptionName)
        {
            return TypeUtilities.ToBoolean(NodeNetworkingCommon.TryGetOption(options, "objectMode"))
                || TypeUtilities.ToBoolean(NodeNetworkingCommon.TryGetOption(options, specificOptionName));
        }
    }
}
