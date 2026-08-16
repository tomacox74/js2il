# Runtime lifecycle

`RuntimeLifecycle` is the common bootstrap and teardown path for standalone
`Engine` execution, hosted `JsRuntimeInstance` modules, and future workers. It
performs one ordered operation:

1. use a supplied `RuntimeAgentCluster` or create an isolated cluster;
2. create one agent and realm, then install the standard realm services;
3. apply explicit host bootstrap inputs before JavaScript is entered;
4. configure the compiled module assembly and execution metadata;
5. resolve the agent scheduler and thread-affine event-loop pump;
6. enter a root `RuntimeExecutionContext`, run the entry point, and pump work;
7. exit the frame and dispose realm, agent, and owned cluster in reverse order.

Every partial-startup failure follows the same teardown path. When a caller
supplies a cluster, the lifecycle removes only the agent it created so another
agent can reuse that cluster. The internal hosted options use this boundary for
future worker bootstrap. A borrowed test service container retains its existing
ownership graph, but its agent async context is reset after execution.

Hosted runtimes suppress any `AsyncLocal` runtime frame and invocation state
inherited from the caller before creating their graph. The state is restored
only after the hosted lifecycle has exited its own root frame and disposed its
owned graph. A dedicated CLR thread is therefore an executor choice, not the
source of runtime identity.

`JsRuntimeInstance` uses the agent shutdown token as its only cancellation
source after creation. Disposal requests cooperative agent shutdown, wakes its
queue, lets the script thread exit the root frame, and then disposes the
lifecycle. Initialization exceptions are published to the host only after the
agent has been unregistered and partial realm state has been released.

JavaScript wrappers, module state, globals, schedulers, and callbacks remain
owned by the realm or agent described in
[Runtime ownership](RuntimeOwnership.md); `RuntimeLifecycle` coordinates those
owners but does not introduce another state owner.
