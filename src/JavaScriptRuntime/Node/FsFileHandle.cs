using System;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using JavaScriptRuntime;
using JavaScriptRuntime.EngineCore;

namespace JavaScriptRuntime.Node
{
    public sealed class FileHandle
    {
        private readonly IIOScheduler _ioScheduler;
        private readonly string _path;
        private readonly bool _append;
        private FileStream? _stream;
        private bool _closed;

        internal FileHandle(string path, FileStream stream, bool append, int fileDescriptor, IIOScheduler ioScheduler)
        {
            _path = path;
            _append = append;
            _stream = stream;
            fd = fileDescriptor;
            _ioScheduler = ioScheduler;
        }

        public double fd { get; }

        public object read(object? buffer)
            => read(buffer, null, null, null);

        public object read(object? buffer, object? offset)
            => read(buffer, offset, null, null);

        public object read(object? buffer, object? offset, object? length)
            => read(buffer, offset, length, null);

        public object read(object? buffer, object? offset, object? length, object? position)
        {
            if (buffer is not Buffer targetBuffer)
            {
                return Promise.reject(new TypeError("The \"buffer\" argument must be an instance of Buffer."))!;
            }

            var promiseWithResolvers = Promise.withResolvers();
            _ioScheduler.BeginIo();

            try
            {
                var targetSpan = targetBuffer.AsWritableSpan();
                var resolvedOffset = FsCommon.CoerceNonNegativeInt(offset, 0);
                if (resolvedOffset > targetSpan.Length)
                {
                    throw new RangeError("The value of \"offset\" is out of range.");
                }

                var defaultLength = targetSpan.Length - resolvedOffset;
                var resolvedLength = FsCommon.CoerceNonNegativeInt(length, defaultLength);
                if (resolvedLength > defaultLength)
                {
                    throw new RangeError("The value of \"length\" is out of range.");
                }

                var resolvedPosition = CoercePosition(position);

                _ = CompleteReadAsync(targetBuffer, resolvedOffset, resolvedLength, resolvedPosition, promiseWithResolvers);
                return promiseWithResolvers.promise;
            }
            catch (Exception ex)
            {
                _ioScheduler.EndIo(promiseWithResolvers, ex as Error ?? new Error(ex.Message, ex), isError: true);
                return promiseWithResolvers.promise;
            }
        }

        public object write(object? data)
            => write(data, null, null);

        public object write(object? data, object? position)
            => write(data, position, null);

        public object write(object? data, object? position, object? encoding)
        {
            if (data == null || data is JsNull)
            {
                return Promise.reject(new TypeError("The \"data\" argument must be of type string or Buffer or TypedArray or DataView. Received null"))!;
            }

            var promiseWithResolvers = Promise.withResolvers();
            _ioScheduler.BeginIo();

            try
            {
                var resolvedPosition = CoercePosition(position);

                _ = CompleteWriteAsync(data, resolvedPosition, encoding, promiseWithResolvers);
                return promiseWithResolvers.promise;
            }
            catch (Exception ex)
            {
                _ioScheduler.EndIo(promiseWithResolvers, ex as Error ?? new Error(ex.Message, ex), isError: true);
                return promiseWithResolvers.promise;
            }
        }

        public object close()
        {
            var promiseWithResolvers = Promise.withResolvers();
            _ioScheduler.BeginIo();

            _ = CompleteCloseAsync(promiseWithResolvers);
            return promiseWithResolvers.promise;
        }

        private async Task CompleteReadAsync(Buffer buffer, int offset, int length, long? position, PromiseWithResolvers promiseWithResolvers)
        {
            try
            {
                var stream = GetOpenStream();
                if (position.HasValue)
                {
                    stream.Position = position.Value;
                }

                var temp = new byte[length];
                var bytesRead = await stream.ReadAsync(temp, 0, length).ConfigureAwait(false);
                if (bytesRead > 0)
                {
                    temp.AsSpan(0, bytesRead).CopyTo(buffer.AsWritableSpan().Slice(offset, bytesRead));
                }

                dynamic result = new ExpandoObject();
                result.bytesRead = (double)bytesRead;
                result.buffer = buffer;
                _ioScheduler.EndIo(promiseWithResolvers, result, isError: false);
            }
            catch (Exception ex)
            {
                _ioScheduler.EndIo(promiseWithResolvers, TranslateFileHandleError("read", ex), isError: true);
            }
        }

