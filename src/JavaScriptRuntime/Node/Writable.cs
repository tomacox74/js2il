using System;
using System.Collections.Generic;

namespace JavaScriptRuntime.Node
{
    public class Writable : EventEmitter
    {
        private const int DefaultHighWaterMark = 16;

        private readonly Queue<object?> _buffer = new();
        private bool _ended;
        private bool _writing;
        private bool _needDrain;
        private bool _destroyed;
        private bool _finishQueued;
        private bool _finishEmitted;
        private bool _closeEmitted;

        public double highWaterMark = DefaultHighWaterMark;

        public bool writable => !_ended && !_destroyed;

        public virtual bool destroyed => _destroyed;

        public object? _write = null;

        public bool writableObjectMode { get; }

        public Writable(object? options = null)
        {
            writableObjectMode = ResolveObjectMode(options, "writableObjectMode");

            var configuredHighWaterMark = NodeNetworkingCommon.TryGetOption(options, "highWaterMark");
            if (configuredHighWaterMark != null && configuredHighWaterMark is not JsNull)
            {
                highWaterMark = CoerceHighWaterMark(configuredHighWaterMark);
            }
        }

        public bool write(object? chunk)
        {
            if (_destroyed)
            {
                emit("error", new Error("Cannot write after destroy"));
                return false;
            }

            if (_ended)
            {
                throw new Error("Cannot write after end");
            }

            _buffer.Enqueue(chunk);

            var threshold = ResolveHighWaterMark();
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
                    DrainBufferedWrites();
                }
                finally
                {
                    _writing = false;
                    FinalizeWritableState();
                }
            }

            return !_destroyed && canAcceptMore;
        }

        public bool write(object? chunk, object? encoding)
        {
            return write(chunk);
        }

        public bool write(object? chunk, object? encoding, object? callback)
        {
            return write(chunk);
        }

        public virtual void end()
        {
            if (_destroyed)
            {
                return;
            }

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
                    DrainBufferedWrites();
                }
                finally
                {
                    _writing = false;
                    FinalizeWritableState();
                }
            }
            else
            {
                FinalizeWritableState();
            }
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
            if (callback is Delegate del)
            {
                once("finish", del);
            }

            end(chunk);
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
            _needDrain = false;
            _buffer.Clear();

            NodeNetworkingCommon.ScheduleOnEventLoop(null, () =>
            {
                if (error != null && error is not JsNull)
                {
                    emit("error", error);
                }

                EmitClose();
            });
        }

        protected virtual void InvokeWrite(object? chunk)
        {
            if (_write is not Delegate writeFunc)
            {
                return;
            }

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
            catch (Exception ex)
            {
                destroy(ex as Error ?? new Error(ex.Message, ex));
            }
        }

        private void DrainBufferedWrites()
        {
            while (_buffer.Count > 0 && !_destroyed)
            {
                var chunk = _buffer.Dequeue();
                InvokeWrite(chunk);
            }
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

        private void EmitDrainAsync()
        {
            NodeNetworkingCommon.ScheduleOnEventLoop(null, () =>
            {
                if (_destroyed)
                {
                    return;
                }

                emit("drain");
            });
        }

        private void EmitFinishAsync()
        {
            if (_destroyed || _finishQueued || _finishEmitted)
            {
                return;
            }

            _finishQueued = true;
            NodeNetworkingCommon.ScheduleOnEventLoop(null, () =>
            {
                _finishQueued = false;
                if (_destroyed || _finishEmitted)
                {
                    return;
                }

                _finishEmitted = true;
                emit("finish");
            });
        }

        private void FinalizeWritableState()
        {
            if (_destroyed)
            {
                return;
            }

            if (_needDrain && _buffer.Count == 0)
            {
                _needDrain = false;
                EmitDrainAsync();
            }

            if (_ended && _buffer.Count == 0)
            {
                EmitFinishAsync();
            }
        }

        private int ResolveHighWaterMark()
        {
            try
            {
                var threshold = (int)JavaScriptRuntime.TypeUtilities.ToNumber(highWaterMark);
                return threshold < 1 ? 1 : threshold;
            }
            catch
            {
                return DefaultHighWaterMark;
            }
        }

        private static double CoerceHighWaterMark(object? value)
        {
            try
            {
                var threshold = JavaScriptRuntime.TypeUtilities.ToNumber(value);
                return threshold < 1 || double.IsNaN(threshold) || double.IsInfinity(threshold)
                    ? DefaultHighWaterMark
                    : threshold;
            }
            catch
            {
                return DefaultHighWaterMark;
            }
        }

        private static bool ResolveObjectMode(object? options, string specificOptionName)
        {
            return TypeUtilities.ToBoolean(NodeNetworkingCommon.TryGetOption(options, "objectMode"))
                || TypeUtilities.ToBoolean(NodeNetworkingCommon.TryGetOption(options, specificOptionName));
        }
    }
}
