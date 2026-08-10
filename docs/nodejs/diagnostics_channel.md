# Module: diagnostics_channel

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 24.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/diagnostics_channel.html) |

## Implementation

- `src/JavaScriptRuntime/Node/DiagnosticsChannel.cs`
- `src/JavaScriptRuntime/Node/Contracts/IDiagnosticsChannelModule.Generated.cs`

## Notes

Provides the low-overhead named in-process channels used by Undici. Named channels preserve identity, `hasSubscribers` avoids publish work when no listener is installed, and publishing calls subscribers synchronously. The experimental tracing and AsyncLocalStorage store APIs remain explicit unavailable contract members.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| channel(name), hasSubscribers(name), subscribe(name, onMessage), and unsubscribe(name, onMessage) | function | supported | [docs](https://nodejs.org/api/diagnostics_channel.html) |
| Channel.publish(message) | function | supported | [docs](https://nodejs.org/api/diagnostics_channel.html#channelpublishmessage) |
| tracingChannel(), TracingChannel, bindStore(), unbindStore(), and runStores() | function | not-supported | [docs](https://nodejs.org/api/diagnostics_channel.html) |

## API Details

### channel(name), hasSubscribers(name), subscribe(name, onMessage), and unsubscribe(name, onMessage)

Supports string and symbol names, module-level and channel-instance subscription management, reusable channel identity, and listener snapshotting during synchronous publish.

**Tests:**
- `Jroc.Tests.Node.DiagnosticsChannel.ExecutionTests.Require_DiagnosticsChannel` (`tests/Jroc.Tests/Node/DiagnosticsChannel/ExecutionTests.cs`)
- `Jroc.Tests.Node.DiagnosticsChannel.GeneratorTests.Require_DiagnosticsChannel` (`tests/Jroc.Tests/Node/DiagnosticsChannel/GeneratorTests.cs`)

### Channel.publish(message)

Calls the subscriber snapshot with `(message, channelName)` synchronously; the no-subscriber path performs no callback or snapshot allocation.

### tracingChannel(), TracingChannel, bindStore(), unbindStore(), and runStores()

These Node 24 APIs remain present in the complete generated contract and fail explicitly.
