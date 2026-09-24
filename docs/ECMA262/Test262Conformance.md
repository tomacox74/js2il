# JROC Test262 Conformance Detail

[Back to ECMA-262 Coverage Index](Index.md)

For resumable passing-but-unported discovery, see the [Test262 artifact catalog](Test262Catalog.md).
Its MVP-runner evidence is separate from the native conformance results in this report.

This report provides detailed Test262 conformance evidence for the current development branch following [JROC v0.12.27](https://github.com/tomacox74/js2il/releases/tag/v0.12.27).

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
| Built-in objects and APIs | 16,270 | 12 | 7,230 | 23,512 | **69.20%** |
| Language syntax and semantics | 15,458 | 46 | 8,143 | 23,647 | **65.37%** |
| **Total** | 31,728 | 58 | 16,459 | 48,245 | **65.76%** |

## Language Areas

| Feature | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| `arguments-object` | 199 | 0 | 64 | 263 | **75.67%** |
| `asi` | 102 | 0 | 0 | 102 | **100.00%** |
| `block-scope` | 144 | 1 | 0 | 145 | **99.31%** |
| `comments` | 22 | 0 | 30 | 52 | **42.31%** |
| `computed-property-names` | 0 | 0 | 48 | 48 | **0.00%** |
| `destructuring` | 18 | 0 | 1 | 19 | **94.74%** |
| `directive-prologue` | 55 | 0 | 7 | 62 | **88.71%** |
| `eval-code` | 0 | 0 | 347 | 347 | **0.00%** |
| `export` | 3 | 0 | 0 | 3 | **100.00%** |
| `expressions` | 7,131 | 10 | 3,897 | 11,038 | **64.60%** |
| `function-code` | 196 | 0 | 21 | 217 | **90.32%** |
| `future-reserved-words` | 55 | 0 | 0 | 55 | **100.00%** |
| `global-code` | 25 | 0 | 17 | 42 | **59.52%** |
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
| `statements` | 6,483 | 35 | 2,819 | 9,337 | **69.43%** |
| `types` | 91 | 0 | 22 | 113 | **80.53%** |
| `white-space` | 51 | 0 | 16 | 67 | **76.12%** |

## Expression Features

| Feature | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| `addition` | 45 | 0 | 3 | 48 | **93.75%** |
| `array` | 14 | 0 | 38 | 52 | **26.92%** |
| `arrow-function` | 336 | 0 | 7 | 343 | **97.96%** |
| `assignment` | 489 | 0 | 0 | 489 | **100.00%** |
| `assignmenttargettype` | 318 | 0 | 6 | 324 | **98.15%** |
| `async-arrow-function` | 40 | 0 | 20 | 60 | **66.67%** |
| `async-function` | 38 | 0 | 55 | 93 | **40.86%** |
| `async-generator` | 212 | 0 | 411 | 623 | **34.03%** |
| `await` | 9 | 0 | 13 | 22 | **40.91%** |
| `bitwise-and` | 29 | 0 | 1 | 30 | **96.67%** |
| `bitwise-not` | 8 | 0 | 8 | 16 | **50.00%** |
| `bitwise-or` | 29 | 0 | 1 | 30 | **96.67%** |
| `bitwise-xor` | 29 | 0 | 1 | 30 | **96.67%** |
| `call` | 73 | 1 | 18 | 92 | **79.35%** |
| `class` | 2,302 | 0 | 1,757 | 4,059 | **56.71%** |
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
| `object` | 808 | 6 | 356 | 1,170 | **69.06%** |
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
| `class` | 2,283 | 0 | 2,084 | 4,367 | **52.28%** |
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
| `generators` | 197 | 0 | 69 | 266 | **74.06%** |
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
| `AggregateError` | 24 | 0 | 1 | 25 | **96.00%** |
| `Array` | 3,045 | 0 | 36 | 3,081 | **98.83%** |
| `ArrayBuffer` | 191 | 0 | 5 | 196 | **97.45%** |
| `ArrayIteratorPrototype` | 18 | 0 | 9 | 27 | **66.67%** |
| `AsyncDisposableStack` | 103 | 0 | 1 | 104 | **99.04%** |
| `AsyncFromSyncIteratorPrototype` | 1 | 0 | 37 | 38 | **2.63%** |
| `AsyncFunction` | 17 | 0 | 1 | 18 | **94.44%** |
| `AsyncGeneratorFunction` | 9 | 0 | 14 | 23 | **39.13%** |
| `AsyncGeneratorPrototype` | 2 | 0 | 46 | 48 | **4.17%** |
| `AsyncIteratorPrototype` | 3 | 0 | 10 | 13 | **23.08%** |
| `Atomics` | 71 | 0 | 311 | 382 | **18.59%** |
| `BigInt` | 76 | 0 | 1 | 77 | **98.70%** |
| `Boolean` | 49 | 0 | 2 | 51 | **96.08%** |
| `DataView` | 550 | 0 | 11 | 561 | **98.04%** |
| `Date` | 575 | 0 | 19 | 594 | **96.80%** |
| `decodeURI` | 10 | 0 | 45 | 55 | **18.18%** |
| `decodeURIComponent` | 0 | 0 | 56 | 56 | **0.00%** |
| `DisposableStack` | 92 | 0 | 1 | 93 | **98.92%** |
| `encodeURI` | 10 | 0 | 21 | 31 | **32.26%** |
| `encodeURIComponent` | 0 | 0 | 31 | 31 | **0.00%** |
| `Error` | 55 | 0 | 3 | 58 | **94.83%** |
| `eval` | 0 | 0 | 10 | 10 | **0.00%** |
| `FinalizationRegistry` | 46 | 0 | 1 | 47 | **97.87%** |
| `Function` | 409 | 1 | 99 | 509 | **80.35%** |
| `GeneratorFunction` | 17 | 0 | 6 | 23 | **73.91%** |
| `GeneratorPrototype` | 45 | 0 | 16 | 61 | **73.77%** |
| `global` | 19 | 10 | 0 | 29 | **65.52%** |
| `Infinity` | 5 | 0 | 1 | 6 | **83.33%** |
| `isFinite` | 9 | 0 | 6 | 15 | **60.00%** |
| `isNaN` | 9 | 0 | 6 | 15 | **60.00%** |
| `Iterator` | 403 | 0 | 107 | 510 | **79.02%** |
| `JSON` | 162 | 0 | 3 | 165 | **98.18%** |
| `Map` | 197 | 0 | 7 | 204 | **96.57%** |
| `MapIteratorPrototype` | 6 | 0 | 5 | 11 | **54.55%** |
| `Math` | 327 | 0 | 0 | 327 | **100.00%** |
| `NaN` | 3 | 0 | 3 | 6 | **50.00%** |
| `NativeErrors` | 88 | 0 | 6 | 94 | **93.62%** |
| `Number` | 337 | 0 | 1 | 338 | **99.70%** |
| `Object` | 3,338 | 0 | 73 | 3,411 | **97.86%** |
| `parseFloat` | 32 | 0 | 22 | 54 | **59.26%** |
| `parseInt` | 42 | 0 | 13 | 55 | **76.36%** |
| `Promise` | 384 | 0 | 293 | 677 | **56.72%** |
| `Proxy` | 267 | 0 | 44 | 311 | **85.85%** |
| `Reflect` | 152 | 0 | 1 | 153 | **99.35%** |
| `RegExp` | 1,050 | 0 | 829 | 1,879 | **55.88%** |
| `RegExpStringIteratorPrototype` | 15 | 0 | 2 | 17 | **88.24%** |
| `Set` | 383 | 0 | 0 | 383 | **100.00%** |
| `SetIteratorPrototype` | 1 | 0 | 10 | 11 | **9.09%** |
| `ShadowRealm` | 0 | 0 | 64 | 64 | **0.00%** |
| `SharedArrayBuffer` | 69 | 0 | 35 | 104 | **66.35%** |
| `String` | 1,194 | 1 | 28 | 1,223 | **97.63%** |
| `StringIteratorPrototype` | 6 | 0 | 1 | 7 | **85.71%** |
| `SuppressedError` | 20 | 0 | 2 | 22 | **90.91%** |
| `Symbol` | 76 | 0 | 22 | 98 | **77.55%** |
| `Temporal` | 0 | 0 | 4,584 | 4,584 | **0.00%** |
| `ThrowTypeError` | 0 | 0 | 14 | 14 | **0.00%** |
| `TypedArray` | 1,365 | 0 | 73 | 1,438 | **94.92%** |
| `TypedArrayConstructors` | 593 | 0 | 143 | 736 | **80.57%** |
| `Uint8Array` | 46 | 0 | 22 | 68 | **67.65%** |
| `undefined` | 0 | 0 | 8 | 8 | **0.00%** |
| `WeakMap` | 141 | 0 | 0 | 141 | **100.00%** |
| `WeakRef` | 28 | 0 | 1 | 29 | **96.55%** |
| `WeakSet` | 85 | 0 | 0 | 85 | **100.00%** |

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
