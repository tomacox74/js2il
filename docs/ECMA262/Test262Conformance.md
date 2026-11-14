# JROC Test262 Coverage Summary

[ECMA-262 Coverage Index](Index.md) | [History and runner notes](Test262ConformanceHistory.md)

Coverage for the current development branch. Counts and percentages are per standalone test, not execution variant.

[Language](#language-areas) | [Expressions](#expression-features) | [Statements](#statement-and-declaration-features) | [Built-ins](#built-in-objects-and-apis) | [Annex B](#annex-b-features)

## Overall ECMA-262 Areas

| Area | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| Annex B | 60 | 0 | 1,026 | 1,086 | **5.52%** |
| Built-in objects and APIs | 18,873 | 37 | 4,602 | 23,512 | **80.27%** |
| Language syntax and semantics | 22,576 | 626 | 441 | 23,643 | **95.49%** |
| **Total** | 41,509 | 663 | 6,069 | 48,241 | **86.05%** |

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
| `expressions` | 10,787 | 248 | 3 | 11,038 | **97.73%** |
| `function-code` | 204 | 13 | 0 | 217 | **94.01%** |
| `future-reserved-words` | 55 | 0 | 0 | 55 | **100.00%** |
| `global-code` | 41 | 1 | 0 | 42 | **97.62%** |
| `identifier-resolution` | 8 | 0 | 6 | 14 | **57.14%** |
| `identifiers` | 268 | 0 | 0 | 268 | **100.00%** |
| `import` | 127 | 0 | 0 | 127 | **100.00%** |
| `keywords` | 25 | 0 | 0 | 25 | **100.00%** |
| `line-terminators` | 32 | 9 | 0 | 41 | **78.05%** |
| `literals` | 507 | 27 | 0 | 534 | **94.94%** |
| `module-code` | 548 | 0 | 46 | 594 | **92.26%** |
| `punctuators` | 11 | 0 | 0 | 11 | **100.00%** |
| `reserved-words` | 27 | 0 | 0 | 27 | **100.00%** |
| `rest-parameters` | 11 | 0 | 0 | 11 | **100.00%** |
| `source-text` | 1 | 0 | 0 | 1 | **100.00%** |
| `statementList` | 40 | 40 | 0 | 80 | **50.00%** |
| `statements` | 9,049 | 256 | 32 | 9,337 | **96.92%** |
| `types` | 106 | 7 | 0 | 113 | **93.81%** |
| `white-space` | 51 | 16 | 0 | 67 | **76.12%** |

## Expression Features

| Feature | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| `addition` | 47 | 1 | 0 | 48 | **97.92%** |
| `array` | 52 | 0 | 0 | 52 | **100.00%** |
| `arrow-function` | 341 | 2 | 0 | 343 | **99.42%** |
| `assignment` | 483 | 2 | 0 | 485 | **99.59%** |
| `assignmenttargettype` | 324 | 0 | 0 | 324 | **100.00%** |
| `async-arrow-function` | 59 | 1 | 0 | 60 | **98.33%** |
| `async-function` | 89 | 4 | 0 | 93 | **95.70%** |
| `async-generator` | 618 | 5 | 0 | 623 | **99.20%** |
| `await` | 22 | 0 | 0 | 22 | **100.00%** |
| `bitwise-and` | 29 | 1 | 0 | 30 | **96.67%** |
| `bitwise-not` | 15 | 1 | 0 | 16 | **93.75%** |
| `bitwise-or` | 29 | 1 | 0 | 30 | **96.67%** |
| `bitwise-xor` | 29 | 1 | 0 | 30 | **96.67%** |
| `call` | 75 | 16 | 1 | 92 | **81.52%** |
| `class` | 3,955 | 104 | 0 | 4,059 | **97.44%** |
| `coalesce` | 24 | 0 | 0 | 24 | **100.00%** |
| `comma` | 5 | 1 | 0 | 6 | **83.33%** |
| `compound-assignment` | 434 | 20 | 0 | 454 | **95.59%** |
| `concatenation` | 4 | 1 | 0 | 5 | **80.00%** |
| `conditional` | 21 | 1 | 0 | 22 | **95.45%** |
| `delete` | 68 | 1 | 0 | 69 | **98.55%** |
| `division` | 43 | 2 | 0 | 45 | **95.56%** |
| `does-not-equals` | 36 | 2 | 0 | 38 | **94.74%** |
| `dynamic-import` | 940 | 1 | 0 | 941 | **99.89%** |
| `equals` | 45 | 2 | 0 | 47 | **95.74%** |
| `exponentiation` | 44 | 0 | 0 | 44 | **100.00%** |
| `function` | 258 | 6 | 0 | 264 | **97.73%** |
| `generators` | 285 | 5 | 0 | 290 | **98.28%** |
| `greater-than-or-equal` | 42 | 1 | 0 | 43 | **97.67%** |
| `greater-than` | 48 | 1 | 0 | 49 | **97.96%** |
| `grouping` | 8 | 1 | 0 | 9 | **88.89%** |
| `import.meta` | 21 | 1 | 0 | 22 | **95.45%** |
| `in` | 35 | 1 | 0 | 36 | **97.22%** |
| `instanceof` | 42 | 1 | 0 | 43 | **97.67%** |
| `left-shift` | 44 | 1 | 0 | 45 | **97.78%** |
| `less-than-or-equal` | 46 | 1 | 0 | 47 | **97.87%** |
| `less-than` | 44 | 1 | 0 | 45 | **97.78%** |
| `logical-and` | 17 | 1 | 0 | 18 | **94.44%** |
| `logical-assignment` | 78 | 0 | 0 | 78 | **100.00%** |
| `logical-not` | 17 | 2 | 0 | 19 | **89.47%** |
| `logical-or` | 17 | 1 | 0 | 18 | **94.44%** |
| `member-expression` | 1 | 0 | 0 | 1 | **100.00%** |
| `modulus` | 39 | 1 | 0 | 40 | **97.50%** |
| `multiplication` | 39 | 1 | 0 | 40 | **97.50%** |
| `new.target` | 14 | 0 | 0 | 14 | **100.00%** |
| `new` | 57 | 2 | 0 | 59 | **96.61%** |
| `object` | 1,148 | 22 | 0 | 1,170 | **98.12%** |
| `optional-chaining` | 37 | 1 | 0 | 38 | **97.37%** |
| `postfix-decrement` | 37 | 0 | 0 | 37 | **100.00%** |
| `postfix-increment` | 38 | 0 | 0 | 38 | **100.00%** |
| `prefix-decrement` | 34 | 0 | 0 | 34 | **100.00%** |
| `prefix-increment` | 33 | 0 | 0 | 33 | **100.00%** |
| `property-accessors` | 19 | 2 | 0 | 21 | **90.48%** |
| `relational` | 1 | 0 | 0 | 1 | **100.00%** |
| `right-shift` | 36 | 1 | 0 | 37 | **97.30%** |
| `strict-does-not-equals` | 28 | 2 | 0 | 30 | **93.33%** |
| `strict-equals` | 28 | 2 | 0 | 30 | **93.33%** |
| `subtraction` | 37 | 1 | 0 | 38 | **97.37%** |
| `super` | 90 | 4 | 0 | 94 | **95.74%** |
| `tagged-template` | 21 | 4 | 2 | 27 | **77.78%** |
| `tco-pos.js` | 1 | 0 | 0 | 1 | **100.00%** |
| `template-literal` | 56 | 1 | 0 | 57 | **98.25%** |
| `this` | 2 | 4 | 0 | 6 | **33.33%** |
| `typeof` | 15 | 1 | 0 | 16 | **93.75%** |
| `unary-minus` | 13 | 1 | 0 | 14 | **92.86%** |
| `unary-plus` | 15 | 2 | 0 | 17 | **88.24%** |
| `unsigned-right-shift` | 44 | 1 | 0 | 45 | **97.78%** |
| `void` | 8 | 1 | 0 | 9 | **88.89%** |
| `yield` | 63 | 0 | 0 | 63 | **100.00%** |

## Statement and Declaration Features

| Feature | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| `async-function` | 72 | 2 | 0 | 74 | **97.30%** |
| `async-generator` | 300 | 1 | 0 | 301 | **99.67%** |
| `await-using` | 93 | 1 | 0 | 94 | **98.94%** |
| `block` | 21 | 0 | 0 | 21 | **100.00%** |
| `break` | 19 | 0 | 1 | 20 | **95.00%** |
| `class` | 4,261 | 106 | 0 | 4,367 | **97.57%** |
| `const` | 134 | 1 | 1 | 136 | **98.53%** |
| `continue` | 23 | 0 | 1 | 24 | **95.83%** |
| `debugger` | 1 | 0 | 1 | 2 | **50.00%** |
| `do-while` | 30 | 0 | 6 | 36 | **83.33%** |
| `empty` | 1 | 0 | 1 | 2 | **50.00%** |
| `expression` | 1 | 0 | 2 | 3 | **33.33%** |
| `for-await-of` | 1,234 | 0 | 0 | 1,234 | **100.00%** |
| `for-in` | 102 | 13 | 0 | 115 | **88.70%** |
| `for-of` | 744 | 7 | 0 | 751 | **99.07%** |
| `for` | 376 | 9 | 0 | 385 | **97.66%** |
| `function` | 422 | 29 | 0 | 451 | **93.57%** |
| `generators` | 263 | 3 | 0 | 266 | **98.87%** |
| `if` | 60 | 9 | 0 | 69 | **86.96%** |
| `labeled` | 22 | 2 | 0 | 24 | **91.67%** |
| `let` | 144 | 1 | 0 | 145 | **99.31%** |
| `return` | 16 | 0 | 0 | 16 | **100.00%** |
| `switch` | 88 | 23 | 0 | 111 | **79.28%** |
| `throw` | 14 | 0 | 0 | 14 | **100.00%** |
| `try` | 186 | 15 | 0 | 201 | **92.54%** |
| `using` | 76 | 2 | 0 | 78 | **97.44%** |
| `variable` | 163 | 15 | 0 | 178 | **91.57%** |
| `while` | 19 | 0 | 19 | 38 | **50.00%** |
| `with` | 164 | 17 | 0 | 181 | **90.61%** |

## Built-in Objects and APIs

| Feature | Verified passing | Known unsupported | No published result | Applicable tests | Verified |
|---|---:|---:|---:|---:|---:|
| `AbstractModuleSource` | 0 | 0 | 8 | 8 | **0.00%** |
| `AggregateError` | 25 | 0 | 0 | 25 | **100.00%** |
| `Array` | 3,081 | 0 | 0 | 3,081 | **100.00%** |
| `ArrayBuffer` | 195 | 0 | 1 | 196 | **99.49%** |
| `ArrayIteratorPrototype` | 27 | 0 | 0 | 27 | **100.00%** |
| `AsyncDisposableStack` | 104 | 0 | 0 | 104 | **100.00%** |
| `AsyncFromSyncIteratorPrototype` | 38 | 0 | 0 | 38 | **100.00%** |
| `AsyncFunction` | 17 | 0 | 1 | 18 | **94.44%** |
| `AsyncGeneratorFunction` | 21 | 0 | 2 | 23 | **91.30%** |
| `AsyncGeneratorPrototype` | 48 | 0 | 0 | 48 | **100.00%** |
| `AsyncIteratorPrototype` | 13 | 0 | 0 | 13 | **100.00%** |
| `Atomics` | 363 | 0 | 19 | 382 | **95.03%** |
| `BigInt` | 76 | 0 | 1 | 77 | **98.70%** |
| `Boolean` | 50 | 0 | 1 | 51 | **98.04%** |
| `DataView` | 559 | 0 | 2 | 561 | **99.64%** |
| `Date` | 594 | 0 | 0 | 594 | **100.00%** |
| `decodeURI` | 55 | 0 | 0 | 55 | **100.00%** |
| `decodeURIComponent` | 55 | 0 | 1 | 56 | **98.21%** |
| `DisposableStack` | 93 | 0 | 0 | 93 | **100.00%** |
| `encodeURI` | 31 | 0 | 0 | 31 | **100.00%** |
| `encodeURIComponent` | 31 | 0 | 0 | 31 | **100.00%** |
| `Error` | 57 | 0 | 1 | 58 | **98.28%** |
| `eval` | 0 | 0 | 10 | 10 | **0.00%** |
| `FinalizationRegistry` | 47 | 0 | 0 | 47 | **100.00%** |
| `Function` | 487 | 7 | 15 | 509 | **95.68%** |
| `GeneratorFunction` | 22 | 0 | 1 | 23 | **95.65%** |
| `GeneratorPrototype` | 61 | 0 | 0 | 61 | **100.00%** |
| `global` | 19 | 10 | 0 | 29 | **65.52%** |
| `Infinity` | 6 | 0 | 0 | 6 | **100.00%** |
| `isFinite` | 15 | 0 | 0 | 15 | **100.00%** |
| `isNaN` | 15 | 0 | 0 | 15 | **100.00%** |
| `Iterator` | 510 | 0 | 0 | 510 | **100.00%** |
| `JSON` | 162 | 0 | 3 | 165 | **98.18%** |
| `Map` | 197 | 0 | 7 | 204 | **96.57%** |
| `MapIteratorPrototype` | 7 | 0 | 4 | 11 | **63.64%** |
| `Math` | 327 | 0 | 0 | 327 | **100.00%** |
| `NaN` | 6 | 0 | 0 | 6 | **100.00%** |
| `NativeErrors` | 94 | 0 | 0 | 94 | **100.00%** |
| `Number` | 338 | 0 | 0 | 338 | **100.00%** |
| `Object` | 3,400 | 1 | 10 | 3,411 | **99.68%** |
| `parseFloat` | 54 | 0 | 0 | 54 | **100.00%** |
| `parseInt` | 55 | 0 | 0 | 55 | **100.00%** |
| `Promise` | 651 | 0 | 26 | 677 | **96.16%** |
| `Proxy` | 267 | 0 | 44 | 311 | **85.85%** |
| `Reflect` | 152 | 0 | 1 | 153 | **99.35%** |
| `RegExp` | 1,872 | 7 | 0 | 1,879 | **99.63%** |
| `RegExpStringIteratorPrototype` | 15 | 0 | 2 | 17 | **88.24%** |
| `Set` | 383 | 0 | 0 | 383 | **100.00%** |
| `SetIteratorPrototype` | 1 | 0 | 10 | 11 | **9.09%** |
| `ShadowRealm` | 0 | 0 | 64 | 64 | **0.00%** |
| `SharedArrayBuffer` | 103 | 0 | 1 | 104 | **99.04%** |
| `String` | 1,211 | 12 | 0 | 1,223 | **99.02%** |
| `StringIteratorPrototype` | 6 | 0 | 1 | 7 | **85.71%** |
| `SuppressedError` | 20 | 0 | 2 | 22 | **90.91%** |
| `Symbol` | 76 | 0 | 22 | 98 | **77.55%** |
| `Temporal` | 301 | 0 | 4,283 | 4,584 | **6.57%** |
| `ThrowTypeError` | 0 | 0 | 14 | 14 | **0.00%** |
| `TypedArray` | 1,397 | 0 | 41 | 1,438 | **97.15%** |
| `TypedArrayConstructors` | 733 | 0 | 3 | 736 | **99.59%** |
| `Uint8Array` | 68 | 0 | 0 | 68 | **100.00%** |
| `undefined` | 7 | 0 | 1 | 8 | **87.50%** |
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

## Reading the Tables

- **Verified passing**: the native harness confirms the expected outcome, including expected errors for negative tests.
- **Known unsupported**: explicitly excluded behavior, primarily `eval`.
- **No published result**: no published conformance result; does not imply support or failure.
- **Verified**: verified passing divided by applicable tests.

## Scope and Source

Counts cover unique standalone tests under `test/annexB`, `test/built-ins`, and `test/language` at the [pinned Test262 revision](https://github.com/tc39/test262/tree/2b2ecead6e828dd9af13a9ec72065e645724a50f/test). ECMA-402, staging, harness self-tests, and `_FIXTURE.js` support files are excluded.

Feature names follow upstream directories. Language-area totals overlap their expression and statement drill-downs; do not add them together. Native results include branch-local validation, not a claim of central trusted acceptance.
