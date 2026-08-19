# Residual invocation-state transport

Issue #1890 evaluated replacing the residual `RuntimeServices` invocation
frame with a reusable `[ThreadStatic]` stack. The production design remains
the single immutable `AsyncLocal` frame introduced by issue #1887.

## Boundary inventory

Most calls carry invocation data explicitly:

- receiver-aware built-ins receive `this` and fixed-arity arguments directly;
- synchronous generated callables receive `GeneratedInvocationContext` when
  their ABI supports it;
- generated constructors receive `new.target` explicitly;
- async and generator scopes retain state that must survive suspension,
  including their captured `this` value and compiler-generated locals.

The ambient compatibility frame remains for:

- named-function identity, `arguments.callee`, bound `with` environments, and
  lexical `super`;
- generated array adapters that cannot use the explicit-context ABI;
- generator and async-generator entry before their scope captures resumable
  state;
- legacy constructors and accessors that still read `this`, arguments, or
  `new.target` through `RuntimeServices`.

State crosses the following boundaries:

| Boundary | Transport and isolation |
| --- | --- |
| Nested and reentrant calls | `CallableOperations` pushes one immutable frame and restores it in `finally`. |
| Exceptions | The same `finally` restoration prevents state leaks. |
| Synchronous generators | `GeneratorScope.ThisValue` is captured at creation and reinstalled around every resume. |
| Async functions | `AsyncScope` captures `this`; compiler-generated scope fields retain locals needed after `await`. |
| Async generators | `AsyncGeneratorScope` retains resumable state and reinstalls captured `this` around `MoveNext`. |
| Promise reactions and jobs | Callbacks re-enter through `CallableOperations` on the owning runtime scheduler. |
| Root realm/agent entry | `RuntimeExecutionContext.EnterAsRoot()` captures, clears, and restores the complete compatibility state. |
| Arbitrary task/thread hops | `AsyncLocal` snapshots the immutable frame with the CLR `ExecutionContext`. |
| Concurrent tasks | Copy-on-write frame replacement prevents one continuation from mutating another continuation's snapshot. |

## Measured alternatives

Run the focused comparison:

```bash
dotnet run -c Release \
  --project tests/performance/Benchmarks/Benchmarks.csproj -- \
  --callable-abi --filter "*"
```

The relevant designs are:

| Design | Steady-state allocation | Status |
| --- | ---: | --- |
| Explicit generated/built-in context | 0 B/call | Preferred and used whenever supported |
| Single immutable `AsyncLocal` frame | 200 B/call | Residual compatibility fallback |
| Reusable `[ThreadStatic]` frame prototype | 0 B/call | Rejected for production |

The `AsyncLocal` result is a 70.9% reduction from the 688 B/call multi-slot
control measured by issue #1885.

## Decision

A thread-static stack is safe only while execution stays on one CLR thread or
passes through a boundary that JROC explicitly instruments. That is not the
complete runtime contract. Active compatibility state currently flows through
arbitrary `Task.Run` and CLR `ExecutionContext` captures; hosted lifecycle
suppression must then clear the inherited state for a child runtime and restore
it afterward.

JROC cannot intercept every task, host callback, or third-party
`ExecutionContext` capture. Attaching a mutable thread-static snapshot to the
existing runtime `AsyncLocal` would also be incorrect: captured continuations
would share the same mutable object instead of receiving point-in-time state.
Making that snapshot copy-on-write would require an `AsyncLocal` publication
at each call boundary and recreate the allocation cost the thread-static design
was intended to remove.

Therefore the zero-allocation stack remains a benchmark control, not runtime
architecture. Future work should continue shrinking the residual set through
explicit parameters and state-machine fields. The single immutable
`AsyncLocal` frame is the safer fallback for the compatibility paths that
remain.
