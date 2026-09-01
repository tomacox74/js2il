# JROC Test262 Conformance Detail

[Back to ECMA-262 Coverage Index](Index.md)

This report provides detailed Test262 conformance evidence for the current development branch following [JROC v0.12.15](https://github.com/tomacox74/js2il/releases/tag/v0.12.15).

## How to Read This Report

- **Verified passing**: the current development branch successfully executes the corresponding Test262 test.
- **Known unsupported**: the test exercises behavior explicitly excluded from the release, primarily `eval`.
- **No published result**: JROC has not published a conformance result for the test. This does not imply either support or failure.
- **Verified**: verified passing tests divided by applicable tests in that row.

Counts are unique, standalone Test262 files. Strict and non-strict execution variants are not counted separately.

## Overall ECMA-262 Areas

| Area | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| Annex B | 0 | 0 | 1,086 | 1,086 | **0.00%** |
| Built-in objects and APIs | 8,596 | 12 | 14,904 | 23,512 | **36.56%** |
| Language syntax and semantics | 3,688 | 46 | 19,909 | 23,643 | **15.60%** |
| **Total** | 12,284 | 58 | 35,899 | 48,241 | **25.46%** |

## Language Areas

| Feature | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| `arguments-object` | 15 | 0 | 248 | 263 | **5.70%** |
| `asi` | 0 | 0 | 102 | 102 | **0.00%** |
| `block-scope` | 35 | 1 | 109 | 145 | **24.14%** |
| `comments` | 0 | 0 | 52 | 52 | **0.00%** |
| `computed-property-names` | 0 | 0 | 48 | 48 | **0.00%** |
| `destructuring` | 18 | 0 | 1 | 19 | **94.74%** |
| `directive-prologue` | 11 | 0 | 51 | 62 | **17.74%** |
| `eval-code` | 0 | 0 | 347 | 347 | **0.00%** |
| `export` | 3 | 0 | 0 | 3 | **100.00%** |
| `expressions` | 737 | 10 | 10,291 | 11,038 | **6.68%** |
| `function-code` | 35 | 0 | 182 | 217 | **16.13%** |
| `future-reserved-words` | 0 | 0 | 55 | 55 | **0.00%** |
| `global-code` | 0 | 0 | 42 | 42 | **0.00%** |
| `identifier-resolution` | 0 | 0 | 14 | 14 | **0.00%** |
| `identifiers` | 5 | 0 | 263 | 268 | **1.87%** |
| `import` | 10 | 0 | 117 | 127 | **7.87%** |
| `keywords` | 0 | 0 | 25 | 25 | **0.00%** |
| `line-terminators` | 0 | 0 | 41 | 41 | **0.00%** |
| `literals` | 77 | 0 | 457 | 534 | **14.42%** |
| `module-code` | 34 | 0 | 560 | 594 | **5.72%** |
| `punctuators` | 0 | 0 | 11 | 11 | **0.00%** |
| `reserved-words` | 20 | 0 | 7 | 27 | **74.07%** |
| `rest-parameters` | 9 | 0 | 2 | 11 | **81.82%** |
| `source-text` | 0 | 0 | 1 | 1 | **0.00%** |
| `statementList` | 0 | 0 | 80 | 80 | **0.00%** |
| `statements` | 2,668 | 35 | 6,634 | 9,337 | **28.57%** |
| `types` | 11 | 0 | 102 | 113 | **9.73%** |
| `white-space` | 0 | 0 | 67 | 67 | **0.00%** |

## Expression Features

| Feature | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| `addition` | 10 | 0 | 38 | 48 | **20.83%** |
| `array` | 14 | 0 | 38 | 52 | **26.92%** |
| `arrow-function` | 61 | 0 | 282 | 343 | **17.78%** |
| `assignment` | 56 | 0 | 429 | 485 | **11.55%** |
| `assignmenttargettype` | 0 | 0 | 324 | 324 | **0.00%** |
| `async-arrow-function` | 24 | 0 | 36 | 60 | **40.00%** |
| `async-function` | 0 | 0 | 93 | 93 | **0.00%** |
| `async-generator` | 16 | 0 | 607 | 623 | **2.57%** |
| `await` | 3 | 0 | 19 | 22 | **13.64%** |
| `bitwise-and` | 8 | 0 | 22 | 30 | **26.67%** |
| `bitwise-not` | 8 | 0 | 8 | 16 | **50.00%** |
| `bitwise-or` | 8 | 0 | 22 | 30 | **26.67%** |
| `bitwise-xor` | 8 | 0 | 22 | 30 | **26.67%** |
| `call` | 19 | 1 | 72 | 92 | **20.65%** |
| `class` | 88 | 0 | 3,971 | 4,059 | **2.17%** |
| `coalesce` | 18 | 0 | 6 | 24 | **75.00%** |
| `comma` | 5 | 0 | 1 | 6 | **83.33%** |
| `compound-assignment` | 0 | 0 | 454 | 454 | **0.00%** |
| `concatenation` | 0 | 0 | 5 | 5 | **0.00%** |
| `conditional` | 12 | 0 | 10 | 22 | **54.55%** |
| `delete` | 5 | 0 | 64 | 69 | **7.25%** |
| `division` | 4 | 0 | 41 | 45 | **8.89%** |
| `does-not-equals` | 7 | 0 | 31 | 38 | **18.42%** |
| `dynamic-import` | 0 | 0 | 941 | 941 | **0.00%** |
| `equals` | 16 | 0 | 31 | 47 | **34.04%** |
| `exponentiation` | 2 | 0 | 42 | 44 | **4.55%** |
| `function` | 60 | 3 | 201 | 264 | **22.73%** |
| `generators` | 31 | 0 | 259 | 290 | **10.69%** |
| `greater-than-or-equal` | 5 | 0 | 38 | 43 | **11.63%** |
| `greater-than` | 12 | 0 | 37 | 49 | **24.49%** |
| `grouping` | 6 | 0 | 3 | 9 | **66.67%** |
| `import.meta` | 0 | 0 | 22 | 22 | **0.00%** |
| `in` | 10 | 0 | 26 | 36 | **27.78%** |
| `instanceof` | 6 | 0 | 37 | 43 | **13.95%** |
| `left-shift` | 11 | 0 | 34 | 45 | **24.44%** |
| `less-than-or-equal` | 5 | 0 | 42 | 47 | **10.64%** |
| `less-than` | 6 | 0 | 39 | 45 | **13.33%** |
| `logical-and` | 8 | 0 | 10 | 18 | **44.44%** |
| `logical-assignment` | 14 | 0 | 64 | 78 | **17.95%** |
| `logical-not` | 6 | 0 | 13 | 19 | **31.58%** |
| `logical-or` | 8 | 0 | 10 | 18 | **44.44%** |
| `member-expression` | 1 | 0 | 0 | 1 | **100.00%** |
| `modulus` | 4 | 0 | 36 | 40 | **10.00%** |
| `multiplication` | 4 | 0 | 36 | 40 | **10.00%** |
| `new.target` | 0 | 0 | 14 | 14 | **0.00%** |
| `new` | 0 | 0 | 59 | 59 | **0.00%** |
| `object` | 41 | 6 | 1,123 | 1,170 | **3.50%** |
| `optional-chaining` | 6 | 0 | 32 | 38 | **15.79%** |
| `postfix-decrement` | 2 | 0 | 35 | 37 | **5.41%** |
| `postfix-increment` | 2 | 0 | 36 | 38 | **5.26%** |
| `prefix-decrement` | 2 | 0 | 32 | 34 | **5.88%** |
| `prefix-increment` | 6 | 0 | 27 | 33 | **18.18%** |
| `property-accessors` | 3 | 0 | 18 | 21 | **14.29%** |
| `relational` | 0 | 0 | 1 | 1 | **0.00%** |
| `right-shift` | 6 | 0 | 31 | 37 | **16.22%** |
| `strict-does-not-equals` | 8 | 0 | 22 | 30 | **26.67%** |
| `strict-equals` | 10 | 0 | 20 | 30 | **33.33%** |
| `subtraction` | 4 | 0 | 34 | 38 | **10.53%** |
| `super` | 0 | 0 | 94 | 94 | **0.00%** |
| `tagged-template` | 0 | 0 | 27 | 27 | **0.00%** |
| `tco-pos.js` | 1 | 0 | 0 | 1 | **100.00%** |
| `template-literal` | 3 | 0 | 54 | 57 | **5.26%** |
| `this` | 0 | 0 | 6 | 6 | **0.00%** |
| `typeof` | 1 | 0 | 15 | 16 | **6.25%** |
| `unary-minus` | 8 | 0 | 6 | 14 | **57.14%** |
| `unary-plus` | 10 | 0 | 7 | 17 | **58.82%** |
| `unsigned-right-shift` | 15 | 0 | 30 | 45 | **33.33%** |
| `void` | 4 | 0 | 5 | 9 | **44.44%** |
| `yield` | 16 | 0 | 47 | 63 | **25.40%** |

## Statement and Declaration Features

| Feature | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| `async-function` | 33 | 0 | 41 | 74 | **44.59%** |
| `async-generator` | 0 | 0 | 301 | 301 | **0.00%** |
| `await-using` | 2 | 0 | 92 | 94 | **2.13%** |
| `block` | 17 | 0 | 4 | 21 | **80.95%** |
| `break` | 13 | 0 | 7 | 20 | **65.00%** |
| `class` | 130 | 0 | 4,237 | 4,367 | **2.98%** |
| `const` | 33 | 1 | 102 | 136 | **24.26%** |
| `continue` | 16 | 0 | 8 | 24 | **66.67%** |
| `debugger` | 0 | 0 | 2 | 2 | **0.00%** |
| `do-while` | 24 | 0 | 12 | 36 | **66.67%** |
| `empty` | 1 | 0 | 1 | 2 | **50.00%** |
| `expression` | 1 | 0 | 2 | 3 | **33.33%** |
| `for-await-of` | 1,234 | 0 | 0 | 1,234 | **100.00%** |
| `for-in` | 102 | 13 | 0 | 115 | **88.70%** |
| `for-of` | 744 | 7 | 0 | 751 | **99.07%** |
| `for` | 61 | 0 | 324 | 385 | **15.84%** |
| `function` | 45 | 14 | 392 | 451 | **9.98%** |
| `generators` | 0 | 0 | 266 | 266 | **0.00%** |
| `if` | 30 | 0 | 39 | 69 | **43.48%** |
| `labeled` | 12 | 0 | 12 | 24 | **50.00%** |
| `let` | 9 | 0 | 136 | 145 | **6.21%** |
| `return` | 1 | 0 | 15 | 16 | **6.25%** |
| `switch` | 20 | 0 | 91 | 111 | **18.02%** |
| `throw` | 14 | 0 | 0 | 14 | **100.00%** |
| `try` | 87 | 0 | 114 | 201 | **43.28%** |
| `using` | 2 | 0 | 76 | 78 | **2.56%** |
| `variable` | 18 | 0 | 160 | 178 | **10.11%** |
| `while` | 19 | 0 | 19 | 38 | **50.00%** |
| `with` | 0 | 0 | 181 | 181 | **0.00%** |

## Built-in Objects and APIs

| Feature | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| `AbstractModuleSource` | 0 | 0 | 8 | 8 | **0.00%** |
| `AggregateError` | 20 | 0 | 5 | 25 | **80.00%** |
| `Array` | 2,381 | 0 | 700 | 3,081 | **77.28%** |
| `ArrayBuffer` | 188 | 0 | 8 | 196 | **95.92%** |
| `ArrayIteratorPrototype` | 8 | 0 | 19 | 27 | **29.63%** |
| `AsyncDisposableStack` | 0 | 0 | 104 | 104 | **0.00%** |
| `AsyncFromSyncIteratorPrototype` | 0 | 0 | 38 | 38 | **0.00%** |
| `AsyncFunction` | 17 | 0 | 1 | 18 | **94.44%** |
| `AsyncGeneratorFunction` | 6 | 0 | 17 | 23 | **26.09%** |
| `AsyncGeneratorPrototype` | 0 | 0 | 48 | 48 | **0.00%** |
| `AsyncIteratorPrototype` | 0 | 0 | 13 | 13 | **0.00%** |
| `Atomics` | 20 | 0 | 362 | 382 | **5.24%** |
| `BigInt` | 49 | 0 | 28 | 77 | **63.64%** |
| `Boolean` | 14 | 0 | 37 | 51 | **27.45%** |
| `DataView` | 155 | 0 | 406 | 561 | **27.63%** |
| `Date` | 246 | 0 | 348 | 594 | **41.41%** |
| `decodeURI` | 10 | 0 | 45 | 55 | **18.18%** |
| `decodeURIComponent` | 0 | 0 | 56 | 56 | **0.00%** |
| `DisposableStack` | 0 | 0 | 93 | 93 | **0.00%** |
| `encodeURI` | 10 | 0 | 21 | 31 | **32.26%** |
| `encodeURIComponent` | 0 | 0 | 31 | 31 | **0.00%** |
| `Error` | 19 | 0 | 39 | 58 | **32.76%** |
| `eval` | 0 | 0 | 10 | 10 | **0.00%** |
| `FinalizationRegistry` | 20 | 0 | 27 | 47 | **42.55%** |
| `Function` | 44 | 1 | 464 | 509 | **8.64%** |
| `GeneratorFunction` | 14 | 0 | 9 | 23 | **60.87%** |
| `GeneratorPrototype` | 0 | 0 | 61 | 61 | **0.00%** |
| `global` | 19 | 10 | 0 | 29 | **65.52%** |
| `Infinity` | 0 | 0 | 6 | 6 | **0.00%** |
| `isFinite` | 9 | 0 | 6 | 15 | **60.00%** |
| `isNaN` | 9 | 0 | 6 | 15 | **60.00%** |
| `Iterator` | 9 | 0 | 501 | 510 | **1.76%** |
| `JSON` | 89 | 0 | 76 | 165 | **53.94%** |
| `Map` | 142 | 0 | 62 | 204 | **69.61%** |
| `MapIteratorPrototype` | 1 | 0 | 10 | 11 | **9.09%** |
| `Math` | 150 | 0 | 177 | 327 | **45.87%** |
| `NaN` | 0 | 0 | 6 | 6 | **0.00%** |
| `NativeErrors` | 15 | 0 | 79 | 94 | **15.96%** |
| `Number` | 171 | 0 | 167 | 338 | **50.59%** |
| `Object` | 2,740 | 0 | 671 | 3,411 | **80.33%** |
| `parseFloat` | 32 | 0 | 22 | 54 | **59.26%** |
| `parseInt` | 42 | 0 | 13 | 55 | **76.36%** |
| `Promise` | 78 | 0 | 599 | 677 | **11.52%** |
| `Proxy` | 13 | 0 | 298 | 311 | **4.18%** |
| `Reflect` | 54 | 0 | 99 | 153 | **35.29%** |
| `RegExp` | 118 | 0 | 1,761 | 1,879 | **6.28%** |
| `RegExpStringIteratorPrototype` | 5 | 0 | 12 | 17 | **29.41%** |
| `Set` | 96 | 0 | 287 | 383 | **25.07%** |
| `SetIteratorPrototype` | 1 | 0 | 10 | 11 | **9.09%** |
| `ShadowRealm` | 0 | 0 | 64 | 64 | **0.00%** |
| `SharedArrayBuffer` | 37 | 0 | 67 | 104 | **35.58%** |
| `String` | 1,080 | 1 | 142 | 1,223 | **88.31%** |
| `StringIteratorPrototype` | 0 | 0 | 7 | 7 | **0.00%** |
| `SuppressedError` | 20 | 0 | 2 | 22 | **90.91%** |
| `Symbol` | 50 | 0 | 48 | 98 | **51.02%** |
| `Temporal` | 0 | 0 | 4,584 | 4,584 | **0.00%** |
| `ThrowTypeError` | 0 | 0 | 14 | 14 | **0.00%** |
| `TypedArray` | 149 | 0 | 1,289 | 1,438 | **10.36%** |
| `TypedArrayConstructors` | 11 | 0 | 725 | 736 | **1.49%** |
| `Uint8Array` | 35 | 0 | 33 | 68 | **51.47%** |
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
