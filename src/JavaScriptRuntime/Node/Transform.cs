using System;

namespace JavaScriptRuntime.Node
{
    public class Transform : Duplex
    {
        private bool _readEndedPushed = false;
        private bool _readEndQueued = false;

        public object? _transform = null;

        // Constructor for subclassing
        public Transform(object? options = null) : base(options) { }

        protected override void InvokeWrite(object? chunk)
        {
            if (_transform != null && _transform is Delegate transformFunc)
            {
                try
                {
                    var previousThis = RuntimeServices.SetCurrentThis(this);
                    try
                    {
                        Closure.InvokeWithArgs(transformFunc, System.Array.Empty<object>(), new[] { chunk });
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
            else
            {
                // Default transform behavior is identity.
                push(chunk);
            }
        }

        private void EndReadableSide()
        {
            if (_readEndedPushed)
            {
                return;
            }

            _readEndedPushed = true;
            _readEndQueued = false;
            try
            {
                push(null);
            }
            catch when (destroyed)
            {
            }
        }

        private void QueueReadableEndAfterFinish()
        {
            if (_readEndedPushed || _readEndQueued || destroyed)
            {
                return;
            }

            _readEndQueued = true;
            once("finish", new Func<object[], object?[], object?>((_, _) =>
            {
                EndReadableSide();
                return null;
            }));
        }

        public override void end()
        {
            QueueReadableEndAfterFinish();
            base.end();
        }

        public override void end(object? chunk)
        {
            QueueReadableEndAfterFinish();
            base.end(chunk);
        }

        public override void end(object? chunk, object? callback)
        {
            QueueReadableEndAfterFinish();
            base.end(chunk, callback);
        }
    }
}
