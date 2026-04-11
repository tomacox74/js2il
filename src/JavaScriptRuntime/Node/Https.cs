using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace JavaScriptRuntime.Node
{
    [NodeModule("https")]
    public sealed class Https
    {
        public Type IncomingMessage => typeof(HttpIncomingMessage);

        public Type ServerResponse => typeof(HttpServerResponse);

        public Type ClientRequest => typeof(HttpClientRequest);

        public Type Server => typeof(HttpsServer);

        public HttpsServer createServer(object[] args)
        {
            var sourceArgs = args ?? System.Array.Empty<object>();
            object? options = null;
            Delegate? requestListener = null;

            if (sourceArgs.Length > 0)
            {
                if (sourceArgs[0] is Delegate listener)
                {
                    requestListener = listener;
                }
                else
                {
                    options = sourceArgs[0];
                    if (sourceArgs.Length > 1 && sourceArgs[1] is Delegate nextListener)
                    {
                        requestListener = nextListener;
                    }
                }
            }

            var server = new HttpsServer(options);
            if (requestListener != null)
            {
                server.on("request", requestListener);
            }

            return server;
        }

        public HttpClientRequest request(object[] args)
        {
            var sourceArgs = args ?? System.Array.Empty<object>();
            var options = HttpRequestOptions.Parse(
                sourceArgs,
                defaultMethod: "GET",
                scheme: "https",
                defaultPort: 443,
                moduleName: "node:https");

            ValidateAgent(options.Agent);

            var tlsOptions = TlsOptionParser.ParseRequestClientOptions(sourceArgs, options, "node:https");
            options.SocketFactory = () => new TlsSocket(tlsOptions);
            options.Agent = false;

            var request = new HttpClientRequest(options);
            if (options.Callback != null)
            {
                request.on("response", options.Callback);
            }

            return request;
        }

        public HttpClientRequest get(object[] args)
        {
            var request = this.request(args);
            request.end();
            return request;
        }

        private static void ValidateAgent(object? agent)
        {
            if (agent == null || agent is JsNull)
            {
                return;
            }

            if (agent is bool booleanAgent)
            {
                if (!booleanAgent)
                {
                    return;
                }

                throw new TypeError("node:https request options currently support only agent=false or omitted agent settings.");
            }

            throw new TypeError("node:https request options currently support only agent=false or omitted agent settings.");
        }
    }

    [NodeModule("tls")]
    public sealed class Tls
    {
        public Type TLSSocket => typeof(TlsSocket);

        public Type Server => typeof(TlsServer);

        public Type SecureContext => typeof(TlsSecureContext);

        public TlsServer createServer(object[] args)
        {
            var sourceArgs = args ?? System.Array.Empty<object>();
            object? options = null;
            Delegate? secureConnectionListener = null;

            if (sourceArgs.Length > 0)
            {
                if (sourceArgs[0] is Delegate listener)
                {
                    secureConnectionListener = listener;
                }
                else
                {
                    options = sourceArgs[0];
                    if (sourceArgs.Length > 1 && sourceArgs[1] is Delegate nextListener)
                    {
                        secureConnectionListener = nextListener;
                    }
                }
            }

            var server = new TlsServer(options);
            if (secureConnectionListener != null)
            {
                server.on("secureConnection", secureConnectionListener);
            }

            return server;
        }

        public TlsSocket connect(object[] args)
        {
            var sourceArgs = args ?? System.Array.Empty<object>();
            var options = TlsOptionParser.ParseConnectClientOptions(sourceArgs, "node:tls");
            var socket = new TlsSocket(options);
            if (options.Callback != null)
            {
                socket.once("secureConnect", options.Callback);
            }

            socket.connect(new object[] { (double)options.Port, options.Host });
            return socket;
        }

        public TlsSecureContext createSecureContext(object? options = null)
            => TlsOptionParser.CreateSecureContext(options, "node:tls");
    }

    public sealed class HttpsServer : EventEmitter
    {
        private readonly TlsServer _server;

        public HttpsServer(object? options = null)
        {
            _server = new TlsServer(options);
            _server.on("listening", (Func<object[], object?[], object?>)((scopes, args) =>
            {
                emit("listening");
                return null;
            }));
            _server.on("close", (Func<object[], object?[], object?>)((scopes, args) =>
            {
                emit("close");
                return null;
            }));
            _server.on("error", (Func<object[], object?[], object?>)((scopes, args) =>
            {
                emit("error", args.Length > 0 ? args[0] : new Error("HTTPS server error."));
                return null;
            }));
            _server.on("connection", (Func<object[], object?[], object?>)((scopes, args) =>
            {
                if (args.Length > 0 && args[0] is NetSocket socket)
                {
                    emit("connection", socket);
                    _ = new HttpServer.HttpServerConnectionState(this, socket, "node:https");
                }

                return null;
            }));
            _server.on("secureConnection", (Func<object[], object?[], object?>)((scopes, args) =>
            {
                if (args.Length > 0)
                {
                    emit("secureConnection", args[0]);
                }

                return null;
            }));
        }

        public bool listening => _server.listening;

        public HttpsServer listen(object[] args)
        {
            _server.listen(args);
            return this;
        }

        public object? address() => _server.address();

        public HttpsServer close()
        {
            _server.close();
            return this;
        }

        public HttpsServer close(object? callback)
        {
            _server.close(callback);
            return this;
        }
    }

    public sealed class TlsServer : EventEmitter
    {
        private readonly NetServer _server;

        public TlsServer(object? options = null)
        {
            var parsedOptions = TlsOptionParser.ParseServerOptions(options, "node:tls");
            _server = new NetServer(options, () => new TlsSocket(new TlsAcceptedSocketOptions(parsedOptions.SecureContext, parsedOptions.AllowHalfOpen)));
            _server.on("listening", (Func<object[], object?[], object?>)((scopes, args) =>
            {
                emit("listening");
                return null;
            }));
            _server.on("close", (Func<object[], object?[], object?>)((scopes, args) =>
            {
                emit("close");
                return null;
            }));
            _server.on("error", (Func<object[], object?[], object?>)((scopes, args) =>
            {
                emit("error", args.Length > 0 ? args[0] : new Error("TLS server error."));
                return null;
            }));
            _server.on("connection", (Func<object[], object?[], object?>)((scopes, args) =>
            {
                if (args.Length > 0)
                {
                    emit("connection", args[0]);
                    emit("secureConnection", args[0]);
                }

                return null;
            }));
        }

        public bool listening => _server.listening;

        public TlsServer listen(object[] args)
        {
            _server.listen(args);
            return this;
        }

        public object? address() => _server.address();

        public TlsServer close()
        {
            _server.close();
            return this;
        }

        public TlsServer close(object? callback)
        {
            _server.close(callback);
            return this;
        }
    }

    public sealed class TlsSecureContext
    {
        internal TlsSecureContext(X509Certificate2? serverCertificate)
        {
            ServerCertificate = serverCertificate;
        }

        internal X509Certificate2? ServerCertificate { get; }
    }

    public sealed class TlsSocket : NetSocket
    {
        private readonly TlsClientSocketOptions? _clientOptions;
        private readonly TlsAcceptedSocketOptions? _acceptedOptions;
        private SslStream? _sslStream;
        private string? _authorizationError;

        internal TlsSocket(TlsClientSocketOptions options)
            : base(options.AllowHalfOpen)
        {
            _clientOptions = options;
            authorized = false;
            once("connect", (Func<object[], object?[], object?>)((scopes, args) =>
            {
                emit("secureConnect");
                return null;
            }));
        }

        internal TlsSocket(TlsAcceptedSocketOptions options)
            : base(options.AllowHalfOpen)
        {
            _acceptedOptions = options;
            authorized = true;
        }

        public bool encrypted => true;

        public bool authorized { get; private set; }

        public object? authorizationError => _authorizationError;

        protected override async System.Threading.Tasks.Task<System.IO.Stream> CreateClientStreamAsync(TcpClient client, string host, int port)
        {
            if (_clientOptions == null)
            {
                throw new InvalidOperationException("TLS client options were not configured.");
            }

            var sslStream = new SslStream(client.GetStream(), leaveInnerStreamOpen: false, ValidateRemoteCertificate);
            _sslStream = sslStream;

            try
            {
                var authOptions = new SslClientAuthenticationOptions
                {
                    TargetHost = string.IsNullOrWhiteSpace(_clientOptions.ServerName) ? host : _clientOptions.ServerName,
                    EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
                    CertificateRevocationCheckMode = X509RevocationMode.NoCheck,
                    AllowRenegotiation = false,
                };

                await sslStream.AuthenticateAsClientAsync(authOptions).ConfigureAwait(false);
                return sslStream;
            }
            catch (Exception ex)
            {
                try
                {
                    sslStream.Dispose();
                }
                catch
                {
                }

                throw ex as Error ?? new Error("TLS client handshake failed.", ex);
            }
        }

        protected override async System.Threading.Tasks.Task<System.IO.Stream> CreateAcceptedStreamAsync(TcpClient client)
        {
            if (_acceptedOptions?.SecureContext.ServerCertificate == null)
            {
                throw new Error("TLS server configuration requires a certificate in the current baseline.");
            }

            var sslStream = new SslStream(client.GetStream(), leaveInnerStreamOpen: false);
            _sslStream = sslStream;

            try
            {
                var authOptions = new SslServerAuthenticationOptions
                {
                    ServerCertificate = _acceptedOptions.SecureContext.ServerCertificate,
                    ClientCertificateRequired = false,
                    EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
                    CertificateRevocationCheckMode = X509RevocationMode.NoCheck,
                    AllowRenegotiation = false,
                };

                await sslStream.AuthenticateAsServerAsync(authOptions).ConfigureAwait(false);
                authorized = true;
                _authorizationError = null;
                return sslStream;
            }
            catch (Exception ex)
            {
                try
                {
                    sslStream.Dispose();
                }
                catch
                {
                }

                throw ex as Error ?? new Error("TLS server handshake failed.", ex);
            }
        }

        protected override void ShutdownSend()
        {
            if (_sslStream != null)
            {
                try
                {
                    _sslStream.ShutdownAsync().GetAwaiter().GetResult();
                    return;
                }
                catch
                {
                }
            }

            base.ShutdownSend();
        }

        private bool ValidateRemoteCertificate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        {
            var isAuthorized = sslPolicyErrors == SslPolicyErrors.None && certificate != null;
            authorized = isAuthorized;
            _authorizationError = isAuthorized ? null : (certificate == null ? "Remote certificate was not provided." : sslPolicyErrors.ToString());

            return isAuthorized || _clientOptions?.RejectUnauthorized == false;
        }
    }

    internal sealed class TlsServerOptions
    {
        public required TlsSecureContext SecureContext { get; init; }

        public required bool AllowHalfOpen { get; init; }
    }

    internal sealed class TlsAcceptedSocketOptions
    {
        public TlsAcceptedSocketOptions(TlsSecureContext secureContext, bool allowHalfOpen)
        {
            SecureContext = secureContext;
            AllowHalfOpen = allowHalfOpen;
        }

        public TlsSecureContext SecureContext { get; }

        public bool AllowHalfOpen { get; }
    }

    internal sealed class TlsClientSocketOptions
    {
        public string Host { get; set; } = "127.0.0.1";

        public int Port { get; set; } = 443;

        public bool RejectUnauthorized { get; set; } = true;

        public bool AllowHalfOpen { get; set; }

        public string? ServerName { get; set; }

        public Delegate? Callback { get; set; }
    }

    internal static class TlsOptionParser
    {
        internal static TlsSecureContext CreateSecureContext(object? options, string moduleName)
        {
            EnsureOptionsNotSupported(
                options,
                moduleName,
                "secure context",
                "secureContext",
                "pfx",
                "passphrase",
                "ca",
                "crl",
                "ciphers",
                "ALPNProtocols",
                "SNICallback",
                "requestCert",
                "rejectUnauthorized");

            var key = NodeNetworkingCommon.TryGetOption(options, "key");
            var cert = NodeNetworkingCommon.TryGetOption(options, "cert");
            if (key == null || key is JsNull || cert == null || cert is JsNull)
            {
                if ((key != null && key is not JsNull) || (cert != null && cert is not JsNull))
                {
                    throw new Error($"{moduleName} secure contexts require both key and cert when either option is provided in the current baseline.");
                }

                return new TlsSecureContext(serverCertificate: null);
            }

            var keyPem = CoercePemOptionText(key, "key", moduleName);
            var certPem = CoercePemOptionText(cert, "cert", moduleName);
            return new TlsSecureContext(CreateServerCertificate(certPem, keyPem, moduleName));
        }

        internal static TlsServerOptions ParseServerOptions(object? options, string moduleName)
        {
            EnsureOptionsNotSupported(
                options,
                moduleName,
                "server options",
                "ca",
                "pfx",
                "passphrase",
                "requestCert",
                "rejectUnauthorized",
                "ALPNProtocols",
                "SNICallback");

            var secureContextOption = NodeNetworkingCommon.TryGetOption(options, "secureContext");
            if (secureContextOption != null && secureContextOption is not JsNull
                && (NodeNetworkingCommon.TryGetOption(options, "key") != null || NodeNetworkingCommon.TryGetOption(options, "cert") != null))
            {
                throw new Error($"{moduleName} server options currently support either secureContext or direct key/cert material, but not both together.");
            }

            TlsSecureContext secureContext;
            if (secureContextOption == null || secureContextOption is JsNull)
            {
                secureContext = CreateSecureContext(options, moduleName);
            }
            else if (secureContextOption is TlsSecureContext existingSecureContext)
            {
                secureContext = existingSecureContext;
            }
            else
            {
                throw new TypeError($"{moduleName} server option secureContext must be a tls.SecureContext created by tls.createSecureContext().");
            }

            if (secureContext.ServerCertificate == null)
            {
                throw new Error($"{moduleName} server options require key/cert material or a secureContext with a certificate in the current baseline.");
            }

            return new TlsServerOptions
            {
                SecureContext = secureContext,
                AllowHalfOpen = NodeNetworkingCommon.CoerceBoolean(NodeNetworkingCommon.TryGetOption(options, "allowHalfOpen"), defaultValue: false),
            };
        }

        internal static TlsClientSocketOptions ParseConnectClientOptions(object[] args, string moduleName)
        {
            var result = new TlsClientSocketOptions();
            object? optionsObject = null;
            var sourceArgs = args ?? System.Array.Empty<object>();

            if (sourceArgs.Length > 0)
            {
                var first = sourceArgs[0];
                var second = sourceArgs.Length > 1 ? sourceArgs[1] : null;
                var third = sourceArgs.Length > 2 ? sourceArgs[2] : null;

                if (NodeNetworkingCommon.LooksLikeOptionsObject(first))
                {
                    optionsObject = first;
                    result.Callback = second as Delegate;
                }
                else
                {
                    result.Port = NodeNetworkingCommon.CoercePort(first, defaultValue: 443);
                    if (second is string hostText)
                    {
                        result.Host = NodeNetworkingCommon.CoerceHost(hostText);
                        if (third is Delegate thirdCallback)
                        {
                            result.Callback = thirdCallback;
                        }
                    }
                    else if (NodeNetworkingCommon.LooksLikeOptionsObject(second))
                    {
                        optionsObject = second;
                        result.Callback = third as Delegate;
                    }
                    else
                    {
                        result.Callback = second as Delegate;
                    }
                }
            }

            ApplyClientOptions(result, optionsObject, moduleName, allowSecureContext: false);
            result.Host = NodeNetworkingCommon.CoerceHost(result.Host);
            return result;
        }

        internal static TlsClientSocketOptions ParseRequestClientOptions(object[] args, HttpRequestOptions requestOptions, string moduleName)
        {
            var result = new TlsClientSocketOptions
            {
                Host = requestOptions.Host,
                Port = requestOptions.Port,
            };

            ApplyClientOptions(result, GetRequestOptionSource(args), moduleName, allowSecureContext: false);
            result.Host = NodeNetworkingCommon.CoerceHost(result.Host);
            if (string.IsNullOrWhiteSpace(result.ServerName))
            {
                result.ServerName = result.Host;
            }

            return result;
        }

        private static void ApplyClientOptions(TlsClientSocketOptions result, object? options, string moduleName, bool allowSecureContext)
        {
            if (options == null || options is JsNull)
            {
                if (string.IsNullOrWhiteSpace(result.ServerName))
                {
                    result.ServerName = result.Host;
                }

                return;
            }

            EnsureOptionsNotSupported(
                options,
                moduleName,
                "client options",
                "ca",
                "key",
                "cert",
                "pfx",
                "passphrase",
                "checkServerIdentity",
                "ALPNProtocols");

            var secureContext = NodeNetworkingCommon.TryGetOption(options, "secureContext");
            if (!allowSecureContext && secureContext != null && secureContext is not JsNull)
            {
                throw new Error($"{moduleName} client options do not yet support secureContext in the current baseline.");
            }

            var host = NodeNetworkingCommon.TryGetStringOption(options, "host")
                ?? NodeNetworkingCommon.TryGetStringOption(options, "hostname");
            if (!string.IsNullOrWhiteSpace(host))
            {
                result.Host = host!;
            }

            var port = NodeNetworkingCommon.TryGetOption(options, "port");
            if (port != null && port is not JsNull)
            {
                result.Port = NodeNetworkingCommon.CoercePort(port, defaultValue: result.Port);
            }

            result.RejectUnauthorized = NodeNetworkingCommon.CoerceBoolean(
                NodeNetworkingCommon.TryGetOption(options, "rejectUnauthorized"),
                defaultValue: true);
            result.AllowHalfOpen = NodeNetworkingCommon.CoerceBoolean(
                NodeNetworkingCommon.TryGetOption(options, "allowHalfOpen"),
                defaultValue: result.AllowHalfOpen);

            var serverName = NodeNetworkingCommon.TryGetStringOption(options, "servername");
            if (!string.IsNullOrWhiteSpace(serverName))
            {
                result.ServerName = serverName;
            }
            else if (string.IsNullOrWhiteSpace(result.ServerName))
            {
                result.ServerName = result.Host;
            }
        }

        private static object? GetRequestOptionSource(object[] args)
        {
            var sourceArgs = args ?? System.Array.Empty<object>();
            if (sourceArgs.Length == 0)
            {
                return null;
            }

            var first = sourceArgs[0];
            if (first is string || first is URL)
            {
                if (sourceArgs.Length > 1
                    && sourceArgs[1] is not URL
                    && NodeNetworkingCommon.LooksLikeOptionsObject(sourceArgs[1]))
                {
                    return sourceArgs[1];
                }

                return null;
            }

            return NodeNetworkingCommon.LooksLikeOptionsObject(first) ? first : null;
        }

        private static string CoercePemOptionText(object? value, string optionName, string moduleName)
        {
            if (value is string text)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    throw new TypeError($"{moduleName} option '{optionName}' must be a non-empty PEM string or Buffer.");
                }

                return text;
            }

            if (value is Buffer buffer)
            {
                var textValue = Encoding.UTF8.GetString(buffer.ToByteArray());
                if (string.IsNullOrWhiteSpace(textValue))
                {
                    throw new TypeError($"{moduleName} option '{optionName}' must be a non-empty PEM string or Buffer.");
                }

                return textValue;
            }

            if (value is byte[] bytes)
            {
                var textValue = Encoding.UTF8.GetString(bytes);
                if (string.IsNullOrWhiteSpace(textValue))
                {
                    throw new TypeError($"{moduleName} option '{optionName}' must be a non-empty PEM string or Buffer.");
                }

                return textValue;
            }

            throw new TypeError($"{moduleName} option '{optionName}' must be a PEM string or Buffer.");
        }

        private static X509Certificate2 CreateServerCertificate(string certPem, string keyPem, string moduleName)
        {
            try
            {
                using var certificate = X509Certificate2.CreateFromPem(certPem, keyPem);
                var pfx = certificate.Export(X509ContentType.Pfx);
                return X509CertificateLoader.LoadPkcs12(
                    pfx,
                    (string?)null,
                    X509KeyStorageFlags.Exportable | X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.PersistKeySet);
            }
            catch (Exception ex)
            {
                throw new Error($"{moduleName} could not parse the provided PEM certificate/key material.", ex);
            }
        }

        private static void EnsureOptionsNotSupported(object? options, string moduleName, string optionGroupDescription, params string[] unsupportedNames)
        {
            if (options == null || options is JsNull)
            {
                return;
            }

            foreach (var name in unsupportedNames)
            {
                var value = NodeNetworkingCommon.TryGetOption(options, name);
                if (value != null && value is not JsNull)
                {
                    throw new Error($"{moduleName} {optionGroupDescription} do not yet support '{name}' in the current baseline.");
                }
            }
        }

    }
}
