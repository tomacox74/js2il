using System;

namespace JavaScriptRuntime.Node
{
    [NodeModule("https")]
    public sealed class Https
    {
        public object createServer(object[] args) => throw CreateNotSupportedError();

        public object request(object[] args) => throw CreateNotSupportedError();

        public object get(object[] args) => throw CreateNotSupportedError();

        private static Error CreateNotSupportedError()
            => new("node:https is not implemented yet. The current networking baseline only supports non-TLS loopback flows via node:http and node:net.");
    }

    [NodeModule("tls")]
    public sealed class Tls
    {
        public object createServer(object[] args) => throw CreateNotSupportedError();

        public object connect(object[] args) => throw CreateNotSupportedError();

        public object createSecureContext(object? options = null) => throw CreateNotSupportedError();

        private static Error CreateNotSupportedError()
            => new("node:tls is not implemented yet. TLS handshakes and secure sockets are deferred beyond the current networking baseline.");
    }
}
