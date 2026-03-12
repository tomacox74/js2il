# Module: tls

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | not-supported |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/tls.html) |

## Implementation

- `JavaScriptRuntime/Node/Https.cs`

## Notes

The runtime currently exposes explicit diagnostics for node:tls but does not implement secure contexts, TLS handshakes, or TLSSocket semantics yet.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| createServer(...) | function | not-supported | [docs](https://nodejs.org/api/tls.html#tlscreateserveroptions-secureconnectionlistener) |
| connect(...) | function | not-supported | [docs](https://nodejs.org/api/tls.html#tlsconnectoptions-callback) |
| createSecureContext([options]) | function | not-supported | [docs](https://nodejs.org/api/tls.html#tlscreatesecurecontextoptions) |

## API Details

### createServer(...)

Throws a runtime Error noting that TLS servers are not implemented yet.

### connect(...)

Throws a runtime Error noting that TLS client sockets are not implemented yet.

### createSecureContext([options])

Throws a runtime Error noting that secure context creation is not implemented yet.
