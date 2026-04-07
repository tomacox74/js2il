using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using JavaScriptRuntime.EngineCore;

namespace JavaScriptRuntime.Node
{
    [NodeModule("net")]
    public sealed class Net
    {
        public Type Server => typeof(NetServer);

        public Type Socket => typeof(NetSocket);

        public NetServer createServer(object[] args)
        {
            var srcArgs = args ?? System.Array.Empty<object>();
            object? options = null;
            Delegate? connectionListener = null;

            if (srcArgs.Length > 0)
            {
                if (srcArgs[0] is Delegate listener)
                {
                    connectionListener = listener;
                }
                else
                {
                    options = srcArgs[0];
                    if (srcArgs.Length > 1 && srcArgs[1] is Delegate nextListener)
                    {
                        connectionListener = nextListener;
                    }
                }
            }

            var server = new NetServer(options);
            if (connectionListener != null)
            {
                server.on("connection", connectionListener);
            }

            return server;
        }

        public NetSocket connect(object[] args)
        {
            var socket = new NetSocket();
            return socket.connect(args);
        }

        public NetSocket createConnection(object[] args) => connect(args);
    }

    internal static class NodeNetworkingCommon
    {
        internal static object? TryGetOption(object? options, string name)
        {
            if (options == null || options is JsNull)
            {
                return null;
            }

            try
            {
                if (options is ExpandoObject expando)
                {
                    var expandoDict = (IDictionary<string, object?>)expando;
                    if (expandoDict.TryGetValue(name, out var expandoValue))
                    {
                        return expandoValue;
                    }
                }

                if (options is IDictionary<string, object?> dict && dict.TryGetValue(name, out var dictValue))
                {
                    return dictValue;
                }

                return ObjectRuntime.GetProperty(options, name);
            }
            catch
            {
                return null;
            }
        }

        internal static string? TryGetStringOption(object? options, string name)
            => TryGetOption(options, name)?.ToString();

        internal static bool LooksLikeOptionsObject(object? value)
        {
            return value != null
                && value is not JsNull
                && value is not string
                && value is not Delegate
                && value is not double
                && value is not int
                && value is not long
                && value is not short;
        }

        internal static int CoercePort(object? value, int defaultValue = 0)
        {
            if (value == null || value is JsNull)
            {
                return defaultValue;
            }

            try
            {
                var number = TypeUtilities.ToNumber(value);
                if (double.IsNaN(number) || double.IsInfinity(number))
                {
                    return defaultValue;
                }

                var port = (int)number;
                if (port < 0 || port > 65535)
                {
                    throw new RangeError("Port must be between 0 and 65535.");
                }

                return port;
            }
            catch (RangeError)
            {
                throw;
            }
            catch
            {
                return defaultValue;
            }
        }

        internal static bool CoerceBoolean(object? value, bool defaultValue = false)
        {
            if (value == null || value is JsNull)
            {
                return defaultValue;
            }

            if (value is bool boolean)
            {
                return boolean;
            }

            return TypeUtilities.ToBoolean(value);
        }

        internal static int CoerceHttpStatusCode(object? value, int defaultValue = 200)
        {
            if (value == null || value is JsNull)
            {
                return defaultValue;
            }

            double number;
            try
            {
                number = TypeUtilities.ToNumber(value);
            }
            catch (Exception ex)
            {
                throw new TypeError("HTTP status code must be a number.", ex);
            }

            if (double.IsNaN(number) || double.IsInfinity(number))
            {
                throw new TypeError("HTTP status code must be a finite number.");
            }

            if (number != System.Math.Truncate(number))
            {
                throw new RangeError("HTTP status code must be an integer.");
            }

            var statusCode = (int)number;
            if (statusCode < 100 || statusCode > 999)
            {
                throw new RangeError("HTTP status code must be between 100 and 999.");
            }

            return statusCode;
        }

        internal static string CoerceHost(string? host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                return "127.0.0.1";
            }

            var normalized = host.Trim();
            if (string.Equals(normalized, "localhost", StringComparison.OrdinalIgnoreCase))
            {
                return "127.0.0.1";
            }

            return normalized;
        }

        internal static IPAddress ResolveAddress(string host)
        {
            var normalized = CoerceHost(host);
            if (string.Equals(normalized, "0.0.0.0", StringComparison.OrdinalIgnoreCase))
            {
                return IPAddress.Any;
            }

            if (IPAddress.TryParse(normalized, out var address))
            {
                return address;
            }

            var resolved = Dns.GetHostAddresses(normalized)
                .FirstOrDefault(candidate => candidate.AddressFamily == AddressFamily.InterNetwork);
            if (resolved != null)
            {
                return resolved;
            }

            throw new Error($"Unable to resolve host '{host}'.");
        }

