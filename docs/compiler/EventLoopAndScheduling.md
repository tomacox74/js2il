# Event Loop and Scheduling

JS2IL's runtime provides a small, deterministic event loop to support JavaScript semantics for:

- Promise reactions ("microtasks")
- timers (`setTimeout` / `setInterval`)
- immediates (`setImmediate`)

A key design goal is practical compatibility with Node.js behaviour (especially around timers/immediates and Promise job draining), while still keeping the implementation small and deterministic.

This document describes the *runtime contract* between:

1) the thread-safe scheduler state (where work is enqueued)
2) the single-threaded event loop pump (where work is executed)

## Relevant Specification Concepts

ECMA-262 defines the concept of *Jobs* and describes host operations used to enqueue and run them.

- **Jobs** are defined in ECMA-262 §9.5, “Jobs and Host Operations to Enqueue Jobs” (in particular, the first paragraph defines a Job; the next paragraph describes that Jobs are scheduled by the host and introduces host hooks such as HostEnqueuePromiseJob): https://262.ecma-international.org/14.0/#sec-jobs
- Promise reactions are enqueued by abstract operations that ultimately invoke **HostEnqueuePromiseJob** (ECMA-262 §9.5.4): https://262.ecma-international.org/14.0/#sec-hostenqueuepromisejob
- Promise reaction processing is described by **TriggerPromiseReactions** (ECMA-262 §27.2.1.8), which enqueues a Job for each reaction and uses HostEnqueuePromiseJob: https://262.ecma-international.org/14.0/#sec-triggerpromisereactions
- The spec also groups the relevant Job forms under “Promise Jobs” (ECMA-262 §27.2.2): https://262.ecma-international.org/14.0/#sec-promise-jobs

In JS2IL, Promise reaction jobs map to a **microtask queue** and are drained during each event-loop iteration.

## Specification compliance

ECMA-262 intentionally does not define a single, universal “event loop”. Instead, it defines:

- what a **Job** is and that Jobs are scheduled and run by the **host** (ECMA-262 §9.5)
- the **host hook** used by Promise machinery to enqueue promise reaction jobs (HostEnqueuePromiseJob, ECMA-262 §9.5.4)

Given that, JS2IL’s goal is:

- to be compliant with the ECMAScript Job/Promise semantics, while
- providing a small, deterministic, Node-like host loop for common APIs (timers/immediates).

Concretely, JS2IL makes the following compatibility guarantees:

- **Promise reactions are enqueued as Jobs/microtasks.** When a Promise settles, its reactions are queued onto the microtask queue (see TriggerPromiseReactions, ECMA-262 §27.2.1.8).
- **Microtask checkpoints are bounded to avoid starvation.** After each callback that JS2IL treats as a host “task” (e.g., a `setImmediate` callback or a due timer callback), the pump drains a bounded number of microtasks before continuing. This preserves forward progress for timers/macrotasks in long microtask chains.

JS2IL also implements the JobCallback host abstractions used by the spec to carry host-defined context alongside scheduled Jobs:

- **JobCallback Records** (ECMA-262 §9.5.1)
- **HostMakeJobCallback** (ECMA-262 §9.5.2)
- **HostCallJobCallback** (ECMA-262 §9.5.3)

Note that the APIs `setTimeout`/`setInterval`/`setImmediate` are host-defined (not standardized by ECMA-262), so the exact ordering between timer/immediate phases is ultimately a host-compatibility choice rather than a spec requirement.

Note:

- ECMA-262 does *not* standardize timers like `setTimeout`/`setInterval` or `setImmediate`.
- Those APIs originate from host environments (browsers/HTML, Node.js).
- JS2IL implements Node-like timers to support common JavaScript code.

## Architecture

The engine uses two distinct components:

- `NodeSchedulerState` (thread-safe): owns the queues/timers state and implements `IScheduler` + `IMicrotaskScheduler`.
- `NodeEventLoopPump` (JS thread only): drains `NodeSchedulerState` and executes callbacks.

### Why Split Them?

- The scheduler must be safe to call from multiple threads (e.g., hosting thread scheduling timers while the JS thread is running).
- The event loop itself is single-threaded by design, matching JavaScript execution semantics.

## Scheduling (Thread-Safe)

### Timers

- `setTimeout` schedules a one-shot timer.
- `setInterval` schedules a repeating timer (same id is reused when rescheduling).
- `clearTimeout` removes the timer entry.
- `clearInterval` marks the interval id as canceled; the pump will discard pending interval entries.

When any work is scheduled or canceled, the scheduler signals a wake-up handle so the event loop can resume.

### Immediates

`setImmediate` enqueues work into an immediates queue. Immediates are treated as high priority work within a tick.

### Microtasks

Promise reactions (from `Promise.prototype.then/catch/finally`) enqueue a microtask.

Microtasks are *always* drained at the end of each event-loop iteration.

## Execution Order (Event Loop)

Each `RunOneIteration()` performs the following steps (simplified):

1) Drain a bounded number of immediates
2) Promote at most one due timer to the macro queue (rescheduling intervals immediately)
3) Execute at most one macro callback
4) Drain a bounded number of microtasks

Additionally, a microtask checkpoint runs after each immediate callback.

## Waiting / Idling

When the host wants to wait for more work (or a future timer), the pump computes:

- `0ms` if there is runnable work now
- otherwise, a wait duration until the next timer is due (clamped by a maximum)

The wake-up handle is signaled whenever new work is scheduled.
