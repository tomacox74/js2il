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

Provides a focused TCP baseline for IPv4 loopback scenarios. The current slice supports createServer(), connect()/createConnection(), Server.listen()/address()/close(), and Socket write()/end()/destroy() with EventEmitter data/end/close/connect events. Socket chunks are surfaced as UTF-8 text; advanced socket controls (pause/resume, setEncoding, ref/unref, keepAlive, timeouts, half-open tuning, and binary framing controls) are not implemented yet.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| createServer([connectionListener]) | function | supported | [docs](https://nodejs.org/api/net.html#netcreateserveroptions-connectionlistener) |
| connect(...) | function | supported | [docs](https://nodejs.org/api/net.html#netconnect) |
| createConnection(...) | function | supported | [docs](https://nodejs.org/api/net.html#netcreateconnection) |
| Server | class | partial | [docs](https://nodejs.org/api/net.html#class-netserver) |
| Socket | class | partial | [docs](https://nodejs.org/api/net.html#class-netsocket) |

## API Details

### createServer([connectionListener])

Creates an EventEmitter-backed TCP server. The connection listener receives a Socket instance once the server accepts a client.

**Tests:**
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_CreateServer_Connect_Basic` (`Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_CreateServer_Connect_Basic` (`Js2IL.Tests/Node/Net/GeneratorTests.cs`)

### connect(...)

Supports port/host/callback and { port, host/hostname } option shapes for IPv4 TCP connections. Returns a Socket instance immediately and emits connect when the TCP handshake completes.

**Tests:**
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_CreateServer_Connect_Basic` (`Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_CreateServer_Connect_Basic` (`Js2IL.Tests/Node/Net/GeneratorTests.cs`)

### createConnection(...)

Alias of connect(...).

**Tests:**
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_CreateServer_Connect_Basic` (`Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_CreateServer_Connect_Basic` (`Js2IL.Tests/Node/Net/GeneratorTests.cs`)

### Server

EventEmitter-backed TCP server supporting listening/close lifecycles, address() introspection, and connection events for accepted sockets.

**Tests:**
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_CreateServer_Connect_Basic` (`Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_CreateServer_Connect_Basic` (`Js2IL.Tests/Node/Net/GeneratorTests.cs`)

### Socket

Duplex/EventEmitter-backed TCP socket supporting connect, write, end, destroy, remote/local address properties, and data/end/close/connect events. Read chunks are currently surfaced as UTF-8 strings.

**Tests:**
- `Js2IL.Tests.Node.Net.ExecutionTests.Net_CreateServer_Connect_Basic` (`Js2IL.Tests/Node/Net/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Net.GeneratorTests.Net_CreateServer_Connect_Basic` (`Js2IL.Tests/Node/Net/GeneratorTests.cs`)
