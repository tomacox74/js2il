using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JavaScriptRuntime.Node
{
    [NodeModule("http")]
    public sealed class Http
    {
        public Type IncomingMessage => typeof(HttpIncomingMessage);

        public Type ServerResponse => typeof(HttpServerResponse);

        public Type ClientRequest => typeof(HttpClientRequest);

        public Type Server => typeof(HttpServer);

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
            var options = HttpRequestOptions.Parse(args ?? System.Array.Empty<object>(), defaultMethod: "GET");
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
                    _ = new HttpServerConnectionState(this, socket);
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

        private sealed class HttpServerConnectionState
        {
            private readonly HttpServer _server;
            private readonly NetSocket _socket;
            private readonly StringBuilder _buffer = new();
            private bool _requestEmitted;

            public HttpServerConnectionState(HttpServer server, NetSocket socket)
            {
                _server = server;
                _socket = socket;

                _socket.on("data", (Func<object[], object?[], object?>)((scopes, args) =>
                {
                    if (args.Length > 0)
                    {
                        _buffer.Append(NodeNetworkingCommon.CoerceToText(args[0]));
                        TryEmitRequest(isEndOfStream: false);
                    }

                    return null;
                }));

                _socket.on("end", (Func<object[], object?[], object?>)((scopes, args) =>
                {
                    TryEmitRequest(isEndOfStream: true);
                    return null;
                }));
            }

            private void TryEmitRequest(bool isEndOfStream)
            {
                if (_requestEmitted)
                {
                    return;
                }

                if (!HttpWireParser.TryParseRequest(_buffer.ToString(), isEndOfStream, out var parsed))
                {
                    return;
                }

                _requestEmitted = true;
                var request = HttpIncomingMessage.FromRequest(parsed!, _socket);
                var response = new HttpServerResponse(_socket);
                _server.emit("request", request, response);
                request.DeliverBody();
            }
        }
    }

    public sealed class HttpIncomingMessage : Readable
    {
        private readonly object? _bodyChunk;
        private bool _bodyDelivered;

        private HttpIncomingMessage(
            string? method,
            string? url,
            double statusCode,
            string? statusMessage,
            string? httpVersion,
            JsObject headers,
            object? bodyChunk,
            NetSocket socket)
        {
            this.method = method ?? string.Empty;
            this.url = url ?? string.Empty;
            this.statusCode = statusCode;
            this.statusMessage = statusMessage ?? string.Empty;
            this.httpVersion = httpVersion ?? "1.1";
            this.headers = headers;
            this.socket = socket;
            _bodyChunk = bodyChunk;
        }

        public string method { get; }

        public string url { get; }

        public double statusCode { get; }

        public string statusMessage { get; }

        public string httpVersion { get; }

        public JsObject headers { get; }

        public NetSocket socket { get; }

        internal static HttpIncomingMessage FromRequest(HttpParsedRequest request, NetSocket socket)
        {
            var body = string.IsNullOrEmpty(request.Body) ? null : request.Body;
            return new HttpIncomingMessage(
                request.Method,
                request.Path,
                statusCode: 0,
                statusMessage: string.Empty,
                httpVersion: request.HttpVersion,
                headers: NodeNetworkingCommon.ToJsObject(request.Headers),
                bodyChunk: body,
                socket: socket);
        }

        internal static HttpIncomingMessage FromResponse(HttpParsedResponse response, NetSocket socket)
        {
            var body = string.IsNullOrEmpty(response.Body) ? null : response.Body;
            return new HttpIncomingMessage(
                method: string.Empty,
                url: string.Empty,
                statusCode: response.StatusCode,
                statusMessage: response.StatusMessage,
                httpVersion: response.HttpVersion,
                headers: NodeNetworkingCommon.ToJsObject(response.Headers),
                bodyChunk: body,
                socket: socket);
        }

        internal void DeliverBody()
        {
            if (_bodyDelivered)
            {
                return;
            }

            _bodyDelivered = true;
            if (_bodyChunk != null && _bodyChunk is not JsNull)
            {
                push(_bodyChunk);
            }

            push(null);
        }
    }

    public sealed class HttpServerResponse : Writable
    {
        private readonly NetSocket _socket;
        private readonly Dictionary<string, string> _headers = new(StringComparer.OrdinalIgnoreCase);
        private readonly MemoryStream _body = new();
        private bool _sent;

        public HttpServerResponse(NetSocket socket)
        {
            _socket = socket;
            _headers["connection"] = "close";
        }

        public double statusCode = 200;

        public string statusMessage = "OK";

        public bool headersSent => _sent;

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
            this.statusCode = NodeNetworkingCommon.CoercePort(statusCode, defaultValue: 200);

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

            _body.Write(bytes, 0, bytes.Length);
        }

        public override void end()
        {
            base.end();
            SendIfNeeded();
        }

        public override void end(object? chunk)
        {
            base.end(chunk);
            SendIfNeeded();
        }

        public override void end(object? chunk, object? callback)
        {
            if (callback is Delegate del)
            {
                once("finish", del);
            }

            base.end(chunk, callback);
            SendIfNeeded();
        }

        private void ApplyHeaders(object? headers)
        {
            foreach (var header in NodeNetworkingCommon.ToHeaderDictionary(headers))
            {
                _headers[header.Key] = header.Value;
            }
        }

        private void SendIfNeeded()
        {
            if (_sent)
            {
                return;
            }

            _sent = true;
            var responseBody = _body.ToArray();
            if (!_headers.ContainsKey("content-length"))
            {
                _headers["content-length"] = responseBody.Length.ToString();
            }

            if (!_headers.ContainsKey("connection"))
            {
                _headers["connection"] = "close";
            }

            if (string.IsNullOrEmpty(statusMessage))
            {
                statusMessage = NodeNetworkingCommon.GetStatusMessage((int)statusCode);
            }

            var builder = new StringBuilder();
            builder.Append("HTTP/1.1 ")
                .Append((int)statusCode)
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
            if (responseBody.Length > 0)
            {
                _socket.end(new Buffer(responseBody));
            }
            else
            {
                _socket.end();
            }
        }
    }

    public sealed class HttpClientRequest : Writable
    {
        private readonly HttpRequestOptions _options;
        private readonly NetSocket _socket;
        private readonly MemoryStream _body = new();
        private readonly StringBuilder _responseBuffer = new();
        private bool _started;
        private bool _responseEmitted;

        internal HttpClientRequest(HttpRequestOptions options)
        {
            _options = options;
            _socket = new NetSocket();
            _socket.on("connect", (Func<object[], object?[], object?>)((scopes, args) =>
            {
                SendRequest();
                return null;
            }));
            _socket.on("data", (Func<object[], object?[], object?>)((scopes, args) =>
            {
                if (args.Length > 0)
                {
                    _responseBuffer.Append(NodeNetworkingCommon.CoerceToText(args[0]));
                    TryEmitResponse(isEndOfStream: false);
                }

                return null;
            }));
            _socket.on("end", (Func<object[], object?[], object?>)((scopes, args) =>
            {
                TryEmitResponse(isEndOfStream: true);
                return null;
            }));
            _socket.on("error", (Func<object[], object?[], object?>)((scopes, args) =>
            {
                emit("error", args.Length > 0 ? args[0] : new Error("HTTP client request error."));
                return null;
            }));
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
            _socket.destroy();
        }

        protected override void InvokeWrite(object? chunk)
        {
            var bytes = NodeNetworkingCommon.CoerceToBytes(chunk);
            if (bytes.Length == 0)
            {
                return;
            }

            _body.Write(bytes, 0, bytes.Length);
        }

        public override void end()
        {
            base.end();
            StartIfNeeded();
        }

        public override void end(object? chunk)
        {
            base.end(chunk);
            StartIfNeeded();
        }

        public override void end(object? chunk, object? callback)
        {
            if (callback is Delegate del)
            {
                once("finish", del);
            }

            base.end(chunk, callback);
            StartIfNeeded();
        }

        private void StartIfNeeded()
        {
            if (_started)
            {
                return;
            }

            _started = true;
            _socket.connect(new object[] { (double)_options.Port, _options.Host });
        }

        private void SendRequest()
        {
            var bodyBytes = _body.ToArray();
            if (!_options.Headers.ContainsKey("host"))
            {
                _options.Headers["host"] = _options.Port == 80
                    ? _options.Host
                    : $"{_options.Host}:{_options.Port}";
            }

            if (!_options.Headers.ContainsKey("content-length"))
            {
                _options.Headers["content-length"] = bodyBytes.Length.ToString();
            }

            if (!_options.Headers.ContainsKey("connection"))
            {
                _options.Headers["connection"] = "close";
            }

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
            if (bodyBytes.Length > 0)
            {
                _socket.end(new Buffer(bodyBytes));
            }
            else
            {
                _socket.end();
            }
        }

        private void TryEmitResponse(bool isEndOfStream)
        {
            if (_responseEmitted)
            {
                return;
            }

            if (!HttpWireParser.TryParseResponse(_responseBuffer.ToString(), isEndOfStream, out var parsed))
            {
                return;
            }

            _responseEmitted = true;
            var response = HttpIncomingMessage.FromResponse(parsed!, _socket);
            emit("response", response);
            response.DeliverBody();
        }
    }

    internal sealed class HttpRequestOptions
    {
        public string Host { get; set; } = "127.0.0.1";

        public int Port { get; set; } = 80;

        public string Path { get; set; } = "/";

        public string Method { get; set; } = "GET";

        public Delegate? Callback { get; set; }

        public Dictionary<string, string> Headers { get; } = new(StringComparer.OrdinalIgnoreCase);

        public static HttpRequestOptions Parse(object[] args, string defaultMethod)
        {
            var result = new HttpRequestOptions
            {
                Method = defaultMethod,
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
                ApplyUrl(result, urlText);
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

        private static void ApplyUrl(HttpRequestOptions options, string urlText)
        {
            var uri = new Uri(urlText, UriKind.Absolute);
            if (!string.Equals(uri.Scheme, "http", StringComparison.OrdinalIgnoreCase))
            {
                throw new Error("Only http:// URLs are supported by node:http in the current baseline.");
            }

            options.Host = uri.Host;
            options.Port = uri.IsDefaultPort ? 80 : uri.Port;
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

            foreach (var header in NodeNetworkingCommon.ToHeaderDictionary(NodeNetworkingCommon.TryGetOption(value, "headers")))
            {
                options.Headers[header.Key] = header.Value;
            }
        }
    }

    internal static class HttpWireParser
    {
        internal static bool TryParseRequest(string raw, bool isEndOfStream, out HttpParsedRequest? parsed)
        {
            parsed = null;
            if (!TrySplitMessage(raw, out var startLine, out var headers, out var body, isEndOfStream, treatMissingLengthAsComplete: true))
            {
                return false;
            }

            var parts = startLine.Split(' ');
            if (parts.Length < 2)
            {
                return false;
            }

            parsed = new HttpParsedRequest(
                Method: parts[0],
                Path: parts[1],
                HttpVersion: parts.Length > 2 ? parts[2].Replace("HTTP/", string.Empty, StringComparison.OrdinalIgnoreCase) : "1.1",
                Headers: headers,
                Body: body);
            return true;
        }

        internal static bool TryParseResponse(string raw, bool isEndOfStream, out HttpParsedResponse? parsed)
        {
            parsed = null;
            if (!TrySplitMessage(raw, out var startLine, out var headers, out var body, isEndOfStream, treatMissingLengthAsComplete: false))
            {
                return false;
            }

            var firstSpace = startLine.IndexOf(' ');
            if (firstSpace < 0 || firstSpace + 1 >= startLine.Length)
            {
                return false;
            }

            var remainder = startLine.Substring(firstSpace + 1);
            var secondSpace = remainder.IndexOf(' ');
            var statusCodeText = secondSpace >= 0 ? remainder.Substring(0, secondSpace) : remainder;
            if (!int.TryParse(statusCodeText, out var statusCode))
            {
                return false;
            }

            var statusMessage = secondSpace >= 0 ? remainder.Substring(secondSpace + 1) : NodeNetworkingCommon.GetStatusMessage(statusCode);
            var httpVersion = startLine.StartsWith("HTTP/", StringComparison.OrdinalIgnoreCase)
                ? startLine.Substring("HTTP/".Length, firstSpace - "HTTP/".Length)
                : "1.1";

            parsed = new HttpParsedResponse(
                StatusCode: statusCode,
                StatusMessage: statusMessage,
                HttpVersion: httpVersion,
                Headers: headers,
                Body: body);
            return true;
        }

        private static bool TrySplitMessage(
            string raw,
            out string startLine,
            out Dictionary<string, string> headers,
            out string body,
            bool isEndOfStream,
            bool treatMissingLengthAsComplete)
        {
            startLine = string.Empty;
            headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            body = string.Empty;

            var headerEnd = raw.IndexOf("\r\n\r\n", StringComparison.Ordinal);
            if (headerEnd < 0)
            {
                return false;
            }

            var headerSection = raw.Substring(0, headerEnd);
            var headerLines = headerSection.Split(new[] { "\r\n" }, StringSplitOptions.None);
            if (headerLines.Length == 0)
            {
                return false;
            }

            startLine = headerLines[0];
            for (var i = 1; i < headerLines.Length; i++)
            {
                var separator = headerLines[i].IndexOf(':');
                if (separator <= 0)
                {
                    continue;
                }

                var name = NodeNetworkingCommon.NormalizeHeaderName(headerLines[i].Substring(0, separator));
                var value = headerLines[i].Substring(separator + 1).Trim();
                if (name.Length == 0)
                {
                    continue;
                }

                headers[name] = value;
            }

            var contentLength = 0;
            var hasContentLength = headers.TryGetValue("content-length", out var contentLengthText)
                && int.TryParse(contentLengthText, out contentLength);
            var payload = raw.Substring(headerEnd + 4);

            if (hasContentLength)
            {
                if (payload.Length < contentLength && !isEndOfStream)
                {
                    return false;
                }

                body = payload.Substring(0, System.Math.Min(contentLength, payload.Length));
                return true;
            }

            if (!treatMissingLengthAsComplete && !isEndOfStream)
            {
                return false;
            }

            body = payload;
            return true;
        }
    }

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
