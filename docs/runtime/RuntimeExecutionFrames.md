# Runtime execution frames

`RuntimeExecutionContext` is the single ambient pointer for active JROC
execution. It flows through asynchronous continuations with `AsyncLocal` and
identifies:

- the current `RuntimeRealm` and owning `RuntimeAgent`;
- the realm's `ServiceContainer`;
- hosted/standalone bootstrap metadata;
- current module filename and directory;
- the realm-specific global object and property descriptor store.

Use `Enter()` for balanced nested realm entry. Use `EnterAsRoot()` for a new
engine, hosted runtime, or worker bootstrap. Root entry suppresses inherited
runtime overrides and JavaScript invocation state (`this`, arguments,
`new.target`, callee, lexical-super state, and direct constructor/call stacks)
until the scope exits.

Scopes must be disposed in reverse entry order. Disposal restores the complete
previous ambient state and throws for out-of-order exit. Entering a disposed
realm is rejected.

`GlobalThis.ServiceProvider`, module path lookup, runtime property descriptors,
and the engine's test/bootstrap service override all resolve through this
frame. Thread identity is not a runtime identity boundary.

Residual invocation values use one immutable `AsyncLocal` frame. Copy-on-write
replacement preserves point-in-time snapshots across task/thread hops, while
root frame entry explicitly captures, clears, and restores that frame plus the
thread-affine direct-call stacks so a new agent cannot inherit its creator's
call state. Root engine and hosted entry scopes remain synchronous and
thread-affine while those stacks use thread-static storage. The rejected
thread-static invocation-frame evaluation is documented in
[Residual invocation-state transport](ResidualInvocationState.md).
