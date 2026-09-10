# JROC Test262 Conformance Detail

[Back to ECMA-262 Coverage Index](Index.md)

For resumable passing-but-unported discovery, see the [Test262 artifact catalog](Test262Catalog.md).
Its MVP-runner evidence is separate from the native conformance results in this report.

This report provides detailed Test262 conformance evidence for the current development branch following [JROC v0.12.21](https://github.com/tomacox74/js2il/releases/tag/v0.12.21).

## How to Read This Report

- **Verified passing**: the native harness verifies the corresponding Test262 outcome: positive tests execute successfully, while negative tests are rejected at their declared parse or runtime phase.
- **Known unsupported**: the test exercises behavior explicitly excluded from the release, primarily `eval`.
- **No published result**: JROC has not published a conformance result for the test. This does not imply either support or failure.
- **Verified**: verified passing tests divided by applicable tests in that row.

Counts are unique, standalone Test262 files. Strict and non-strict execution variants are not counted separately.

## Overall ECMA-262 Areas

| Area | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| Annex B | 0 | 0 | 1,086 | 1,086 | **0.00%** |
| Built-in objects and APIs | 14,583 | 12 | 8,917 | 23,512 | **62.02%** |
| Language syntax and semantics | 13,912 | 46 | 9,685 | 23,643 | **58.84%** |
| **Total** | 28,495 | 58 | 19,688 | 48,241 | **59.07%** |

## Language Areas

| Feature | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| `arguments-object` | 176 | 0 | 87 | 263 | **66.92%** |
| `asi` | 102 | 0 | 0 | 102 | **100.00%** |
| `block-scope` | 35 | 1 | 109 | 145 | **24.14%** |
| `comments` | 22 | 0 | 30 | 52 | **42.31%** |
| `computed-property-names` | 0 | 0 | 48 | 48 | **0.00%** |
| `destructuring` | 18 | 0 | 1 | 19 | **94.74%** |
| `directive-prologue` | 55 | 0 | 7 | 62 | **88.71%** |
| `eval-code` | 0 | 0 | 347 | 347 | **0.00%** |
| `export` | 3 | 0 | 0 | 3 | **100.00%** |
| `expressions` | 6,790 | 10 | 4,238 | 11,038 | **61.51%** |
| `function-code` | 40 | 0 | 177 | 217 | **18.43%** |
| `future-reserved-words` | 55 | 0 | 0 | 55 | **100.00%** |
| `global-code` | 25 | 0 | 17 | 42 | **59.52%** |
| `identifier-resolution` | 8 | 0 | 6 | 14 | **57.14%** |
| `identifiers` | 89 | 0 | 179 | 268 | **33.21%** |
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
| `statements` | 5,746 | 35 | 3,556 | 9,337 | **61.54%** |
| `types` | 11 | 0 | 102 | 113 | **9.73%** |
| `white-space` | 51 | 0 | 16 | 67 | **76.12%** |

## Expression Features

| Feature | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| `addition` | 45 | 0 | 3 | 48 | **93.75%** |
| `array` | 14 | 0 | 38 | 52 | **26.92%** |
| `arrow-function` | 336 | 0 | 7 | 343 | **97.96%** |
| `assignment` | 485 | 0 | 0 | 485 | **100.00%** |
| `assignmenttargettype` | 310 | 0 | 14 | 324 | **95.68%** |
| `async-arrow-function` | 40 | 0 | 20 | 60 | **66.67%** |
| `async-function` | 38 | 0 | 55 | 93 | **40.86%** |
| `async-generator` | 212 | 0 | 411 | 623 | **34.03%** |
| `await` | 9 | 0 | 13 | 22 | **40.91%** |
| `bitwise-and` | 29 | 0 | 1 | 30 | **96.67%** |
| `bitwise-not` | 8 | 0 | 8 | 16 | **50.00%** |
| `bitwise-or` | 29 | 0 | 1 | 30 | **96.67%** |
| `bitwise-xor` | 29 | 0 | 1 | 30 | **96.67%** |
| `call` | 73 | 1 | 18 | 92 | **79.35%** |
| `class` | 2,295 | 0 | 1,764 | 4,059 | **56.54%** |
| `coalesce` | 22 | 0 | 2 | 24 | **91.67%** |
| `comma` | 5 | 0 | 1 | 6 | **83.33%** |
| `compound-assignment` | 355 | 0 | 99 | 454 | **78.19%** |
| `concatenation` | 0 | 0 | 5 | 5 | **0.00%** |
| `conditional` | 14 | 0 | 8 | 22 | **63.64%** |
| `delete` | 7 | 0 | 62 | 69 | **10.14%** |
| `division` | 43 | 0 | 2 | 45 | **95.56%** |
| `does-not-equals` | 36 | 0 | 2 | 38 | **94.74%** |
| `dynamic-import` | 301 | 0 | 640 | 941 | **31.99%** |
| `equals` | 45 | 0 | 2 | 47 | **95.74%** |
| `exponentiation` | 2 | 0 | 42 | 44 | **4.55%** |
| `function` | 246 | 3 | 15 | 264 | **93.18%** |
| `generators` | 212 | 0 | 78 | 290 | **73.10%** |
| `greater-than-or-equal` | 42 | 0 | 1 | 43 | **97.67%** |
| `greater-than` | 48 | 0 | 1 | 49 | **97.96%** |
| `grouping` | 6 | 0 | 3 | 9 | **66.67%** |
| `import.meta` | 0 | 0 | 22 | 22 | **0.00%** |
| `in` | 10 | 0 | 26 | 36 | **27.78%** |
| `instanceof` | 6 | 0 | 37 | 43 | **13.95%** |
| `left-shift` | 45 | 0 | 0 | 45 | **100.00%** |
| `less-than-or-equal` | 46 | 0 | 1 | 47 | **97.87%** |
| `less-than` | 44 | 0 | 1 | 45 | **97.78%** |
| `logical-and` | 17 | 0 | 1 | 18 | **94.44%** |
| `logical-assignment` | 48 | 0 | 30 | 78 | **61.54%** |
| `logical-not` | 6 | 0 | 13 | 19 | **31.58%** |
| `logical-or` | 17 | 0 | 1 | 18 | **94.44%** |
| `member-expression` | 1 | 0 | 0 | 1 | **100.00%** |
| `modulus` | 39 | 0 | 1 | 40 | **97.50%** |
| `multiplication` | 39 | 0 | 1 | 40 | **97.50%** |
| `new.target` | 11 | 0 | 3 | 14 | **78.57%** |
| `new` | 32 | 0 | 27 | 59 | **54.24%** |
| `object` | 800 | 6 | 364 | 1,170 | **68.38%** |
| `optional-chaining` | 6 | 0 | 32 | 38 | **15.79%** |
| `postfix-decrement` | 2 | 0 | 35 | 37 | **5.41%** |
| `postfix-increment` | 2 | 0 | 36 | 38 | **5.26%** |
| `prefix-decrement` | 2 | 0 | 32 | 34 | **5.88%** |
| `prefix-increment` | 6 | 0 | 27 | 33 | **18.18%** |
| `property-accessors` | 3 | 0 | 18 | 21 | **14.29%** |
| `relational` | 0 | 0 | 1 | 1 | **0.00%** |
| `right-shift` | 36 | 0 | 1 | 37 | **97.30%** |
| `strict-does-not-equals` | 28 | 0 | 2 | 30 | **93.33%** |
| `strict-equals` | 25 | 0 | 5 | 30 | **83.33%** |
| `subtraction` | 37 | 0 | 1 | 38 | **97.37%** |
| `super` | 42 | 0 | 52 | 94 | **44.68%** |
| `tagged-template` | 0 | 0 | 27 | 27 | **0.00%** |
| `tco-pos.js` | 1 | 0 | 0 | 1 | **100.00%** |
| `template-literal` | 3 | 0 | 54 | 57 | **5.26%** |
| `this` | 0 | 0 | 6 | 6 | **0.00%** |
| `typeof` | 1 | 0 | 15 | 16 | **6.25%** |
| `unary-minus` | 8 | 0 | 6 | 14 | **57.14%** |
| `unary-plus` | 10 | 0 | 7 | 17 | **58.82%** |
| `unsigned-right-shift` | 45 | 0 | 0 | 45 | **100.00%** |
| `void` | 4 | 0 | 5 | 9 | **44.44%** |
| `yield` | 27 | 0 | 36 | 63 | **42.86%** |

## Statement and Declaration Features

| Feature | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| `async-function` | 45 | 0 | 29 | 74 | **60.81%** |
| `async-generator` | 89 | 0 | 212 | 301 | **29.57%** |
| `await-using` | 27 | 0 | 67 | 94 | **28.72%** |
| `block` | 21 | 0 | 0 | 21 | **100.00%** |
| `break` | 19 | 0 | 1 | 20 | **95.00%** |
| `class` | 2,275 | 0 | 2,092 | 4,367 | **52.10%** |
| `const` | 59 | 1 | 76 | 136 | **43.38%** |
| `continue` | 23 | 0 | 1 | 24 | **95.83%** |
| `debugger` | 1 | 0 | 1 | 2 | **50.00%** |
| `do-while` | 30 | 0 | 6 | 36 | **83.33%** |
| `empty` | 1 | 0 | 1 | 2 | **50.00%** |
| `expression` | 1 | 0 | 2 | 3 | **33.33%** |
| `for-await-of` | 1,234 | 0 | 0 | 1,234 | **100.00%** |
| `for-in` | 102 | 13 | 0 | 115 | **88.70%** |
| `for-of` | 744 | 7 | 0 | 751 | **99.07%** |
| `for` | 110 | 0 | 275 | 385 | **28.57%** |
| `function` | 402 | 14 | 35 | 451 | **89.14%** |
| `generators` | 197 | 0 | 69 | 266 | **74.06%** |
| `if` | 59 | 0 | 10 | 69 | **85.51%** |
| `labeled` | 21 | 0 | 3 | 24 | **87.50%** |
| `let` | 36 | 0 | 109 | 145 | **24.83%** |
| `return` | 1 | 0 | 15 | 16 | **6.25%** |
| `switch` | 86 | 0 | 25 | 111 | **77.48%** |
| `throw` | 14 | 0 | 0 | 14 | **100.00%** |
| `try` | 89 | 0 | 112 | 201 | **44.28%** |
| `using` | 23 | 0 | 55 | 78 | **29.49%** |
| `variable` | 18 | 0 | 160 | 178 | **10.11%** |
| `while` | 19 | 0 | 19 | 38 | **50.00%** |
| `with` | 0 | 0 | 181 | 181 | **0.00%** |

## Built-in Objects and APIs

| Feature | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| `AbstractModuleSource` | 0 | 0 | 8 | 8 | **0.00%** |
| `AggregateError` | 24 | 0 | 1 | 25 | **96.00%** |
| `Array` | 2,871 | 0 | 210 | 3,081 | **93.18%** |
| `ArrayBuffer` | 189 | 0 | 7 | 196 | **96.43%** |
| `ArrayIteratorPrototype` | 8 | 0 | 19 | 27 | **29.63%** |
| `AsyncDisposableStack` | 100 | 0 | 4 | 104 | **96.15%** |
| `AsyncFromSyncIteratorPrototype` | 0 | 0 | 38 | 38 | **0.00%** |
| `AsyncFunction` | 17 | 0 | 1 | 18 | **94.44%** |
| `AsyncGeneratorFunction` | 6 | 0 | 17 | 23 | **26.09%** |
| `AsyncGeneratorPrototype` | 0 | 0 | 48 | 48 | **0.00%** |
| `AsyncIteratorPrototype` | 0 | 0 | 13 | 13 | **0.00%** |
| `Atomics` | 71 | 0 | 311 | 382 | **18.59%** |
| `BigInt` | 49 | 0 | 28 | 77 | **63.64%** |
| `Boolean` | 14 | 0 | 37 | 51 | **27.45%** |
| `DataView` | 502 | 0 | 59 | 561 | **89.48%** |
| `Date` | 521 | 0 | 73 | 594 | **87.71%** |
| `decodeURI` | 10 | 0 | 45 | 55 | **18.18%** |
| `decodeURIComponent` | 0 | 0 | 56 | 56 | **0.00%** |
| `DisposableStack` | 92 | 0 | 1 | 93 | **98.92%** |
| `encodeURI` | 10 | 0 | 21 | 31 | **32.26%** |
| `encodeURIComponent` | 0 | 0 | 31 | 31 | **0.00%** |
| `Error` | 19 | 0 | 39 | 58 | **32.76%** |
| `eval` | 0 | 0 | 10 | 10 | **0.00%** |
| `FinalizationRegistry` | 20 | 0 | 27 | 47 | **42.55%** |
| `Function` | 392 | 1 | 116 | 509 | **77.01%** |
| `GeneratorFunction` | 14 | 0 | 9 | 23 | **60.87%** |
| `GeneratorPrototype` | 0 | 0 | 61 | 61 | **0.00%** |
| `global` | 19 | 10 | 0 | 29 | **65.52%** |
| `Infinity` | 0 | 0 | 6 | 6 | **0.00%** |
| `isFinite` | 9 | 0 | 6 | 15 | **60.00%** |
| `isNaN` | 9 | 0 | 6 | 15 | **60.00%** |
| `Iterator` | 317 | 0 | 193 | 510 | **62.16%** |
| `JSON` | 161 | 0 | 4 | 165 | **97.58%** |
| `Map` | 142 | 0 | 62 | 204 | **69.61%** |
| `MapIteratorPrototype` | 1 | 0 | 10 | 11 | **9.09%** |
| `Math` | 327 | 0 | 0 | 327 | **100.00%** |
| `NaN` | 0 | 0 | 6 | 6 | **0.00%** |
| `NativeErrors` | 15 | 0 | 79 | 94 | **15.96%** |
| `Number` | 282 | 0 | 56 | 338 | **83.43%** |
| `Object` | 3,200 | 0 | 211 | 3,411 | **93.81%** |
| `parseFloat` | 32 | 0 | 22 | 54 | **59.26%** |
| `parseInt` | 42 | 0 | 13 | 55 | **76.36%** |
| `Promise` | 380 | 0 | 297 | 677 | **56.13%** |
| `Proxy` | 45 | 0 | 266 | 311 | **14.47%** |
| `Reflect` | 54 | 0 | 99 | 153 | **35.29%** |
| `RegExp` | 1,006 | 0 | 873 | 1,879 | **53.54%** |
| `RegExpStringIteratorPrototype` | 5 | 0 | 12 | 17 | **29.41%** |
| `Set` | 382 | 0 | 1 | 383 | **99.74%** |
| `SetIteratorPrototype` | 1 | 0 | 10 | 11 | **9.09%** |
| `ShadowRealm` | 0 | 0 | 64 | 64 | **0.00%** |
| `SharedArrayBuffer` | 69 | 0 | 35 | 104 | **66.35%** |
| `String` | 1,093 | 1 | 129 | 1,223 | **89.37%** |
| `StringIteratorPrototype` | 0 | 0 | 7 | 7 | **0.00%** |
| `SuppressedError` | 20 | 0 | 2 | 22 | **90.91%** |
| `Symbol` | 62 | 0 | 36 | 98 | **63.27%** |
| `Temporal` | 0 | 0 | 4,584 | 4,584 | **0.00%** |
| `ThrowTypeError` | 0 | 0 | 14 | 14 | **0.00%** |
| `TypedArray` | 1,162 | 0 | 276 | 1,438 | **80.81%** |
| `TypedArrayConstructors` | 573 | 0 | 163 | 736 | **77.85%** |
| `Uint8Array` | 46 | 0 | 22 | 68 | **67.65%** |
| `undefined` | 0 | 0 | 8 | 8 | **0.00%** |
| `WeakMap` | 105 | 0 | 36 | 141 | **74.47%** |
| `WeakRef` | 20 | 0 | 9 | 29 | **68.97%** |
| `WeakSet` | 75 | 0 | 10 | 85 | **88.24%** |

## Annex B Features

| Feature | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| `built-ins/Array` | 0 | 0 | 1 | 1 | **0.00%** |
| `built-ins/Date` | 0 | 0 | 24 | 24 | **0.00%** |
| `built-ins/escape` | 0 | 0 | 16 | 16 | **0.00%** |
| `built-ins/Function` | 0 | 0 | 6 | 6 | **0.00%** |
| `built-ins/Object` | 0 | 0 | 1 | 1 | **0.00%** |
| `built-ins/RegExp` | 0 | 0 | 62 | 62 | **0.00%** |
| `built-ins/String` | 0 | 0 | 111 | 111 | **0.00%** |
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

The applicable corpus is the 48,241 ECMA-262 tests under `test/annexB`, `test/built-ins`, and `test/language` at the [pinned Test262 revision](https://github.com/tc39/test262/tree/2b2ecead6e828dd9af13a9ec72065e645724a50f/test). ECMA-402 internationalization tests, staging proposals, Test262 harness self-tests, and `_FIXTURE.js` support files are outside this report.

Feature names follow the upstream Test262 directory taxonomy. The broad language-area table and its expression and statement drill-downs overlap by design; do not add totals across those tables.
