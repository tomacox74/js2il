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

Provides a practical HTTP/1.1 baseline on top of the runtime's node:net transport. Requests and responses now stream through IncomingMessage incrementally as Buffer chunks by default, use automatic chunked transfer encoding/decoding when content-length is absent, and support sequential keep-alive reuse through new http.Agent({ keepAlive: true }) flows. Unsupported advanced behaviors such as CONNECT tunneling, Upgrade/WebSocket handshakes, Expect/100-continue, HTTP pipelining, HTTP/2, and HTTPS URLs currently fail explicitly or remain out of scope.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| createServer([requestListener]) | function | supported | [docs](https://nodejs.org/api/http.html#httpcreateserveroptions-requestlistener) |
| request(options[, callback]) | function | supported | [docs](https://nodejs.org/api/http.html#httprequestoptions-callback) |
| get(url[, callback]) | function | supported | [docs](https://nodejs.org/api/http.html#httpgetoptions-callback) |
| IncomingMessage | class | partial | [docs](https://nodejs.org/api/http.html#class-httpincomingmessage) |
| ServerResponse | class | partial | [docs](https://nodejs.org/api/http.html#class-httpserverresponse) |
| Server | class | partial | [docs](https://nodejs.org/api/http.html#class-httpserver) |
| Agent | class | partial | [docs](https://nodejs.org/api/http.html#class-httpagent) |

## API Details

### createServer([requestListener])

Creates an HTTP server whose request listener receives IncomingMessage and ServerResponse objects. Requests are parsed as HTTP/1.1 over node:net, request bodies stream incrementally, and unsupported CONNECT/upgrade/Expect requests are rejected explicitly rather than partially emulated.

**Tests:**
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_CreateServer_Get_Loopback` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Request_Post_Basic` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Request_Streaming_Chunked_RequestBody` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Response_Streaming_Chunked_ResponseBody` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Agent_KeepAlive_Reuses_Connection` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_CreateServer_Get_Loopback` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Request_Post_Basic` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Request_Streaming_Chunked_RequestBody` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Response_Streaming_Chunked_ResponseBody` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Agent_KeepAlive_Reuses_Connection` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)

### request(options[, callback])

Supports host/hostname, port, path, method, headers, and agent options. Request bodies stream incrementally through write()/end(); when content-length is absent the runtime emits chunked framing. agent:false disables pooling, while new http.Agent({ keepAlive: true }) enables sequential socket reuse per host:port. CONNECT, Upgrade, and Expect/100-continue requests throw explicit errors.

**Tests:**
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Request_Post_Basic` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Request_Streaming_Chunked_RequestBody` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Agent_KeepAlive_Reuses_Connection` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Request_Unsupported_Connect_Fails_Clearly` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Request_Post_Basic` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Request_Streaming_Chunked_RequestBody` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Agent_KeepAlive_Reuses_Connection` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Request_Unsupported_Connect_Fails_Clearly` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)

### get(url[, callback])

Supports absolute http:// URLs or request-options objects and automatically ends the request. The response callback fires as soon as headers are parsed, and the returned IncomingMessage then streams body chunks incrementally.

**Tests:**
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_CreateServer_Get_Loopback` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Response_Streaming_Chunked_ResponseBody` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Agent_KeepAlive_Reuses_Connection` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_CreateServer_Get_Loopback` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Response_Streaming_Chunked_ResponseBody` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Agent_KeepAlive_Reuses_Connection` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)

### IncomingMessage

Readable/EventEmitter-backed request or response object exposing method/url/statusCode/statusMessage/httpVersion/headers/socket/complete. Body data streams incrementally as Buffer chunks by default; inherited setEncoding('utf8') converts buffered and future chunks into text.

**Tests:**
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_CreateServer_Get_Loopback` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Request_Post_Basic` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Request_Streaming_Chunked_RequestBody` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Response_Streaming_Chunked_ResponseBody` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_CreateServer_Get_Loopback` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Request_Post_Basic` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Request_Streaming_Chunked_RequestBody` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Response_Streaming_Chunked_ResponseBody` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)

### ServerResponse

Writable/EventEmitter-backed server response supporting statusCode/statusMessage, setHeader(), getHeader(), writeHead(), write(), and end(). Responses stream incrementally, use chunked transfer encoding automatically when content-length is absent, keep supported HTTP/1.1 connections open when framing allows reuse, and suppress bodies for 1xx/204/304 statuses.

**Tests:**
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_CreateServer_Get_Loopback` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Request_Post_Basic` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Response_Streaming_Chunked_ResponseBody` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Agent_KeepAlive_Reuses_Connection` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_CreateServer_Get_Loopback` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Request_Post_Basic` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Response_Streaming_Chunked_ResponseBody` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Agent_KeepAlive_Reuses_Connection` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)

### Server

EventEmitter-backed HTTP server with listen(), address(), close(), and forwarded connection events delegated to the underlying node:net server. Supports sequential keep-alive requests over a connection in the current slice; HTTP pipelining and protocol upgrades remain unsupported.

**Tests:**
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_CreateServer_Get_Loopback` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Request_Post_Basic` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Agent_KeepAlive_Reuses_Connection` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_CreateServer_Get_Loopback` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Request_Post_Basic` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Agent_KeepAlive_Reuses_Connection` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)

### Agent

EventEmitter-backed connection pool. new http.Agent({ keepAlive: true }) reuses sequential client sockets per host:port; keepAlive defaults to false, and advanced scheduling/maxSockets options are not implemented yet.

**Tests:**
- `Js2IL.Tests.Node.Http.ExecutionTests.Http_Agent_KeepAlive_Reuses_Connection` (`tests/Js2IL.Tests/Node/Http/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Http.GeneratorTests.Http_Agent_KeepAlive_Reuses_Connection` (`tests/Js2IL.Tests/Node/Http/GeneratorTests.cs`)
