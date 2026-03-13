using System;

namespace JavaScriptRuntime
{
    // Minimal Proxy implementation supporting the traps needed by domino:
    // - get
    // - set
    // - has
    [IntrinsicObject("Proxy")]
    public sealed class Proxy
    {
        internal object Target { get; }
        internal object Handler { get; }

        public Proxy(object target, object handler)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }
    }
}
