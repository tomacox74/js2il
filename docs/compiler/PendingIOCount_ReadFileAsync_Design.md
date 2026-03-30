# Pending IO Count for `fs/promises.readFile` Async Flow

- **Status**: Draft for review
- **Date**: 2026-02-19
- **Scope**: Minimal runtime design change for `fs/promises.readFile` only

## Context

`fs/promises.readFile` currently performs synchronous disk reads (`ReadAllText` / `ReadAllBytes`) before returning a Promise. This blocks the runtime event loop thread during file I/O.

We need a minimal design that:

1. Uses true async I/O (`ReadAllTextAsync` in this version).
2. Tracks in-flight I/O using internal scheduler accounting.
3. Calls `BeginIo()` when `readFile` starts.
4. Calls `EndIo(promiseWithResolvers, result, isError)` for completion.
5. Guarantees completion and decrement handling for both success and error paths.

## Proposed Design

Add scheduler-level pending I/O accounting to `NodeSchedulerState`, then refactor `FSPromises.readFile` to:

- Capture `IIOScheduler` once when `FSPromises` is created.
- Call `BeginIo()` once per invocation.
- Start async file read (`ReadAllTextAsync`).
- Call `EndIo(promiseWithResolvers, result, isError)` after I/O completion.
- Let scheduler internals settle Promise and decrement I/O tracking on event-loop callback.

Introduce `IIOScheduler` so Node modules do not depend on `NodeSchedulerState` directly.

Internal pending I/O accounting affects event-loop liveness (`HasPendingWork`) only; it does not make work runnable immediately (`HasPendingWorkNow`).
This document is intentionally scoped to Promise-based async I/O; non-Promise async patterns are future work.

## Goals and Non-Goals

### Goals

- Eliminate event-loop blocking in `fs/promises.readFile` for text reads.
- Preserve Promise semantics and existing error translation behavior.
- Avoid race conditions by mutating pending I/O count on event-loop thread for decrement.
- Keep change surface small and isolated.

### Non-Goals

- No broad Node parity expansion (AbortSignal/options object semantics beyond existing behavior).
- No changes to `fs.readFileSync`.
- No generalized async rewrite for all fs/promises APIs in this document.

## Affected Classes

## 0) `JavaScriptRuntime.EngineCore.IIOScheduler`

### New Interface

- `void BeginIo()`
- `void EndIo(PromiseWithResolvers promiseWithResolvers, object? result, bool isError = false)`

### Purpose

- Provides an abstraction boundary so Node module implementations avoid direct dependency on `NodeSchedulerState`.
- Hides pending I/O count and queueing details from Node module code.
- Makes Promise completion explicit for Promise-returning APIs in this phase.
- Avoids per-call service resolution by allowing module instances to retain scheduler dependency.

## 1) `JavaScriptRuntime.EngineCore.NodeSchedulerState`

### New/Updated Properties

- Internal pending I/O counter field.
  - Backed by an atomic field for cross-thread visibility.
  - Not exposed through `IIOScheduler`.

### New Methods

- `public void BeginIo()`
  - Atomically increments internal pending I/O counter.
  - Called on entry to `FSPromises.readFile` before scheduling async work.

- `public void EndIo(PromiseWithResolvers promiseWithResolvers, object? result, bool isError = false)`.
  - Scheduler resolves or rejects the provided Promise based on `isError`.
  - Scheduler decrements internal pending I/O counter in `finally` on event-loop callback path.
  - Defensive clamp/guard recommended to avoid negative count on misuse.

### Updated Methods

- `internal bool HasPendingWork()`
  - Include internal pending I/O count in liveness check.
  - Purpose: keep runtime alive while async file work is in flight.

- `internal bool HasPendingWorkNow(long nowTicks)`
  - No pending I/O change in this method.
  - Rationale: pending I/O is not runnable work until completion callback is enqueued.

## 2) `JavaScriptRuntime.Node.FSPromises`

### Updated Method

- `public object? readFile(object? path, object? options = null)`

#### Entry behavior

1. Validate/coerce input path and options using existing logic.
2. Use cached `IIOScheduler` field captured at module construction.
3. Create deferred Promise resolver pair (`Promise.withResolvers()`).
4. Call `scheduler.BeginIo()`.
5. Start background async I/O via `ReadAllTextAsync` (this document version).

#### Completion behavior

- On background completion (success or error), call `scheduler.EndIo(promiseWithResolvers, result, isError)`.
- Scheduler handles:
  1. Event-loop callback scheduling.
  2. Promise resolution/rejection.
  3. Internal pending I/O decrement in `finally`.

#### Error handling requirements

- If async read throws, map with existing `TranslateReadFileError` behavior.
- Ensure decrement executes for translated errors and unexpected exceptions.
- Ensure single-settlement/single-decrement semantics (guard against duplicate completion).

