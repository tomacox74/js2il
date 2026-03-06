using System;
using System.Collections.Generic;

namespace JavaScriptRuntime.Node
{
    public class Duplex : Writable
    {
        private readonly Queue<object?> _readBuffer = new();
        private bool _readEnded = false;
        private bool _readEndEmitted = false;
        private int _pipePauseCount = 0;

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

            if (_pipePauseCount > 0 || listenerCount("data") == 0)
            {
                _readBuffer.Enqueue(chunk);
            }
            else
            {
                emit("data", chunk);
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
                while (!paused && _readBuffer.Count > 0)
                {
                    var chunk = _readBuffer.Dequeue();
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