        internal static void ScheduleOnEventLoop(Action action)
            => ScheduleOnEventLoop(null, action);

        internal static void ScheduleOnEventLoop(NodeSchedulerState? scheduler, Action action)
        {
            try
            {
                var targetScheduler = scheduler ?? GlobalThis.ServiceProvider?.Resolve<NodeSchedulerState>();
                if (targetScheduler != null)
                {
                    targetScheduler.QueueNextTick(action);
                    return;
                }
            }
            catch
            {
            }

            action();
        }

        internal static void ScheduleImmediateOnEventLoop(NodeSchedulerState? scheduler, Action action)
        {
            try
            {
                var targetScheduler = scheduler ?? GlobalThis.ServiceProvider?.Resolve<NodeSchedulerState>();
                if (targetScheduler != null)
                {
                    ((IScheduler)targetScheduler).ScheduleImmediate(action);
                    return;
                }
            }
            catch
            {
            }

            ScheduleOnEventLoop(scheduler, action);
        }

        internal static PromiseWithResolvers CreateIoPromise(Action<object?>? onSuccess = null, Action<object?>? onError = null)
        {
            JsFunc1 resolve = (scopes, newTarget, value) =>
            {
                onSuccess?.Invoke(value);
                return null;
            };

            JsFunc1 reject = (scopes, newTarget, reason) =>
            {
                onError?.Invoke(reason);
                return null;
            };

            return new PromiseWithResolvers(new Promise(), resolve, reject);
        }

        internal static byte[] CoerceToBytes(object? chunk)
        {
            if (chunk == null || chunk is JsNull)
            {
                return System.Array.Empty<byte>();
            }

            if (chunk is Buffer buffer)
            {
                return buffer.ToByteArray();
            }

            if (chunk is byte[] bytes)
            {
                return (byte[])bytes.Clone();
            }

            if (chunk is string text)
            {
                return Encoding.UTF8.GetBytes(text);
            }

            return Encoding.UTF8.GetBytes(DotNet2JSConversions.ToString(chunk));
        }

        internal static string CoerceToText(object? chunk)
        {
            if (chunk == null || chunk is JsNull)
            {
                return string.Empty;
            }

            if (chunk is string text)
            {
                return text;
            }

            if (chunk is Buffer buffer)
            {
                return Encoding.UTF8.GetString(buffer.ToByteArray());
            }

            if (chunk is byte[] bytes)
            {
                return Encoding.UTF8.GetString(bytes);
            }

            return DotNet2JSConversions.ToString(chunk);
        }

        internal static Dictionary<string, string> ToHeaderDictionary(object? headers)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (headers == null || headers is JsNull)
            {
                return result;
            }

            if (headers is IDictionary<string, object?> dict)
            {
                foreach (var entry in dict)
                {
                    var key = NormalizeHeaderName(entry.Key);
                    if (key.Length == 0)
                    {
                        continue;
                    }

                    result[key] = entry.Value?.ToString() ?? string.Empty;
                }
            }