## 3) `JavaScriptRuntime.EngineCore.Engine` (No signature changes)

### Behavioral dependency

- Existing execute loop that consults `HasPendingWork()` will now remain alive while internal pending I/O count is greater than zero.
- No direct code contract changes required in this document; behavior changes through scheduler liveness semantics.

## Dataflow

## Success Path (`readFile` resolves)

```mermaid
sequenceDiagram
    participant JS as JavaScript Caller
    participant FSP as FSPromises.readFile
    participant SCH as NodeSchedulerState
    participant IO as .NET Async File IO
    participant LOOP as Event Loop Thread
    participant PR as Promise

    JS->>FSP: readFile(path, options)
    FSP->>SCH: BeginIo()
    FSP->>IO: Start ReadAllTextAsync(...)
    FSP-->>JS: Return Promise immediately
    IO-->>FSP: Text result
    FSP->>SCH: EndIo(withResolvers, text, false)
    LOOP->>PR: resolve(text)
    LOOP->>SCH: decrement internal pending I/O (finally)
```

## Failure Path (`readFile` rejects)

```mermaid
sequenceDiagram
    participant JS as JavaScript Caller
    participant FSP as FSPromises.readFile
    participant SCH as NodeSchedulerState
    participant IO as .NET Async File IO
    participant LOOP as Event Loop Thread
    participant PR as Promise

    JS->>FSP: readFile(path, options)
    FSP->>SCH: BeginIo()
    FSP->>IO: Start ReadAllTextAsync(...)
    FSP-->>JS: Return Promise immediately
    IO-->>FSP: Exception (e.g., FileNotFound)
    FSP->>FSP: TranslateReadFileError(ex)
    FSP->>SCH: EndIo(withResolvers, translatedError, true)
    LOOP->>PR: reject(translatedError)
    LOOP->>SCH: decrement internal pending I/O (finally)
```

## Host vs Standalone Behavior

## Standalone execution (CLI-style compile/run)

- Runtime loop exits when no pending work remains.
- With this design, in-flight async `readFile` contributes to `HasPendingWork` via internal pending I/O accounting.
- Effect: process no longer exits early while file I/O is still in flight.
- Observable difference: `readFile` Promise resolves/rejects asynchronously without blocking the loop thread.

## Hosted runtime (embedded `JsRuntimeInstance`)

- Host pump commonly runs continuously until explicit shutdown.
- Internal pending I/O accounting still provides correctness guarantees for Promise completion and future lifecycle decisions.
- Main observable difference is responsiveness and race safety (decrement on event-loop thread), not host process lifetime.

## Invariants

1. `BeginIo()` happens exactly once per `readFile` invocation after successful scheduler resolution.
2. `EndIo(promiseWithResolvers, result, isError)` is called exactly once per invocation.
3. Promise settlement (resolve/reject) is performed by scheduler on event-loop callback path.
4. Internal pending I/O count never remains elevated after settlement.
5. Promise settlement (resolve/reject) occurs on event-loop thread.

## Edge Cases and Required Handling

- **Scheduler unavailable**: fail fast with explicit runtime exception before increment.
- **Exception before increment**: no decrement needed.
- **Exception after increment but before task wiring**: ensure event-loop decrement path is still scheduled.
- **Duplicate completion risk**: use one-time completion guard to prevent double settle/decrement.
- **Callback throws during settlement**: decrement must still run due to `finally`.

## Test Specification

## Unit/behavior checks

1. Internal pending I/O count increases when `readFile` starts and returns to prior value after completion.
2. Missing file/error path rejects Promise and decrements count.
3. Event loop remains alive while pending I/O is in flight (standalone liveness).
4. Decrement happens on event-loop callback path (not worker thread path).

## Suggested test locations

- `Js2IL.Tests/Node/FS/ExecutionTests.cs`
- `Js2IL.Tests/Node/NodeEventLoopPumpTests.cs`

## Backward Compatibility

- Public JavaScript API remains `fs/promises.readFile(path, options?) -> Promise`.
- Error contract remains based on existing `TranslateReadFileError` mapping.
- Timing changes are intentional: completion is no longer tied to synchronous I/O execution.

## Alternatives Considered

1. `Task.Run` wrapping synchronous reads:
   - Smaller diff, but still blocks worker thread and is less scalable than true async I/O.
2. Include pending I/O in `HasPendingWorkNow`:
   - Rejected for now; pending I/O is not immediately runnable work and may distort spin/wait behavior.
3. Direct Promise settlement from worker thread:
   - Rejected due to scheduler/thread-safety concerns.

## Rollout Notes

- Implement as a focused change set in scheduler + `FSPromises.readFile`.
- Keep API surface minimal in this iteration.
- Expand pattern to other fs/promises methods only after this behavior is validated.

## ADR Timing

Formal ADR conversion is intentionally deferred until this design passes review.
