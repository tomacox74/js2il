using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace JavaScriptRuntime.Node
{
    [NodeModule("http")]
    public sealed class Http
    {
        private readonly HttpAgent _globalAgent = new();

        public Type IncomingMessage => typeof(HttpIncomingMessage);

        public Type ServerResponse => typeof(HttpServerResponse);

        public Type ClientRequest => typeof(HttpClientRequest);

        public Type Server => typeof(HttpServer);

        public Type Agent => typeof(HttpAgent);

        public HttpAgent globalAgent => _globalAgent;

        public HttpServer createServer(object[] args)
        {
            var server = new HttpServer();
            var srcArgs = args ?? System.Array.Empty<object>();
            if (srcArgs.Length > 0 && srcArgs[0] is Delegate requestListener)
            {
                server.on("request", requestListener);
            }

            return server;
        }

        public HttpClientRequest request(object[] args)
        {
            var options = HttpRequestOptions.Parse(
                args ?? System.Array.Empty<object>(),
                defaultMethod: "GET",
                scheme: "http",
                defaultPort: 80,
                moduleName: "node:http");
            options.Agent ??= _globalAgent;

            if (options.Agent is bool agentBoolean)
            {
                if (agentBoolean)
                {
                    throw new TypeError("node:http request options only support agent=false or an http.Agent instance in the current runtime.");
                }
            }
            else if (options.Agent != null && options.Agent is not JsNull && options.Agent is not HttpAgent)
            {
                throw new TypeError("node:http request options only support agent=false or an http.Agent instance in the current runtime.");
            }

            var request = new HttpClientRequest(options);
            if (options.Callback != null)
            {
                request.on("response", options.Callback);
            }

            return request;
        }

        public HttpClientRequest get(object[] args)
        {
            var clientRequest = request(args);
            clientRequest.end();
            return clientRequest;
        }
    }

    public sealed class HttpAgent : EventEmitter
    {
        private readonly Dictionary<string, Queue<NetSocket>> _idleSockets = new(StringComparer.Ordinal);
        private readonly Dictionary<NetSocket, (Func<object[], object?[], object?> OnClose, Func<object[], object?[], object?> OnError)> _idleSocketHandlers = new();

        public HttpAgent()
            : this(null)
        {
        }

        public HttpAgent(object? options)
        {
            keepAlive = NodeNetworkingCommon.CoerceBoolean(
                NodeNetworkingCommon.TryGetOption(options, "keepAlive"),
                defaultValue: false);
        }

        public bool keepAlive { get; }

        internal NetSocket AcquireSocket(HttpRequestOptions options, out bool reused)
        {
            var originKey = CreateOriginKey(options);
            if (keepAlive && _idleSockets.TryGetValue(originKey, out var queue))
            {
                while (queue.Count > 0)
                {
                    var candidate = queue.Dequeue();
                    if (candidate.destroyed)
                    {
                        RemoveIdleHandlers(candidate);
                        continue;
                    }

                    RemoveIdleHandlers(candidate);
                    reused = true;
                    return candidate;
                }

                _idleSockets.Remove(originKey);
            }

            reused = false;
            var socket = new NetSocket();
            if (keepAlive)
            {
                socket.setKeepAlive(true);
            }

            return socket;
        }

        internal void ReleaseSocket(HttpRequestOptions options, NetSocket socket)
        {
            if (!keepAlive || socket.destroyed)
            {
                if (!socket.destroyed)
                {
                    socket.destroy();
                }

                return;
            }

            AttachIdleHandlers(socket);
            var originKey = CreateOriginKey(options);
            if (!_idleSockets.TryGetValue(originKey, out var queue))
            {
                queue = new Queue<NetSocket>();
                _idleSockets[originKey] = queue;
            }

            queue.Enqueue(socket);
            emit("free", socket, options.Host, (double)options.Port);
        }

        public void destroy()
        {
            foreach (var queue in _idleSockets.Values)
            {
                while (queue.Count > 0)
                {
                    var socket = queue.Dequeue();
                    RemoveIdleHandlers(socket);
                    if (!socket.destroyed)
                    {
                        socket.destroy();
                    }
                }
            }

            _idleSockets.Clear();
            _idleSocketHandlers.Clear();
        }

        private static string CreateOriginKey(HttpRequestOptions options)
            => $"{options.Host}:{options.Port.ToString(CultureInfo.InvariantCulture)}";

        private void AttachIdleHandlers(NetSocket socket)
        {
            if (_idleSocketHandlers.ContainsKey(socket))
            {
                return;
            }

            Func<object[], object?[], object?> onClose = (scopes, args) =>
            {
                RemoveIdleHandlers(socket);
                return null;
            };
            Func<object[], object?[], object?> onError = (scopes, args) =>
            {
                RemoveIdleHandlers(socket);
                if (!socket.destroyed)
                {
                    socket.destroy();
                }

                return null;
            };

            _idleSocketHandlers[socket] = (onClose, onError);
            socket.on("close", onClose);
            socket.on("error", onError);
        }

        private void RemoveIdleHandlers(NetSocket socket)
        {
            if (!_idleSocketHandlers.TryGetValue(socket, out var handlers))
            {
                return;
            }

            socket.off("close", handlers.OnClose);
            socket.off("error", handlers.OnError);
            _idleSocketHandlers.Remove(socket);
        }
    }

    public sealed class HttpServer : EventEmitter
    {
        private readonly NetServer _server;

        public HttpServer()
        {
            _server = new NetServer();
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
                emit("error", args.Length > 0 ? args[0] : new Error("HTTP server error."));
                return null;
            }));
            _server.on("connection", (Func<object[], object?[], object?>)((scopes, args) =>
            {
                if (args.Length > 0 && args[0] is NetSocket socket)
                {
                    emit("connection", socket);
                    _ = new HttpServerConnectionState(this, socket, "node:http");
                }

                return null;
            }));
        }

        public bool listening => _server.listening;

        public HttpServer listen(object[] args)
        {
            _server.listen(args);
            return this;
        }

        public object? address() => _server.address();

        public HttpServer close()
        {
            _server.close();
            return this;
        }

        public HttpServer close(object? callback)
        {
            _server.close(callback);
            return this;
        }

        internal sealed class HttpServerConnectionState
        {
            private readonly EventEmitter _server;
            private readonly NetSocket _socket;
            private readonly string _moduleName;
            private readonly HttpMessageStreamDecoder _decoder = new(HttpMessageKind.Request);
            private HttpIncomingMessage? _activeRequest;
            private HttpServerResponse? _activeResponse;
            private bool _closed;

            public HttpServerConnectionState(EventEmitter server, NetSocket socket, string moduleName)
            {
                _server = server;
                _socket = socket;
                _moduleName = moduleName;

                _socket.on("data", (Func<object[], object?[], object?>)((scopes, args) =>
                {
                    if (_closed || args.Length == 0)
                    {
                        return null;
                    }

                    var bytes = NodeNetworkingCommon.CoerceToBytes(args[0]);
                    _decoder.Append(bytes);
                    ProcessDecoderEvents();
                    return null;
                }));

                _socket.on("end", (Func<object[], object?[], object?>)((scopes, args) =>
                {
                    if (_closed)
                    {
                        return null;
                    }

                    _decoder.CompleteInput();
                    ProcessDecoderEvents();
                    return null;
                }));
            }

            private void ProcessDecoderEvents()
            {
                foreach (var messageEvent in _decoder.Drain())
                {
                    if (_closed)
                    {
                        return;
                    }

                    switch (messageEvent)
                    {
                        case HttpMessageStartEvent start:
                            HandleRequestStart(start.Head);
                            break;

                        case HttpMessageBodyEvent body:
                            _activeRequest?.AppendBodyChunk(body.Chunk);
                            break;

                        case HttpMessageEndEvent end:
                            _activeRequest?.CompleteBody();
                            _activeRequest = null;
                            _activeResponse?.MarkRequestComplete(end.CanReuseConnection);
                            break;

                        case HttpMessageErrorEvent error:
                            HandleDecoderError(error.Error);
                            return;
                    }
                }
            }

            private void HandleRequestStart(HttpParsedMessageHead head)
            {
                if (_activeResponse != null && !_activeResponse.Completed)
                {
                    HandleDecoderError(new Error("HTTP pipelining is not supported in the current runtime."));
                    return;
                }

                if (HttpRequestOptions.TryGetUnsupportedFeatureMessage(head.Method, head.Headers, _moduleName, out var unsupportedMessage))
                {
                    WriteSimpleErrorResponse(501, "Not Implemented", unsupportedMessage);
                    _closed = true;
                    return;
                }

                _activeRequest = HttpIncomingMessage.FromRequest(head, _socket);
                _activeResponse = new HttpServerResponse(_socket, head.CanKeepAlive, OnResponseCompleted);
                _server.emit("request", _activeRequest, _activeResponse);
            }

            private void OnResponseCompleted(HttpServerResponse response)
            {
                if (_closed)
                {
                    return;
                }

                if (!response.ShouldKeepAlive)
                {
                    _closed = true;
                    if (!_socket.destroyed)
                    {
                        _socket.end();
                    }
                }
            }

            private void HandleDecoderError(Error error)
            {
                _closed = true;
                _activeRequest?.destroy(error);
                _activeResponse?.destroy(error);
                if (!_socket.destroyed)
                {
                    _socket.destroy(error);
                }
            }

            private void WriteSimpleErrorResponse(int statusCode, string statusMessage, string body)
            {
                var bodyBytes = Encoding.UTF8.GetBytes(body);
                var builder = new StringBuilder();
                builder.Append("HTTP/1.1 ")
                    .Append(statusCode.ToString(CultureInfo.InvariantCulture))
                    .Append(' ')
                    .Append(statusMessage)
                    .Append("\r\n")
                    .Append("content-length: ")
                    .Append(bodyBytes.Length.ToString(CultureInfo.InvariantCulture))
                    .Append("\r\n")
                    .Append("content-type: text/plain; charset=utf-8\r\n")
                    .Append("connection: close\r\n")
                    .Append("\r\n");

                _socket.write(builder.ToString());
                if (bodyBytes.Length > 0)
                {
                    _socket.end(new Buffer(bodyBytes));
                }
                else
                {
                    _socket.end();
                }
            }
        }
    }

    public sealed class HttpIncomingMessage : Readable
    {
        private bool _bodyCompleted;

        private HttpIncomingMessage(
            string? method,
            string? url,
            double statusCode,
            string? statusMessage,
            string? httpVersion,
            JsObject headers,
            NetSocket socket)
        {
            this.method = method ?? string.Empty;
            this.url = url ?? string.Empty;
            this.statusCode = statusCode;
            this.statusMessage = statusMessage ?? string.Empty;
            this.httpVersion = httpVersion ?? "1.1";
            this.headers = headers;
            this.socket = socket;
        }

        public string method { get; }

        public string url { get; }

        public double statusCode { get; }

        public string statusMessage { get; }

        public string httpVersion { get; }

        public JsObject headers { get; }

        public NetSocket socket { get; }

        public bool complete { get; private set; }

        internal static HttpIncomingMessage FromRequest(HttpParsedMessageHead request, NetSocket socket)
        {
            return new HttpIncomingMessage(
                method: request.Method,
                url: request.Path,
                statusCode: 0,
                statusMessage: string.Empty,
                httpVersion: request.HttpVersion,
                headers: NodeNetworkingCommon.ToJsObject(request.Headers),
                socket: socket);
        }

        internal static HttpIncomingMessage FromResponse(HttpParsedMessageHead response, NetSocket socket)
        {
            return new HttpIncomingMessage(
                method: string.Empty,
                url: string.Empty,
                statusCode: response.StatusCode,
                statusMessage: response.StatusMessage,
                httpVersion: response.HttpVersion,
                headers: NodeNetworkingCommon.ToJsObject(response.Headers),
                socket: socket);
        }

        internal void AppendBodyChunk(byte[] bytes)
        {
            if (_bodyCompleted || bytes.Length == 0)
            {
                return;
            }

            push(new Buffer(bytes));
        }

        internal void CompleteBody()
        {
            if (_bodyCompleted)
            {
                return;
            }

            _bodyCompleted = true;
            complete = true;
            push(null);
        }
    }

    public sealed class HttpServerResponse : Writable
    {
        private readonly NetSocket _socket;
        private readonly Dictionary<string, string> _headers = new(StringComparer.OrdinalIgnoreCase);
        private readonly Action<HttpServerResponse> _onCompleted;
        private bool _requestAllowsKeepAlive;
        private bool _requestCompleted;
        private bool _completionNotified;
        private bool _headersSent;
        private bool _responseEnded;
        private bool _shouldKeepAlive;
        private bool _useChunkedEncoding;
        private int? _declaredContentLength;
        private int _bodyBytesWritten;

        internal HttpServerResponse(NetSocket socket, bool requestAllowsKeepAlive, Action<HttpServerResponse> onCompleted)
        {
            _socket = socket;
            _requestAllowsKeepAlive = requestAllowsKeepAlive;
            _onCompleted = onCompleted;
        }

        public double statusCode = 200;

        public string statusMessage = "OK";

        public bool headersSent => _headersSent;

        internal bool ShouldKeepAlive => _shouldKeepAlive;

        internal bool Completed => _requestCompleted && _responseEnded;

        public HttpServerResponse setHeader(object? name, object? value)
        {
            var headerName = NodeNetworkingCommon.NormalizeHeaderName(name);
            if (headerName.Length == 0)
            {
                throw new TypeError("Header name must be a non-empty string.");
            }

            _headers[headerName] = value?.ToString() ?? string.Empty;
            return this;
        }

        public object? getHeader(object? name)
        {
            var headerName = NodeNetworkingCommon.NormalizeHeaderName(name);
            return _headers.TryGetValue(headerName, out var value) ? value : null;
        }

        public HttpServerResponse writeHead(object? statusCode)
            => writeHead(statusCode, null, null);

        public HttpServerResponse writeHead(object? statusCode, object? arg1)
            => writeHead(statusCode, arg1, null);

        public HttpServerResponse writeHead(object? statusCode, object? arg1, object? arg2)
        {
            this.statusCode = NodeNetworkingCommon.CoerceHttpStatusCode(statusCode, defaultValue: 200);

            if (arg1 is string statusText)
            {
                statusMessage = statusText;
                ApplyHeaders(arg2);
            }
            else
            {
                ApplyHeaders(arg1);
            }

            return this;
        }

        protected override void InvokeWrite(object? chunk)
        {
            var bytes = NodeNetworkingCommon.CoerceToBytes(chunk);
            if (bytes.Length == 0)
            {
                return;
            }

            EnsureHeadersSent(ending: false);
            WriteBodyBytes(bytes);
        }

        public override void end()
        {
            base.end();
            CompleteResponse();
        }

        public override void end(object? chunk)
        {
            base.end(chunk);
            CompleteResponse();
        }

        public override void end(object? chunk, object? callback)
        {
            if (callback is Delegate del)
            {
                once("finish", del);
            }

            base.end(chunk, callback);
            CompleteResponse();
        }

        internal void MarkRequestComplete(bool requestCanKeepAlive)
        {
            _requestAllowsKeepAlive = requestCanKeepAlive;
            _requestCompleted = true;
            TryNotifyCompleted();
        }

        private void ApplyHeaders(object? headers)
        {
            foreach (var header in NodeNetworkingCommon.ToHeaderDictionary(headers))
            {
                _headers[header.Key] = header.Value;
            }
        }

        private void CompleteResponse()
        {
            if (_responseEnded)
            {
                return;
            }

            EnsureHeadersSent(ending: true);
            if (_useChunkedEncoding && AllowsResponseBody())
            {
                _socket.write(HttpWireParser.EncodeChunkTerminator());
            }

            if (_declaredContentLength.HasValue && _bodyBytesWritten != _declaredContentLength.Value)
            {
                throw new Error("ServerResponse body length does not match the declared content-length.");
            }

            _responseEnded = true;
            TryNotifyCompleted();
        }

        private void EnsureHeadersSent(bool ending)
        {
            if (_headersSent)
            {
                return;
            }

            if (string.IsNullOrEmpty(statusMessage))
            {
                statusMessage = NodeNetworkingCommon.GetStatusMessage((int)statusCode);
            }

            var bodyAllowed = AllowsResponseBody();
            ConfigureTransferSemantics(bodyAllowed, ending);
            _shouldKeepAlive = DetermineKeepAlive(bodyAllowed);
            _headers["connection"] = _shouldKeepAlive ? "keep-alive" : "close";

            var builder = new StringBuilder();
            builder.Append("HTTP/1.1 ")
                .Append(((int)statusCode).ToString(CultureInfo.InvariantCulture))
                .Append(' ')
                .Append(statusMessage)
                .Append("\r\n");
            foreach (var header in _headers)
            {
                builder.Append(header.Key)
                    .Append(": ")
                    .Append(header.Value)
                    .Append("\r\n");
            }

            builder.Append("\r\n");
            _socket.write(builder.ToString());
            _headersSent = true;
        }

        private void ConfigureTransferSemantics(bool bodyAllowed, bool ending)
        {
            if (_headers.TryGetValue("transfer-encoding", out var transferEncoding) && transferEncoding.Length > 0)
            {
                if (!HttpWireParser.IsChunkedTransferEncoding(transferEncoding))
                {
                    throw new Error("Only Transfer-Encoding: chunked is supported by node:http in the current runtime.");
                }

                _useChunkedEncoding = bodyAllowed;
                _declaredContentLength = null;
            }
            else if (_headers.TryGetValue("content-length", out var contentLengthText) && contentLengthText.Length > 0)
            {
                if (!int.TryParse(contentLengthText, NumberStyles.None, CultureInfo.InvariantCulture, out var contentLength) || contentLength < 0)
                {
                    throw new Error("HTTP Content-Length must be a non-negative integer.");
                }

                _declaredContentLength = contentLength;
                _useChunkedEncoding = false;
            }
            else if (bodyAllowed && !ending)
            {
                _useChunkedEncoding = true;
                _declaredContentLength = null;
            }
            else
            {
                _useChunkedEncoding = false;
                _declaredContentLength = 0;
            }

            if (!bodyAllowed)
            {
                _useChunkedEncoding = false;
                _declaredContentLength = 0;
            }

            if (_useChunkedEncoding)
            {
                _headers["transfer-encoding"] = "chunked";
                _headers.Remove("content-length");
            }
            else if (_declaredContentLength.HasValue)
            {
                _headers["content-length"] = _declaredContentLength.Value.ToString(CultureInfo.InvariantCulture);
                _headers.Remove("transfer-encoding");
            }
        }

        private bool DetermineKeepAlive(bool bodyAllowed)
        {
            if (!_requestAllowsKeepAlive)
            {
                return false;
            }

            if (_headers.TryGetValue("connection", out var connectionHeader))
            {
                if (HttpWireParser.HasToken(connectionHeader, "close"))
                {
                    return false;
                }

                if (HttpWireParser.HasToken(connectionHeader, "keep-alive"))
                {
                    return true;
                }
            }

            return _useChunkedEncoding || _declaredContentLength.HasValue || !bodyAllowed;
        }

        private void WriteBodyBytes(byte[] bytes)
        {
            if (!AllowsResponseBody())
            {
                return;
            }

            _bodyBytesWritten += bytes.Length;
            if (_declaredContentLength.HasValue && _bodyBytesWritten > _declaredContentLength.Value)
            {
                throw new Error("ServerResponse body length exceeds the declared content-length.");
            }

            if (_useChunkedEncoding)
            {
                _socket.write(HttpWireParser.EncodeChunk(bytes));
                return;
            }

            _socket.write(new Buffer(bytes));
        }

        private bool AllowsResponseBody()
        {
            var normalizedStatusCode = (int)statusCode;
            return (normalizedStatusCode < 100 || normalizedStatusCode >= 200)
                && normalizedStatusCode != 204
                && normalizedStatusCode != 304;
        }

        private void TryNotifyCompleted()
        {
            if (_completionNotified || !_requestCompleted || !_responseEnded)
            {
                return;
            }

            _completionNotified = true;
            _onCompleted(this);
        }
    }

    public sealed class HttpClientRequest : Writable
    {
        private readonly HttpRequestOptions _options;
        private readonly HttpAgent? _agent;
        private readonly Func<NetSocket>? _socketFactory;
        private readonly HttpMessageStreamDecoder _responseDecoder = new(HttpMessageKind.Response);
        private readonly List<byte[]> _pendingBodyChunks = new();
        private readonly Func<object[], object?[], object?> _onSocketConnect;
        private readonly Func<object[], object?[], object?> _onSocketData;
        private readonly Func<object[], object?[], object?> _onSocketEnd;
        private readonly Func<object[], object?[], object?> _onSocketError;
        private NetSocket? _socket;
        private HttpIncomingMessage? _response;
        private bool _started;
        private bool _socketReady;
        private bool _headersSent;
        private bool _requestEnded;
        private bool _requestFlushed;
        private bool _completionHandled;
        private bool _useChunkedEncoding;
        private int? _declaredContentLength;
        private int _bodyBytesSent;

        internal HttpClientRequest(HttpRequestOptions options)
        {
            _options = options;
            _agent = options.Agent as HttpAgent;
            _socketFactory = options.SocketFactory;

            _onSocketConnect = (scopes, args) =>
            {
                _socketReady = true;
                TryFlushRequest();
                return null;
            };
            _onSocketData = (scopes, args) =>
            {
                if (_completionHandled || args.Length == 0)
                {
                    return null;
                }

                var bytes = NodeNetworkingCommon.CoerceToBytes(args[0]);
                _responseDecoder.Append(bytes);
                ProcessResponseEvents();
                return null;
            };
            _onSocketEnd = (scopes, args) =>
            {
                if (_completionHandled)
                {
                    return null;
                }

                _responseDecoder.CompleteInput();
                ProcessResponseEvents();
                if (!_completionHandled)
                {
                    if (_response == null)
                    {
                        FailRequest(new Error("HTTP response ended before response headers were fully received."));
                    }
                    else if (!_response.complete)
                    {
                        FailRequest(new Error("HTTP response ended before the declared message body was fully received."));
                    }
                }

                return null;
            };
            _onSocketError = (scopes, args) =>
            {
                if (_completionHandled)
                {
                    return null;
                }

                var error = args.Length > 0 && args[0] is Error runtimeError
                    ? runtimeError
                    : new Error("HTTP client request error.");
                FailRequest(error);
                return null;
            };
        }

        public HttpClientRequest setHeader(object? name, object? value)
        {
            var headerName = NodeNetworkingCommon.NormalizeHeaderName(name);
            if (headerName.Length == 0)
            {
                throw new TypeError("Header name must be a non-empty string.");
            }

            _options.Headers[headerName] = value?.ToString() ?? string.Empty;
            return this;
        }

        public object? getHeader(object? name)
        {
            var headerName = NodeNetworkingCommon.NormalizeHeaderName(name);
            return _options.Headers.TryGetValue(headerName, out var value) ? value : null;
        }

        public void abort()
        {
            if (_completionHandled)
            {
                return;
            }

            _completionHandled = true;
            CleanupSocket(reuse: false, destroy: true);
        }

        protected override void InvokeWrite(object? chunk)
        {
            var bytes = NodeNetworkingCommon.CoerceToBytes(chunk);
            if (bytes.Length == 0)
            {
                return;
            }

            StartIfNeeded();
            _pendingBodyChunks.Add(bytes);
            TryFlushRequest();
        }

        public override void end()
        {
            base.end();
            _requestEnded = true;
            StartIfNeeded();
            TryFlushRequest();
        }

        public override void end(object? chunk)
        {
            base.end(chunk);
            _requestEnded = true;
            StartIfNeeded();
            TryFlushRequest();
        }

        public override void end(object? chunk, object? callback)
        {
            if (callback is Delegate del)
            {
                once("finish", del);
            }

            base.end(chunk, callback);
            _requestEnded = true;
            StartIfNeeded();
            TryFlushRequest();
        }

        private void StartIfNeeded()
        {
            if (_started)
            {
                return;
            }

            if (HttpRequestOptions.TryGetUnsupportedFeatureMessage(_options.Method, _options.Headers, _options.ModuleName, out var unsupportedMessage))
            {
                throw new Error(unsupportedMessage);
            }

            _started = true;
            NetSocket socket;
            var reusedSocket = false;
            if (_agent != null)
            {
                socket = _agent.AcquireSocket(_options, out reusedSocket);
            }
            else
            {
                socket = _socketFactory?.Invoke() ?? new NetSocket();
            }

            AttachSocket(socket);

            if (_agent?.keepAlive == true && _socket != null)
            {
                _socket.setKeepAlive(true);
            }

            if (_socket == null)
            {
                return;
            }

            if (reusedSocket)
            {
                _socketReady = true;
                TryFlushRequest();
                return;
            }

            _socket.connect(new object[] { (double)_options.Port, _options.Host });
        }

        private void AttachSocket(NetSocket socket)
        {
            _socket = socket;
            _socket.on("connect", _onSocketConnect);
            _socket.on("data", _onSocketData);
            _socket.on("end", _onSocketEnd);
            _socket.on("error", _onSocketError);
        }

        private void DetachSocket()
        {
            if (_socket == null)
            {
                return;
            }

            _socket.off("connect", _onSocketConnect);
            _socket.off("data", _onSocketData);
            _socket.off("end", _onSocketEnd);
            _socket.off("error", _onSocketError);
        }

        private void TryFlushRequest()
        {
            if (!_started || !_socketReady || _socket == null || _completionHandled)
            {
                return;
            }

            while (_pendingBodyChunks.Count > 0)
            {
                EnsureRequestHeadersSent(ending: false);
                var bytes = _pendingBodyChunks[0];
                _pendingBodyChunks.RemoveAt(0);
                WriteRequestBodyChunk(bytes);
            }

            if (_requestEnded && !_requestFlushed)
            {
                EnsureRequestHeadersSent(ending: true);
                if (_useChunkedEncoding)
                {
                    _socket.write(HttpWireParser.EncodeChunkTerminator());
                }

                if (_declaredContentLength.HasValue && _bodyBytesSent != _declaredContentLength.Value)
                {
                    throw new Error("ClientRequest body length does not match the declared content-length.");
                }

                _requestFlushed = true;
            }
        }

        private void EnsureRequestHeadersSent(bool ending)
        {
            if (_headersSent || _socket == null)
            {
                return;
            }

            if (!_options.Headers.ContainsKey("host"))
            {
                _options.Headers["host"] = _options.Port == _options.DefaultPort
                    ? _options.Host
                    : $"{_options.Host}:{_options.Port.ToString(CultureInfo.InvariantCulture)}";
            }

            ConfigureRequestTransferSemantics(ending);
            _options.Headers["connection"] = DetermineRequestKeepAlive() ? "keep-alive" : "close";

            var builder = new StringBuilder();
            builder.Append(_options.Method)
                .Append(' ')
                .Append(_options.Path)
                .Append(" HTTP/1.1\r\n");
            foreach (var header in _options.Headers)
            {
                builder.Append(header.Key)
                    .Append(": ")
                    .Append(header.Value)
                    .Append("\r\n");
            }

            builder.Append("\r\n");
            _socket.write(builder.ToString());
            _headersSent = true;
        }

        private void ConfigureRequestTransferSemantics(bool ending)
        {
            if (_options.Headers.TryGetValue("transfer-encoding", out var transferEncoding) && transferEncoding.Length > 0)
            {
                if (!HttpWireParser.IsChunkedTransferEncoding(transferEncoding))
                {
                    throw new Error("Only Transfer-Encoding: chunked is supported by node:http in the current runtime.");
                }

                _useChunkedEncoding = true;
                _declaredContentLength = null;
            }
            else if (_options.Headers.TryGetValue("content-length", out var contentLengthText) && contentLengthText.Length > 0)
            {
                if (!int.TryParse(contentLengthText, NumberStyles.None, CultureInfo.InvariantCulture, out var contentLength) || contentLength < 0)
                {
                    throw new Error("HTTP Content-Length must be a non-negative integer.");
                }

                _declaredContentLength = contentLength;
                _useChunkedEncoding = false;
            }
            else if (!ending)
            {
                _useChunkedEncoding = true;
                _declaredContentLength = null;
            }
            else
            {
                _useChunkedEncoding = false;
                _declaredContentLength = 0;
            }

            if (_useChunkedEncoding)
            {
                _options.Headers["transfer-encoding"] = "chunked";
                _options.Headers.Remove("content-length");
            }
            else if (_declaredContentLength.HasValue)
            {
                _options.Headers["content-length"] = _declaredContentLength.Value.ToString(CultureInfo.InvariantCulture);
                _options.Headers.Remove("transfer-encoding");
            }
        }

        private bool DetermineRequestKeepAlive()
        {
            if (_options.Headers.TryGetValue("connection", out var connectionHeader))
            {
                if (HttpWireParser.HasToken(connectionHeader, "close"))
                {
                    return false;
                }

                if (HttpWireParser.HasToken(connectionHeader, "keep-alive"))
                {
                    return true;
                }
            }

            return _agent?.keepAlive == true;
        }

        private void WriteRequestBodyChunk(byte[] bytes)
        {
            if (_socket == null)
            {
                return;
            }

            _bodyBytesSent += bytes.Length;
            if (_declaredContentLength.HasValue && _bodyBytesSent > _declaredContentLength.Value)
            {
                throw new Error("ClientRequest body length exceeds the declared content-length.");
            }

            if (_useChunkedEncoding)
            {
                _socket.write(HttpWireParser.EncodeChunk(bytes));
                return;
            }

            _socket.write(new Buffer(bytes));
        }

        private void ProcessResponseEvents()
        {
            foreach (var messageEvent in _responseDecoder.Drain())
            {
                switch (messageEvent)
                {
                    case HttpMessageStartEvent start:
                        if (_socket == null)
                        {
                            FailRequest(new Error("HTTP response socket became unavailable before headers were processed."));
                            return;
                        }

                        _response = HttpIncomingMessage.FromResponse(start.Head, _socket);
                        emit("response", _response);
                        break;

                    case HttpMessageBodyEvent body:
                        _response?.AppendBodyChunk(body.Chunk);
                        break;

                    case HttpMessageEndEvent end:
                        CompleteRequest(end.CanReuseConnection && _agent?.keepAlive == true);
                        _response?.CompleteBody();
                        return;

                    case HttpMessageErrorEvent error:
                        FailRequest(error.Error);
                        return;
                }
            }
        }

        private void CompleteRequest(bool reuseSocket)
        {
            if (_completionHandled)
            {
                return;
            }

            _completionHandled = true;
            CleanupSocket(reuseSocket, destroy: false);
        }

        private void FailRequest(Error error)
        {
            if (_completionHandled)
            {
                return;
            }

            _completionHandled = true;
            _response?.destroy(error);
            CleanupSocket(reuse: false, destroy: true);
            emit("error", error);
        }

        private void CleanupSocket(bool reuse, bool destroy)
        {
            if (_socket == null)
            {
                return;
            }

            var socket = _socket;
            DetachSocket();
            _socket = null;
            _socketReady = false;

            if (destroy)
            {
                if (!socket.destroyed)
                {
                    socket.destroy();
                }

                return;
            }

            if (reuse && _agent != null)
            {
                _agent.ReleaseSocket(_options, socket);
                return;
            }

            if (!socket.destroyed)
            {
                socket.destroy();
            }
        }
    }

    internal sealed class HttpRequestOptions
    {
        public string Host { get; set; } = "127.0.0.1";

        public int Port { get; set; } = 80;

        public int DefaultPort { get; set; } = 80;

        public string Path { get; set; } = "/";

        public string Method { get; set; } = "GET";

        public string Scheme { get; set; } = "http";

        public string ModuleName { get; set; } = "node:http";

        public Delegate? Callback { get; set; }

        public object? Agent { get; set; }

        internal Func<NetSocket>? SocketFactory { get; set; }

        public Dictionary<string, string> Headers { get; } = new(StringComparer.OrdinalIgnoreCase);

        public static HttpRequestOptions Parse(object[] args, string defaultMethod, string scheme, int defaultPort, string moduleName)
        {
            var result = new HttpRequestOptions
            {
                Method = defaultMethod,
                Port = defaultPort,
                DefaultPort = defaultPort,
                Scheme = scheme,
                ModuleName = moduleName,
            };

            if (args.Length == 0)
            {
                return result;
            }

            var primary = args[0];
            var secondary = args.Length > 1 ? args[1] : null;
            var tertiary = args.Length > 2 ? args[2] : null;

            if (primary is string urlText)
            {
                ApplyUrl(result, urlText, scheme, defaultPort, moduleName);
                if (secondary != null && secondary is not JsNull && secondary is not Delegate)
                {
                    ApplyObjectOptions(result, secondary);
                }

                result.Callback = tertiary as Delegate ?? secondary as Delegate;
            }
            else if (TryApplyUrlObject(result, primary, scheme, defaultPort, moduleName))
            {
                if (secondary != null && secondary is not JsNull && secondary is not Delegate)
                {
                    ApplyObjectOptions(result, secondary);
                }

                result.Callback = tertiary as Delegate ?? secondary as Delegate;
            }
            else if (primary is Delegate callbackOnly)
            {
                result.Callback = callbackOnly;
            }
            else if (primary != null && primary is not JsNull)
            {
                ApplyObjectOptions(result, primary);
                result.Callback = secondary as Delegate;
            }

            if (string.IsNullOrWhiteSpace(result.Path))
            {
                result.Path = "/";
            }
            else if (!result.Path.StartsWith("/", StringComparison.Ordinal))
            {
                result.Path = "/" + result.Path;
            }

            return result;
        }

        private static bool TryApplyUrlObject(HttpRequestOptions options, object? value, string scheme, int defaultPort, string moduleName)
        {
            if (value is not URL url)
            {
                return false;
            }

            ApplyUrl(options, url.href, scheme, defaultPort, moduleName);
            return true;
        }

        internal static bool TryGetUnsupportedFeatureMessage(string method, IDictionary<string, string> headers, string moduleName, out string message)
        {
            if (string.Equals(method, "CONNECT", StringComparison.OrdinalIgnoreCase))
            {
                message = $"{moduleName} CONNECT requests are not supported in the current runtime.";
                return true;
            }

            if (headers.TryGetValue("upgrade", out var upgradeHeader) && !string.IsNullOrWhiteSpace(upgradeHeader))
            {
                message = $"{moduleName} upgrade requests are not supported in the current runtime.";
                return true;
            }

            if (headers.TryGetValue("connection", out var connectionHeader) && HttpWireParser.HasToken(connectionHeader, "upgrade"))
            {
                message = $"{moduleName} upgrade requests are not supported in the current runtime.";
                return true;
            }

            if (headers.TryGetValue("expect", out var expectHeader) && !string.IsNullOrWhiteSpace(expectHeader))
            {
                message = $"{moduleName} Expect/100-continue flows are not supported in the current runtime.";
                return true;
            }

            message = string.Empty;
            return false;
        }

        private static void ApplyUrl(HttpRequestOptions options, string urlText, string scheme, int defaultPort, string moduleName)
        {
            Uri uri;
            try
            {
                uri = new Uri(urlText, UriKind.Absolute);
            }
            catch (UriFormatException ex)
            {
                throw new TypeError($"Invalid URL '{urlText}' for {moduleName} request.", ex);
            }

            if (!string.Equals(uri.Scheme, scheme, StringComparison.OrdinalIgnoreCase))
            {
                throw new Error($"Only {scheme}:// URLs are supported by {moduleName} in the current baseline.");
            }

            options.Host = uri.Host;
            options.Port = uri.IsDefaultPort ? defaultPort : uri.Port;
            options.Path = string.IsNullOrEmpty(uri.PathAndQuery) ? "/" : uri.PathAndQuery;
        }

        private static void ApplyObjectOptions(HttpRequestOptions options, object value)
        {
            var host = NodeNetworkingCommon.TryGetStringOption(value, "host")
                ?? NodeNetworkingCommon.TryGetStringOption(value, "hostname");
            if (!string.IsNullOrWhiteSpace(host))
            {
                options.Host = NodeNetworkingCommon.CoerceHost(host);
            }

            var port = NodeNetworkingCommon.TryGetOption(value, "port");
            if (port != null && port is not JsNull)
            {
                options.Port = NodeNetworkingCommon.CoercePort(port, defaultValue: options.Port);
            }

            var path = NodeNetworkingCommon.TryGetStringOption(value, "path");
            if (!string.IsNullOrWhiteSpace(path))
            {
                options.Path = path;
            }

            var method = NodeNetworkingCommon.TryGetStringOption(value, "method");
            if (!string.IsNullOrWhiteSpace(method))
            {
                options.Method = method!.ToUpperInvariant();
            }

            var agent = NodeNetworkingCommon.TryGetOption(value, "agent");
            if (agent != null || agent is bool)
            {
                options.Agent = agent;
            }

            foreach (var header in NodeNetworkingCommon.ToHeaderDictionary(NodeNetworkingCommon.TryGetOption(value, "headers")))
            {
                options.Headers[header.Key] = header.Value;
            }
        }
    }

    internal static class HttpWireParser
    {
        internal static bool TryParseRequest(byte[] raw, bool isEndOfStream, out HttpParsedRequest? parsed)
        {
            parsed = null;
            if (!TryParseSingleMessage(HttpMessageKind.Request, raw, isEndOfStream, out var head, out var body))
            {
                return false;
            }

            parsed = new HttpParsedRequest(
                Method: head!.Method,
                Path: head.Path,
                HttpVersion: head.HttpVersion,
                Headers: head.Headers,
                Body: Encoding.UTF8.GetString(body));
            return true;
        }

        internal static bool TryParseResponse(byte[] raw, bool isEndOfStream, out HttpParsedResponse? parsed)
        {
            parsed = null;
            if (!TryParseSingleMessage(HttpMessageKind.Response, raw, isEndOfStream, out var head, out var body))
            {
                return false;
            }

            parsed = new HttpParsedResponse(
                StatusCode: head!.StatusCode,
                StatusMessage: head.StatusMessage,
                HttpVersion: head.HttpVersion,
                Headers: head.Headers,
                Body: Encoding.UTF8.GetString(body));
            return true;
        }

        internal static bool HasToken(string value, string token)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var pieces = value.Split(',');
            foreach (var piece in pieces)
            {
                if (string.Equals(piece.Trim(), token, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsChunkedTransferEncoding(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var sawChunked = false;
            var pieces = value.Split(',');
            foreach (var piece in pieces)
            {
                var normalized = piece.Trim();
                if (normalized.Length == 0)
                {
                    continue;
                }

                if (!string.Equals(normalized, "chunked", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                sawChunked = true;
            }

            return sawChunked;
        }

        internal static Buffer EncodeChunk(byte[] bytes)
        {
            var prefix = Encoding.ASCII.GetBytes(bytes.Length.ToString("X", CultureInfo.InvariantCulture) + "\r\n");
            var result = new byte[prefix.Length + bytes.Length + 2];
            System.Buffer.BlockCopy(prefix, 0, result, 0, prefix.Length);
            System.Buffer.BlockCopy(bytes, 0, result, prefix.Length, bytes.Length);
            result[result.Length - 2] = (byte)'\r';
            result[result.Length - 1] = (byte)'\n';
            return new Buffer(result);
        }

        internal static Buffer EncodeChunkTerminator()
            => new Buffer(new byte[] { (byte)'0', (byte)'\r', (byte)'\n', (byte)'\r', (byte)'\n' });

        private static bool TryParseSingleMessage(
            HttpMessageKind kind,
            byte[] raw,
            bool isEndOfStream,
            out HttpParsedMessageHead? head,
            out byte[] body)
        {
            head = null;
            body = System.Array.Empty<byte>();

            var decoder = new HttpMessageStreamDecoder(kind);
            decoder.Append(raw);
            if (isEndOfStream)
            {
                decoder.CompleteInput();
            }

            var bodyChunks = new List<byte[]>();
            foreach (var messageEvent in decoder.Drain())
            {
                switch (messageEvent)
                {
                    case HttpMessageStartEvent start:
                        head = start.Head;
                        break;

                    case HttpMessageBodyEvent chunk:
                        bodyChunks.Add(chunk.Chunk);
                        break;

                    case HttpMessageEndEvent:
                        body = CombineChunks(bodyChunks);
                        return head != null;

                    case HttpMessageErrorEvent:
                        return false;
                }
            }

            return false;
        }

        private static byte[] CombineChunks(List<byte[]> chunks)
        {
            if (chunks.Count == 0)
            {
                return System.Array.Empty<byte>();
            }

            var totalLength = 0;
            foreach (var chunk in chunks)
            {
                totalLength += chunk.Length;
            }

            var combined = new byte[totalLength];
            var offset = 0;
            foreach (var chunk in chunks)
            {
                System.Buffer.BlockCopy(chunk, 0, combined, offset, chunk.Length);
                offset += chunk.Length;
            }

            return combined;
        }
    }

    internal sealed class HttpMessageStreamDecoder
    {
        private readonly HttpMessageKind _kind;
        private readonly List<byte> _buffer = new();
        private HttpParsedMessageHead? _currentHead;
        private HttpBodyMode _currentBodyMode;
        private int _offset;
        private int _remainingContentLength;
        private int _pendingChunkLength = -1;
        private bool _readingChunkTrailers;
        private bool _inputEnded;
        private bool _fatalError;

        internal HttpMessageStreamDecoder(HttpMessageKind kind)
        {
            _kind = kind;
        }

        internal void Append(byte[] bytes)
        {
            if (bytes.Length == 0 || _fatalError)
            {
                return;
            }

            _buffer.AddRange(bytes);
        }

        internal void CompleteInput()
        {
            _inputEnded = true;
        }

        internal List<HttpMessageEvent> Drain()
        {
            var events = new List<HttpMessageEvent>();
            if (_fatalError)
            {
                return events;
            }

            while (true)
            {
                var madeProgress = false;

                if (_currentHead == null)
                {
                    if (!TryParseHead(out var head, out var parseError))
                    {
                        if (parseError != null)
                        {
                            events.Add(new HttpMessageErrorEvent(parseError));
                            _fatalError = true;
                        }

                        break;
                    }

                    BeginMessage(head!);
                    events.Add(new HttpMessageStartEvent(head!));
                    madeProgress = true;

                    if (_currentBodyMode == HttpBodyMode.None)
                    {
                        events.Add(new HttpMessageEndEvent(head!, CanReuseCurrentConnection()));
                        ResetCurrentMessage();
                        continue;
                    }
                }

                if (!TryDrainCurrentBody(events, out var bodyProgress, out var bodyError))
                {
                    if (bodyError != null)
                    {
                        events.Add(new HttpMessageErrorEvent(bodyError));
                        _fatalError = true;
                    }

                    break;
                }

                if (!madeProgress && !bodyProgress)
                {
                    break;
                }
            }

            CompactBuffer();
            return events;
        }

        private void BeginMessage(HttpParsedMessageHead head)
        {
            _currentHead = head;
            _currentBodyMode = head.BodyMode;
            _remainingContentLength = head.ContentLength ?? 0;
            _pendingChunkLength = -1;
            _readingChunkTrailers = false;
        }

        private void ResetCurrentMessage()
        {
            _currentHead = null;
            _currentBodyMode = HttpBodyMode.None;
            _remainingContentLength = 0;
            _pendingChunkLength = -1;
            _readingChunkTrailers = false;
        }

        private bool TryParseHead(out HttpParsedMessageHead? head, out Error? error)
        {
            head = null;
            error = null;

            var headerEnd = FindHeaderTerminator(_offset);
            if (headerEnd < 0)
            {
                if (_inputEnded && UnreadCount > 0)
                {
                    error = new Error("HTTP message ended before the header section was complete.");
                }

                return false;
            }

            var headerSection = GetAsciiString(_offset, headerEnd - _offset);
            Advance((headerEnd - _offset) + 4);
            var headerLines = headerSection.Split(new[] { "\r\n" }, StringSplitOptions.None);
            if (headerLines.Length == 0 || headerLines[0].Length == 0)
            {
                error = new Error("HTTP message start line was empty.");
                return false;
            }

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var i = 1; i < headerLines.Length; i++)
            {
                var separatorIndex = headerLines[i].IndexOf(':');
                if (separatorIndex <= 0)
                {
                    continue;
                }

                var name = NodeNetworkingCommon.NormalizeHeaderName(headerLines[i].Substring(0, separatorIndex));
                if (name.Length == 0)
                {
                    continue;
                }

                headers[name] = headerLines[i].Substring(separatorIndex + 1).Trim();
            }

            var startLine = headerLines[0];
            if (!TryDetermineBodyMode(headers, startLine, out var bodyMode, out var contentLength, out var keepAlive, out var metadataError))
            {
                error = metadataError;
                return false;
            }

            if (_kind == HttpMessageKind.Request)
            {
                var parts = startLine.Split(' ');
                if (parts.Length < 2)
                {
                    error = new Error($"Invalid HTTP request line '{startLine}'.");
                    return false;
                }

                head = new HttpParsedMessageHead(
                    Kind: _kind,
                    Method: parts[0],
                    Path: parts[1],
                    StatusCode: 0,
                    StatusMessage: string.Empty,
                    HttpVersion: parts.Length > 2 ? parts[2].Replace("HTTP/", string.Empty, StringComparison.OrdinalIgnoreCase) : "1.1",
                    Headers: headers,
                    CanKeepAlive: keepAlive,
                    BodyMode: bodyMode,
                    ContentLength: contentLength);
                return true;
            }

            var firstSpace = startLine.IndexOf(' ');
            if (firstSpace < 0 || firstSpace + 1 >= startLine.Length)
            {
                error = new Error($"Invalid HTTP response line '{startLine}'.");
                return false;
            }

            var remainder = startLine.Substring(firstSpace + 1);
            var secondSpace = remainder.IndexOf(' ');
            var statusCodeText = secondSpace >= 0 ? remainder.Substring(0, secondSpace) : remainder;
            if (!int.TryParse(statusCodeText, NumberStyles.None, CultureInfo.InvariantCulture, out var statusCode))
            {
                error = new Error($"Invalid HTTP response status code '{statusCodeText}'.");
                return false;
            }

            var statusMessage = secondSpace >= 0 ? remainder.Substring(secondSpace + 1) : NodeNetworkingCommon.GetStatusMessage(statusCode);
            var httpVersion = startLine.StartsWith("HTTP/", StringComparison.OrdinalIgnoreCase)
                ? startLine.Substring("HTTP/".Length, firstSpace - "HTTP/".Length)
                : "1.1";

            head = new HttpParsedMessageHead(
                Kind: _kind,
                Method: string.Empty,
                Path: string.Empty,
                StatusCode: statusCode,
                StatusMessage: statusMessage,
                HttpVersion: httpVersion,
                Headers: headers,
                CanKeepAlive: keepAlive,
                BodyMode: bodyMode,
                ContentLength: contentLength);
            return true;
        }

        private bool TryDetermineBodyMode(
            IDictionary<string, string> headers,
            string startLine,
            out HttpBodyMode bodyMode,
            out int? contentLength,
            out bool keepAlive,
            out Error? error)
        {
            bodyMode = HttpBodyMode.None;
            contentLength = null;
            error = null;
            keepAlive = IsKeepAliveByDefault(startLine);

            if (headers.TryGetValue("connection", out var connectionHeader))
            {
                if (HttpWireParser.HasToken(connectionHeader, "close"))
                {
                    keepAlive = false;
                }
                else if (HttpWireParser.HasToken(connectionHeader, "keep-alive"))
                {
                    keepAlive = true;
                }
            }

            if (headers.TryGetValue("transfer-encoding", out var transferEncoding) && transferEncoding.Length > 0)
            {
                if (!HttpWireParser.IsChunkedTransferEncoding(transferEncoding))
                {
                    error = new Error("Only Transfer-Encoding: chunked is supported by node:http in the current runtime.");
                    return false;
                }

                bodyMode = HttpBodyMode.Chunked;
                return true;
            }

            if (headers.TryGetValue("content-length", out var contentLengthText) && contentLengthText.Length > 0)
            {
                if (!int.TryParse(contentLengthText, NumberStyles.None, CultureInfo.InvariantCulture, out var parsedLength) || parsedLength < 0)
                {
                    error = new Error("HTTP Content-Length must be a non-negative integer.");
                    return false;
                }

                contentLength = parsedLength;
                bodyMode = parsedLength == 0 ? HttpBodyMode.None : HttpBodyMode.ContentLength;
                return true;
            }

            if (_kind == HttpMessageKind.Response)
            {
                if (TryGetResponseStatusCode(startLine, out var statusCode))
                {
                    if ((statusCode >= 100 && statusCode < 200) || statusCode == 204 || statusCode == 304)
                    {
                        bodyMode = HttpBodyMode.None;
                    }
                    else
                    {
                        bodyMode = HttpBodyMode.UntilClose;
                    }
                }
            }

            return true;
        }

        private bool TryDrainCurrentBody(List<HttpMessageEvent> events, out bool madeProgress, out Error? error)
        {
            madeProgress = false;
            error = null;

            switch (_currentBodyMode)
            {
                case HttpBodyMode.ContentLength:
                    return TryDrainContentLengthBody(events, out madeProgress, out error);

                case HttpBodyMode.Chunked:
                    return TryDrainChunkedBody(events, out madeProgress, out error);

                case HttpBodyMode.UntilClose:
                    return TryDrainUntilCloseBody(events, out madeProgress, out error);

                default:
                    return true;
            }
        }

        private bool TryDrainContentLengthBody(List<HttpMessageEvent> events, out bool madeProgress, out Error? error)
        {
            madeProgress = false;
            error = null;

            if (_remainingContentLength == 0)
            {
                events.Add(new HttpMessageEndEvent(_currentHead!, CanReuseCurrentConnection()));
                ResetCurrentMessage();
                madeProgress = true;
                return true;
            }

            if (UnreadCount == 0)
            {
                if (_inputEnded)
                {
                    error = new Error("HTTP message ended before the declared Content-Length body was fully received.");
                    return false;
                }

                return true;
            }

            var bytesToRead = System.Math.Min(UnreadCount, _remainingContentLength);
            var chunk = ReadBytes(bytesToRead);
            _remainingContentLength -= chunk.Length;
            if (chunk.Length > 0)
            {
                events.Add(new HttpMessageBodyEvent(chunk));
                madeProgress = true;
            }

            if (_remainingContentLength == 0)
            {
                events.Add(new HttpMessageEndEvent(_currentHead!, CanReuseCurrentConnection()));
                ResetCurrentMessage();
                madeProgress = true;
            }

            return true;
        }

        private bool TryDrainChunkedBody(List<HttpMessageEvent> events, out bool madeProgress, out Error? error)
        {
            madeProgress = false;
            error = null;

            while (true)
            {
                if (_readingChunkTrailers)
                {
                    if (!TryReadLine(out var trailerLine))
                    {
                        if (_inputEnded)
                        {
                            error = new Error("HTTP chunked trailers were incomplete.");
                            return false;
                        }

                        return true;
                    }

                    madeProgress = true;
                    if (trailerLine.Length == 0)
                    {
                        events.Add(new HttpMessageEndEvent(_currentHead!, CanReuseCurrentConnection()));
                        ResetCurrentMessage();
                        return true;
                    }

                    continue;
                }

                if (_pendingChunkLength < 0)
                {
                    if (!TryReadLine(out var chunkSizeLine))
                    {
                        if (_inputEnded)
                        {
                            error = new Error("HTTP chunked message ended before the next chunk size line was complete.");
                            return false;
                        }

                        return true;
                    }

                    var extensionIndex = chunkSizeLine.IndexOf(';');
                    var sizeText = (extensionIndex >= 0 ? chunkSizeLine.Substring(0, extensionIndex) : chunkSizeLine).Trim();
                    if (!int.TryParse(sizeText, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var chunkLength) || chunkLength < 0)
                    {
                        error = new Error($"Invalid HTTP chunk size '{chunkSizeLine}'.");
                        return false;
                    }

                    _pendingChunkLength = chunkLength;
                    madeProgress = true;
                    if (chunkLength == 0)
                    {
                        _readingChunkTrailers = true;
                        continue;
                    }
                }

                if (UnreadCount < _pendingChunkLength + 2)
                {
                    if (_inputEnded)
                    {
                        error = new Error("HTTP chunked message ended before the declared chunk bytes were fully received.");
                        return false;
                    }

                    return true;
                }

                var chunk = ReadBytes(_pendingChunkLength);
                if (!ConsumeCrlf())
                {
                    error = new Error("HTTP chunk data must be terminated by CRLF.");
                    return false;
                }

                _pendingChunkLength = -1;
                if (chunk.Length > 0)
                {
                    events.Add(new HttpMessageBodyEvent(chunk));
                    madeProgress = true;
                }
            }
        }

        private bool TryDrainUntilCloseBody(List<HttpMessageEvent> events, out bool madeProgress, out Error? error)
        {
            madeProgress = false;
            error = null;

            if (UnreadCount > 0)
            {
                var chunk = ReadBytes(UnreadCount);
                if (chunk.Length > 0)
                {
                    events.Add(new HttpMessageBodyEvent(chunk));
                    madeProgress = true;
                }
            }

            if (_inputEnded)
            {
                events.Add(new HttpMessageEndEvent(_currentHead!, CanReuseCurrentConnection()));
                ResetCurrentMessage();
                madeProgress = true;
            }

            return true;
        }

        private bool TryReadLine(out string line)
        {
            line = string.Empty;
            for (var i = _offset; i <= _buffer.Count - 2; i++)
            {
                if (_buffer[i] == '\r' && _buffer[i + 1] == '\n')
                {
                    line = GetAsciiString(_offset, i - _offset);
                    Advance((i - _offset) + 2);
                    return true;
                }
            }

            return false;
        }

        private bool ConsumeCrlf()
        {
            if (UnreadCount < 2 || _buffer[_offset] != '\r' || _buffer[_offset + 1] != '\n')
            {
                return false;
            }

            Advance(2);
            return true;
        }

        private bool CanReuseCurrentConnection()
            => _currentHead != null && _currentHead.CanKeepAlive && _currentBodyMode != HttpBodyMode.UntilClose;

        private int FindHeaderTerminator(int startIndex)
        {
            for (var i = startIndex; i <= _buffer.Count - 4; i++)
            {
                if (_buffer[i] == '\r'
                    && _buffer[i + 1] == '\n'
                    && _buffer[i + 2] == '\r'
                    && _buffer[i + 3] == '\n')
                {
                    return i;
                }
            }

            return -1;
        }

        private string GetAsciiString(int startIndex, int length)
        {
            if (length <= 0)
            {
                return string.Empty;
            }

            var bytes = new byte[length];
            _buffer.CopyTo(startIndex, bytes, 0, length);
            return Encoding.ASCII.GetString(bytes);
        }

        private byte[] ReadBytes(int count)
        {
            if (count <= 0)
            {
                return System.Array.Empty<byte>();
            }

            var bytes = new byte[count];
            _buffer.CopyTo(_offset, bytes, 0, count);
            Advance(count);
            return bytes;
        }

        private void Advance(int count)
        {
            _offset += count;
        }

        private void CompactBuffer()
        {
            if (_offset == 0)
            {
                return;
            }

            if (_offset >= _buffer.Count)
            {
                _buffer.Clear();
                _offset = 0;
                return;
            }

            _buffer.RemoveRange(0, _offset);
            _offset = 0;
        }

        private int UnreadCount => _buffer.Count - _offset;

        private static bool IsKeepAliveByDefault(string startLine)
        {
            if (startLine.IndexOf("HTTP/1.0", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return false;
            }

            return true;
        }

        private static bool TryGetResponseStatusCode(string startLine, out int statusCode)
        {
            statusCode = 0;
            var firstSpace = startLine.IndexOf(' ');
            if (firstSpace < 0 || firstSpace + 1 >= startLine.Length)
            {
                return false;
            }

            var remainder = startLine.Substring(firstSpace + 1);
            var secondSpace = remainder.IndexOf(' ');
            var statusCodeText = secondSpace >= 0 ? remainder.Substring(0, secondSpace) : remainder;
            return int.TryParse(statusCodeText, NumberStyles.None, CultureInfo.InvariantCulture, out statusCode);
        }
    }

    internal enum HttpMessageKind
    {
        Request,
        Response,
    }

    internal enum HttpBodyMode
    {
        None,
        ContentLength,
        Chunked,
        UntilClose,
    }

    internal abstract record HttpMessageEvent;

    internal sealed record HttpMessageStartEvent(HttpParsedMessageHead Head) : HttpMessageEvent;

    internal sealed record HttpMessageBodyEvent(byte[] Chunk) : HttpMessageEvent;

    internal sealed record HttpMessageEndEvent(HttpParsedMessageHead Head, bool CanReuseConnection) : HttpMessageEvent;

    internal sealed record HttpMessageErrorEvent(Error Error) : HttpMessageEvent;

    internal sealed record HttpParsedMessageHead(
        HttpMessageKind Kind,
        string Method,
        string Path,
        double StatusCode,
        string StatusMessage,
        string HttpVersion,
        Dictionary<string, string> Headers,
        bool CanKeepAlive,
        HttpBodyMode BodyMode,
        int? ContentLength);

    internal sealed record HttpParsedRequest(
        string Method,
        string Path,
        string HttpVersion,
        Dictionary<string, string> Headers,
        string Body);

    internal sealed record HttpParsedResponse(
        double StatusCode,
        string StatusMessage,
        string HttpVersion,
        Dictionary<string, string> Headers,
        string Body);
}
