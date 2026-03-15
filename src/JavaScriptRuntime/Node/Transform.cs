using System;

namespace JavaScriptRuntime.Node
{
    public class Transform : Duplex
    {
        private bool _readEndedPushed = false;

        public object? _transform = null;

        // Constructor for subclassing
        public Transform() { }

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
            try
            {
                push(null);
            }
            catch when (destroyed)
            {
            }
        }

        public override void end()
        {
            base.end();
            EndReadableSide();
        }

        public override void end(object? chunk)
        {
            base.end(chunk);
            EndReadableSide();
        }

        public override void end(object? chunk, object? callback)
        {
            base.end(chunk, callback);
            EndReadableSide();
        }
    }
}
