# Module: http

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/http.html) |

## Implementation

- `src/JavaScriptRuntime/Node/Http.cs`
- `src/JavaScriptRuntime/Node/Net.cs`

## Notes

Provides a focused HTTP/1.1 baseline for loopback and local-integration scenarios. The current slice supports createServer(), request(), and get() on top of the runtime's minimal node:net transport, with EventEmitter-backed request/response objects, Content-Length framing, and connection-close completion semantics. Chunked transfer encoding, keep-alive/agents, upgrade handling, HTTPS URLs, and broader streaming/header edge cases are not implemented yet.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| createServer([requestListener]) | function | supported | [docs](https://nodejs.org/api/http.html#httpcreateserveroptions-requestlistener) |
| request(options[, callback]) | function | supported | [docs](https://nodejs.org/api/http.html#httprequestoptions-callback) |
| get(url[, callback]) | function | supported | [docs](https://nodejs.org/api/http.html#httpgetoptions-callback) |
| IncomingMessage | class | partial | [docs](https://nodejs.org/api/http.html#class-httpincomingmessage) |
| ServerResponse | class | partial | [docs](https://nodejs.org/api/http.html#class-httpserverresponse) |
| Server | class | partial | [docs](https://nodejs.org/api/http.html#class-httpserver) |

## API Details

### createServer([requestListener])

Creates an HTTP server whose request listener receives IncomingMessage and ServerResponse objects. Requests are parsed as HTTP/1.1 over the minimal node:net transport; body data is currently delivered as a single UTF-8 chunk followed by end.

**Tests:**
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_CreateServer_Get_Loopback` (`Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Request_Post_Basic` (`Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_CreateServer_Get_Loopback` (`Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Request_Post_Basic` (`Js2IL.Tests/Node/Http/GeneratorTests.cs`)

### request(options[, callback])

Supports host/hostname, port, path, method, and headers options. Request bodies may be written with write()/end(); the response callback fires once the full response headers/body have been buffered.

**Tests:**
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Request_Post_Basic` (`Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Request_Post_Basic` (`Js2IL.Tests/Node/Http/GeneratorTests.cs`)

### get(url[, callback])

Supports absolute http:// URLs and automatically ends the request. The response callback receives a buffered IncomingMessage response.

**Tests:**
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_CreateServer_Get_Loopback` (`Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_CreateServer_Get_Loopback` (`Js2IL.Tests/Node/Http/GeneratorTests.cs`)

### IncomingMessage

Readable/EventEmitter-backed request or response object exposing method/url/statusCode/statusMessage/httpVersion/headers/socket. Body data is currently surfaced as a single UTF-8 text chunk followed by end.

**Tests:**
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_CreateServer_Get_Loopback` (`Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Request_Post_Basic` (`Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_CreateServer_Get_Loopback` (`Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Request_Post_Basic` (`Js2IL.Tests/Node/Http/GeneratorTests.cs`)

### ServerResponse

Writable/EventEmitter-backed server response supporting statusCode/statusMessage, setHeader(), getHeader(), writeHead(), write(), and end(). Responses are emitted as HTTP/1.1 with Content-Length and connection-close semantics.

**Tests:**
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_CreateServer_Get_Loopback` (`Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Request_Post_Basic` (`Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_CreateServer_Get_Loopback` (`Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Request_Post_Basic` (`Js2IL.Tests/Node/Http/GeneratorTests.cs`)

### Server

EventEmitter-backed HTTP server with listen(), address(), and close() delegated to the minimal node:net server implementation. Intended for local loopback scenarios in the current slice.

**Tests:**
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_CreateServer_Get_Loopback` (`Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Request_Post_Basic` (`Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_CreateServer_Get_Loopback` (`Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Request_Post_Basic` (`Js2IL.Tests/Node/Http/GeneratorTests.cs`)
