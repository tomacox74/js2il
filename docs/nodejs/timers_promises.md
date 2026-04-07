# Module: timers/promises

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | completed |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/timers.html#timers-promises-api) |

## Implementation

- `src/JavaScriptRuntime/Node/TimersPromises.cs`
- `src/JavaScriptRuntime/Abort.cs`

## Notes

This module now exposes the Promise-based one-shot timer helpers plus the async-iterator `setInterval(...)` contract on top of the existing Node scheduler/event-loop implementation, so ordering continues to match process.nextTick, Promise microtasks, setImmediate, and timer-global behavior. AbortController/AbortSignal cancellation is supported for the documented timers/promises paths.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| setTimeout(delay[, value[, options]]) | function | supported | [docs](https://nodejs.org/api/timers.html#timerspromisessettimeoutdelay-value-options) |
| setImmediate([value[, options]]) | function | supported | [docs](https://nodejs.org/api/timers.html#timerspromisessetimmediatevalue-options) |
| setInterval(delay[, value[, options]]) | function | supported | [docs](https://nodejs.org/api/timers.html#timerspromisessetintervaldelay-value-options) |

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

Returns an async iterator that yields the supplied value for each elapsed interval. Pending ticks are queued until consumed, `return()`/`for await ... break` tear down the repeating timer, and `options.signal` aborts active iteration with AbortError/ABORT_ERR.

**Tests:**
- `Js2IL.Tests.Node.TimersPromises.ExecutionTests.TimersPromises_SetInterval_ForAwait_BreaksAndTearsDown` (`Js2IL.Tests/Node/TimersPromises/ExecutionTests.cs`)
- `Js2IL.Tests.Node.TimersPromises.GeneratorTests.TimersPromises_SetInterval_ForAwait_BreaksAndTearsDown` (`Js2IL.Tests/Node/TimersPromises/GeneratorTests.cs`)
- `Js2IL.Tests.Node.TimersPromises.ExecutionTests.TimersPromises_SetInterval_Backpressure_And_Return` (`Js2IL.Tests/Node/TimersPromises/ExecutionTests.cs`)
- `Js2IL.Tests.Node.TimersPromises.GeneratorTests.TimersPromises_SetInterval_Backpressure_And_Return` (`Js2IL.Tests/Node/TimersPromises/GeneratorTests.cs`)
- `Js2IL.Tests.Node.TimersPromises.ExecutionTests.TimersPromises_SetInterval_Abort_RejectsActiveIterator` (`Js2IL.Tests/Node/TimersPromises/ExecutionTests.cs`)
- `Js2IL.Tests.Node.TimersPromises.GeneratorTests.TimersPromises_SetInterval_Abort_RejectsActiveIterator` (`Js2IL.Tests/Node/TimersPromises/GeneratorTests.cs`)
