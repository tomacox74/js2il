# Module: https

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | not-supported |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/https.html) |

## Implementation

- `src/JavaScriptRuntime/Node/Https.cs`

## Notes

The current runtime exposes explicit diagnostics for node:https but does not implement TLS-backed HTTP yet. Calls to createServer(), request(), or get() throw a runtime Error explaining that only the non-TLS node:http baseline is currently available.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| createServer(...) | function | not-supported | [docs](https://nodejs.org/api/https.html#httpscreateserveroptions-requestlistener) |
| request(...) | function | not-supported | [docs](https://nodejs.org/api/https.html#httpsrequestoptions-callback) |
| get(...) | function | not-supported | [docs](https://nodejs.org/api/https.html#httpsgetoptions-callback) |

## API Details

### createServer(...)

Throws a runtime Error noting that TLS-backed HTTP servers are not implemented yet.

### request(...)

Throws a runtime Error noting that HTTPS client requests are not implemented yet.

### get(...)

Throws a runtime Error noting that HTTPS client requests are not implemented yet.
