# Module: assert

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 24.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/assert.html) |

## Implementation

- `src/JavaScriptRuntime/Node/AssertModule.cs`
- `src/JavaScriptRuntime/Node/Contracts/IAssertModule.Generated.cs`

## Notes

Supports the callable CommonJS export used by packages such as Undici, Node-style AssertionError metadata, strict and loose scalar equality assertions, regular-expression assertions, synchronous throw assertions, fail, and ifError. Deep structural comparisons, asynchronous assertions, Assert, and CallTracker are present in the generated Node 24 contract but currently fail explicitly. This production module is separate from and does not alter the private test262 assertion harness.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| assert(value[, message]) / ok(value[, message]) | function | supported | [docs](https://nodejs.org/api/assert.html#assertvalue-message) |
| equal / notEqual / strictEqual / notStrictEqual | function | supported | [docs](https://nodejs.org/api/assert.html#assertstrictequalactual-expected-message) |
| match / doesNotMatch | function | supported | [docs](https://nodejs.org/api/assert.html#assertmatchstring-regexp-message) |
| throws / doesNotThrow | function | partial | [docs](https://nodejs.org/api/assert.html#assertthrowsfn-error-message) |
| AssertionError | property | supported | [docs](https://nodejs.org/api/assert.html#class-assertassertionerror) |
| strict | property | partial | [docs](https://nodejs.org/api/assert.html#strict-assertion-mode) |

## API Details

### assert(value[, message]) / ok(value[, message])

**Tests:**
- `Jroc.Tests.Node.AssertModule.ExecutionTests.Require_Assert_Callable_And_Core_Methods` (`tests/Jroc.Tests/Node/AssertModule/ExecutionTests.cs`)

### equal / notEqual / strictEqual / notStrictEqual

**Tests:**
- `Jroc.Tests.Node.AssertModule.ExecutionTests.Require_Assert_Callable_And_Core_Methods` (`tests/Jroc.Tests/Node/AssertModule/ExecutionTests.cs`)

### match / doesNotMatch

**Tests:**
- `Jroc.Tests.Node.AssertModule.ExecutionTests.Require_Assert_Callable_And_Core_Methods` (`tests/Jroc.Tests/Node/AssertModule/ExecutionTests.cs`)

### throws / doesNotThrow

Supports synchronous callables and regular-expression matching against thrown error messages.

**Tests:**
- `Jroc.Tests.Node.AssertModule.ExecutionTests.Require_Assert_Callable_And_Core_Methods` (`tests/Jroc.Tests/Node/AssertModule/ExecutionTests.cs`)

### AssertionError

Exposes name, message, actual, expected, operator, generatedMessage, and ERR_ASSERTION code metadata.

**Tests:**
- `Jroc.Tests.Node.AssertModule.ExecutionTests.Require_Assert_AssertionError_Metadata` (`tests/Jroc.Tests/Node/AssertModule/ExecutionTests.cs`)

### strict

Exposes a callable strict-mode assertion object whose equal and notEqual methods use strict comparisons.