        private async Task CompleteWriteAsync(object? data, long? position, object? encoding, PromiseWithResolvers promiseWithResolvers)
        {
            try
            {
                var stream = GetOpenStream();
                if (_append)
                {
                    stream.Seek(0, SeekOrigin.End);
                }
                else if (position.HasValue)
                {
                    stream.Position = position.Value;
                }

                byte[] bytes;
                object? resultBuffer = data;
                if (data is Buffer buffer)
                {
                    bytes = buffer.ToByteArray();
                }
                else if (data is byte[] byteArray)
                {
                    bytes = byteArray;
                }
                else
                {
                    var text = data?.ToString() ?? string.Empty;
                    var textEncoding = FsEncodingOptions.TryGetTextEncoding(encoding, out var resolvedEncoding)
                        ? resolvedEncoding!
                        : FsEncodingOptions.Utf8NoBom;
                    bytes = textEncoding.GetBytes(text);
                }

                await stream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                await stream.FlushAsync().ConfigureAwait(false);

                dynamic result = new ExpandoObject();
                result.bytesWritten = (double)bytes.Length;
                result.buffer = resultBuffer;
                _ioScheduler.EndIo(promiseWithResolvers, result, isError: false);
            }
            catch (Exception ex)
            {
                _ioScheduler.EndIo(promiseWithResolvers, TranslateFileHandleError("write", ex), isError: true);
            }
        }

        private async Task CompleteCloseAsync(PromiseWithResolvers promiseWithResolvers)
        {
            try
            {
                await Task.Run(() =>
                {
                    if (_closed)
                    {
                        return;
                    }

                    _closed = true;
                    _stream?.Dispose();
                    _stream = null;
                }).ConfigureAwait(false);

                _ioScheduler.EndIo(promiseWithResolvers, null, isError: false);
            }
            catch (Exception ex)
            {
                _ioScheduler.EndIo(promiseWithResolvers, TranslateFileHandleError("close", ex), isError: true);
            }
        }

        private FileStream GetOpenStream()
        {
            if (_closed || _stream == null)
            {
                throw FsCommon.CreateBadFileDescriptorError("file closed");
            }

            return _stream;
        }

        private static long? CoercePosition(object? position)
        {
            if (position == null || position is JsNull)
            {
                return null;
            }

            var number = TypeUtilities.ToNumber(position);
            if (double.IsNaN(number) || double.IsInfinity(number))
            {
                throw new RangeError("The value of \"position\" is out of range.");
            }

            if (number < 0)
            {
                throw new RangeError("The value of \"position\" is out of range.");
            }

            return (long)number;
        }

        private Error TranslateFileHandleError(string operation, Exception ex)
        {
            if (ex is Error error)
            {
                return error;
            }

            if (ex is ObjectDisposedException)
            {
                return FsCommon.CreateBadFileDescriptorError(operation);
            }

            return operation switch
            {
                "read" => FsCommon.TranslateReadFileError(_path, ex),
                "write" => FsCommon.TranslateWriteFileError(_path, ex),
                "close" => ex as Error ?? FsCommon.CreateBadFileDescriptorError("close"),
                _ => new Error(ex.Message, ex)
            };
        }
    }

    internal sealed class FileReadStream : Readable
    {
        private readonly IIOScheduler _ioScheduler;
        private readonly PromiseWithResolvers? _lifetimePromise;
        private readonly string _path;
        private readonly object? _flags;
        private readonly int _highWaterMark;
        private readonly long? _start;
        private readonly long? _endInclusive;
        private FileStream? _stream;
        private bool _closed;
        private bool _ioCompleted;
        private bool _started;

        internal FileReadStream(object? path, object? options, IIOScheduler ioScheduler)
        {
            _path = path?.ToString() ?? string.Empty;
            _flags = FsCommon.GetOption(options, "flags");
            _highWaterMark = FsCommon.GetIntOption(options, "highWaterMark", 64 * 1024);
            _start = FsCommon.GetNullableLongOption(options, "start");
            _endInclusive = FsCommon.GetNullableLongOption(options, "end");
            _ioScheduler = ioScheduler;
            _lifetimePromise = NodeNetworkingCommon.CreateIoPromise();
            _ioScheduler.BeginIo();

            var encoding = FsCommon.GetOption(options, "encoding");
            if (encoding != null && encoding is not JsNull)
            {
                setEncoding(encoding);
            }
        }

        public new FileReadStream on(object? eventName, object? listener)
        {
            base.on(eventName, listener);
            EnsureStarted();
            return this;
        }

        public new FileReadStream addListener(object? eventName, object? listener)
            => on(eventName, listener);

        private void EnsureStarted()
        {
            if (_started)
            {
                return;
            }

            _started = true;
            NodeNetworkingCommon.ScheduleOnEventLoop(Start);
        }

        public override void destroy(object? error)
        {
            CloseStream();
            base.destroy(error);
            NodeNetworkingCommon.ScheduleOnEventLoop(CompleteIo);
        }

        private void Start()
        {
            try
            {
                if (string.IsNullOrEmpty(_path))
                {
                    throw new Error("Path must be a non-empty string");
                }

                _stream = FsCommon.OpenFileStream(_path, _flags, defaultFlags: "r", share: FileShare.ReadWrite);
                if (_start.HasValue)
                {
                    _stream.Position = _start.Value;
                }

                ReadLoop();
            }
            catch (Exception ex)
            {
                var error = ex as Error ?? FsCommon.TranslateOpenError(_path, ex);
                NodeNetworkingCommon.ScheduleOnEventLoop(() => destroy(error));
            }
        }

