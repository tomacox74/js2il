# ECMA-262 Remaining Feature Work (JS2IL v0.7.2)

This document summarizes **ECMA-262 language features that are not yet fully supported** by JS2IL as of **v0.7.2 (2026-01-17)**.

Basis:
- Source of truth: per-subsection JSON docs under `docs/ECMA262/**/Section*.json` (see `support.entries`).
- This document has not been regenerated since moving feature coverage into subsection JSON; treat counts/sections below as historical.

If you want a **clause-by-clause** view of ECMA-262 (language + built-ins), see:
- [docs/ECMA262/Index.md](Index.md)
	- Indexed clauses (tc39.es): **2176**
	- Clauses with tracked status in the coverage matrix: **104**
	- Clauses currently `Untracked` (need audit / need coverage rows): **2072**

## Summary

Overall (coverage-file rows):
- Total tracked feature rows: **196**
- Supported: **187**
- Supported with Limitations: **7**
- Not Yet Supported: **2**

Remaining work (high-level themes):
- **Function calling / parameters**: rest parameters and spread at call sites are not implemented.
- **Lexical bindings**: `let/const` are missing Temporal Dead Zone (TDZ) errors (reads-before-init).
- **Iteration**: `for-of` and `for-in` exist but do not implement full iterator/prototype-chain semantics.
- **Control-flow edge cases**: `try/finally` has gaps around unhandled throws at top-level (tests indicate skips).
- **Text processing & JSON**: `String.prototype.replace` and `JSON.parse` are present with notable spec gaps.

Remaining work by spec section (count of non-supported rows):
- **13** (ECMAScript Language: Expressions): **4**
- **14** (ECMAScript Language: Statements and Declarations): **3**
- **24** (Text Processing): **2**

## Intrinsics & built-ins (runtime surface)

The “Detailed list” below was previously driven by a dedicated feature-coverage file. That file has been removed in favor of `support.entries` embedded into subsection JSON.

This section enumerates the **current runtime intrinsic surface area** so it’s easier to reason about “what exists at all” vs. “what is spec-complete”.

### Global bindings (`GlobalThis`)

Exposed by [JavaScriptRuntime/GlobalThis.cs](../../JavaScriptRuntime/GlobalThis.cs):
- Globals: `console`, `process`, `Infinity`, `NaN`
- Global functions: `parseInt`, `setTimeout`, `clearTimeout`, `setImmediate`, `clearImmediate`, `setInterval`, `clearInterval`

### Intrinsic objects (`[IntrinsicObject]` registry)

Discovered via [JavaScriptRuntime/IntrinsicObjectRegistry.cs](../../JavaScriptRuntime/IntrinsicObjectRegistry.cs) scanning runtime types annotated with `[IntrinsicObject("...")]`.

Currently registered intrinsic object names include:
- `AggregateError`, `Array`, `Boolean`, `Date`
- `Error`, `EvalError`, `RangeError`, `ReferenceError`, `SyntaxError`, `TypeError`, `URIError`
- `Int32Array`, `JSON`, `Math`, `Number`, `Object`, `Promise`, `RegExp`, `Set`, `String`

Note: “present in the registry” means JS2IL can recognize the identifier as an intrinsic (when not shadowed) and may lower selected operations directly. It does **not** imply full ECMA-262 method/property coverage for that object.

### Node module intrinsics (`[NodeModule]`)

Node module shims are tracked separately from ECMA-262 language features. Registered modules currently include:
- `child_process`, `fs`, `fs/promises`, `os`, `path`, `perf_hooks`, `process`

See [docs/nodejs/NodeSupport.md](../nodejs/NodeSupport.md) for the Node-side coverage view.

### Known intrinsic gaps explicitly called out elsewhere

From [docs/archive/LoweringPipeline_Migration_PunchList.md](../archive/LoweringPipeline_Migration_PunchList.md):
- **Callable-only intrinsics (no `new`)**: `String(x)`, `Number(x)`, `Boolean(x)` are supported, but other callable-only intrinsics are not yet supported: `Date(...)`, `RegExp(...)`, `Error(...)`, `Array(...)`, `Object(...)`, `Symbol(...)`, `BigInt(...)`.

## Detailed list (Supported with Limitations + Not Yet Supported)

Status note:
- This document historically used `Partially Supported`; it maps to `Supported with Limitations` under the current ECMA-262 coverage taxonomy.

