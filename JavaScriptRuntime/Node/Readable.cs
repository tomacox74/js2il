using System;
using System.Collections.Generic;

namespace JavaScriptRuntime.Node
{
    public class Readable : EventEmitter
    {
        private readonly Queue<object?> _buffer = new();
        private bool _ended = false;
        private bool _endEmitted = false;

        public bool readable => !_ended;

        // Constructor for subclassing
        public Readable() { }

        // Push data into the internal buffer
        public bool push(object? chunk)
        {
            if (_ended)
            {
                throw new Error("Cannot push after EOF");
            }

            if (chunk == null || chunk is JsNull)
            {
                _ended = true;
                EmitEndIfReady();
                return false;
            }

            if (listenerCount("data") > 0)
            {
                emit("data", chunk);
            }
            else
            {
                _buffer.Enqueue(chunk);
            }
            return true;
        }

        // Read data from the stream
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
                return null;
            }

            return null;
        }

        public object? read(object? size)
        {
            // For simplicity, ignore size parameter in baseline
            return read();
        }

        // Pipe this readable stream to a writable stream
        public Writable pipe(object? destination)
        {
            if (destination is not Writable writable)
            {
                throw new TypeError("Pipe destination must be a Writable stream");
            }

            Func<object[], object?[], object?>? onData = null;
            Func<object[], object?[], object?>? onEnd = null;
            Func<object[], object?[], object?>? onError = null;

            onData = (scopes, args) =>
            {
                if (args.Length > 0)
                {
                    writable.write(args[0]);
                }
                return null;
            };

            onEnd = (scopes, args) =>
            {
                if (onData != null)
                {
                    off("data", onData);
                }
                if (onError != null)
                {
                    off("error", onError);
                }
                writable.end();
                return null;
            };

            onError = (scopes, args) =>
            {
                if (onData != null)
                {
                    off("data", onData);
                }
                if (args.Length > 0)
                {
                    writable.emit("error", args[0]);
                }
                return null;
            };

            on("data", onData);
            once("end", onEnd);
            once("error", onError);

            return writable;
        }

        private void EmitEndIfReady()
        {
            if (_ended && !_endEmitted && _buffer.Count == 0)
            {
                _endEmitted = true;
                emit("end");
            }
        }

        // Helper to simulate reading process
        protected void _read()
        {
            // To be overridden by subclasses
        }
    }
}
