# Module: events

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/events.html) |

## Implementation

- `JavaScriptRuntime/Node/Events.cs, JavaScriptRuntime/Node/EventEmitter.cs`

## Notes

Initial events module baseline focused on EventEmitter instance listener lifecycle and emission behavior. Advanced APIs (errorMonitor, setMaxListeners/getMaxListeners, prepend* variants, rawListeners, async iterator helpers) are not implemented yet.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| EventEmitter | class | supported | [docs](https://nodejs.org/api/events.html#class-eventemitter) |

## API Details

### EventEmitter

Supports baseline listener APIs for common patterns: on/addListener, once, off/removeListener, emit (including argument forwarding), listenerCount, and removeAllListeners.

**Tests:**
- `Js2IL.Tests.Node.Events.ExecutionTests.Events_EventEmitter_On_Off_Once` (`Js2IL.Tests/Node/Events/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Events.GeneratorTests.Events_EventEmitter_On_Off_Once` (`Js2IL.Tests/Node/Events/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Events.ExecutionTests.Events_EventEmitter_Emit_Args` (`Js2IL.Tests/Node/Events/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Events.GeneratorTests.Events_EventEmitter_Emit_Args` (`Js2IL.Tests/Node/Events/GeneratorTests.cs`)
