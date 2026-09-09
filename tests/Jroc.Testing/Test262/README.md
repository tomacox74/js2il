# Native Test262 harness

The native helpers implement the Test262 harness at the revision recorded in
`tests/test262/test262.pin.json`. Fixtures retain their upstream assertions and
frontmatter; helper implementations do not require JavaScript source injection.

## Async completion protocol

`Test262SharedAssertHarness` creates completion state for each execution. After
the engine has drained its event loop, an `async` fixture must have called
`$DONE` exactly once. Missing completion and repeated completion are failures.
Truthy errors passed to `$DONE` are recorded outside promise reactions and
reported after execution; later completion cannot erase an earlier failure.

The pinned `doneprintHandle.js` tests the **truthiness** of its error argument.
Accordingly, `$DONE(undefined)`, `$DONE(null)`, `$DONE(0)`, `$DONE(false)`, and
`$DONE("")` all indicate success, as do other JavaScript falsy values. The pinned
`asyncHelpers.js` forwards both promise rejections and synchronous throws to
`$DONE(error)`. The native `asyncTest` preserves this behavior, including
unwrapping JavaScript thrown values before applying the completion protocol.

This upstream protocol is ambiguous: a rejection or throw with a falsy value
is indistinguishable from successful completion once forwarded to `$DONE`.
The harness deliberately preserves that protocol rather than introducing
fixture-specific exceptions or imposing a stronger rejection rule. Truthy
assertion errors still fail, and an unsettled promise without `$DONE` does not
pass.

`assert.throwsAsync` follows its separate upstream assertion contract: it requires
an asynchronous rejection with an object whose constructor is exactly the
expected constructor. Synchronous throws, fulfillment, non-thenables, and
primitive rejection values fail that assertion, including falsy primitives.

Focused regressions are in `tests/Jroc.Tests/Test262AsyncHarnessTests.cs`.
