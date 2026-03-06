using System;
using System.Collections.Generic;

namespace JavaScriptRuntime.Node
{
    public class Readable : EventEmitter
    {
        private readonly Queue<object?> _buffer = new();
        private bool _ended = false;
        private bool _endEmitted = false;
        private int _pipePauseCount = 0;

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

            if (_pipePauseCount > 0 || listenerCount("data") == 0)
            {
                _buffer.Enqueue(chunk);
            }
            else
            {
                emit("data", chunk);
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

            bool paused = false;
            bool drainAttached = false;
            Func<object[], object?[], object?>? onData = null;
            Func<object[], object?[], object?>? onEnd = null;
            Func<object[], object?[], object?>? onError = null;
            Func<object[], object?[], object?>? onDrain = null;

            void Pause()
            {
                if (paused) return;
                paused = true;
                _pipePauseCount++;
            }

            void Resume()
            {
                if (!paused)
                {
                    FlushBuffered();
                    return;
                }

                paused = false;
                if (_pipePauseCount > 0) _pipePauseCount--;
                FlushBuffered();
            }

            void Cleanup()
            {
                if (onData != null) off("data", onData);
                if (onEnd != null) off("end", onEnd);
                if (onError != null) off("error", onError);

                if (drainAttached && onDrain != null)
                {
                    drainAttached = false;
                    writable.off("drain", onDrain);
                }

                if (paused)
                {
                    paused = false;
                    if (_pipePauseCount > 0) _pipePauseCount--;
                }
            }

            void FlushBuffered()
            {
                while (!paused && _buffer.Count > 0)
                {
                    var chunk = _buffer.Dequeue();
                    var canContinue = writable.write(chunk);
                    if (!canContinue)
                    {
                        Pause();
                        if (!drainAttached && onDrain != null)
                        {
                            drainAttached = true;
                            writable.on("drain", onDrain);
                        }
                        break;
                    }
                }

                EmitEndIfReady();
            }

            onDrain = (scopes, args) =>
            {
                if (drainAttached)
                {
                    drainAttached = false;
                    writable.off("drain", onDrain);
                }

                Resume();
                return null;
            };

            onData = (scopes, args) =>
            {
                if (args.Length > 0)
                {
                    var canContinue = writable.write(args[0]);
                    if (!canContinue)
                    {
                        Pause();
                        if (!drainAttached)
                        {
                            drainAttached = true;
                            writable.on("drain", onDrain);
                        }
                    }
                }
                return null;
            };

            onEnd = (scopes, args) =>
            {
                Cleanup();
                writable.end();
                return null;
            };

            onError = (scopes, args) =>
            {
                var err = args.Length > 0 ? args[0] : null;
                Cleanup();
                if (err != null)
                {
                    writable.emit("error", err);
                }
                return null;
            };

            on("data", onData);
            on("end", onEnd);
            on("error", onError);

            // Flush any pre-buffered chunks now that piping has begun.
            FlushBuffered();

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
