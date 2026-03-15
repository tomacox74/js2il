# Module: tls

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/tls.html) |

## Implementation

- `src/JavaScriptRuntime/Node/Https.cs`

## Notes

Provides a practical TLS baseline over .NET SslStream for loopback/local integration scenarios. The supported slice covers PEM-backed server credentials via tls.createSecureContext(...), tls.createServer(...), and tls.connect(...) with TLSSocket instances, plus rejectUnauthorized: false for local self-signed client flows. Custom CA trust, client certificates, ALPN, and broader TLS/OpenSSL option parity are still unsupported and fail clearly where surfaced.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| createServer(...) | function | partial | [docs](https://nodejs.org/api/tls.html#tlscreateserveroptions-secureconnectionlistener) |
| connect(...) | function | partial | [docs](https://nodejs.org/api/tls.html#tlsconnectoptions-callback) |
| createSecureContext([options]) | function | partial | [docs](https://nodejs.org/api/tls.html#tlscreatesecurecontextoptions) |
| Server | class | partial | [docs](https://nodejs.org/api/tls.html#class-tlsserver) |
| TLSSocket | class | partial | [docs](https://nodejs.org/api/tls.html#class-tlstlssocket) |

## API Details

### createServer(...)

Creates a TLS server that accepts PEM-backed key/cert material directly or through a tls.createSecureContext(...) result. The server emits secureConnection once the TLS handshake succeeds for the supported loopback/local slice.

**Tests:**
- `Js2IL.Tests.Node.Https.ExecutionTests.Tls_CreateServer_Connect_Basic` (`Js2IL.Tests/Node/Https/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Https.ExecutionTests.Tls_CreateSecureContext_Server_Handshake` (`Js2IL.Tests/Node/Https/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Https.ExecutionTests.Tls_CreateServer_Unsupported_RequestCert_Fails_Clearly` (`Js2IL.Tests/Node/Https/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Https.GeneratorTests.Tls_CreateServer_Connect_Basic` (`Js2IL.Tests/Node/Https/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Https.GeneratorTests.Tls_CreateSecureContext_Server_Handshake` (`Js2IL.Tests/Node/Https/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Https.GeneratorTests.Tls_CreateServer_Unsupported_RequestCert_Fails_Clearly` (`Js2IL.Tests/Node/Https/GeneratorTests.cs`)

### connect(...)

Creates a TLSSocket client connection and emits secureConnect once the TLS handshake succeeds. The current baseline supports host/port option shapes plus rejectUnauthorized: false for self-signed local certificates.

**Tests:**
- `Js2IL.Tests.Node.Https.ExecutionTests.Tls_CreateServer_Connect_Basic` (`Js2IL.Tests/Node/Https/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Https.ExecutionTests.Tls_CreateSecureContext_Server_Handshake` (`Js2IL.Tests/Node/Https/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Https.GeneratorTests.Tls_CreateServer_Connect_Basic` (`Js2IL.Tests/Node/Https/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Https.GeneratorTests.Tls_CreateSecureContext_Server_Handshake` (`Js2IL.Tests/Node/Https/GeneratorTests.cs`)

### createSecureContext([options])

Creates a minimal SecureContext carrying PEM key/cert material for the supported server-side TLS baseline. Empty contexts are allowed, but the current runtime requires key/cert material before they can back tls.createServer(...) or https.createServer(...).

**Tests:**
- `Js2IL.Tests.Node.Https.ExecutionTests.Tls_CreateSecureContext_Server_Handshake` (`Js2IL.Tests/Node/Https/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Https.RuntimeTests.Tls_CreateSecureContext_KeyWithoutCert_ThrowsClearError` (`Js2IL.Tests/Node/Https/RuntimeTests.cs`)
- `Js2IL.Tests.Node.Https.GeneratorTests.Tls_CreateSecureContext_Server_Handshake` (`Js2IL.Tests/Node/Https/GeneratorTests.cs`)

### Server

EventEmitter-backed TLS server supporting listen(), address(), close(), connection, and secureConnection in the supported local TLS slice.

**Tests:**
- `Js2IL.Tests.Node.Https.ExecutionTests.Tls_CreateServer_Connect_Basic` (`Js2IL.Tests/Node/Https/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Https.ExecutionTests.Tls_CreateSecureContext_Server_Handshake` (`Js2IL.Tests/Node/Https/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Https.GeneratorTests.Tls_CreateServer_Connect_Basic` (`Js2IL.Tests/Node/Https/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Https.GeneratorTests.Tls_CreateSecureContext_Server_Handshake` (`Js2IL.Tests/Node/Https/GeneratorTests.cs`)

### TLSSocket

Duplex/EventEmitter-backed TLS socket with encrypted, authorized, authorizationError, data/end/close/error behavior, and secureConnect on client connections once the handshake completes.

**Tests:**
- `Js2IL.Tests.Node.Https.ExecutionTests.Tls_CreateServer_Connect_Basic` (`Js2IL.Tests/Node/Https/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Https.GeneratorTests.Tls_CreateServer_Connect_Basic` (`Js2IL.Tests/Node/Https/GeneratorTests.cs`)