            return result;
        }

        internal static JsObject ToJsObject(IDictionary<string, string> values)
        {
            var result = new JsObject();
            foreach (var entry in values)
            {
                result.SetString(entry.Key, entry.Value);
            }

            return result;
        }

        internal static string NormalizeHeaderName(object? name)
            => (name?.ToString() ?? string.Empty).Trim().ToLowerInvariant();

        internal static object CreateAddressRecord(string host, int port)
        {
            var result = new ExpandoObject();
            var dict = (IDictionary<string, object?>)result;
            dict["address"] = host;
            dict["family"] = "IPv4";
            dict["port"] = (double)port;
            return result;
        }

        internal static string GetStatusMessage(int statusCode)
        {
            return statusCode switch
            {
                200 => "OK",
                201 => "Created",
                202 => "Accepted",
                204 => "No Content",
                400 => "Bad Request",
                404 => "Not Found",
                500 => "Internal Server Error",
                _ => "OK",
            };
        }
    }

    internal sealed class Utf8ChunkDecoder
    {
        private readonly Decoder _decoder = Encoding.UTF8.GetDecoder();

        internal string Decode(byte[] bytes, int count, bool flush)
        {
            if (count < 0 || count > bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            var charBufferLength = Encoding.UTF8.GetMaxCharCount(count + 4);
            var chars = new char[charBufferLength];
            var written = _decoder.GetChars(bytes, 0, count, chars, 0, flush);
            if (written == 0)
            {
                return string.Empty;
            }

            return new string(chars, 0, written);
        }

        internal string Flush()
            => Decode(System.Array.Empty<byte>(), 0, flush: true);
    }

    public class NetServer : EventEmitter
    {
        private readonly object? _options;
        private readonly Func<NetSocket>? _socketFactory;
        private readonly object _sync = new();
        private readonly List<NetSocket> _connections = new();
        private readonly bool _allowHalfOpen;
        private TcpListener? _listener;
        private bool _closingRequested;
        private string _host = "127.0.0.1";
        private int _port;
        private IIOScheduler? _ioScheduler;
        private NodeSchedulerState? _nodeScheduler;

        private IIOScheduler IoScheduler => _ioScheduler
            ??= GlobalThis.ServiceProvider?.Resolve<IIOScheduler>()
                ?? throw new InvalidOperationException("IIOScheduler is not available for net.");

        private NodeSchedulerState NodeScheduler => _nodeScheduler
            ??= GlobalThis.ServiceProvider?.Resolve<NodeSchedulerState>()
                ?? throw new InvalidOperationException("NodeSchedulerState is not available for net.");

        public NetServer(object? options = null)
            : this(options, null)
        {
        }

        internal NetServer(object? options, Func<NetSocket>? socketFactory)
        {
            _options = options;
            _socketFactory = socketFactory;
            _allowHalfOpen = NodeNetworkingCommon.CoerceBoolean(
                NodeNetworkingCommon.TryGetOption(options, "allowHalfOpen"),
                defaultValue: false);
        }

        public bool listening => _listener != null;

        public NetServer listen(object[] args)
        {
            if (_listener != null)
            {
                throw new Error("Server is already listening.");
            }

            ParseListenArgs(args ?? System.Array.Empty<object>(), out var port, out var host, out var callback);
            if (callback != null)
            {
                once("listening", callback);
            }

            _host = NodeNetworkingCommon.CoerceHost(host);
            var bindAddress = NodeNetworkingCommon.ResolveAddress(_host);
            _listener = new TcpListener(bindAddress, port);
            _listener.Start();
            _port = ((IPEndPoint)_listener.LocalEndpoint).Port;
            _closingRequested = false;
            var ioScheduler = IoScheduler;
            var nodeScheduler = NodeScheduler;

            var lifetimePromise = NodeNetworkingCommon.CreateIoPromise(
                onSuccess: _ => emit("close"),
                onError: reason =>
                {
                    emit("error", reason);
                    emit("close");
                });

            ioScheduler.BeginIo();
            _ = AcceptLoopAsync(_listener, lifetimePromise, ioScheduler, nodeScheduler);
            NodeNetworkingCommon.ScheduleOnEventLoop(nodeScheduler, () => emit("listening"));
            return this;
        }

        public object? address()
        {
            if (_listener == null)
            {
                return null;
            }

            return NodeNetworkingCommon.CreateAddressRecord(_host, _port);
        }

        public NetServer close()
        {
            return close(null);
        }

        public NetServer close(object? callback)
        {
            if (callback is Delegate del)
            {
                if (_listener == null)
                {
                    NodeNetworkingCommon.ScheduleOnEventLoop(_nodeScheduler, () =>
                    {
                        Closure.InvokeWithArgs(del, RuntimeServices.EmptyScopes, System.Array.Empty<object?>());
                    });
                }
                else
                {
                    once("close", del);
                }
            }

            if (_listener == null)
            {
                return this;
            }

            _closingRequested = true;
            var listener = _listener;
            _listener = null;

            try
            {
                listener.Stop();
            }
            catch
            {
            }

            CloseActiveConnections();
            return this;
        }

        private async Task AcceptLoopAsync(TcpListener listener, PromiseWithResolvers lifetimePromise, IIOScheduler ioScheduler, NodeSchedulerState nodeScheduler)
        {
            try
            {
                while (!_closingRequested)
                {
                    TcpClient? client = null;
                    try
                    {
                        client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                    }
                    catch (ObjectDisposedException) when (_closingRequested)
                    {
                        break;
                    }
                    catch (InvalidOperationException) when (_closingRequested)
                    {
                        break;
                    }
                    catch (SocketException) when (_closingRequested)
                    {
                        break;
                    }

                    if (client == null)
                    {
                        continue;
                    }

                    var socket = _socketFactory?.Invoke() ?? new NetSocket(_allowHalfOpen);
                    socket.AttachSchedulers(ioScheduler, nodeScheduler);
                    try
                    {
                        await socket.AttachAcceptedClientAsync(client).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        var error = ex as Error ?? new Error(ex.Message, ex);
                        NodeNetworkingCommon.ScheduleOnEventLoop(nodeScheduler, () => emit("error", error));
                        continue;
                    }

                    lock (_sync)
                    {
                        _connections.Add(socket);
                    }

                    socket.once("close", (Func<object[], object?[], object?>)((scopes, args) =>
                    {
                        lock (_sync)
                        {
                            _connections.Remove(socket);
                        }

                        return null;
                    }));
                    NodeNetworkingCommon.ScheduleOnEventLoop(nodeScheduler, () =>
                    {
                        emit("connection", socket);
                        socket.ActivateConnectedSocket();
                    });
                }

                ioScheduler.EndIo(lifetimePromise, null, isError: false);
            }
            catch (Exception ex)
            {
                ioScheduler.EndIo(
                    lifetimePromise,
                    ex as Error ?? new Error(ex.Message, ex),
                    isError: true);
            }
        }

        private void CloseActiveConnections()
        {
            NetSocket[] connections;
            lock (_sync)
            {
                connections = _connections.ToArray();
                _connections.Clear();
            }

            foreach (var connection in connections)
            {
                try
                {
                    connection.destroy();
                }
                catch
                {
                }
            }
        }

        private void ParseListenArgs(object[] args, out int port, out string? host, out Delegate? callback)
        {
            port = 0;
            host = null;
            callback = null;

            if (args.Length == 0)
            {
                port = NodeNetworkingCommon.CoercePort(NodeNetworkingCommon.TryGetOption(_options, "port"));
                host = NodeNetworkingCommon.TryGetStringOption(_options, "host")
                    ?? NodeNetworkingCommon.TryGetStringOption(_options, "hostname");
                return;
            }

            if (NodeNetworkingCommon.LooksLikeOptionsObject(args[0]))
            {
                port = NodeNetworkingCommon.CoercePort(NodeNetworkingCommon.TryGetOption(args[0], "port"));
                host = NodeNetworkingCommon.TryGetStringOption(args[0], "host")
                    ?? NodeNetworkingCommon.TryGetStringOption(args[0], "hostname");
                if (args.Length > 1 && args[1] is Delegate optCallback)
                {
                    callback = optCallback;
                }

                return;
            }

            port = NodeNetworkingCommon.CoercePort(args[0]);

            if (args.Length > 1)
            {
                if (args[1] is Delegate callbackArg)
                {
                    callback = callbackArg;
                    return;
                }

                host = args[1]?.ToString();
            }

            if (args.Length > 2 && args[2] is Delegate finalCallback)
            {
                callback = finalCallback;
            }
        }
    }

    public class NetSocket : Duplex
    {
        private readonly Queue<byte[]> _pendingWrites = new();
        private readonly bool _allowHalfOpen;
        private readonly object _timeoutSync = new();
        private TcpClient? _client;
        private System.IO.Stream? _stream;
        private bool _connectInProgress;
        private bool _connected;
        private bool _canWriteToStream;
        private bool _outputClosed;
        private bool _readSideEnded;
        private bool _closeEmitted;
        private bool _destroyRequested;
        private bool _hadSocketError;
        private bool _keepAliveEnabled;
        private bool _keepAliveConfigured;
        private object? _timeoutHandle;
        private double _timeoutMilliseconds;
        private long _timeoutGeneration;
        private double _bytesRead;
        private double _bytesWritten;
        private IIOScheduler? _ioScheduler;
        private NodeSchedulerState? _nodeScheduler;

        private IIOScheduler IoScheduler => _ioScheduler
            ??= GlobalThis.ServiceProvider?.Resolve<IIOScheduler>()
                ?? throw new InvalidOperationException("IIOScheduler is not available for net.");

        private NodeSchedulerState NodeScheduler => _nodeScheduler
            ??= GlobalThis.ServiceProvider?.Resolve<NodeSchedulerState>()
                ?? throw new InvalidOperationException("NodeSchedulerState is not available for net.");

        public bool connecting => _connectInProgress && !_connected;

        public override bool destroyed => _destroyRequested || _closeEmitted;

        public bool allowHalfOpen => _allowHalfOpen;

        public string remoteAddress => TryGetRemoteEndpoint()?.Address.ToString() ?? string.Empty;

        public double remotePort => TryGetRemoteEndpoint()?.Port ?? 0;

        public string localAddress => TryGetLocalEndpoint()?.Address.ToString() ?? string.Empty;

        public double localPort => TryGetLocalEndpoint()?.Port ?? 0;

        public double bytesRead => _bytesRead;

        public double bytesWritten => _bytesWritten;

        public NetSocket()
            : this((object?)null)
        {
        }

        public NetSocket(object? options)
        {
            _allowHalfOpen = NodeNetworkingCommon.CoerceBoolean(
                NodeNetworkingCommon.TryGetOption(options, "allowHalfOpen"),
                defaultValue: false);
        }

        internal NetSocket(bool allowHalfOpen)
        {
            _allowHalfOpen = allowHalfOpen;
        }

        protected virtual Task<System.IO.Stream> CreateClientStreamAsync(TcpClient client, string host, int port)
            => Task.FromResult<System.IO.Stream>(client.GetStream());

        protected virtual Task<System.IO.Stream> CreateAcceptedStreamAsync(TcpClient client)
            => Task.FromResult<System.IO.Stream>(client.GetStream());

        public NetSocket connect(object[] args)
        {
            if (_connectInProgress || _connected || _client != null)
            {
                throw new Error("Socket is already connected.");
            }

            ParseConnectArgs(args ?? System.Array.Empty<object>(), out var port, out var host, out var callback);
            if (callback != null)
            {
                once("connect", callback);
            }

            _connectInProgress = true;
            _ = IoScheduler;
            _ = NodeScheduler;

            var connectPromise = NodeNetworkingCommon.CreateIoPromise(
                onSuccess: _ =>
                {
                    _connectInProgress = false;
                    _connected = true;
                    emit("connect");
                    ActivateConnectedSocket();
                },
                onError: reason =>
                {
                    _connectInProgress = false;
                    emit("error", reason);
                });

            IoScheduler.BeginIo();
            _ = ConnectAsync(NodeNetworkingCommon.CoerceHost(host), port, connectPromise);
            return this;
        }

        public NetSocket setTimeout(object? timeout)
            => setTimeout(timeout, null);

        public NetSocket setTimeout(object? timeout, object? callback)
        {
            if (callback is Delegate del)
            {
                once("timeout", del);
            }

            double timeoutMilliseconds;
            if (timeout == null || timeout is JsNull)
            {
                timeoutMilliseconds = 0;
            }
            else
            {
                timeoutMilliseconds = TypeUtilities.ToNumber(timeout);
                if (double.IsNaN(timeoutMilliseconds) || timeoutMilliseconds < 0)
                {
                    timeoutMilliseconds = 0;
                }
            }

            lock (_timeoutSync)
            {
                _timeoutMilliseconds = timeoutMilliseconds;
            }

            if (timeoutMilliseconds <= 0)
            {
                ClearInactivityTimeout();
                return this;
            }

            ResetInactivityTimeout();
            return this;
        }

        public NetSocket setKeepAlive(object? enable)
            => setKeepAlive(enable, null);

        public NetSocket setKeepAlive(object? enable, object? initialDelay)
        {
            if (initialDelay != null && initialDelay is not JsNull)
            {
                var delayMilliseconds = TypeUtilities.ToNumber(initialDelay);
                if (!double.IsNaN(delayMilliseconds) && !double.IsInfinity(delayMilliseconds) && delayMilliseconds != 0)
                {
                    throw new Error("NetSocket.setKeepAlive currently supports enable/disable only; initialDelay is not implemented.");
                }
            }

            _keepAliveEnabled = NodeNetworkingCommon.CoerceBoolean(enable, defaultValue: false);
            _keepAliveConfigured = true;
            ApplyKeepAliveOption();
            return this;
        }

        public NetSocket setNoDelay()
            => setNoDelay(null);

        public NetSocket setNoDelay(object? noDelay)
        {
            return this;
        }

        public override void destroy()
        {
            destroy(null);
        }

        public override void destroy(object? error)
        {
            lock (_timeoutSync)
            {
                if (_destroyRequested)
                {
                    return;
                }

                _destroyRequested = true;
            }
            ClearInactivityTimeout();

            if (error != null && error is not JsNull)
            {
                _hadSocketError = true;
                NodeNetworkingCommon.ScheduleOnEventLoop(_nodeScheduler, () => emit("error", error));
            }

            try
            {
                _stream?.Dispose();
            }
            catch
            {
            }

            try
            {
                _client?.Close();
            }
            catch
            {
            }

            if (_client == null && !_connectInProgress)
            {
                NodeNetworkingCommon.ScheduleOnEventLoop(_nodeScheduler, () => FinalizeReadableSide(hadError: _hadSocketError));
            }
        }

        public override void end()
        {
            base.end();
            CloseOutputSide();
        }

        public override void end(object? chunk)
        {
            base.end(chunk);
            CloseOutputSide();
        }

        public override void end(object? chunk, object? callback)
        {
            if (callback is Delegate del)
            {
                once("finish", del);
            }

            base.end(chunk, callback);
            CloseOutputSide();
        }

        internal void AttachClient(TcpClient client)
        {
            AttachConnectedStream(client, client.GetStream());
        }

        internal async Task AttachAcceptedClientAsync(TcpClient client)
        {
            System.IO.Stream? stream = null;
            try
            {
                stream = await CreateAcceptedStreamAsync(client).ConfigureAwait(false);
                AttachConnectedStream(client, stream);
            }
            catch
            {
                try
                {
                    stream?.Dispose();
                }
                catch
                {
                }

                try
                {
                    client.Dispose();
                }
                catch
                {
                }

                throw;
            }
        }

        protected void AttachConnectedStream(TcpClient client, System.IO.Stream stream)
        {
            _client = client;
            _stream = stream;
            _connected = true;
            _connectInProgress = false;
            _canWriteToStream = false;
            ApplyKeepAliveOption();
        }

        internal void AttachSchedulers(IIOScheduler ioScheduler, NodeSchedulerState nodeScheduler)
        {
            _ioScheduler = ioScheduler;
            _nodeScheduler = nodeScheduler;
        }

        internal void ActivateConnectedSocket()
        {
            if (_destroyRequested || _client == null || _stream == null)
            {
                return;
            }

            if (_canWriteToStream)
            {
                return;
            }

            _canWriteToStream = true;
            FlushPendingWrites();
            if (_outputClosed)
            {
                ShutdownSend();
            }

            StartReadLoop();
            ResetInactivityTimeout();
        }

        protected override void InvokeWrite(object? chunk)
        {
            var bytes = NodeNetworkingCommon.CoerceToBytes(chunk);
            if (bytes.Length == 0)
            {
                return;
            }

            if (!_canWriteToStream || _stream == null)
            {
                _pendingWrites.Enqueue(bytes);
                return;
            }

            try
            {
                _stream.Write(bytes, 0, bytes.Length);
                _bytesWritten += bytes.Length;
                ResetInactivityTimeout();
            }
            catch (ObjectDisposedException ex)
            {
                HandleStreamWriteException(ex);
            }
            catch (IOException ex)
            {
                HandleStreamWriteException(ex);
            }
            catch (InvalidOperationException ex)
            {
                HandleStreamWriteException(ex);
            }
            catch (Exception ex)
            {
                HandleStreamWriteException(ex);
            }
        }

        private async Task ConnectAsync(string host, int port, PromiseWithResolvers connectPromise)
        {
            TcpClient? client = null;
            System.IO.Stream? stream = null;
            try
            {
                client = new TcpClient(AddressFamily.InterNetwork);
                await client.ConnectAsync(NodeNetworkingCommon.ResolveAddress(host), port).ConfigureAwait(false);
                stream = await CreateClientStreamAsync(client, host, port).ConfigureAwait(false);
                AttachConnectedStream(client, stream);
                IoScheduler.EndIo(connectPromise, null, isError: false);
            }
            catch (Exception ex)
            {
                try
                {
                    stream?.Dispose();
                }
                catch
                {
                }

                try
                {
                    client?.Dispose();
                }
                catch
                {
                }

                IoScheduler.EndIo(
                    connectPromise,
                    ex as Error ?? new Error(ex.Message, ex),
                    isError: true);
            }
        }

        private void StartReadLoop()
        {
            if (_stream == null || _destroyRequested)
            {
                return;
            }

            var lifetimePromise = NodeNetworkingCommon.CreateIoPromise(
                onSuccess: _ => FinalizeReadableSide(hadError: false),
                onError: reason =>
                {
                    emit("error", reason);
                    FinalizeReadableSide(hadError: true);
                });

            IoScheduler.BeginIo();
            _ = ReadLoopAsync(_stream, lifetimePromise);
        }

        private async Task ReadLoopAsync(System.IO.Stream stream, PromiseWithResolvers lifetimePromise)
        {
            var buffer = new byte[4096];
            try
            {
                while (true)
                {
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                    if (bytesRead <= 0)
                    {
                        break;
                    }

                    var chunkBytes = new byte[bytesRead];
                    System.Buffer.BlockCopy(buffer, 0, chunkBytes, 0, bytesRead);
                    _bytesRead += bytesRead;
                    ResetInactivityTimeout();
                    EmitReadChunk(new Buffer(chunkBytes), immediate: false);
                }

                IoScheduler.EndIo(lifetimePromise, null, isError: false);
            }
            catch (ObjectDisposedException) when (_destroyRequested)
            {
                IoScheduler.EndIo(lifetimePromise, null, isError: false);
            }
            catch (IOException) when (_destroyRequested)
            {
                IoScheduler.EndIo(lifetimePromise, null, isError: false);
            }
            catch (Exception ex)
            {
                _hadSocketError = true;
                IoScheduler.EndIo(
                    lifetimePromise,
                    ex as Error ?? new Error(ex.Message, ex),
                    isError: true);
            }
        }

        private void EmitReadChunk(object? chunk, bool immediate)
        {
            if (chunk == null || chunk is JsNull)
            {
                return;
            }

            if (chunk is string text && text.Length == 0)
            {
                return;
            }

            if (chunk is Buffer buffer && buffer.length == 0)
            {
                return;
            }

            void Deliver()
            {
                try
                {
                    push(chunk);
                }
                catch (Exception ex)
                {
                    _hadSocketError = true;
                    emit("error", ex as Error ?? new Error(ex.Message, ex));
                }
            }

            if (immediate)
            {
                NodeNetworkingCommon.ScheduleImmediateOnEventLoop(_nodeScheduler, Deliver);
                return;
            }

            NodeNetworkingCommon.ScheduleOnEventLoop(_nodeScheduler, Deliver);
        }

        private void FlushPendingWrites()
        {
            if (!_canWriteToStream || _stream == null)
            {
                return;
            }

            while (_pendingWrites.Count > 0 && !_destroyRequested)
            {
                var bytes = _pendingWrites.Peek();
                try
                {
                    _stream.Write(bytes, 0, bytes.Length);
                    _pendingWrites.Dequeue();
                }
                catch (ObjectDisposedException ex)
                {
                    HandleStreamWriteException(ex);
                    break;
                }
                catch (IOException ex)
                {
                    HandleStreamWriteException(ex);
                    break;
                }
                catch (InvalidOperationException ex)
                {
                    HandleStreamWriteException(ex);
                    break;
                }
                catch (Exception ex)
                {
                    HandleStreamWriteException(ex);
                    break;
                }
            }
        }

        private void HandleStreamWriteException(Exception ex)
        {
            _canWriteToStream = false;
            if (_destroyRequested)
            {
                return;
            }

            _hadSocketError = true;
            destroy(ex as Error ?? new Error(ex.Message, ex));
        }

        private void CloseOutputSide()
        {
            if (_outputClosed)
            {
                TryFinalizeClose();
                return;
            }

            _outputClosed = true;
            if (_canWriteToStream)
            {
                FlushPendingWrites();
                ShutdownSend();
            }

            TryFinalizeClose();
        }

        protected virtual void ShutdownSend()
        {
            try
            {
                _client?.Client.Shutdown(SocketShutdown.Send);
            }
            catch
            {
            }
        }

        private void FinalizeReadableSide(bool hadError)
        {
            if (_readSideEnded)
            {
                if (hadError)
                {
                    _hadSocketError = true;
                }

                TryFinalizeClose();
                return;
            }

            _readSideEnded = true;
            if (hadError)
            {
                _hadSocketError = true;
            }

            // Deliver EOF behind any already-queued data callbacks so consumers never observe
            // socket end before the final scheduled data chunk.
            NodeNetworkingCommon.ScheduleOnEventLoop(_nodeScheduler, () =>
            {
                try
                {
                    push(null);
                }
                catch
                {
                }

                if (!_allowHalfOpen && !_outputClosed && !_destroyRequested)
                {
                    NodeNetworkingCommon.ScheduleImmediateOnEventLoop(_nodeScheduler, () =>
                    {
                        if (_closeEmitted || _outputClosed || _destroyRequested)
                        {
                            TryFinalizeClose();
                            return;
                        }

                        CloseOutputSide();
                    });
                    return;
                }

                TryFinalizeClose();
            });
        }

        private void TryFinalizeClose()
        {
            if (_closeEmitted)
            {
                return;
            }

            if (!_destroyRequested)
            {
                if (!_readSideEnded)
                {
                    return;
                }

                if (!_outputClosed)
                {
                    return;
                }
            }

            CompleteClose();
        }

        private void CompleteClose()
        {
            lock (_timeoutSync)
            {
                if (_closeEmitted)
                {
                    return;
                }

                _closeEmitted = true;
            }

            ClearInactivityTimeout();
            emit("close", _hadSocketError);

            try
            {
                _stream?.Dispose();
            }
            catch
            {
            }

            try
            {
                _client?.Close();
            }
            catch
            {
            }

            _stream = null;
            _client = null;
            _connected = false;
            _canWriteToStream = false;
        }

        private void ApplyKeepAliveOption()
        {
            if (!_keepAliveConfigured || _client == null)
            {
                return;
            }

            _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, _keepAliveEnabled);
        }

        private void ResetInactivityTimeout()
        {
            NodeSchedulerState scheduler;
            object? previousHandle;
            double timeoutMilliseconds;
            long generation;

            lock (_timeoutSync)
            {
                if (_timeoutMilliseconds <= 0 || _destroyRequested || _closeEmitted)
                {
                    return;
                }

                scheduler = _nodeScheduler ?? NodeScheduler;
                previousHandle = _timeoutHandle;
                _timeoutHandle = null;
                timeoutMilliseconds = _timeoutMilliseconds;
                generation = ++_timeoutGeneration;
            }

            if (previousHandle != null)
            {
                ((IScheduler)scheduler).Cancel(previousHandle);
            }
 
            object? nextHandle = null;
            nextHandle = ((IScheduler)scheduler).Schedule(
                () => EmitTimeout(generation, nextHandle),
                TimeSpan.FromMilliseconds(timeoutMilliseconds));

            bool cancelNextHandle = false;
            lock (_timeoutSync)
            {
                if (generation != _timeoutGeneration || _destroyRequested || _closeEmitted || _timeoutMilliseconds <= 0)
                {
                    cancelNextHandle = true;
                }
                else
                {
                    _timeoutHandle = nextHandle;
                }
            }

            if (cancelNextHandle && nextHandle != null)
            {
                ((IScheduler)scheduler).Cancel(nextHandle);
            }
        }

        private void ClearInactivityTimeout()
        {
            NodeSchedulerState? scheduler;
            object? handle;
            lock (_timeoutSync)
            {
                _timeoutGeneration++;
                handle = _timeoutHandle;
                _timeoutHandle = null;
                scheduler = _nodeScheduler;
            }

            if (handle != null && scheduler != null)
            {
                ((IScheduler)scheduler).Cancel(handle);
            }
        }

        private void EmitTimeout(long generation, object? handle)
        {
            bool shouldEmit = false;
            lock (_timeoutSync)
            {
                if (generation != _timeoutGeneration || !ReferenceEquals(_timeoutHandle, handle))
                {
                    return;
                }

                _timeoutHandle = null;
                _timeoutGeneration++;
                if (!_destroyRequested && !_closeEmitted && _timeoutMilliseconds > 0)
                {
                    shouldEmit = true;
                }
            }

            if (shouldEmit)
            {
                emit("timeout");
            }
        }

        private void ParseConnectArgs(object[] args, out int port, out string host, out Delegate? callback)
        {
            port = 0;
            host = "127.0.0.1";
            callback = null;

            if (args.Length == 0)
            {
                throw new TypeError("The \"port\" argument must be specified.");
            }

            if (NodeNetworkingCommon.LooksLikeOptionsObject(args[0]))
            {
                port = NodeNetworkingCommon.CoercePort(NodeNetworkingCommon.TryGetOption(args[0], "port"));
                host = NodeNetworkingCommon.CoerceHost(
                    NodeNetworkingCommon.TryGetStringOption(args[0], "host")
                    ?? NodeNetworkingCommon.TryGetStringOption(args[0], "hostname"));
                if (args.Length > 1 && args[1] is Delegate optCallback)
                {
                    callback = optCallback;
                }
            }
            else
            {
                port = NodeNetworkingCommon.CoercePort(args[0]);
                if (args.Length > 1)
                {
                    if (args[1] is Delegate cb)
                    {
                        callback = cb;
                    }
                    else
                    {
                        host = NodeNetworkingCommon.CoerceHost(args[1]?.ToString());
                    }
                }

                if (args.Length > 2 && args[2] is Delegate thirdCallback)
                {
                    callback = thirdCallback;
                }
            }

            if (port <= 0)
            {
                throw new RangeError("The \"port\" argument must be a positive number.");
            }
        }
        private IPEndPoint? TryGetRemoteEndpoint()
            => _client?.Client.RemoteEndPoint as IPEndPoint;

        private IPEndPoint? TryGetLocalEndpoint()
            => _client?.Client.LocalEndPoint as IPEndPoint;
    }
}
