# Module: events

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | completed |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/events.html) |

## Implementation

- `JavaScriptRuntime/Node/Events.cs, JavaScriptRuntime/Node/EventEmitter.cs`

## Notes

Complete EventEmitter implementation for event-driven programming patterns. Supports core instance listener lifecycle and emission behavior APIs plus events.errorMonitor and async helper APIs (events.on/events.once). Advanced features like captureRejections and newListener/removeListener event semantics are not implemented.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| EventEmitter | class | supported | [docs](https://nodejs.org/api/events.html#class-eventemitter) |

## API Details

### EventEmitter

Comprehensive EventEmitter implementation supporting all core listener lifecycle APIs: on/addListener, once, off/removeListener, emit (with argument forwarding), listenerCount, removeAllListeners, prependListener, prependOnceListener, eventNames, listeners, rawListeners, setMaxListeners, getMaxListeners, and errorMonitor behavior for error events.

**Tests:**
- `Js2IL.Tests.Node.Events.ExecutionTests.Events_EventEmitter_On_Off_Once` (`Js2IL.Tests/Node/Events/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Events.GeneratorTests.Events_EventEmitter_On_Off_Once` (`Js2IL.Tests/Node/Events/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Events.ExecutionTests.Events_EventEmitter_Emit_Args` (`Js2IL.Tests/Node/Events/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Events.GeneratorTests.Events_EventEmitter_Emit_Args` (`Js2IL.Tests/Node/Events/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Events.ExecutionTests.Events_EventEmitter_Complete` (`Js2IL.Tests/Node/Events/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Events.GeneratorTests.Events_EventEmitter_Complete` (`Js2IL.Tests/Node/Events/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Events.ExecutionTests.Events_EventEmitter_ErrorMonitor` (`Js2IL.Tests/Node/Events/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Events.GeneratorTests.Events_EventEmitter_ErrorMonitor` (`Js2IL.Tests/Node/Events/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Events.ExecutionTests.Events_AsyncHelpers_On_Once` (`Js2IL.Tests/Node/Events/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Events.GeneratorTests.Events_AsyncHelpers_On_Once` (`Js2IL.Tests/Node/Events/GeneratorTests.cs`)
