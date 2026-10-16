# JROC Test262 Conformance Detail

[Back to ECMA-262 Coverage Index](Index.md)

For resumable passing-but-unported discovery, see the [Test262 artifact catalog](Test262Catalog.md).
Its MVP-runner evidence is separate from the native conformance results in this report.

This report provides detailed Test262 conformance evidence for the current development branch following [JROC v0.12.32](https://github.com/tomacox74/js2il/releases/tag/v0.12.32).

## How to Read This Report

- **Verified passing**: the native harness verifies the corresponding Test262 outcome: positive tests execute successfully, while negative tests are rejected at their declared parse or runtime phase.
- **Known unsupported**: the test exercises behavior explicitly excluded from the release, primarily `eval`.
- **No published result**: JROC has not published a conformance result for the test. This does not imply either support or failure.
- **Verified**: verified passing tests divided by applicable tests in that row.

Counts are unique, standalone Test262 files. Strict and non-strict execution variants are not counted separately.

Array and Date now have verified passing results for all applicable tests at the
pinned revision. String's remaining twelve known-unsupported cases require eval.
All newly verified cases passed every required native execution variant on the
development branch. This is branch-local evidence; central reporting was
unavailable, and these results are not presented as central trusted acceptance.

Async-arrow-function, async-function, await, and delete expression features now
have results for every applicable pinned case. Six cases require unsupported
eval; all other cases have verified native results, including all required
variants of the newly verified cases. This completion uses the same explicitly
branch-local evidence scope.

The core-expression operator batch adds 197 registrations: 181 verified passing
cases (including 54 parse-negative cases) and 16 explicit eval skips. The
affected operator rows are reconciled against unique pinned files and fresh
native results for their already-registered cases, producing a net increase of
306 verified passing results in this report. These rows now contain 484 verified
passing cases and 16 known-unsupported eval cases; comma retains one unverified
proper-tail-call case, and coalesce retains two outside this batch. Candidate
selection used current central registration reporting through the registered
MCP connector. Native validation remains branch-local, not central trusted
acceptance.

New-expression, property-accessor, and this-keyword groups now have results for
all applicable pinned cases: 78 verified passing and eight unsupported eval
cases. This batch adds 51 exact fixtures: 43 verified passing, including two
parse-negative cases, and eight eval skips. All required native variants of
both the new and previously registered cases were checked under one frozen
Release build. Catalogue MCP tools were unavailable for this batch, so
selection and acceptance are explicitly local diagnosis/PR validation, not
fresh central reporting or trusted central acceptance.

Yield expressions now have verified passing results for all 63 applicable
pinned cases, including all 123 required native execution variants. The 36
newly verified cases cover operand errors and delegated iterator next, return,
throw, result-access, and cleanup semantics. No case in this area depends on
unsupported eval. Selection and validation are branch-local: catalogue MCP
tools were unavailable, and the Data API denied the reporting schema. These
results are not presented as central trusted acceptance.

Dynamic import now has results for every applicable pinned case: 940 verified
passing and one unsupported eval case. All 1,776 required native variants of the
940 runnable cases pass under one frozen Release build. Coverage includes
specifier and options evaluation, import attributes, namespace exotic-object
operations, cached evaluation failures, source-phase rejection for JavaScript,
and deferred evaluation with asynchronous dependencies. These results are
branch-local; catalogue MCP tools were unavailable and the Data API denied the
reporting schema. They are not central trusted acceptance.

## Native test runner

Each runnable `[Fact]` in `tests/Jroc.Test262.Tests` remains an independent xUnit case.
During execution, the runner discovers registered (non-skipped) fixture calls in the
C# files beside each `JavaScript/` folder and compiles those entries into one
in-memory assembly for that folder. Nested fixtures under the same `JavaScript/`
directory share it; a nested test directory with its own `JavaScript/` directory
has a separate assembly. Logical entry names avoid CLR identifier collisions,
while the original fixture paths remain the source paths for debugging.
Fixtures marked `flags: [module]` explicitly select the Module parse goal
without changing their paths, even when their source has no import or export declarations.
Other fixtures select the standard Script goal, rejecting top-level return and
import/export declarations and instantiating realm-wide global declarations.
Script-goal entries use private logical paths while retaining their original
source paths for diagnostics. Importing the original path therefore creates a
distinct Module Record rather than reusing the Script Record. Computed imports
bundle sibling module/JSON support files without executing them as entries.
The native `$262.evalScript` helper compiles additional global scripts into
collectible assemblies in the current realm; it does not implement JavaScript
direct or indirect `eval`. Script completion values are not currently exposed
by this host helper.
The assembly is loaded once per folder and unloaded when the xUnit execution
run ends; each case executes only its own entry with fresh runtime services,
test262 helpers, async completion, and output capture.

Expected parse/compilation-failure facts are deliberately **not** part of the
shared successful assembly: they retain separate in-memory compilation and
their own diagnostic assertions. Skipped facts and support files are not
compiled as independent entries. An unexpected failure building a runnable
folder fails its test cases rather than retrying them as individual assemblies.
Worker-based Atomics tests use a native `$262.agent` host implementation with
independent compiled agent scripts, shared buffer backing stores, reports, and
bounded worker cleanup.

## Overall ECMA-262 Areas

| Area | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| Annex B | 60 | 0 | 1,026 | 1,086 | **5.52%** |
| Built-in objects and APIs | 17,170 | 24 | 6,318 | 23,512 | **73.03%** |
| Language syntax and semantics | 19,818 | 248 | 3,581 | 23,647 | **83.81%** |
| **Total** | 37,048 | 272 | 10,925 | 48,245 | **76.79%** |

## Language Areas

| Feature | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| `arguments-object` | 261 | 2 | 0 | 263 | **99.24%** |
| `asi` | 102 | 0 | 0 | 102 | **100.00%** |
| `block-scope` | 144 | 1 | 0 | 145 | **99.31%** |
| `comments` | 46 | 6 | 0 | 52 | **88.46%** |
| `computed-property-names` | 48 | 0 | 0 | 48 | **100.00%** |
| `destructuring` | 19 | 0 | 0 | 19 | **100.00%** |
| `directive-prologue` | 55 | 0 | 7 | 62 | **88.71%** |
| `eval-code` | 0 | 0 | 347 | 347 | **0.00%** |
| `export` | 3 | 0 | 0 | 3 | **100.00%** |
| `expressions` | 10,828 | 189 | 21 | 11,038 | **98.10%** |
| `function-code` | 204 | 13 | 0 | 217 | **94.01%** |
| `future-reserved-words` | 55 | 0 | 0 | 55 | **100.00%** |
| `global-code` | 41 | 1 | 0 | 42 | **97.62%** |
| `identifier-resolution` | 8 | 0 | 6 | 14 | **57.14%** |
| `identifiers` | 152 | 0 | 116 | 268 | **56.72%** |
| `import` | 10 | 0 | 117 | 127 | **7.87%** |
| `keywords` | 25 | 0 | 0 | 25 | **100.00%** |
| `line-terminators` | 25 | 0 | 16 | 41 | **60.98%** |
| `literals` | 505 | 0 | 29 | 534 | **94.57%** |
| `module-code` | 34 | 0 | 560 | 594 | **5.72%** |
| `punctuators` | 11 | 0 | 0 | 11 | **100.00%** |
| `reserved-words` | 26 | 0 | 1 | 27 | **96.30%** |
| `rest-parameters` | 9 | 0 | 2 | 11 | **81.82%** |
| `source-text` | 1 | 0 | 0 | 1 | **100.00%** |
| `statementList` | 40 | 0 | 40 | 80 | **50.00%** |
| `statements` | 6,887 | 36 | 2,414 | 9,337 | **73.76%** |
| `types` | 91 | 0 | 22 | 113 | **80.53%** |
| `white-space` | 51 | 0 | 16 | 67 | **76.12%** |

## Expression Features

| Feature | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| `addition` | 47 | 1 | 0 | 48 | **97.92%** |
| `array` | 52 | 0 | 0 | 52 | **100.00%** |
| `arrow-function` | 341 | 2 | 0 | 343 | **99.42%** |
| `assignment` | 489 | 0 | 0 | 489 | **100.00%** |
| `assignmenttargettype` | 324 | 0 | 0 | 324 | **100.00%** |
| `async-arrow-function` | 59 | 1 | 0 | 60 | **98.33%** |
| `async-function` | 89 | 4 | 0 | 93 | **95.70%** |
| `async-generator` | 618 | 5 | 0 | 623 | **99.20%** |
| `await` | 22 | 0 | 0 | 22 | **100.00%** |
| `bitwise-and` | 29 | 0 | 1 | 30 | **96.67%** |
| `bitwise-not` | 15 | 1 | 0 | 16 | **93.75%** |
| `bitwise-or` | 29 | 0 | 1 | 30 | **96.67%** |
| `bitwise-xor` | 29 | 0 | 1 | 30 | **96.67%** |
| `call` | 73 | 1 | 18 | 92 | **79.35%** |
| `class` | 3,955 | 104 | 0 | 4,059 | **97.44%** |
| `coalesce` | 22 | 0 | 2 | 24 | **91.67%** |
| `comma` | 4 | 1 | 1 | 6 | **66.67%** |
| `compound-assignment` | 434 | 20 | 0 | 454 | **95.59%** |
| `concatenation` | 0 | 0 | 5 | 5 | **0.00%** |
| `conditional` | 21 | 1 | 0 | 22 | **95.45%** |
| `delete` | 68 | 1 | 0 | 69 | **98.55%** |
| `division` | 43 | 0 | 2 | 45 | **95.56%** |
| `does-not-equals` | 36 | 0 | 2 | 38 | **94.74%** |
| `dynamic-import` | 940 | 1 | 0 | 941 | **99.89%** |
| `equals` | 45 | 0 | 2 | 47 | **95.74%** |
| `exponentiation` | 2 | 0 | 42 | 44 | **4.55%** |
| `function` | 246 | 3 | 15 | 264 | **93.18%** |
| `generators` | 262 | 0 | 28 | 290 | **90.34%** |
| `greater-than-or-equal` | 42 | 0 | 1 | 43 | **97.67%** |
| `greater-than` | 48 | 0 | 1 | 49 | **97.96%** |
| `grouping` | 8 | 1 | 0 | 9 | **88.89%** |
| `import.meta` | 0 | 0 | 22 | 22 | **0.00%** |
| `in` | 35 | 1 | 0 | 36 | **97.22%** |
| `instanceof` | 42 | 1 | 0 | 43 | **97.67%** |
| `left-shift` | 45 | 0 | 0 | 45 | **100.00%** |
| `less-than-or-equal` | 46 | 0 | 1 | 47 | **97.87%** |
| `less-than` | 44 | 0 | 1 | 45 | **97.78%** |
| `logical-and` | 17 | 1 | 0 | 18 | **94.44%** |
| `logical-assignment` | 78 | 0 | 0 | 78 | **100.00%** |
| `logical-not` | 17 | 2 | 0 | 19 | **89.47%** |
| `logical-or` | 17 | 1 | 0 | 18 | **94.44%** |
| `member-expression` | 1 | 0 | 0 | 1 | **100.00%** |
| `modulus` | 39 | 0 | 1 | 40 | **97.50%** |
| `multiplication` | 39 | 0 | 1 | 40 | **97.50%** |
| `new.target` | 11 | 0 | 3 | 14 | **78.57%** |
| `new` | 57 | 2 | 0 | 59 | **96.61%** |
| `object` | 1,148 | 22 | 0 | 1,170 | **98.12%** |
| `optional-chaining` | 37 | 1 | 0 | 38 | **97.37%** |
| `postfix-decrement` | 37 | 0 | 0 | 37 | **100.00%** |
| `postfix-increment` | 38 | 0 | 0 | 38 | **100.00%** |
| `prefix-decrement` | 34 | 0 | 0 | 34 | **100.00%** |
| `prefix-increment` | 33 | 0 | 0 | 33 | **100.00%** |
| `property-accessors` | 19 | 2 | 0 | 21 | **90.48%** |
| `relational` | 0 | 0 | 1 | 1 | **0.00%** |
| `right-shift` | 36 | 0 | 1 | 37 | **97.30%** |
| `strict-does-not-equals` | 28 | 0 | 2 | 30 | **93.33%** |
| `strict-equals` | 25 | 0 | 5 | 30 | **83.33%** |
| `subtraction` | 37 | 0 | 1 | 38 | **97.37%** |
| `super` | 42 | 0 | 52 | 94 | **44.68%** |
| `tagged-template` | 0 | 0 | 27 | 27 | **0.00%** |
| `tco-pos.js` | 1 | 0 | 0 | 1 | **100.00%** |
| `template-literal` | 3 | 0 | 54 | 57 | **5.26%** |
| `this` | 2 | 4 | 0 | 6 | **33.33%** |
| `typeof` | 15 | 1 | 0 | 16 | **93.75%** |
| `unary-minus` | 13 | 1 | 0 | 14 | **92.86%** |
| `unary-plus` | 15 | 2 | 0 | 17 | **88.24%** |
| `unsigned-right-shift` | 45 | 0 | 0 | 45 | **100.00%** |
| `void` | 8 | 1 | 0 | 9 | **88.89%** |
| `yield` | 63 | 0 | 0 | 63 | **100.00%** |

## Statement and Declaration Features

| Feature | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| `async-function` | 45 | 0 | 29 | 74 | **60.81%** |
| `async-generator` | 300 | 1 | 0 | 301 | **99.67%** |
| `await-using` | 27 | 0 | 67 | 94 | **28.72%** |
| `block` | 21 | 0 | 0 | 21 | **100.00%** |
| `break` | 19 | 0 | 1 | 20 | **95.00%** |
| `class` | 2,476 | 0 | 1,891 | 4,367 | **56.70%** |
| `const` | 134 | 1 | 1 | 136 | **98.53%** |
| `continue` | 23 | 0 | 1 | 24 | **95.83%** |
| `debugger` | 1 | 0 | 1 | 2 | **50.00%** |
| `do-while` | 30 | 0 | 6 | 36 | **83.33%** |
| `empty` | 1 | 0 | 1 | 2 | **50.00%** |
| `expression` | 1 | 0 | 2 | 3 | **33.33%** |
| `for-await-of` | 1,234 | 0 | 0 | 1,234 | **100.00%** |
| `for-in` | 102 | 13 | 0 | 115 | **88.70%** |
| `for-of` | 744 | 7 | 0 | 751 | **99.07%** |
| `for` | 368 | 0 | 17 | 385 | **95.58%** |
| `function` | 402 | 14 | 35 | 451 | **89.14%** |
| `generators` | 247 | 0 | 19 | 266 | **92.86%** |
| `if` | 59 | 0 | 10 | 69 | **85.51%** |
| `labeled` | 21 | 0 | 3 | 24 | **87.50%** |
| `let` | 140 | 0 | 5 | 145 | **96.55%** |
| `return` | 1 | 0 | 15 | 16 | **6.25%** |
| `switch` | 86 | 0 | 25 | 111 | **77.48%** |
| `throw` | 14 | 0 | 0 | 14 | **100.00%** |
| `try` | 152 | 0 | 49 | 201 | **75.62%** |
| `using` | 23 | 0 | 55 | 78 | **29.49%** |
| `variable` | 35 | 0 | 143 | 178 | **19.66%** |
| `while` | 19 | 0 | 19 | 38 | **50.00%** |
| `with` | 0 | 0 | 181 | 181 | **0.00%** |

## Built-in Objects and APIs

| Feature | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| `AbstractModuleSource` | 0 | 0 | 8 | 8 | **0.00%** |
| `AggregateError` | 25 | 0 | 0 | 25 | **100.00%** |
| `Array` | 3,081 | 0 | 0 | 3,081 | **100.00%** |
| `ArrayBuffer` | 192 | 0 | 4 | 196 | **97.96%** |
| `ArrayIteratorPrototype` | 21 | 0 | 6 | 27 | **77.78%** |
| `AsyncDisposableStack` | 104 | 0 | 0 | 104 | **100.00%** |
| `AsyncFromSyncIteratorPrototype` | 1 | 0 | 37 | 38 | **2.63%** |
| `AsyncFunction` | 17 | 0 | 1 | 18 | **94.44%** |
| `AsyncGeneratorFunction` | 14 | 0 | 9 | 23 | **60.87%** |
| `AsyncGeneratorPrototype` | 7 | 0 | 41 | 48 | **14.58%** |
| `AsyncIteratorPrototype` | 13 | 0 | 0 | 13 | **100.00%** |
| `Atomics` | 346 | 0 | 36 | 382 | **90.58%** |
| `BigInt` | 76 | 0 | 1 | 77 | **98.70%** |
| `Boolean` | 50 | 0 | 1 | 51 | **98.04%** |
| `DataView` | 552 | 0 | 9 | 561 | **98.40%** |
| `Date` | 594 | 0 | 0 | 594 | **100.00%** |
| `decodeURI` | 10 | 0 | 45 | 55 | **18.18%** |
| `decodeURIComponent` | 0 | 0 | 56 | 56 | **0.00%** |
| `DisposableStack` | 93 | 0 | 0 | 93 | **100.00%** |
| `encodeURI` | 10 | 0 | 21 | 31 | **32.26%** |
| `encodeURIComponent` | 0 | 0 | 31 | 31 | **0.00%** |
| `Error` | 57 | 0 | 1 | 58 | **98.28%** |
| `eval` | 0 | 0 | 10 | 10 | **0.00%** |
| `FinalizationRegistry` | 47 | 0 | 0 | 47 | **100.00%** |
| `Function` | 422 | 1 | 86 | 509 | **82.91%** |
| `GeneratorFunction` | 21 | 0 | 2 | 23 | **91.30%** |
| `GeneratorPrototype` | 51 | 0 | 10 | 61 | **83.61%** |
| `global` | 19 | 10 | 0 | 29 | **65.52%** |
| `Infinity` | 5 | 0 | 1 | 6 | **83.33%** |
| `isFinite` | 9 | 0 | 6 | 15 | **60.00%** |
| `isNaN` | 9 | 0 | 6 | 15 | **60.00%** |
| `Iterator` | 503 | 0 | 7 | 510 | **98.63%** |
| `JSON` | 162 | 0 | 3 | 165 | **98.18%** |
| `Map` | 197 | 0 | 7 | 204 | **96.57%** |
| `MapIteratorPrototype` | 6 | 0 | 5 | 11 | **54.55%** |
| `Math` | 327 | 0 | 0 | 327 | **100.00%** |
| `NaN` | 3 | 0 | 3 | 6 | **50.00%** |
| `NativeErrors` | 94 | 0 | 0 | 94 | **100.00%** |
| `Number` | 338 | 0 | 0 | 338 | **100.00%** |
| `Object` | 3,369 | 1 | 41 | 3,411 | **98.77%** |
| `parseFloat` | 32 | 0 | 22 | 54 | **59.26%** |
| `parseInt` | 42 | 0 | 13 | 55 | **76.36%** |
| `Promise` | 452 | 0 | 225 | 677 | **66.77%** |
| `Proxy` | 267 | 0 | 44 | 311 | **85.85%** |
| `Reflect` | 152 | 0 | 1 | 153 | **99.35%** |
| `RegExp` | 1,174 | 0 | 705 | 1,879 | **62.48%** |
| `RegExpStringIteratorPrototype` | 15 | 0 | 2 | 17 | **88.24%** |
| `Set` | 383 | 0 | 0 | 383 | **100.00%** |
| `SetIteratorPrototype` | 1 | 0 | 10 | 11 | **9.09%** |
| `ShadowRealm` | 0 | 0 | 64 | 64 | **0.00%** |
| `SharedArrayBuffer` | 95 | 0 | 9 | 104 | **91.35%** |
| `String` | 1,211 | 12 | 0 | 1,223 | **99.02%** |
| `StringIteratorPrototype` | 6 | 0 | 1 | 7 | **85.71%** |
| `SuppressedError` | 20 | 0 | 2 | 22 | **90.91%** |
| `Symbol` | 76 | 0 | 22 | 98 | **77.55%** |
| `Temporal` | 0 | 0 | 4,584 | 4,584 | **0.00%** |
| `ThrowTypeError` | 0 | 0 | 14 | 14 | **0.00%** |
| `TypedArray` | 1,377 | 0 | 61 | 1,438 | **95.76%** |
| `TypedArrayConstructors` | 703 | 0 | 33 | 736 | **95.52%** |
| `Uint8Array` | 64 | 0 | 4 | 68 | **94.12%** |
| `undefined` | 0 | 0 | 8 | 8 | **0.00%** |
| `WeakMap` | 141 | 0 | 0 | 141 | **100.00%** |
| `WeakRef` | 29 | 0 | 0 | 29 | **100.00%** |
| `WeakSet` | 85 | 0 | 0 | 85 | **100.00%** |

## Annex B Features

| Feature | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| `built-ins/Array` | 0 | 0 | 1 | 1 | **0.00%** |
| `built-ins/Date` | 21 | 0 | 3 | 24 | **87.50%** |
| `built-ins/escape` | 0 | 0 | 16 | 16 | **0.00%** |
| `built-ins/Function` | 0 | 0 | 6 | 6 | **0.00%** |
| `built-ins/Object` | 0 | 0 | 1 | 1 | **0.00%** |
| `built-ins/RegExp` | 0 | 0 | 62 | 62 | **0.00%** |
| `built-ins/String` | 39 | 0 | 72 | 111 | **35.14%** |
| `built-ins/TypedArrayConstructors` | 0 | 0 | 1 | 1 | **0.00%** |
| `built-ins/unescape` | 0 | 0 | 19 | 19 | **0.00%** |
| `language/comments` | 0 | 0 | 8 | 8 | **0.00%** |
| `language/eval-code` | 0 | 0 | 469 | 469 | **0.00%** |
| `language/expressions` | 0 | 0 | 26 | 26 | **0.00%** |
| `language/function-code` | 0 | 0 | 159 | 159 | **0.00%** |
| `language/global-code` | 0 | 0 | 153 | 153 | **0.00%** |
| `language/literals` | 0 | 0 | 8 | 8 | **0.00%** |
| `language/statements` | 0 | 0 | 22 | 22 | **0.00%** |

## Scope and Source

The applicable corpus is the 48,245 ECMA-262 tests under `test/annexB`, `test/built-ins`, and `test/language` at the [pinned Test262 revision](https://github.com/tc39/test262/tree/2b2ecead6e828dd9af13a9ec72065e645724a50f/test). ECMA-402 internationalization tests, staging proposals, Test262 harness self-tests, and `_FIXTURE.js` support files are outside this report.

Feature names follow the upstream Test262 directory taxonomy. The broad language-area table and its expression and statement drill-downs overlap by design; do not add totals across those tables.
