# Module: net

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/net.html) |

## Implementation

- `src/JavaScriptRuntime/Node/Net.cs`

## Notes

Provides a focused TCP baseline for IPv4 loopback scenarios. Sockets emit Buffer chunks by default, support inherited pause()/resume()/utf8 setEncoding(), expose idle timeouts via setTimeout(), allow keepAlive enable/disable via setKeepAlive(), accept setNoDelay() as a compatibility no-op, and honor allowHalfOpen on accepted server sockets so delayed responses can complete before local shutdown. Advanced socket controls such as keepAlive initialDelay, non-UTF-8 encodings, ref/unref, and broader non-loopback or non-IPv4 coverage are still not implemented.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| createServer([options][, connectionListener]) | function | supported | [docs](https://nodejs.org/api/net.html#netcreateserveroptions-connectionlistener) |
| connect(...) | function | supported | [docs](https://nodejs.org/api/net.html#netconnect) |
| createConnection(...) | function | supported | [docs](https://nodejs.org/api/net.html#netcreateconnection) |
| Server | class | partial | [docs](https://nodejs.org/api/net.html#class-netserver) |
| Socket | class | partial | [docs](https://nodejs.org/api/net.html#class-netsocket) |
| Socket.prototype.setEncoding(encoding) | method | supported | [docs](https://nodejs.org/api/net.html#socketsetencodingencoding) |
| Socket.prototype.setTimeout(timeout[, callback]) | method | supported | [docs](https://nodejs.org/api/net.html#socketsettimeouttimeout-callback) |
| Socket.prototype.setKeepAlive([enable[, initialDelay]]) | method | partial | [docs](https://nodejs.org/api/net.html#socketsetkeepaliveenable-initialdelay) |
| Socket.prototype.setNoDelay([noDelay]) | method | partial | [docs](https://nodejs.org/api/net.html#socketsetnodelaynodelay) |

## API Details

### createServer([options][, connectionListener])

Creates an EventEmitter-backed TCP server. The connection listener receives a Socket instance once the server accepts a client. The current slice supports allowHalfOpen on accepted sockets; broader server socket options are still deferred.

**Tests:**
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_CreateServer_Connect_Basic` (`tests/Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_CreateServer_Connect_Basic` (`tests/Js2IL.Tests/Node/Net/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_CreateServer_AllowHalfOpen_Delayed_Response` (`tests/Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_CreateServer_AllowHalfOpen_Delayed_Response` (`tests/Js2IL.Tests/Node/Net/GeneratorTests.cs`)

### connect(...)

Supports port/host/callback and { port, host/hostname } option shapes for IPv4 TCP connections. Returns a Socket instance immediately, emits connect when the TCP handshake completes, and surfaces Buffer reads unless text mode is enabled with setEncoding('utf8').

**Tests:**
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_CreateServer_Connect_Basic` (`tests/Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_CreateServer_Connect_Basic` (`tests/Js2IL.Tests/Node/Net/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_Socket_Binary_Data_Defaults_To_Buffer` (`tests/Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_Socket_Binary_Data_Defaults_To_Buffer` (`tests/Js2IL.Tests/Node/Net/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_Socket_SetEncoding_Utf8` (`tests/Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_Socket_SetEncoding_Utf8` (`tests/Js2IL.Tests/Node/Net/GeneratorTests.cs`)

### createConnection(...)

Alias of connect(...).

**Tests:**
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_CreateServer_Connect_Basic` (`tests/Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_CreateServer_Connect_Basic` (`tests/Js2IL.Tests/Node/Net/GeneratorTests.cs`)

### Server

EventEmitter-backed TCP server supporting listening/close lifecycles, address() introspection, connection events for accepted sockets, and allowHalfOpen-delayed response flows.

**Tests:**
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_CreateServer_Connect_Basic` (`tests/Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_CreateServer_Connect_Basic` (`tests/Js2IL.Tests/Node/Net/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_CreateServer_AllowHalfOpen_Delayed_Response` (`tests/Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_CreateServer_AllowHalfOpen_Delayed_Response` (`tests/Js2IL.Tests/Node/Net/GeneratorTests.cs`)

### Socket

Duplex/EventEmitter-backed TCP socket supporting connect, write, end, destroy, remote/local address properties, Buffer-by-default data events, utf8 setEncoding(), setTimeout(), setKeepAlive(enable), setNoDelay() compatibility no-op calls, and allowHalfOpen delayed responses. Unsupported keepAlive initialDelay requests return an explicit runtime diagnostic.

**Tests:**
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_CreateServer_Connect_Basic` (`tests/Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_CreateServer_Connect_Basic` (`tests/Js2IL.Tests/Node/Net/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_Socket_Binary_Data_Defaults_To_Buffer` (`tests/Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_Socket_Binary_Data_Defaults_To_Buffer` (`tests/Js2IL.Tests/Node/Net/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_Socket_SetEncoding_Utf8` (`tests/Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_Socket_SetEncoding_Utf8` (`tests/Js2IL.Tests/Node/Net/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_Socket_Timeout_Allows_Graceful_End` (`tests/Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_Socket_Timeout_Allows_Graceful_End` (`tests/Js2IL.Tests/Node/Net/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_CreateServer_AllowHalfOpen_Delayed_Response` (`tests/Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_CreateServer_AllowHalfOpen_Delayed_Response` (`tests/Js2IL.Tests/Node/Net/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_Socket_KeepAlive_And_Unsupported_Options` (`tests/Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_Socket_KeepAlive_And_Unsupported_Options` (`tests/Js2IL.Tests/Node/Net/GeneratorTests.cs`)

### Socket.prototype.setEncoding(encoding)

Opts a socket into text mode. The current slice supports utf8 only and preserves streaming multibyte decoding across socket reads.

**Tests:**
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_Socket_SetEncoding_Utf8` (`tests/Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_Socket_SetEncoding_Utf8` (`tests/Js2IL.Tests/Node/Net/GeneratorTests.cs`)

### Socket.prototype.setTimeout(timeout[, callback])

Schedules timeout notifications for idle sockets without destroying the connection automatically. The timeout callback is wired as a one-time timeout listener in the supported slice.

**Tests:**
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_Socket_Timeout_Allows_Graceful_End` (`tests/Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_Socket_Timeout_Allows_Graceful_End` (`tests/Js2IL.Tests/Node/Net/GeneratorTests.cs`)

### Socket.prototype.setKeepAlive([enable[, initialDelay]])

Supports enabling or disabling OS-level TCP keepAlive on the underlying socket. Nonzero initialDelay values return an explicit runtime diagnostic.

**Tests:**
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_Socket_KeepAlive_And_Unsupported_Options` (`tests/Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_Socket_KeepAlive_And_Unsupported_Options` (`tests/Js2IL.Tests/Node/Net/GeneratorTests.cs`)

### Socket.prototype.setNoDelay([noDelay])

Accepted as a compatibility no-op so common zero-argument callers do not fail, but the current runtime does not model TCP no-delay state.

**Tests:**
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_Socket_KeepAlive_And_Unsupported_Options` (`tests/Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_Socket_KeepAlive_And_Unsupported_Options` (`tests/Js2IL.Tests/Node/Net/GeneratorTests.cs`)