Formatting:
- Each entry is listed under its paragraph/subsection from the coverage JSON.
- “Spec” links go to the relevant ECMA-262 section in tc39.es.

### Section 13: ECMAScript Language: Expressions

#### 13.2: Declarations

##### 13.2.1: let/const
- Spec: https://tc39.es/ecma262/#sec-let-and-const-declarations

| Status | Feature | Notes |
|---|---|---|
| Supported with Limitations | let/const | Block scoping, shadowing chain, nested function capture, and simple const initialization implemented. Temporal dead zone access error (Variable_TemporalDeadZoneAccess.js) and reads before initialization are still pending. |

##### 13.2.3: Function declarations
- Spec: https://tc39.es/ecma262/#sec-function-definitions

| Status | Feature | Notes |
|---|---|---|
| Supported with Limitations | Arrow functions | Covers expression- and block-bodied arrows, multiple parameters, nested functions, closure capture across scopes (including returning functions that capture globals/locals), and default parameter values. Not yet supported: rest parameters, lexical this/arguments semantics, and spread at call sites. |

##### 13.2.3.1: Default parameters, Rest parameters
- Spec: https://tc39.es/ecma262/#sec-function-definitions-runtime-semantics-evaluation

| Status | Feature | Notes |
|---|---|---|
| Not Yet Supported | Rest parameters |  |

##### 13.2.5: Spread syntax
- Spec: https://tc39.es/ecma262/#sec-argument-lists-runtime-semantics-argumentlistevaluation

| Status | Feature | Notes |
|---|---|---|
| Not Yet Supported | Spread syntax |  |

### Section 14: ECMAScript Language: Statements and Declarations

#### 14.7.5: The for-of Statement

##### 14.7.5.1: Runtime Semantics: ForInOfBodyEvaluation (for-of)
- Spec: https://tc39.es/ecma262/#sec-runtime-semantics-forinofbodyevaluation

| Status | Feature | Notes |
|---|---|---|
| Supported with Limitations | for-of over arrays (enumerate values) | Lowered using the iterator protocol (GetIterator + next() + value/done) and attempts IteratorClose on abrupt completion. Supports built-ins and user-defined iterables via Symbol.iterator, with remaining gaps documented in the subsection JSON notes. |

##### 14.7.5.2: Runtime Semantics: ForInOfBodyEvaluation (for-in)
- Spec: https://tc39.es/ecma262/#sec-runtime-semantics-forinofbodyevaluation

| Status | Feature | Notes |
|---|---|---|
| Supported with Limitations | for-in over objects (enumerate enumerable keys) | Lowered via a native For-In Iterator (EnumerateObjectProperties/CreateForInIterator) and consumed through IteratorNext + IteratorResultDone/Value. Deletions during enumeration are respected; prototype/exotic-object fidelity is still not exhaustive. |

#### 14.16: The try Statement

##### 14.16.1: Runtime Semantics: TryStatement Evaluation
- Spec: https://tc39.es/ecma262/#sec-try-statement

| Status | Feature | Notes |
|---|---|---|
| Supported with Limitations | try/finally (no catch) | Finally emission is in place and executes on normal and return exits. Execution test for unhandled throw is skipped pending top-level unhandled Error semantics; generator snapshot verifies structure. |

### Section 24: Text Processing

#### 24.1: String Objects

##### 24.1.3.5: String.prototype.replace
- Spec: https://tc39.es/ecma262/#sec-string.prototype.replace

| Status | Feature | Notes |
|---|---|---|
| Supported with Limitations | String.prototype.replace (regex literal, string replacement) | Supported when the receiver is String(x), the pattern is a regular expression literal, and the replacement is a string. Global (g) and ignoreCase (i) flags are honored. Function replacement, non-regex patterns, and other flags are not yet implemented. Implemented via host intrinsic JavaScriptRuntime.String.Replace and dynamic resolution in IL generator. |

#### 24.5: JSON Object

##### 24.5.1: JSON.parse
- Spec: https://tc39.es/ecma262/#sec-json.parse

| Status | Feature | Notes |
|---|---|---|
| Supported with Limitations | JSON.parse | Implemented via host intrinsic JavaScriptRuntime.JSON.Parse(string). Maps invalid input to SyntaxError and non-string input to TypeError. Reviver parameter is not supported. Objects become ExpandoObject, arrays use JavaScriptRuntime.Array, numbers use double. |

---

When `support.entries` data in the subsection JSON docs changes, this document should be refreshed so scheduling data stays accurate.

