# Module: timers/promises

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/timers.html#timers-promises-api) |

## Implementation

- `src/JavaScriptRuntime/Node/TimersPromises.cs`
- `src/JavaScriptRuntime/Abort.cs`

## Notes

This baseline exposes the Promise-based one-shot timer helpers on top of the existing Node scheduler/event-loop implementation, so ordering continues to match process.nextTick, Promise microtasks, setImmediate, and timer-global behavior. Minimal AbortController/AbortSignal support is included for the timers/promises cancellation path.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| setTimeout(delay[, value[, options]]) | function | supported | [docs](https://nodejs.org/api/timers.html#timerspromisessettimeoutdelay-value-options) |
| setImmediate([value[, options]]) | function | supported | [docs](https://nodejs.org/api/timers.html#timerspromisessetimmediatevalue-options) |
| setInterval(delay[, value[, options]]) | function | not-supported | [docs](https://nodejs.org/api/timers.html#timerspromisessetintervaldelay-value-options) |

## API Details

### setTimeout(delay[, value[, options]])

Returns a Promise that resolves with the supplied value after the existing timer scheduler fires. Supports options.signal cancellation and rejects with AbortError/ABORT_ERR when aborted before resolution.

**Tests:**
- `Js2IL.Tests.Node.TimersPromises.ExecutionTests.TimersPromises_SetTimeout_AwaitsValue` (`Js2IL.Tests/Node/TimersPromises/ExecutionTests.cs`)
- `Js2IL.Tests.Node.TimersPromises.GeneratorTests.TimersPromises_SetTimeout_AwaitsValue` (`Js2IL.Tests/Node/TimersPromises/GeneratorTests.cs`)
- `Js2IL.Tests.Node.TimersPromises.ExecutionTests.TimersPromises_Abort_RejectsSupportedOneShotApis` (`Js2IL.Tests/Node/TimersPromises/ExecutionTests.cs`)
- `Js2IL.Tests.Node.TimersPromises.ExecutionTests.TimersPromises_Ordering_WithNextTick_AndPromiseMicrotasks` (`Js2IL.Tests/Node/TimersPromises/ExecutionTests.cs`)

### setImmediate([value[, options]])

Returns a Promise that resolves during the existing immediate phase with the supplied value. Supports options.signal cancellation and rejects with AbortError/ABORT_ERR when aborted before resolution.

**Tests:**
- `Js2IL.Tests.Node.TimersPromises.ExecutionTests.TimersPromises_SetImmediate_AwaitsValue` (`Js2IL.Tests/Node/TimersPromises/ExecutionTests.cs`)
- `Js2IL.Tests.Node.TimersPromises.GeneratorTests.TimersPromises_SetImmediate_AwaitsValue` (`Js2IL.Tests/Node/TimersPromises/GeneratorTests.cs`)
- `Js2IL.Tests.Node.TimersPromises.ExecutionTests.TimersPromises_Abort_RejectsSupportedOneShotApis` (`Js2IL.Tests/Node/TimersPromises/ExecutionTests.cs`)
- `Js2IL.Tests.Node.TimersPromises.ExecutionTests.TimersPromises_Ordering_WithNextTick_AndPromiseMicrotasks` (`Js2IL.Tests/Node/TimersPromises/ExecutionTests.cs`)

### setInterval(delay[, value[, options]])

The async-iterator contract is deferred for now. Calls currently reject with a clear runtime error instead of exposing a partial iterator surface.

**Tests:**
- `Js2IL.Tests.Node.TimersPromises.ExecutionTests.TimersPromises_SetInterval_RejectsClearly` (`Js2IL.Tests/Node/TimersPromises/ExecutionTests.cs`)
- `Js2IL.Tests.Node.TimersPromises.GeneratorTests.TimersPromises_SetInterval_RejectsClearly` (`Js2IL.Tests/Node/TimersPromises/GeneratorTests.cs`)
