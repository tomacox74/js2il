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

Both `timers` and `node:timers` resolve to this module. This focused implementation provides the timeout APIs required by Undici's snapshot recorder; immediate and interval module exports are not yet implemented.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| setTimeout(callback[, delay[, ...args]]) | function | supported | [docs](https://nodejs.org/api/timers.html#settimeoutcallback-delay-args) |
| clearTimeout(timeout) | function | supported | [docs](https://nodejs.org/api/timers.html#cleartimeouttimeout) |
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

### timeout.refresh()

Restarts an active or completed one-shot timeout using its original delay and returns the same handle. A handle canceled by clearTimeout() remains canceled.
