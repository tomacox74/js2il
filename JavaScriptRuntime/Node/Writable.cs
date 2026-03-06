using System;
using System.Collections.Generic;

namespace JavaScriptRuntime.Node
{
    public class Writable : EventEmitter
    {
        private const int DefaultHighWaterMark = 16;
        private readonly Queue<object?> _buffer = new();
        private bool _ended = false;
        private bool _writing = false;
        private bool _needDrain = false;

        // Basic backpressure configuration (highWaterMark is typically set via options in Node).
        public double highWaterMark = DefaultHighWaterMark;

        public bool writable => !_ended;

        // Constructor for subclassing
        public Writable() { }

        private void EmitDrainAsync()
        {
            try
            {
                var sp = JavaScriptRuntime.GlobalThis.ServiceProvider;
                if (sp != null && sp.TryResolve<JavaScriptRuntime.EngineCore.NodeSchedulerState>(out var scheduler) && scheduler != null)
                {
                    scheduler.QueueNextTick(() => emit("drain"));
                    return;
                }
            }
            catch
            {
                // Ignore and fall back to synchronous emit.
            }

            emit("drain");
        }

        // Write data to the stream
        public bool write(object? chunk)
        {
            if (_ended)
            {
                throw new Error("Cannot write after end");
            }

            _buffer.Enqueue(chunk);

            int threshold;
            try
            {
                threshold = (int)JavaScriptRuntime.TypeUtilities.ToNumber(highWaterMark);
            }
            catch
            {
                threshold = DefaultHighWaterMark;
            }

            if (threshold < 1) threshold = 1;

            var canAcceptMore = _buffer.Count < threshold;
            if (!canAcceptMore)
            {
                _needDrain = true;
            }

            if (!_writing)
            {
                _writing = true;
                try
                {
                    while (_buffer.Count > 0)
                    {
                        _doWrite();
                    }
                }
                finally
                {
                    _writing = false;
                }

                if (_needDrain && _buffer.Count == 0)
                {
                    _needDrain = false;
                    EmitDrainAsync();
                }
            }

            return canAcceptMore;
        }

        public bool write(object? chunk, object? encoding)
        {
            // Ignore encoding in baseline implementation
            return write(chunk);
        }

        public bool write(object? chunk, object? encoding, object? callback)
        {
            // Ignore encoding and callback in baseline implementation
            return write(chunk);
        }

        // End the stream
        public virtual void end()
        {
            if (_ended)
            {
                emit("error", new Error("write after end"));
                return;
            }

            _ended = true;

            if (!_writing)
            {
                _writing = true;
                try
                {
                    while (_buffer.Count > 0)
                    {
                        _doWrite();
                    }
                }
                finally
                {
                    _writing = false;
                }
            }

            if (_needDrain && _buffer.Count == 0)
            {
                _needDrain = false;
                EmitDrainAsync();
            }

            emit("finish");
        }

        public virtual void end(object? chunk)
        {
            if (chunk != null && chunk is not JsNull)
            {
                write(chunk);
            }
            end();
        }

        public virtual void end(object? chunk, object? callback)
        {
            end(chunk);
            // Ignore callback in baseline implementation
        }

        // Internal write implementation
        private void _doWrite()
        {
            if (_buffer.Count > 0)
            {
                var chunk = _buffer.Dequeue();
                InvokeWrite(chunk);
            }
        }

        // To be overridden by subclasses or set by user code
        public object? _write = null;

        protected virtual void InvokeWrite(object? chunk)
        {
            if (_write != null && _write is Delegate writeFunc)
            {
                try
                {
                    var previousThis = RuntimeServices.SetCurrentThis(this);
                    try
                    {
                        Closure.InvokeWithArgs(writeFunc, System.Array.Empty<object>(), new[] { chunk });
                    }
                    finally
                    {
                        RuntimeServices.SetCurrentThis(previousThis);
                    }
                }
                catch
                {
                    // Ignore errors in user write function for baseline
                }
            }
        }
    }
}
