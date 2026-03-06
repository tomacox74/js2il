using System;
using System.Collections.Generic;

namespace JavaScriptRuntime.Node
{
    public class Duplex : Writable
    {
        private readonly Queue<object?> _readBuffer = new();
        private bool _readEnded = false;
        private bool _readEndEmitted = false;

        public bool readable => !_readEnded;

        // Constructor for subclassing
        public Duplex() { }

        public bool push(object? chunk)
        {
            if (_readEnded)
            {
                throw new Error("Cannot push after EOF");
            }

            if (chunk == null || chunk is JsNull)
            {
                _readEnded = true;
                EmitEndIfReady();
                return false;
            }

            if (listenerCount("data") > 0)
            {
                emit("data", chunk);
            }
            else
            {
                _readBuffer.Enqueue(chunk);
            }

            return true;
        }

        public object? read()
        {
            if (_readBuffer.Count > 0)
            {
                var value = _readBuffer.Dequeue();
                EmitEndIfReady();
                return value;
            }

            if (_readEnded)
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

        public Writable pipe(object? destination)
        {
            if (destination is not Writable writable)
            {
                throw new TypeError("Pipe destination must be a Writable stream");
            }

            bool paused = false;
            Func<object[], object?[], object?>? onData = null;
            Func<object[], object?[], object?>? onEnd = null;
            Func<object[], object?[], object?>? onError = null;
            Func<object[], object?[], object?>? onDrain = null;

            void Pause()
            {
                if (paused) return;
                paused = true;
                if (onData != null) off("data", onData);
            }

            void FlushBuffered()
            {
                while (!paused && _readBuffer.Count > 0)
                {
                    var chunk = _readBuffer.Dequeue();
                    var canContinue = writable.write(chunk);
                    if (!canContinue)
                    {
                        Pause();
                        if (onDrain != null)
                        {
                            writable.once("drain", onDrain);
                        }
                        break;
                    }
                }

                EmitEndIfReady();
            }

            void Resume()
            {
                if (!paused)
                {
                    FlushBuffered();
                    return;
                }

                paused = false;
                if (onData != null) on("data", onData);
                FlushBuffered();
            }

            onDrain = (scopes, args) =>
            {
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
                        writable.once("drain", onDrain);
                    }
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

            // Flush any pre-buffered chunks now that piping has begun.
            FlushBuffered();

            return writable;
        }

        private void EmitEndIfReady()
        {
            if (_readEnded && !_readEndEmitted && _readBuffer.Count == 0)
            {
                _readEndEmitted = true;
                emit("end");
            }
        }

        protected void _read()
        {
            // To be overridden by subclasses
        }
    }
}
