# Runtime agent scheduling

`RuntimeAgentSchedulingState` is the single owner of asynchronous execution
state shared by all realms in one `RuntimeAgent`:

- `NodeSchedulerState`, including timers, immediates, next-tick callbacks,
  Promise microtasks, cleanup jobs, pending I/O, and its wake handle;
- the thread-affine `NodeEventLoopPump`;
- `AsyncContextRuntime` and async-hooks propagation;
- the `FinalizationRegistryHost` and its cleanup-job scheduling;
- the agent cooperative-shutdown token.

Realm service containers expose these objects through dependency injection, but
the registrations are reserved ownership services. Resolving a scheduler,
event-loop, async-context, or finalization service from two realms in one agent
returns the same instance. Two agents always receive independent instances and
can drain work concurrently without a global scheduler lock.

The scheduler records the clock and wake handle used when it is first created.
The event-loop pump is built from those same dependencies even when another
realm resolves it later. The thread that first resolves the pump is its
executor; pump methods reject calls from any other thread.

## Cross-thread work

`RuntimeAgent.EnqueueFromExternalThread` is the wake-up boundary for producers
such as I/O completion threads. It places work on the receiving agent's
immediate queue without reading or capturing mutable async-hook state on the
producer thread. The callback runs only when the owning executor drains the
queue. Network and DNS completion paths use the same raw external queue.

Normal JavaScript scheduling APIs still capture the current agent async context
before queueing work. This preserves `AsyncLocalStorage` and async-hook behavior
for timers, Promise jobs, next-tick callbacks, and immediates created while
JavaScript is running.

## Shutdown

Agent shutdown is cooperative and deterministic:

1. cancel the agent shutdown token and signal its wake handle;
2. dispose the agent's realms in reverse creation order;
3. stop the pump from starting another callback;
4. clear finalization registrations, timers, queues, and pending I/O;
5. dispose the wake handle.

Disposing one agent does not cancel, reset, drain, or dispose another agent.
Concurrent disposal waits for the callback already executing on the owner
thread, then prevents the next queued callback from starting.

`JsRuntimeInstance` observes the agent shutdown token after runtime
initialization. Its pre-initialization host queue still has a bootstrap
cancellation source; issue #1828 will unify that startup path with the common
agent/realm lifecycle factory.
