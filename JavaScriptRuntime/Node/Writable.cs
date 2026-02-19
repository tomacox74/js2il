using System;
using System.Collections.Generic;

namespace JavaScriptRuntime.Node
{
    public class Writable : EventEmitter
    {
        private const int BackpressureThreshold = 16;
        private readonly Queue<object?> _buffer = new();
        private bool _ended = false;
        private bool _writing = false;
        private bool _needDrain = false;

        public bool writable => !_ended;

        // Constructor for subclassing
        public Writable() { }

        // Write data to the stream
        public bool write(object? chunk)
        {
            if (_ended)
            {
                throw new Error("Cannot write after end");
            }

            _buffer.Enqueue(chunk);

            var canAcceptMore = _buffer.Count < BackpressureThreshold;
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
                    emit("drain");
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
        public void end()
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
                emit("drain");
            }

            emit("finish");
        }

        public void end(object? chunk)
        {
            if (chunk != null && chunk is not JsNull)
            {
                write(chunk);
            }
            end();
        }

        public void end(object? chunk, object? callback)
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
