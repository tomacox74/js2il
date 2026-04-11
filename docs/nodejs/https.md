# Module: https

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/https.html) |

## Implementation

- `src/JavaScriptRuntime/Node/Https.cs`

## Notes

Provides a practical HTTPS baseline by reusing the runtime's streamed/chunked node:http request-response pipeline over TLS-backed sockets. The supported slice covers PEM-backed server certificates for loopback/local integration, https.createServer(...), https.request(...), and https.get(...) with the existing IncomingMessage/ServerResponse object model. Advanced TLS/client options such as https.Agent pooling, custom CAs, client certificates, ALPN, and broader TLS tuning remain unsupported and fail clearly where they are currently surfaced.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| createServer(...) | function | partial | [docs](https://nodejs.org/api/https.html#httpscreateserveroptions-requestlistener) |
| request(...) | function | partial | [docs](https://nodejs.org/api/https.html#httpsrequestoptions-callback) |
| get(...) | function | partial | [docs](https://nodejs.org/api/https.html#httpsgetoptions-callback) |
| Server | class | partial | [docs](https://nodejs.org/api/https.html#class-httpsserver) |

## API Details

### createServer(...)

Creates an HTTPS server that reuses the node:http request/response lifecycle over TLS sockets. The supported slice accepts PEM key/cert material (or a tls.createSecureContext(...) carrying that material) for loopback/local integration scenarios.

**Tests:**
- `Js2IL.Tests.Node.Https.ExecutionTests.Https_Get_Loopback_SelfSigned` (`tests/Js2IL.Tests/Node/Https/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Https.ExecutionTests.Https_Request_Post_Basic` (`tests/Js2IL.Tests/Node/Https/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Https.GeneratorTests.Https_Get_Loopback_SelfSigned` (`tests/Js2IL.Tests/Node/Https/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Https.GeneratorTests.Https_Request_Post_Basic` (`tests/Js2IL.Tests/Node/Https/GeneratorTests.cs`)

### request(...)

Issues HTTPS client requests on port 443 by default, preserving the existing node:http ClientRequest/IncomingMessage object model. The current baseline supports object or https:// / WHATWG URL inputs, including URL-plus-options call shapes where second-argument overrides still control headers, path, and rejectUnauthorized: false for local self-signed test scenarios; custom CA trust, secureContext on the client side, and https.Agent pooling are not yet implemented.

**Tests:**
- `Js2IL.Tests.Node.Https.ExecutionTests.Https_Request_Post_Basic` (`tests/Js2IL.Tests/Node/Https/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Https.GeneratorTests.Https_Request_Post_Basic` (`tests/Js2IL.Tests/Node/Https/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Https.ExecutionTests.Https_Request_UrlObject_WithOptions` (`tests/Js2IL.Tests/Node/Https/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Https.GeneratorTests.Https_Request_UrlObject_WithOptions` (`tests/Js2IL.Tests/Node/Https/GeneratorTests.cs`)

### get(...)

Convenience wrapper over https.request(...) that ends the request immediately. The supported slice covers loopback/local GET requests against PEM-backed HTTPS servers with rejectUnauthorized: false for self-signed local certificates.

**Tests:**
- `Js2IL.Tests.Node.Https.ExecutionTests.Https_Get_Loopback_SelfSigned` (`tests/Js2IL.Tests/Node/Https/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Https.GeneratorTests.Https_Get_Loopback_SelfSigned` (`tests/Js2IL.Tests/Node/Https/GeneratorTests.cs`)

### Server

EventEmitter-backed HTTPS server that composes the tls server baseline with the existing node:http request parser and response writer. request, connection, secureConnection, listen, address, and close are wired for the supported local TLS slice.

**Tests:**
- `Js2IL.Tests.Node.Https.ExecutionTests.Https_Get_Loopback_SelfSigned` (`tests/Js2IL.Tests/Node/Https/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Https.ExecutionTests.Https_Request_Post_Basic` (`tests/Js2IL.Tests/Node/Https/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Https.GeneratorTests.Https_Get_Loopback_SelfSigned` (`tests/Js2IL.Tests/Node/Https/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Https.GeneratorTests.Https_Request_Post_Basic` (`tests/Js2IL.Tests/Node/Https/GeneratorTests.cs`)
