# Module: timers

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/timers.html) |

## Implementation

- `src/JavaScriptRuntime/Node/TimersModule.cs`
- `src/JavaScriptRuntime/Timers.cs`

## Notes

Both `timers` and `node:timers` resolve to this module. All six top-level scheduling and cancellation functions delegate to JROC's existing Node event-loop timer implementation; advanced Immediate and Timeout handle methods remain partial.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| setTimeout(callback[, delay[, ...args]]) | function | supported | [docs](https://nodejs.org/api/timers.html#settimeoutcallback-delay-args) |
| clearTimeout(timeout) | function | supported | [docs](https://nodejs.org/api/timers.html#cleartimeouttimeout) |
| setImmediate(callback[, ...args]) | function | supported | [docs](https://nodejs.org/api/timers.html#setimmediatecallback-args) |
| clearImmediate(immediate) | function | supported | [docs](https://nodejs.org/api/timers.html#clearimmediateimmediate) |
| setInterval(callback[, delay[, ...args]]) | function | supported | [docs](https://nodejs.org/api/timers.html#setintervalcallback-delay-args) |
| clearInterval(timeout) | function | supported | [docs](https://nodejs.org/api/timers.html#clearintervaltimeout) |
| timeout.refresh() | function | supported | [docs](https://nodejs.org/api/timers.html#timeoutrefresh) |

## API Details

### setTimeout(callback[, delay[, ...args]])

Schedules a one-shot callback through JROC's Node event loop and returns a refreshable Timeout handle.

**Tests:**
- `Jroc.Tests.Node.Timers.ExecutionTests.Require_Timers_RefreshableTimeout` (`tests/Jroc.Tests/Node/Timers/ExecutionTests.cs`)
- `Jroc.Tests.Node.Timers.GeneratorTests.Require_Timers_RefreshableTimeout` (`tests/Jroc.Tests/Node/Timers/GeneratorTests.cs`)

### clearTimeout(timeout)

Cancels an active Timeout handle. Repeated cleanup is safe, and refresh() does not reactivate a canceled handle.

**Tests:**
- `Jroc.Tests.Node.Timers.ExecutionTests.Timeout_RefreshAfterClear_DoesNotReactivate` (`tests/Jroc.Tests/Node/Timers/ExecutionTests.cs`)
- `Jroc.Tests.Node.Timers.GeneratorTests.Timeout_RefreshAfterClear_DoesNotReactivate` (`tests/Jroc.Tests/Node/Timers/GeneratorTests.cs`)

### setImmediate(callback[, ...args])

Delegates the module export to JROC's existing immediate queue and forwards callback arguments.

**Tests:**
- `Jroc.Tests.Node.Timers.ExecutionTests.SetImmediate_WithArgs_PassesCorrectly` (`tests/Jroc.Tests/Node/Timers/ExecutionTests.cs`)
- `Jroc.Tests.Node.Timers.ExecutionTests.SetImmediate_ExecutesBeforeSetTimeout` (`tests/Jroc.Tests/Node/Timers/ExecutionTests.cs`)

### clearImmediate(immediate)

Cancels an immediate through the same scheduler used by the global timer API.

**Tests:**
- `Jroc.Tests.Node.Timers.ExecutionTests.ClearImmediate_CancelsCallback` (`tests/Jroc.Tests/Node/Timers/ExecutionTests.cs`)

### setInterval(callback[, delay[, ...args]])

Delegates the module export to JROC's repeating timer scheduler.

**Tests:**
- `Jroc.Tests.Node.Timers.ExecutionTests.SetInterval_ExecutesThreeTimes_ThenClears` (`tests/Jroc.Tests/Node/Timers/ExecutionTests.cs`)

### clearInterval(timeout)

Cancels an active repeating timer through the existing scheduler.

**Tests:**
- `Jroc.Tests.Node.Timers.ExecutionTests.SetInterval_ExecutesThreeTimes_ThenClears` (`tests/Jroc.Tests/Node/Timers/ExecutionTests.cs`)

### timeout.refresh()

Restarts an active or completed one-shot timeout using its original delay and returns the same handle. A handle canceled by clearTimeout() remains canceled.