        private void ReadLoop()
        {
            var stream = _stream;
            if (stream == null)
            {
                return;
            }

            try
            {
                while (!destroyed)
                {
                    var bytesToRead = _highWaterMark;
                    if (_endInclusive.HasValue)
                    {
                        var remaining = _endInclusive.Value - stream.Position + 1;
                        if (remaining <= 0)
                        {
                            break;
                        }

                        bytesToRead = (int)System.Math.Min(bytesToRead, remaining);
                    }

                    var buffer = new byte[bytesToRead];
                    var bytesRead = stream.Read(buffer, 0, bytesToRead);
                    if (bytesRead <= 0)
                    {
                        break;
                    }

                    if (bytesRead != buffer.Length)
                    {
                        var resized = new byte[bytesRead];
                        System.Buffer.BlockCopy(buffer, 0, resized, 0, bytesRead);
                        buffer = resized;
                    }

                    var chunk = Buffer.FromBytes(buffer);
                    if (!destroyed)
                    {
                        push(chunk);
                    }
                }

                if (!destroyed)
                {
                    push(null);
                    CloseStream();
                    EmitCloseOnce();
                    NodeNetworkingCommon.ScheduleOnEventLoop(CompleteIo);
                }
            }
            catch (Exception ex)
            {
                var error = ex as Error ?? FsCommon.TranslateReadFileError(_path, ex);
                NodeNetworkingCommon.ScheduleOnEventLoop(() => destroy(error));
            }
        }

        private void CloseStream()
        {
            if (_closed)
            {
                return;
            }

            _closed = true;
            _stream?.Dispose();
            _stream = null;
        }

        private void EmitCloseOnce()
        {
            if (_closed)
            {
                emit("close");
            }
        }

        private void CompleteIo()
        {
            if (_ioCompleted || _lifetimePromise == null)
            {
                return;
            }

            _ioCompleted = true;
            _ioScheduler.EndIo(_lifetimePromise, null, isError: false);
        }
    }

    internal sealed class FileWriteStream : Writable
    {
        private readonly string _path;
        private readonly object? _flags;
        private readonly object? _encoding;
        private readonly bool _append;
        private FileStream? _stream;
        private bool _closed;
        private bool _closeQueued;

        internal FileWriteStream(object? path, object? options)
        {
            _path = path?.ToString() ?? string.Empty;
            _flags = FsCommon.GetOption(options, "flags");
            _encoding = FsCommon.GetOption(options, "encoding");
            highWaterMark = FsCommon.GetIntOption(options, "highWaterMark", 16);

            try
            {
                var spec = FsCommon.ResolveOpenSpec(_flags, defaultFlags: "w");
                _append = spec.Append;
                _stream = FsCommon.OpenFileStream(_path, spec.NormalizedFlags, defaultFlags: "w", share: FileShare.Read);
            }
            catch (Exception ex)
            {
                var error = ex as Error ?? FsCommon.TranslateOpenError(_path, ex);
                NodeNetworkingCommon.ScheduleOnEventLoop(() => destroy(error));
            }
        }

        public override void destroy(object? error)
        {
            CloseStream(emitClose: false);
            base.destroy(error);
        }

        public override void end()
        {
            base.end();
            QueueClose();
        }

        public override void end(object? chunk)
        {
            base.end(chunk);
            QueueClose();
        }

        public override void end(object? chunk, object? callback)
        {
            base.end(chunk, callback);
            QueueClose();
        }

        protected override void InvokeWrite(object? chunk)
        {
            if (_stream == null || _closed)
            {
                return;
            }

            try
            {
                if (_append)
                {
                    _stream.Seek(0, SeekOrigin.End);
                }

                byte[] bytes;
                if (chunk is Buffer buffer)
                {
                    bytes = buffer.ToByteArray();
                }
                else if (chunk is byte[] byteArray)
                {
                    bytes = byteArray;
                }
                else
                {
                    var text = chunk?.ToString() ?? string.Empty;
                    var encoding = FsEncodingOptions.TryGetTextEncoding(_encoding, out var resolvedEncoding)
                        ? resolvedEncoding!
                        : FsEncodingOptions.Utf8NoBom;
                    bytes = encoding.GetBytes(text);
                }

                _stream.Write(bytes, 0, bytes.Length);
                _stream.Flush();
            }
            catch (Exception ex)
            {
                destroy(ex as Error ?? FsCommon.TranslateWriteFileError(_path, ex));
            }
        }

        private void QueueClose()
        {
            if (_closeQueued)
            {
                return;
            }

            _closeQueued = true;
            NodeNetworkingCommon.ScheduleOnEventLoop(() =>
            {
                CloseStream(emitClose: true);
            });
        }

        private void CloseStream(bool emitClose)
        {
            if (_closed)
            {
                return;
            }

            _closed = true;
            try
            {
                _stream?.Dispose();
            }
            finally
            {
                _stream = null;
            }

            if (emitClose)
            {
                emit("close");
            }
        }
    }
}
