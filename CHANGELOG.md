Warning: truncated output (original token count: 59951)
Total output lines: 2464

# Changelog

All notable changes to this project are documented here.

For older release lines, browse [`docs/archive/changelog/Index.md`](docs/archive/changelog/Index.md). `CHANGELOG.md` remains the active changelog file.

## Unreleased

- fix(ci): skip Test262 central validation and publication when native screening
  produces no patch, avoiding a missing validation artifact failure for empty batches.
- perf(compiler): reuse immutable boxed boolean and null primitives for
  object-valued constants, captured writes, calls, and resumable spills.
  Preserve typed boolean operations, null/undefined distinctions, and fresh
  JavaScript wrapper objects. Read mutable variable slots dynamically and load
  shared constant boxes directly where stack scheduling permits; reduce Stopwatch
  execution allocation by about 6 MB per invocation without changing either
  workload (#2308).
- perf(runtime): bulk-fill empty-RegExp split results directly in their final
  dense storage, setting array length once instead of growing per character.
  Preserve cached character strings, Unicode surrogate pairs, limits, and
  observable species/exec fallbacks (#746).
- perf(runtime): adaptively promote cached RegExp patterns after 64 dynamic or
  literal constructions, with at most 256 cached/in-flight compiled tiers.
  Preserve independent instance state, permanently retain the interpreted tier
  after promotion failure, and release compiled cache references on eviction.
  Cold patterns and runtimes without dynamic code stay interpreted (#2151).
- perf(runtime): recognize intrinsic RegExp species in guarded split fast paths,
  enumerate bounds for non-Unicode separators without captures, and build empty
  splits directly in pre-sized storage with cached character strings. Return the
  original input for no-match regexp replacements after observable work (#746).

## v0.12.32 - 2026-10-15

- perf(runtime): replace reflective dynamic Math dispatch with fixed-arity,
  receiver-independent built-ins; preserve member selection before argument
  evaluation, JavaScript coercion and mutable globals. Skip compiled static-field
  scans for runtime-owned types and correct oscillator profiling inputs (#2301).
- fix(runtime): initialize the realm's intrinsic RegExp constructor on first
  access so split and matchAll work in fresh hosted realms, including both
  unchanged Dromaeo regexp workloads with unmodified host globals (#2299).

## v0.12.31 - 2026-10-15

- test262: complete pinned new-expression, property-accessor, and this-keyword
  registration with 51 exact upstream fixtures: 43 verified passing (including
  two parse-negative cases) and eight explicit eval skips. Preserve spread
  arguments in HIR for every new-expression callee form, expand constructor
  arguments through the existing iterator-aware runtime dispatch, and retain
  constructor values across argument-side reassignment.
- test262: port 197 pinned core-expression cases: 181 verified passing,
  including 54 parse-negative cases, and 16 explicit eval skips. Reconcile
  existing operator coverage against the pinned corpus. Support short-circuit
  logical assignment to property/index targets, preserve computed update
  reference evaluation and key-coercion order, and dispatch instanceof through
  Symbol.hasInstance with the original receiver and boolean-coerced result.
  Proper-tail-call cases in coalesce and comma remain unverified.
- test262: complete async-arrow-function, async-function, await, and delete
  expression coverage with 73 exact pinned fixtures: 67 runnable cases and
  six explicit eval skips. Reconcile 53 already-registered delete cases omitted
  from the prior report. Preserve identifier references when deleting global
  constants, honor active and captured with-environments, and reject super
  deletion after this/key evaluation without property-key coercion or base
  access. Protected async throws now reach catch/finally before rejection,
  allowing finally throws to override the original completion.
- test262: complete Array, Date, and String registration with 76 exact pinned
  fixtures: 65 verified passing and eleven explicitly skipped for unsupported
  eval. Reconcile an existing three-case Array report overcount. Fix Array
  constructor length, Proxy spreadability keys, generic reduceRight safe-integer
  bounds, intrinsic toString fallback, String receiver brands and Symbol
  construction, Date default-hint coercion and full-range local timezone
  conversion, computed class-method home objects and super calls with spread,
  and duplicate named RegExp captures/backreferences. Add limited v-flag
  code-point matching and ASCII/Any/Han property escapes; advanced Unicode set
  operations remain unsupported. Date.toTemporalInstant returns exact branded,
  realm-owned Temporal.Instant values; other Temporal types and Instant
  arithmetic/parsing/formatting remain unimplemented.
- test262: verify 11 additional pinned fixtures from native batch `1dd3f7c7-0cc1-5787-a3c6-a181ccae9c87`.
- test262: complete pinned object-expression coverage with 341 new fixtures:
  325 verified passing and 16 explicitly skipped for unsupported eval.
  Reconcile 15 already-registered cases from the published remaining count;
  all 1,170 unique cases now have passing or known-unsupported results.
  Coerce computed keys before values, preserve discarded literal reads,
  infer anonymous definition names without renaming reused or spread values,
  honor Proxy spread trap order/enumerability, and omit legacy own
  caller/arguments properties from methods. Execute explicit Module-goal
  sources as ES modules even without imports/exports, including top-level
  await and undefined top-level this.
- test262: complete the remaining 391 async-generator expression cases:
  386 verified passing and five explicitly skipped for unsupported eval.
  Preserve named generator-expression self-bindings across deferred body
  execution and resumption by initializing them once at call time, before
  parameter defaults, instead of overwriting them on subsequent steps.
- test262: complete the remaining 212 async-generator statement/declaration
  cases: 211 verified passing and one explicitly skipped for unsupported eval.
  Await explicit return expressions and return-resumption values with the
  required promise-job ordering, preserve raw yielded values from manual async
  iterators, and route synchronous delegated-protocol exceptions into the
  generator's catch/finally handlers. Align await-field planning across callable
  forms with these implicit suspension points.
- test262: complete the remaining three addition, eight arrow-function, and six
  assignment-target cases: 14 verified passing and three explicitly skipped for
  unsupported eval. Fix addition's abrupt Symbol.toPrimitive conversions,
  recursively bind arrow destructuring targets without binding property keys,
  and honor Symbol.unscopables in active and captured with environments.
- test262: complete 21 function-code, 17 global-code, and one destructuring
  fixture: 25 verified passing and 14 explicitly skipped for direct or indirect
  eval. Add an explicit Script compilation goal and realm-owned global
  declaration/lexical binding semantics, native host-compiled `$262.evalScript`,
  and parse-negative SyntaxError screening. Fix duplicate parameter resolution
  and hoisted functions shadowing parameters or surviving var redeclarations.
  Verify resizable-buffer TypedArray destructuring without additional product changes.
- test262: reconcile 38 previously registered array-expression cases missing
  from the published conformance report. All 52 cases now have verified passing
  results; the reconciled fixtures pass all 76 default/strict variants and
  require no eval exclusions or compiler/runtime fixes.
- test262: complete the remaining 75 pinned compound-assignment cases: 55
  verified passing and 20 explicitly skipped because eval is unsupported.
  Preserve the resolved with-environment binding through getter/RHS side effects,
  enforce strict deleted-binding ReferenceError behavior, and evaluate computed
  compound-assignment bases and property keys only once.
- fix(runtime): enforce monotonic Atomics.waitAsync timeout deadlines instead
  of relying on fixed timer padding, preserving notification and cancellation.
- test262: complete the remaining 148 pinned class-expression fixtures: 87
  verified passing and 61 explicitly skipped because eval is unsupported.
  Fix repeated class identities/private brands, computed field evaluation,
  static initialization, optional private chains, lexical arrow receivers,
  async method failures and restricted method properties. Support guarded
  statically discoverable Function sources and their constructor realms.
  Preserve assignment-inferred class names before static initialization,
  distinguish global references from implicit arguments bindings in prepared
  Function factories, and keep function descriptor overrides in their owning realm.
- fix(compiler): lower public auto-accessors to private-backed getter/setter
  pairs, preserve source locations and initializer order, and reject
  unimplemented computed, private and decorated auto-accessor forms explicitly.
- feat(compiler): execute class decorators in source evaluation/reverse application
  order, with class contexts, validated replacements and added initializers.
  Preserve original class lexical bindings and reject unimplemented element decorators.
- test262: verify 23 additional pinned fixtures from native batch `2ff9167d-ea5d-5a2e-8341-83ec65c872b0`.
- fix(runtime): honor legacy Object accessor/prototype proxy traps, initialize
  Object prototype method metadata, and apply ordered toString brand and
  Symbol.toStringTag lookup without synthetic primitive/Date tags. String
  iterators inherit from Iterator.prototype. Port 20 pinned Object Test262
  cases: 19 verified passing and one explicitly skipped because eval is unsupported.
- test262: verify 5 additional pinned fixtures from native batch `8c13b2ba-4ccf-5ce1-a465-b4aad7a0e645`.
- fix(runtime): honor proxy prototype traps in Object.prototype.isPrototypeOf
  and legacy accessor lookup, check non-object arguments before coercing the
  isPrototypeOf receiver, expose its standard function metadata, and box
  primitive valueOf receivers; verify 10 previously failing pinned Test262 cases.
- fix(runtime): reject hashbang prefixes in dynamic Function-family parameter
  and body source after ordered, single argument coercion. Parse `.mjs` sources
  and Test262 module fixtures using the Module goal, including hashbang-only
  modules. Complete the remaining 30 pinned comments Test262 cases: 24 verified
  passing and six explicitly skipped because eval is unsupported.
- feat(compiler): add opt-in compilation-mode coverage with source locations,
  reasons, counts and percentages, text/JSON CLI and SDK analysis, partial
  unsupported reports, Test262 aggregation independent of correctness, and
  retained bounded CI artifacts.
- test262: verify 41 additional pinned fixtures from native batch `02bf3372-c399-58f9-994f-d0066e480749`.
- test262: verify 3 additional pinned fixtures from native batch `5583f6ad-630a-5ee6-b8ac-0041fd606eab`.
- fix(runtime): coerce computed property keys once, preserve class constructor
  metadata and property ordering, reject static prototype redefinitions, and
  resolve duplicate class accessors through their JavaScript descriptors.
  Complete all 48 pinned computed-property-names Test262 cases without skips.
- fix(runtime): give mapped and unmapped arguments objects an own
  Symbol.iterator property with the realm's original Array values function,
  preserving identity after Array.prototype mutation. Complete the remaining
  64 pinned arguments-object Test262 cases: 62 passing and two explicitly
  skipped because eval is unsupported.
- fix(runtime): expose legacy String anchor, blink, fixed, fontcolor, fontsize,
  and italics methods using shared receiver-first HTML construction and
  double-quote attribute escaping; verify 30 previously failing pinned
  Test262 cases.
- fix(runtime): implement Annex B Date getYear/setYear and the toGMTString
  alias, retain captured date values during year coercion, support local getters
  outside the CLR date range, and expose String big/bold HTML wrappers; verify
  30 previously failing pinned Test262 cases.
- refactor(runtime): move intrinsic metadata and prototype configuration from
  GlobalThis into the owning runtime implementations; keep realm bootstrap
  orchestration and shared Object, TypedArray, and NativeError setup centralized
  only at their appropriate family boundaries.
- fix(runtime): expose TypedArray reduce metadata and shared Array toString
  identity, validate toReversed receivers and post-coercion with indices, and
  implement Uint8Array Base64 decoding options, bounded chunk consumption,
  partial writes, and detached-buffer checks; verify 30 previously failing
  pinned Test262 cases.
- fix(compiler): enforce derived-constructor completion checks even when the
  result is discarded, preserve object return overrides and pre-super captured
  scopes, and check primitive returns after iterator cleanup; verify 30
  previously failing pinned Test262 constructor cases.
- fix(runtime): initialize RegExp getter names and metadata, return undefined
  for intrinsic-prototype flag receivers, and expose RegExp Symbol.species;
  verify 20 previously failing pinned Test262 cases.
- fix(runtime): select concrete TypedArray default prototypes from the new
  target's realm across numeric and BigInt constructors; verify 10 previously
  failing pinned Test262 cases covering no arguments, length, array-like,
  typed-array copy, and buffer construction.
- fix(runtime): complete cross-realm default-prototype selection for NativeError,
  Number, String, WeakRef, and FinalizationRegistry constructors; treat null
  Number/String prototypes as intrinsic fallbacks and verify 10 previously
  failing pinned Test262 cases.
- fix(runtime): use the new target's realm for built-in constructor default
  prototypes, including reflective Boolean wrappers; verify 10 previously
  failing pinned Test262 cases across Boolean, RegExp, collections, buffers,
  and DataView.
- feat(runtime): implement async iterator `Symbol.asyncDispose` with awaited
  return-method completion and promise rejection semantics; correct the
  `Symbol.asyncIterator` function name and verify 10 previously failing pinned
  Test262 AsyncIteratorPrototype cases.
- fix(runtime): correct async-generator intrinsic prototype metadata and
  dynamic-constructor function identity; port 10 previously failing pinned
  Test262 AsyncGeneratorFunction and AsyncGeneratorPrototype cases.
- fix(runtime): complete the synchronous generator intrinsic metadata surface;
  port 10 previously failing pinned Test262 GeneratorFunction and
  GeneratorPrototype cases.
- fix(runtime): preserve constructor realm ownership for cross-realm default
  prototypes and Array species creation; verify 10 previously failing pinned
  Test262 Array and AggregateError cases.
- fix(runtime): retain a reachable function's defining intrinsic graph across GC,
  preventing borrowed `Iterator.from` calls from switching realms; keep intrinsic
  caches collectible by storing long-lived CLR identities in realm-owned
  dictionaries rather than dependent handles (#2234).
- test262: add deterministic native-porting automation for the initial
  computed-property-name basics area, with killable native-harness workers,
  streamed and versioned resumable SQLite evidence, active-build provenance
  checks, byte-faithful registration and coverage generation, and exact patch
  manifests. GitHub App draft-PR publication remains staged pending a
  default-branch acceptance run.
- test262: prioritize current and historical MVP failure hints after validated
  master ranges with a durable post-upload reconciliation cursor, persistent
  pending work, component-aware 80/20 rotating intake and generated-merge
  recursion suppression; expose and verify native screening for async, agent,
  supported helper, module dependency, failure-cleanup and timeout shapes.

## v0.12.30 - 2026-09-30

- fix(compiler): support private receiver field loads and stores when private
  members are accessed from nested class callables and constructors; verify
  28 pinned class-expression cases.
- test262: retain 43 pinned class-expression cases requiring unsupported
  `eval` as skipped tests, ready to enable when `eval` support lands.
- test262: verify 370 additional pinned class-expression cases, primarily
  async methods and private class elements.
- test262: verify another 500 pinned class-expression cases covering class
  methods and related semantics.
- perf(compiler/runtime): pass proven numeric arguments unboxed to guarded
  String `charAt`, `charCodeAt`, `slice`, `substr`, and `substring` intrinsics;
  retain object coercion and prototype-override fallbacks.
- perf(benchmark): prototype scalar and Vector128 repeating-mask blocks for
  Prime's large-step marking path; keep production Prime code unchanged
  pending repeatable crossover and end-to-end Prime measurements.
- test262: verify 500 additional pinned class-expression cases covering class
  elements and parameter destructuring.
- perf(compiler): vectorize canonical local-const Int32Array OR loops with
  literal or immutable numeric bounds, preserving the scalar loop for
  dynamic properties, side-effecting bodies, and unsupported storage.
- perf(compiler/runtime): add guarded portable `Vector128<int>` OR-range
  infrastructure for contiguous `Int32Array` loops, with scalar edges,
  fallback, and disabled-intrinsics benchmark coverage.
- fix(runtime): implement observable RegExp.prototype symbol operations for
  match, matchAll, replace, search, and split, including overridden exec,
  lastIndex and flag access, generic receivers, species construction, and
  method metadata; verify 100 previously failing pinned Test262 cases.
- perf(compiler/runtime): recognize compatible Int32Array-backed bitset count
  loops and count clear bits in whole words with `BitOperations.PopCount`,
  preserving the scalar loop for dynamic bounds and backing storage.
- perf(compiler/runtime): recognize canonical Int32Array-backed first-zero-bit
  loops and search fixed, non-shared words with trailing-zero count; retain the
  original per-bit loop for dynamic indices and detachable/resizable buffers.
- perf(compiler): retain proven signed bitwise values and Int32Array reads in
  native `int32` locals, using numeric conversions only at JavaScript-number
  boundaries; preserve full-width unsigned shifts and dynamic loop arithmetic.
- perf(runtime): use a guarded contiguous `int` view for indexed Int32Array
  reads and writes on fixed backing storage, retaining the byte-oriented path
  for detached, resizable, and growable buffers.

## v0.12.29 - 2026-09-26

- test262: support the native `$262.agent` host and `atomicsHelper.js` for
  worker-based shared-memory tests; port 100 previously failing pinned Atomics
  wait, waitAsync, and notify cases.
- feat(runtime): coordinate synchronous and asynchronous Atomics waits with
  notifications, support BigInt64 waits, and add growable SharedArrayBuffers
  with species-aware slicing; port 100 previously failing pinned Test262 cases.
- fix(compiler): observe mutable global callable and constructor bindings,
  including replacements installed by a host, without disabling intrinsic
  fast paths. Add an explicit AOT host contract for guard-free global calls
  when source and host bindings are stable. Keep BigInt literals independent
  of the mutable `BigInt` global, and expose supported Promise and Uint8Array
  static methods through dynamic member lookup. Preserve direct calls when
  the only use of the global object is a strict identity comparison.
- fix(compiler/runtime): preserve compound operations on private fields and
  accessors, discover nested class callables, keep private accessors off public
  prototypes, initialize public static field descriptors, and invoke private
  static setters with brand checks. Preserve Promise rejection reasons and
  reject invalid `Promise.resolve` receivers; port 100 previously failing
  pinned Test262 cases.
- fix(runtime): allow public class methods on unbound or borrowed receivers and
  make Promise.race constructor-aware with proper capability, resolve, and
  iterator-close semantics; port 110 previously failing pinned Test262 cases.
- fix(compiler): bind synchronous generator parameters when the generator
  function is called, preserving initialized locals until the first resume;
  port 100 previously failing pinned Test262 generator-destructuring cases.
- perf(test262): compile and load one in-memory assembly per runnable
  JavaScript fixture folder during each xUnit run, retaining independent
  test cases and separate compilation for expected failures.
- feat(runtime): implement `Iterator.concat`, `Iterator.zip`, and
  `Iterator.zipKeyed` with iterable acquisition, padding, closing, and
  completion handling; port 100 previously failing pinned Test262 cases.
- feat(runtime): add Atomics integer typed-array read, write, and read-modify-write
  operations, plus isLockFree and pause; port 100 previously failing pinned
  Test262 cases across Number and BigInt typed arrays.
- feat(compiler): compile multiple independent JavaScript entry sources into
  one assembly via a public in-memory API or repeatable CLI inputs, preserving
  separate entry facades, a selected default entry, and per-entry source paths.
- fix(compiler): access static private class fields through their static storage
  in class methods and initializers; port 100 previously failing pinned
  Test262 class-expression and class-declaration cases.
- fix(runtime): implement canonical numeric-index get, has, define, and set
  semantics for Number and BigInt typed arrays, including receiver-aware writes,
  detached buffers, and coercion ordering. Correct ToIndex conversion for
  buffer-backed typed-array constructor offsets and explicit null lengths;
  verify 100 additional pinned Test262 typed-array cases.
- fix(compiler): support literal-named class accessors and fields and
  callable-expression computed class method and accessor names; port 100
  previously failing Test262 class expression and declaration cases.

## v0.12.28 - 2026-09-24

- test262: verify 102 additional pinned block-scope syntax cases covering
  redeclarations, for-in lexical bindings, and function-declaration positions.
- fix(runtime): add DataView Float16 reads and writes, reject writes to immutable
  ArrayBuffers before argument coercion, and correct resizable-view accessors
  and explicit-null byte lengths; port 50 previously failing Test262 DataView
  and ArrayBuffer cases.
- fix(runtime): preserve Date setter argument count and conversion order,
  including invalid-date snapshot behavior, full-year recovery, and TimeClip
  at extended-year limits; port 50 previously failing Test262 Date setter
  cases across local and UTC methods.
- fix(runtime): expose `Function.prototype[Symbol.hasInstance]` with its
  specified metadata and descriptor, including bound-function prototype checks
  and Symbol-as-primitive handling; port 10 previously failing Test262 cases.
- fix(runtime): make the `RegExp.prototype.flags` getter work on arbitrary
  object receivers, reading flag properties in specification order and
  propagating getter errors; port 10 previously failing Test262 cases.
- fix(runtime): snapshot `Array.prototype.reduce` length and dynamically probe
  each index, respecting holes, inherited elements, getters, deletions, and
  length changes; port 10 previously failing native Test262 cases.
- fix(runtime): treat `null` and `undefined` as already frozen or sealed in
  `Object.isFrozen` and `Object.isSealed`; port 10 pinned Test262 `Object` cases
  covering integrity checks, argument-object accessor descriptors, and
  observable `entries`/`values` operations.

## v0.12.27 - 2026-09-23

- perf/tooling: upgrade both benchmark hosts to BenchmarkDotNet
  `0.16.0-preview.2` and add a manual same-runner .NET 10 versus .NET 11 RC1
  JROC comparison workflow with per-child CLR telemetry and a shared
  benchmark profile. Regular benchmark and production targets remain .NET 10.
- fix(runtime): expose compiler-marked public class fields through exact-name
  own-property descriptors with current values and JavaScript class-field
  attributes, without reflecting unrelated CLR fields; port 10 upstream
  Test262 private-method/class-field sequencing cases.
- fix(compiler): recursively register only binding targets from nested
  destructuring parameters in class methods, preventing object property keys
  from leaking into method scope; port 10 corresponding upstream Test262 cases.
- fix(runtime): make `Array.prototype.reduceRight` dynamically probe indexed
  properties across its captured length, correctly handling sparse arrays,
  inherited values, accessors, deletions, and length mutation; port 10
  corresponding upstream Test262 cases.
- test(test262): port 516 upstream `test262` cases to `tests/Jroc.Test262.Tests`,
  covering `Array`/`Array.prototype` iteration and sorting methods,
  `Iterator`/`Iterator.prototype` helper methods, `Map`/`Map.prototype`,
  `GeneratorPrototype`, `Error`/`Error.prototype`, `Boolean`, `BigInt`,
  `FinalizationRegistry`, `ArrayIteratorPrototype`, `Function.prototype.bind`,
  `MapIteratorPrototype`, `Date`, `NativeErrors`, and several other built-ins.
  Extended the native harness with `dateConstants.js` support so ported `Date`
  fixtures relying on the upstream date-constant helper values run natively.
- test(test262): port 591 more upstream `test262` cases to `tests/Jroc.Test262.Tests`,
  covering `TypedArray`/`TypedArray.prototype`, `TypedArrayConstructors`,
  `Proxy`, `Reflect`, `NativeErrors`, `WeakMap.prototype`, `Number.prototype`,
  `Symbol`, `Object.prototype`, and `Promise`. No product or harness changes
  were required; all fixtures already passed under the existing runtime.
- test(test262): port 223 more upstream `test262` cases to `tests/Jroc.Test262.Tests`,
  covering `String`/`String.prototype`, `Object`/`Object.prototype`, `Proxy`,
  `StringIteratorPrototype`, and additional `TypedArray` cases. No product or
  harness changes were required; all fixtures already passed under the
  existing runtime.
- test(test262): port 343 more upstream `test262` cases to `tests/Jroc.Test262.Tests`,
  covering `language/statements` (`variable`, `with`, `try`, `for`, `using`),
  `language/identifiers`, `language/expressions` (`object`, `dynamic-import`),
  `language/arguments-object`, and `language/block-scope`. This is the first
  round to draw candidates primarily from `test/language` rather than
  `test/built-ins`. No product or harness changes were required; all
  fixtures already passed under the existing runtime.
- test(test262): port 433 more upstream `test262` cases to `tests/Jroc.Test262.Tests`,
  covering language expression operators, template literals, language types,
  and additional `RegExp`, `WeakRef`, and `WeakSet` cases. No product or
  harness changes were required; all fixtures already passed under the
  existing runtime.
- test(test262): port 208 more upstream `test262` cases to `tests/Jroc.Test262.Tests`,
  covering `language/function-code`, `Number` numeric-literal string
  conversions, `RegExpStringIteratorPrototype`, additional `RegExp`, and one
  `Set` case. No product or harness changes were required; all fixtures
  already passed under the existing runtime.
- test(test262): port 37 more upstream `test262` cases to `tests/Jroc.Test262.Tests`,
  covering `language/statements/let`, `language/expressions/object`,
  `language/expressions/assignmenttargettype`, and additional
  `language/expressions/assignment` cases. No product or harness changes
  were required; all fixtures already passed under the existing runtime.

## v0.12.26 - 2026-09-19

- perf(runtime): cache the compiled `Regex`, named-group table, and
  capture-reset metadata derived from a JavaScript pattern so re-evaluating the
  same regular expression literal or `new RegExp(source, flags)` no longer
  re-parses and re-compiles the pattern. Re-creating `emoji-regex`'s 14 KB
  literal drops from about 1 ms to 0.6 us, which is the dominant cost in the
  `string-width` mitata scenario (for example `npm ascii 25,000` improves from
  8,761 ms to 200 ms). The cache is keyed by pattern source plus the flags that
  affect compilation, is culture-invariant and atomic under concurrent misses,
  and incrementally evicts entries within both count and source-size limits.
- perf(compiler): lower regular expression literals through weak-keyed,
  immutable templates. Each evaluation still creates a distinct JavaScript
  `RegExp` with independent `lastIndex`, but skips repeated flag parsing and
  compiled-artifact lookup and does not observe a rebound global `RegExp`.
  A literal that fails at runtime (for example an unsupported flag) throws a
  fresh `SyntaxError` on every evaluation rather than replaying one instance.
- perf(compiler): early-bind `String.prototype.codePointAt` and
  `String.prototype.at` to their guarded string intrinsics, matching
  `charAt`/`charCodeAt`. `"世".codePointAt(0)` in a hot loop improves from about
  373 ns to 50 ns per call while preserving the pristine-prototype guard and the
  dynamic fallback.
- perf(compiler): use guarded fast paths for `RegExp.prototype.test` and
  `Number.isSafeInteger`, falling back to ordinary JavaScript dispatch after
  receiver, prototype, method, or global-binding mutation.
- perf(compiler): permit unmodified `let` and `var` function expressions to use
  stable direct calls after proven initialization, including calls reached
  through safely activated hoisted functions, and infer typed leading
  parameters when later parameters use destructuring.
- perf(runtime): iterate `Intl.Segmenter` results lazily with fresh iterators and
  allocation-light segment records instead of eagerly materializing an Array.
  Segment records support `delete` and re-add with ordinary key ordering, and
  the segment iterator prototype exposes `next` plus the
  `"Segmenter String Iterator"` toStringTag while inheriting `[Symbol.iterator]`.
- feat(node): expose the realm-owned `performance` global with
  `performance.now()`, identical to `require("node:perf_hooks").performance`;
  `now` is non-enumerable so `Object.keys(performance)` matches Node.
  The mitata managed runner now warms each case and uses this monotonic,
  high-resolution clock.
- fix(runtime): pack JavaScript arguments into CLR `params` arrays during
  dynamic static-class dispatch, including zero-argument and fixed-prefix
  calls. Remove the temporary JROC-only `string-width` benchmark rewrite so
  every runtime uses the common upstream-derived scenario source.

## v0.12.25 - 2026-09-18

- perf(mitata): measure simple-runner benchmarks in batches lasting at least
  10 ms so JROC and other runtimes cannot round sub-millisecond scenarios down
  to zero. Restore the bundled JROC path for `string-width` so the suite does
  not compile Mitata's unsupported optional dynamic import, and avoid a
  spread-to-variadic setup expression that JROC cannot lower correctly yet.

## v0.12.24 - 2026-09-18

- perf(mitata): use a shared monotonic high-resolution host clock for
  ClearScript, Jint, YantraJS, and Okojo benchmark timing instead of the
  millisecond-resolution `Date.now()`, preventing short scenarios from being
  recorded as zero-duration results.

## v0.12.23 - 2026-09-16

- perf/runtime: restore the `dromaeo-object-string` lead over Jint with
  vectorized ASCII detection before Unicode case conversion and guarded dense
  Array `join` reads. Preserve culture-specific casing, Unicode 16.0 mappings,
  indexed getters, inherited values, and mutation during coercion. The exact
  local scenario improves from 29.35 ms to 24.90-25.01 ms, versus
  Jint 4.16.2 at 26.50-27.18 ms in the corresponding runs.
- perf(compiler): extend the `+=` string concatenation builder to non-captured
  locals whose initializer is not statically a string (for example
  `var s = new String(); s += "a"`). The first append applies the full `+`
  operator (ToPrimitive, Symbol TypeError) and later appends reuse a
  compiler-private accumulator that every JavaScript-visible read
  materializes. `dromaeo-object-string` allocations fall from 46.7 MB to
  21.7 MB per operation and the mean improves to 20.16 ms versus Jint at
  26.58 ms in the same run.

## v0.12.22 - 2026-09-16

- perf/dependencies: update benchmark comparison runtimes to Jint 4.16.2,
  YantraJS.Core 1.2.476, and ClearScript V8 7.5.1.1. Keep Okojo on its
  latest published package, 0.1.2-preview.1, because no stable release is
  available.
- test262/runtime/docs: port 10 previously failing pinned Array and String
  fixtures. Correct Array `length` coercion order, canonical-equivalent
  `localeCompare` results, and Unicode 16.0 full/context-sensitive case
  conversion.
  Optimize Array length validation with an inline-friendly numeric fast path
  and single-pass primitive coercion while retaining both object coercions.
- test262/docs: verify 500 additional current-provenance pinned
  destructuring-binding fixtures: 75 `const`, 258 `for`, 87 `let`, 63
  `try`, and 17 `var` declaration cases. All 1,000 metadata-selected
  strict/non-strict variants and all 500 native fixtures pass without
  compiler, runtime, or harness changes.
- test262/docs: verify 500 additional current-provenance pinned language
  fixtures: 161 arguments-object, 165 class-expression, 53 call-expression,
  42 `super`, 32 `new`, 12 function-expression, 11 `new.target`, 11 `yield`,
  six `await`, two arrow-function, one async-function, and four
  function-code cases. All 934 metadata-selected variants and all 500 native
  fixtures pass without compiler, runtime, or harness changes.
- test262/runtime/docs: port 100 previously failing pinned Object and Proxy
  fixtures covering internal-method forwarding, trap invariants, own-key
  ordering, descriptor transitions, extensibility, sealing, and freezing.
  Correct the associated object-property and Proxy runtime behavior.
- test262/runtime/docs: port 100 previously failing pinned Iterator fixtures
  covering helper protocol validation, callback ordering, iterator closing,
  lazy return forwarding, reentrancy, flatMap flattening, subclassing, and
  intrinsic descriptors. Align iterator records and helper state transitions
  with ECMA-262.
- test262/docs: verify 500 additional current-provenance pinned generator and
  method-definition fixtures: 181 generator expressions, 165 generator
  declarations, 61 async-generator declaration/expression cases, and 93
  object method-definition cases. All 952 metadata-selected variants and all
  500 native fixtures pass; the native Test262 language harness now correctly
  registers parse-negative fixtures alongside execution cases.
- test262/docs: verify 500 additional positive pinned class-declaration
  fixtures: 209 method-parameter-destructuring cases, including static and
  private methods, plus 291 class-element cases covering private names, class
  fields, line-terminator separation, computed Symbol names, and static element
  grammar. All 1,000 metadata-selected strict/non-strict variants and all 500
  native fixtures pass without compiler, runtime, or harness changes.
- test262/docs: verify 500 additional positive pinned class-declaration
  method-parameter-destructuring fixtures: 236 private generator methods, 116
  private static methods, 87 static methods with default parameters, and 61
  private methods with default parameters. All 1,000 metadata-selected
  strict/non-strict variants and all 500 native fixtures pass without compiler,
  runtime, or harness changes.
- test262/runtime/docs: port 100 pinned Promise fixtures covering constructor
  capabilities, combinators, static methods, species, and prototype methods.
  Make Promise.reject constructor-generic so custom constructors receive and
  validate the NewPromiseCapability executor and rejection function.
- test262/docs: verify 100 additional pinned Number fixtures covering
  constructor/static property descriptors and Number.prototype formatting,
  coercion, receiver validation, radix conversion, and callable metadata. All
  200 metadata-selected variants and all 100 native fixtures pass without
  compiler, runtime, or harness changes.
- test262/docs: verify 100 additional explicit-resource-management and Symbol
  fixtures: 88 DisposableStack cases and 12 well-known Symbol descriptor and
  Symbol.prototype cases. All 100 native fixtures pass without compiler,
  runtime, or harness changes.
- test262/docs: verify 500 additional pinned statement syntax parse-negative
  fixtures: 145 class, 66 switch, 49 for, 33 async-generator, 32 generator,
  29 if, 27 let, 26 const, 25 await-using, 21 using, and 47 other
  async-function, block, break, continue, debugger, do-while, labelled, and
  try grammar cases. All 965 metadata-selected variants and all 500 native
  fixtures pass without compiler, runtime, or harness changes.
- test262/runtime/docs: port 100 Test262 fixtures: 66 Array, 30 TypedArray,
  and four DisposableStack cases. Align direct Array calls with the generic
  algorithms for coercion, holes, species, sparse lengths, and resizable views;
  make Array.prototype an Array exotic object; preserve Proxy `has` traps in
  prototype lookup; and return `undefined` for invalid TypedArray indexed
  property reads. Referencing the global `eval` value is now supported while
  invoking unsupported direct eval remains rejected.
- test262/docs: verify 500 additional pinned expression parse-negative fixtures:
  301 dynamic-import, 91 assignment-target, 48 async-generator, 46 class, and
  14 other assignment, call, coalesce, conditional, and delete grammar cases.
  All 960 metadata-selected variants and all 500 native fixtures pass without
  compiler, runtime, or harness changes.
- runtime: correct Array/TypedArray review edge cases: ordered array-like `set`
  reads and overlap handling, species-aware bit-preserving `slice`, permanently
  completed iterators, explicit `lastIndexOf` arguments, empty `join` coercion,
  and observable element `toLocaleString` calls. Expose the affected TypedArray
  methods on the shared prototype and honor custom Object `toString` methods
  from `Object.prototype.toLocaleString`.
- test262/runtime/docs: port 100 Array and TypedArray fixtures covering
  resizable and detached ArrayBuffer views. Harden generic Array iteration,
  search, and locale-string behavior; preserve TypedArray validation, species,
  coercion, and write semantics across buffer state transitions.
- test262/runtime/docs: port 100 previously failing TypedArray fixtures. Add
  TypedArray prototype intrinsic dispatch for `forEach`, `includes`, and
  `subarray`; support RAB/detached-buffer validation and iteration, BigInt
  subarray species construction, and native RAB test helpers.
- test262/docs: verify 412 additional indexed-collection and shared-memory API
  fixtures: 328 TypedArray-family, one ArrayBuffer, 32 SharedArrayBuffer, and
  51 Atomics cases. The native Test262 helper now preserves typed-array
  constructor identity and supplies Uint8Clamped byte-conversion expectations;
  all 412 fixtures pass in the native harness.
- test262/docs: verify 500 additional positive class-declaration fixtures: 409
  class-method parameter-destructuring cases and 91 declaration, computed-name,
  accessor, async-generator, and definition cases. All 1,000 metadata-selected
  strict/non-strict variants and all 500 native fixtures pass without compiler,
  runtime, or harness changes.
- test262/docs: verify 500 additional class-expression fixtures: 236 p…39951 tokens truncated…rmatting plus replacer/toJSON return-value serialization, enabling the newly ported non-`eval` JSON.stringify test262 cases and refreshing ECMA-262 JSON docs.

## v0.9.31 - 2026-06-20

- compiler/runtime/perf: reduce `dromaeo-object-string` execution allocations below Jint while keeping jroc faster by expanding direct string intrinsic lowering, caching single-code-unit string results, avoiding intermediate `Array.join` string lists, presizing literal string split results, and reusing repeated substring/slice/substr outputs through a bounded runtime cache.

## v0.9.30 - 2026-06-19

- compiler/runtime/tests/perf: optimize the `dromaeo-string-base64` scenario by fusing numeric `charCodeAt` calls and lowering eligible local string `+=` accumulators through an internal `StringBuilder`, with regression coverage for preserving plain assignment types after string accumulation.

## v0.9.29 - 2026-06-19

- ci/workflows: switch NuGet publishing to trusted publishing (OIDC) and harden release-following benchmark/smoke workflow gating so reruns wait for a successful `publish-tool.yml` run by release version instead of failing on an earlier failed publish attempt.

## v0.9.28 - 2026-06-14

- repo/tooling/docs/tests: rename project identity from `js2il` to `jroc` across source, solution/tests/docs paths, and runtime-visible naming surfaces; keep `.config/dotnet-tools.json` on published `js2il` until the tool package rename is released.
## v0.9.27 - 2026-06-05

- compiler/runtime/tests/perf: fix issue #1196 by restoring the mitata `string-width` benchmark under jroc, including awaited import-binding rewriting, verifier-safe async `for..of` lowering, destructuring/capture/runtime gaps uncovered by the port (`Intl`, `Number.isSafeInteger`, `node:process`), and a jroc-safe benchmark runner path with focused regression coverage.

## v0.9.26 - 2026-05-29

- tests/docs/test262: record the additional expression/operator `test262` slice in the active changelog, covering conditional, nullish-coalescing, equality/relational, unary-plus, update-expression, and addition cases alongside the refreshed ECMA-262 operator docs.
- tests/docs/test262: port 40 additional statement/control-flow test262 cases across `if`, `switch`, `while`, `do-while`, `break`, `continue`, `try`, and labeled statements, expanding the checked-in slice with additional early-error, switch-lexical-environment, catch-scope, and loop-control coverage while refreshing the linked ECMA-262 statement docs.
- tests/docs/test262: port 40 additional expression/operator `test262` cases across addition, relational comparison, equality, logical, nullish-coalescing, conditional, unary-plus, and update-expression areas, and refresh the linked ECMA-262 operator coverage docs for the expanded checked-in slice.
- tests/docs/test262: port 40 additional function, parameter, arrow-function, rest-parameter, and closure-adjacent execution cases across function expressions/declarations, arrow bodies, strict caller/arguments restrictions, function-call this binding, and parameter/rest binding patterns while refreshing the linked ECMA-262 support notes.
- tests/docs/test262: port 40 additional class/inheritance `test262` cases across declaration/expression computed elements, instance/static accessors, derived-constructor/prototype behavior, heritage validation, and `super` property reads; refresh the linked ECMA-262 support notes for class definitions and `super`.
- tests/docs/test262: port 40 additional object/property/prototype `test262` execution cases under `built-ins/Object/{create,getOwnPropertyDescriptor,getOwnPropertyNames,getPrototypeOf,setPrototypeOf}` and `language/expressions/object`, covering object-literal property-name forms, `Object.create` property-bag behavior, own-property descriptor lookup, own-name reflection, prototype queries, and the supported `Object.setPrototypeOf` surface while refreshing the linked ECMA-262 support evidence.
- tests/docs/test262: port 40 additional Array and TypedArray upstream execution cases into `tests/Jroc.Test262.Tests`, covering array indexing/holes/core prototype methods plus supported typed-array iterator and `%TypedArray%.from` basics, and refresh the linked ECMA-262 evidence.
- tests/docs/test262: port 40 additional String and RegExp execution cases covering String constructor coercion plus `charAt`/`charCodeAt`/`search`/`replace`, and RegExp constructor plus `exec`/`test` match-result behavior; refresh the linked ECMA-262 support evidence.
- runtime/tests/docs/test262: port 40 additional Number/Math/BigInt/Date test262 execution cases covering Number coercion plus `Number.isFinite`/`Number.isNaN`, Math value properties and `sqrt`, BigInt string construction, and additional Date constructor arities.
- compiler/runtime/tests/docs/test262: fix the newly ported collection/iteration cases across Map/Set/WeakMap/WeakSet and `for-of`, including iterable constructor adder semantics, constructor/global metadata, and generator IteratorClose paths.
- compiler/runtime/tests/docs/test262: port 40 additional Promise/async/generator `test262` cases covering Promise.prototype.catch/finally callable/metadata semantics, async-function default/trailing-comma/finally ordering behavior, generator yielded-value flow and string delegation, async-generator `yield`/`await` operand ordering, and `for await..of` destructuring iteration.
- compiler/runtime/tests/docs: add top-level `await` support by parsing awaits outside functions, lowering async module bodies through the existing Promise state-machine pipeline, waiting for the top-level Promise in the generated module wrapper, and covering resolved, pending, and caught-rejection execution paths.

## v0.9.25 - 2026-05-24

- compiler/runtime/tests/docs/test262: fix issue #1101 by aligning arrow/class callable prototype metadata, restricted `caller`/`arguments` accessors, class heritage validation, object literal `__proto__` mutation semantics, and BigInt literal property-name normalization with the newly unskipped expression test262 cases.
- compiler/runtime/tests/docs/test262: fix reference evaluation and coercion semantics for expression cases, including member/identifier GetValue and PutValue behavior plus the covered equality and relational coercion paths.
- compiler/runtime/tests/docs/test262: fix try/labeled/throw completion handling by routing finally-return overrides correctly, preserving indirect-call global `this` for non-strict function values, materializing thrown expression values across short-circuit branches, and unskipping the related supported statement ports.
- runtime/tests/docs/test262: expose the covered intrinsic async/generator constructor objects with the expected callable metadata and extensibility surface, unskipping the corresponding built-in constructor tests while leaving the advanced construction cases tracked separately.
- compiler/tests/docs/test262: support additional statement-level array destructuring cases, including the newly unskipped variable/const, `for`, and `catch` iterator-semantics coverage.
- runtime/tests/docs/test262: align Proxy constructor and callable proxy apply semantics with the newly unskipped test262 coverage, including the covered validation and trap-dispatch cases.
- compiler/tests/docs/test262: fix lexical scope handling for expression forms so arrow functions, class elements, and function expressions capture the correct bindings in the newly unskipped test262 cases.
- runtime/tests/docs/test262: unskip supported Object and JSON built-in coverage for `Object.assign`, `Object.keys`, `Object.values`, and `JSON.stringify`, including the newly fixed primitive-target and ordinary-object serialization cases.
- compiler/tests/docs/test262: support tail-position conditional and logical expression lowering in return paths, unskipping the related expression test262 coverage.
- runtime/tests/docs/test262: align Promise constructor executor and resolve semantics with the newly unskipped test262 coverage, including strict/sloppy executor call context and the covered Promise surface ordering cases.
- runtime/tests/docs/test262: fix Math built-in function metadata for static intrinsic methods, unskipping the related `Math.abs`, `Math.acos`, `Math.floor`, and `Math.max` test262 coverage.
- runtime/tests/docs/test262: support `WeakMap(iterable)` and `WeakSet(iterable)` constructor initialization, unskipping the related weak-collection test262 coverage while keeping broader weak-collection edge cases tracked separately.
- compiler/runtime/tests/docs/test262: fix issue #1097 by unskipping supported `for-in`/`for-of` statement ports, including loop-head TDZ binding checks, inherited array-index setters during `for-in`, typed-array iteration for `Float32Array`, `Int16Array`, and `Int8Array`, and iterator-close semantics for destructuring and generator `for-of` paths while leaving eval-only cases skipped.
- runtime/tests/docs/test262: fix issue #1104 by unskipping supported Array/String built-in ports, including `Array.from` metadata, `Array.isArray(Array.prototype)`, inherited array-index lookup for `reduce`, and array object-to-primitive coercion needed by `String.prototype.indexOf`, `isFinite`, and `isNaN`, while keeping the eval-dependent `indexOf` case explicitly skipped.
- compiler/runtime/tests/docs/test262: fix issue #1107 by unskipping Date, Number, and RegExp built-in test262 ports; Date constructor metadata now matches `name`/`length`, `new Date(date)` copies the Date value without user coercion, Number exposes `EPSILON`, `parseFloat`, and `parseInt`, and discarded `Number(value)` calls still perform abrupt `ToNumber` coercion.
- compiler/runtime/tests/docs/test262: unskip supported non-eval test262 ports covering Date constructor visibility, strict assignment to read-only intrinsic data properties, named function-expression self bindings, and generator method construction/call semantics while keeping eval-dependent cases explicitly skipped.

## v0.9.24 - 2026-05-22

- compiler/runtime/perf: reduce stopwatch benchmark execution overhead by tagging generated functions that do not need ambient invocation context, skipping unnecessary runtime context writes on hot delegate calls, adding zero/one-argument Date constructor overloads, and exposing Date `valueOf()` through the standard object-to-primitive path.

## v0.9.23 - 2026-05-22

- runtime/tests/docs/test262: close issue #1046 by unskipping the remaining `String.prototype.indexOf`, `slice`, and `split` test262 ports, fixing boxed primitive/string coercion, callable metadata/prototype wiring for String built-ins, object-to-primitive ordering, split limit coercion, and empty `RegExp` split behavior.
- tests/test262: close issue #1048 by keeping the class-family test262 ports unskipped and running the duplicate-basename computed-yield class statement ports out-of-process so the full class slice no longer races on generated assembly identity.
- compiler/runtime/tests/test262: fix issue #1049 by supporting computed object destructuring keys, iterator-protocol array destructuring close ordering, Proxy-aware object rest copy semantics, and with-environment binding probes needed by the related test262 destructuring ports.
- runtime/tests/docs/test262: fix issue #1045 by unskipping supported Function constructor and `Function.prototype.call` ports, adding null/undefined global-this substitution for `call`/`apply`, wiring function objects through inherited Object.prototype helpers, preserving non-enumerable Function.prototype method metadata, and keeping eval-only cases tracked in issue #1079.

## v0.9.22 - 2026-05-22




## v0.9.21 - 2026-05-20

- compiler/tests/docs/test262: close issue #1051 by fixing statement-condition truthiness for falsy strings, correcting stale stable-type inference that broke `for..of` `array-expand`, supporting assignment-form destructuring loop heads like `[x.attr]`, unskipping the related supported `test262` ports, refreshing the affected generator snapshots, and tracking the remaining unsupported `eval`-blocked skips separately in issue #1079.
- compiler/runtime/tests/test262: fix issue #1050 by restoring const TDZ behavior for self-references and closed-over bindings, inferring names for const-bound anonymous functions/arrows/generators, respecting static `name` members on anonymous classes, and unskipping the related supported `test262` const cases while leaving the `eval`-blocked `cptn-value` case tracked separately.

## v0.9.20 - 2026-05-17

- compiler/runtime/tests/docs: fix issue #1059 by supporting `super()` calls to regular function-valued class bases, preserving returned replacement objects/proxies as the derived receiver, routing public field initialization through proxy-observable define-property semantics for replacement receivers, and unskipping the related test262 class tests.

## v0.9.19 - 2026-05-17

- tooling/perf/docs: replace the default BenchmarkDotNet direct Node.js process-per-iteration comparison with ClearScript hosted V8, so the .NET benchmark suite compares in-process JavaScript execution paths instead of mixing Node process startup into runtime timings.

## v0.9.18 - 2026-05-16

- tooling/perf/docs: expand the BenchmarkDotNet scenario catalog, make phased jroc benchmark failures surface explicitly instead of being skipped, and make benchmark runs exit non-zero when any benchmark case fails so broken perf scripts are not misreported as successful timings.
- compiler/tests/perf: fix issue #1068 by allowing unknown global constructor identifiers in `new` expressions to lower through the dynamic `ConstructValue` path, restoring `linq-js` benchmark compilation and adding focused try/catch regression coverage for the parser gap.
- compiler/tests/perf: fix issue #1067 by requiring native numeric compound-assignment lowering to operate only on proven unboxed doubles, so boxed loop counters fall back through numeric coercion instead of corrupting `+=` updates; this restores the `dromaeo-string-base64*` benchmarks and adds focused compound-assignment regression coverage.

## v0.9.17 - 2026-05-14

- tooling/perf/docs: add Okojo to the prime benchmark comparison harness, build the new comparison project in the Linux/Windows smoke and performance workflows, and refresh the performance comparison docs.

## v0.9.16 - 2026-05-12

- compiler/runtime/tests/docs: close issue #1058 by modeling derived-constructor `this` as a mutable TDZ binding that stays uninitialized until `super()` completes, so arrows created before `super()` observe the initialized receiver during instance field initializers while non-derived/simple constructor paths keep their direct fast path behavior.

## v0.9.15 - 2026-05-10

- compiler/runtime/tests/docs: fix issue #1048 - class static method ABI now uses the correct CLR-static calling convention, fixing parameter-index misalignment for static class methods with parameters. `Object.getOwnPropertyDescriptor()` correctly identifies getter/setter descriptors on class prototypes and static class objects. Class static block declaration order is fixed by using SSA-map lookups (avoiding spurious TDZ sentinel reads during static initialization). Nested classes that extend a globally-scoped parent class now compile correctly. Net improvement: 618→622 Test262 passing, 17→13 failing.
- runtime/tests/docs/test262: close issue #1047 by fixing JSON.stringify array-replacer/property-order semantics, ordinary-object key ordering after assignment/delete/defineProperty mutations, function-object Object.entries enumeration, and numeric -0 property-key normalization, unskipping the related JSON/Object test262 ports and refreshing ECMA-262 support docs.

## v0.9.14 - 2026-05-08

- compiler/cli/sdk/tests/docs: remove the top-level `"use strict"` requirement and its configurable warning/error flag so JROC accepts both strict and non-strict scripts by default.
- tests/docs: port 400 additional crosscutting `test262` cases into `tests/Jroc.Test262.Tests`, fix the newly added collection/iteration coverage so the PR does not add explicit skips, sync the linked ECMA-262 evidence docs for the newly strengthened areas, and remove stray PR-1010 function snapshot files that should never have been added.
- compiler/runtime/tests/test262: follow PR #1043 by fixing and unskipping 138 previously skipped `test262` ports across arrays/functions, JSON/string/object built-ins, control-flow/binding semantics, and class/expression semantics, reducing the remaining checked-in skip count to 135.

## v0.9.13 - 2026-05-04

- compiler/tests: infer stable primitive parameter types (`number`, `boolean`, and `string`) for direct class method calls when all visible call sites agree, allowing generated IL to use typed parameters and direct primitive operations while keeping delegate-backed callables on the object ABI.

## v0.9.12 - 2026-05-04

- compiler/runtime/tests: fix the prime benchmark regression by restoring direct `Int32Array` numeric-index hot paths (including boxed numeric indexes) instead of falling back to generic item helpers, repairing the corresponding inline typed-array IL emission for verifier-safe direct reads, and adding a boxed-double `TypeUtilities.ToNumber(...)` fast path to reduce coercion overhead in hot loops.

## v0.9.11 - 2026-05-03

- compiler/runtime/tests/test262: remove the remaining skipped representative `test262` execution ports by fixing block/function scope materialization, hosted global service resolution, sloppy assignment and unary lowering edge cases, and several built-in/runtime semantics gaps (including `Object.values`, `Number.isInteger`, `parseInt`, `isNaN`, `isFinite`, `Array.isArray`, loose BigInt equality, and non-constructible intrinsics), bringing the checked-in `Jroc.Test262.Tests` slice to 326 passing tests with 0 skips.

## v0.9.10 - 2026-04-30

- compiler/runtime/node/tests/docs: remove the remaining skipped `Jroc.Test262.Tests` ports by fixing block/switch lexical-scope materialization and block-scope type visibility, constructor `_scopes` initialization/insertion rules, sloppy assignment `ThrowOnError` behavior, Number wrapper / `RegExp.prototype.toString` gaps, and hosted `child_process.fork()` IPC ordering; refresh the affected generator snapshots, add an on-demand Copilot skill for porting individual `test262` cases, and bring the checked-in `test262` slice to 121 passing tests with 0 skips.

## v0.9.9 - 2026-04-27

- compiler/packaging/tests: fix the v0.9.8 Linux and Windows release smoke regression by capturing module-local bindings that shadow runtime global intrinsic names (for example `URL`) when nested CommonJS function expressions close over them, restoring the packaged Hosting.Domino sample build.

## v0.9.8 - 2026-04-27

- runtime/spec/tests: align arrow-function restricted `caller` / `arguments` access by exposing `TypeError` globally and wiring function prototypes.
- tooling/tests/docs: close issue #934 and umbrella #927 by replacing the single post-MVP `test262` expansion bucket with a checked-in rollout plan plus ADR, creating dedicated follow-on issues for modules, async/Promise, raw+harness-heavy, agent/CanBlock, and Intl/environment-sensitive suites so future conformance work lands in bounded slices instead of one vague umbrella.
- tooling/tests/docs: close issue #933 by mapping bounded test262 MVP results back to ECMA-262 support docs and backlog ownership through a checked-in linkage manifest, annotating `summary.json` with clause/doc linkage, surfacing test262 evidence in the relevant ECMA-262 section docs, and documenting when linked failures should update docs vs attach to or create issues.
- tooling/tests/docs: close issue #932 by adding named bounded test262 MVP suites for PR vs nightly runs, exposing local npm entrypoints for those suites, publishing `summary.json` artifacts from a dedicated GitHub Actions workflow, and reusing the repository's scheduled-failure issue pattern for nightly regressions while extending focused runner integration coverage.
- tooling/tests/docs: close issue #931 by formalizing test262 MVP result kinds/verdicts, classifying parse/runtime negatives plus policy skips vs unsupported requirements, emitting a stable `summary.json` baseline artifact for each runner invocation, documenting the contract in a new ADR, and extending focused runner integration coverage.
- tooling/tests: close issue #930 by adding a plain synchronous script MVP test262 runner that resolves the pinned checkout, filters unsupported/module/async/agent cases, executes strict/non-strict variants plus parse/runtime negatives through jroc, emits per-test outcomes with repro commands, and adds focused runner integration coverage.
- tooling/tests: close issue #929 by adding a test262 frontmatter parser with a normalized runner-facing metadata model, explicit MVP blocker/unsupported metadata reporting, and focused integration coverage for representative strict/module/async/negative/fixture cases.
- tooling/tests/docs: close issue #928 by adopting a pinned sparse-checkout intake model for upstream `tc39/test262`, adding the bootstrap CLI + pin metadata used by local/CI acquisition, recording the licensing/update policy in a new ADR, and adding focused integration coverage for the bootstrap describe flow.
- node/http/https/tests/docs: fix WHATWG `URL` + second-argument request-option interop for `http.request(...)` / `https.request(...)` so URL-derived path/query and TLS/client overrides stay aligned with Node call shapes, add focused HTTPS regression coverage plus end-to-end `--url` / `--auto` smoke tests for the ECMA-262 extractor network workflow, and refresh the HTTPS docs.

## v0.9.7 - 2026-04-07

- node/child_process/tests: treat expected fork IPC transport shutdowns from `process.disconnect()` / normal child exit as plain `disconnect` teardown instead of surfacing unhandled socket errors, keeping `fork(..., { silent })` behavior aligned with Node during shutdown.
- node/url/compiler/tests/docs: close issue #946 by exposing `URL` / `URLSearchParams` as shared global constructor values, wiring their constructor/prototype surfaces and `new`-required behavior to match the existing built-in function-object model, teaching bare `new <GlobalThisCtor>(...)` lowering to accept constructible `GlobalThis` properties, adding focused global URL execution/generator coverage, and refreshing the Node url/global docs.
- node/stream/tests/docs: close issue #955 by adding constructor-level stream object-mode flags (`readableObjectMode` / `writableObjectMode`), a focused `node:stream/promises` baseline for Promise-oriented `finished(...)` / `pipeline(...)`, AbortSignal-aware helper cancellation, focused execution/generator coverage, and refreshed Node stream docs.
- node/crypto/tests/docs: close issue #954 by adding a focused `crypto.pbkdf2Sync(...)` baseline over .NET PBKDF2 for string and binary inputs with explicit sha1/sha256/sha384/sha512 support, extending Node crypto error-path coverage for invalid digest/input/range handling, and refreshing the Node crypto docs while keeping broader cipher/asymmetric/Web Crypto gaps explicit.
- node/loader/tests/docs: expand the documented post-#869 package-loader slice by allowing package.json `imports` aliases to target bare package specifiers (including patterned subpaths) in addition to package-local `./...` paths, add focused resolver plus Node module execution/manifest coverage, and clarify the remaining unsupported loader/runtime-probing boundaries.
- node/child_process/tests/docs: expand the post-baseline `fork(...)` slice with explicit `options.silent` stdio control (`true` piped, `false` inherited while keeping IPC), make detached async child-process requests fail with targeted diagnostics instead of being silently approximated, add focused execution/generator coverage, and refresh the Node child_process docs.
- node/timers/tests/docs: complete issue #875's follow-on by implementing the `node:timers/promises.setInterval(...)` async-iterator contract with queued tick/backpressure handling, `AbortSignal` rejection for active iterators, deterministic teardown on `return()` / `for await ... break`, focused execution/generator/runtime coverage, and refreshed Node module docs.
- compiler/tests: fix resumable async/generator scope-array slot mapping for captured outer function bindings, add focused async regression coverage for module-level helper calls that suspend across `await`, and refresh the affected generator snapshot uncovered while investigating issue #842.
- compiler/debug/tests/docs: fix rewritten ES module Portable PDB sequence points by mapping both top-level rewrites and nested user-authored callable bodies back to the original source coordinates, hiding generated interop helper code, extending source-mapped stack-trace coverage for rewritten import/export flows, and documenting the remaining debugger limitations.
- runtime/spec/tests: close issue #935 by wiring keyed-collection constructor values into the `Function.prototype` / `Object.prototype` chain like real built-in function objects, preserving their constructor/prototype descriptor surface, extending focused Map/Set/WeakMap/WeakSet reflective coverage, and documenting that broader constructor-object fidelity for other built-ins remains follow-up work.
- node/fs/tests/docs: advance issue #953 by enriching `fs` / `fs.promises` Stats results with mode, timestamp, and type-predicate metadata across sync/callback/promise stat helpers, adding focused Node fs regression coverage, and documenting that watch APIs plus raw numeric-fd callback parity remain follow-up work.

## v0.9.6 - 2026-04-02

- compiler/runtime/tests: close issue #740 by keeping prime hot-path member-call results in typed locals until true object consumers, teaching the IL emitter/temp allocator to respect materialized destination storage for pinned temps and direct user-class instance calls, adding safe typed-array Int32Array/index fast paths for sieve-heavy loops, and refreshing the affected generator snapshots.
- runtime/spec/tests/docs: close issue #861 by exposing `Map`, `Set`, `WeakMap`, and `WeakSet` as global constructor values, wiring keyed-collection constructor/prototype back-references plus instance prototype stamping, requiring `new` across first-class constructor call paths, tightening built-in `.prototype` descriptor flags, adding focused execution/generator coverage, and refreshing keyed-collection ECMA-262 tracking; broader built-in constructor-function object fidelity remains tracked separately in issue #935.
- node/child_process/hosting/tests/docs: close issue #914 by adding hosted `JsEngine` support for `child_process.fork()` through explicit `JsModuleLoadOptions.CompiledAssemblyPath` configuration, host-overridable `IChildProcessLauncher` process creation, deterministic hosted misconfiguration errors instead of implicit launch-path fallback, focused hosting/child_process regression coverage, and refreshed hosting plus Node child_process docs.
- runtime/spec/tests/docs: close issue #864 by exposing the public `String.prototype` / `%StringIteratorPrototype%` surface, wiring `String.prototype[Symbol.iterator]`, adding `String.fromCodePoint`, `String.raw`, `at`, `codePointAt`, `matchAll`, `padStart`, `padEnd`, `replaceAll`, `isWellFormed`, and `toWellFormed`, tightening the `String.prototype` constructor descriptor flags, fixing `String.raw` final-segment substitution handling, adding focused execution/generator coverage, and refreshing ECMA-262 string tracking/backlog docs.
- runtime/spec/tests/docs: close issue #862 by completing `Map`/`Set` iterable construction, `Map.prototype.forEach`, `Map.prototype[Symbol.iterator]`, the missing `Set.prototype` core methods, and the ES2025 Set algebra surface; align iterable-constructor closing semantics with normal-vs-abrupt completion, reject primitive `Map` entry values while preserving short object entries as `undefined`, add focused constructor regression coverage, and refresh the keyed-collection ECMA-262 tracking/backlog docs.
- runtime/spec/tests/docs: close issue #863 by extending Proxy support with `deleteProperty`, `ownKeys`, `apply`, `construct`, `getPrototypeOf`, `setPrototypeOf`, and `Proxy.revocable`, while tightening supported-surface semantics around object target/handler validation, callable/constructible target gating, and `ownKeys` / `getPrototypeOf` / `construct` trap result checks; add focused Proxy regression coverage and refresh the ECMA-262 backlog/docs.

## v0.9.5 - 2026-03-29

- packaging/hosting/samples/tests: support npm/module-id entrypoints in `Jroc.SDK` with Node-style package resolution, keep `ModuleResolutionBaseDirectory` / `JrocModuleResolutionBaseDirectory` available only for non-default layouts, and simplify `Hosting.Domino` so the host project restores domino in place without a separate `compiler/` directory.
- compiler/perf/tests: refine captured lexical TDZ lowering by separating semantic TDZ participation from runtime field guards so safe captured `let`/`const` bindings keep typed scope fields and direct loads, while conservatively preserving runtime guards for captures that can still be observed before initialization; this recovers the PR #909 benchmark-adjacent regressions and refreshes the affected generator coverage.
- tests/node/child_process: stabilize the `fork(...)` message-passing coverage under full-suite load by widening the child fallback exit window and refreshing the paired execution/generator expectations.

## v0.9.4 - 2026-03-16

- compiler/runtime/tests: fix computed class elements follow-up gaps by treating non-literal computed names as runtime keys, preserving receiver semantics for `obj[key]()`, and aligning private accessor edge cases for class expressions plus getter-only/setter-only access.
- compiler/runtime/spec/tests/docs: close issue #772 by lowering dynamic `import()` against canonical compiled-module ids, aligning the entry-module `require` context with the emitted module manifest, returning the same pragmatic ESM/CJS namespace-default projection from dynamic `import()` as the static ESM lowering, adding focused Import execution/generator coverage, and refreshing ECMA-262 tracking while leaving full module-record semantics to issue #857.
- compiler/spec/tests: finish the issue #857 follow-up on the PR #908 module-record work by restricting `RequestedModules` and evaluation planning to static ESM imports/re-exports (instead of mixed runtime dependencies), preventing `.js` source-text modules with static syntax from silently falling back to dynamic CommonJS export validation, marking evaluation metadata as planned rather than already evaluated, and adding focused ModuleLoader/import regressions for the corrected graph semantics.
- node/net/tests/docs: close issue #874 by emitting `Buffer` socket reads by default, keeping UTF-8 text mode opt-in via `setEncoding()`, adding `setTimeout()` / `setKeepAlive()` baselines plus `allowHalfOpen` delayed-response handling, keeping `setNoDelay()` as a compatibility no-op, and refreshing focused Node `net` coverage and tracking docs.
- node/http/tests/docs: close issue #871 by streaming `IncomingMessage` bodies incrementally as `Buffer` chunks, adding chunked transfer encoding/decoding for request and response flows, introducing a sequential keep-alive `http.Agent` baseline plus forwarded server `connection` events, surfacing explicit diagnostics for deferred CONNECT/upgrade/Expect behavior, and refreshing focused Node HTTP coverage/tracking docs.
- node/zlib/tests/docs: close issue #876 by adding a focused `node:zlib` baseline with `gzipSync(...)` / `gunzipSync(...)`, `createGzip(...)` / `createGunzip(...)` stream composition over the current Transform implementation, Node-like `level` coercion in the `-1..9` range, corrected finish/end ordering for the delivered gzip/gunzip pipeline, explicit deferred-option diagnostics, focused execution/generator coverage, and refreshed Node tracking/docs.
- node/timers/tests/docs: close issue #875 by adding a `node:timers/promises` baseline with Promise-based `setTimeout(...)` / `setImmediate(...)`, `options.signal` cancellation that rejects with `AbortError`, focused ordering coverage against `process.nextTick` and Promise microtasks, and explicit deferred `setInterval(...)` diagnostics plus module docs/tracking updates.
- node/https/tls/tests/docs: close issue #870 by adding a PEM-backed loopback/local TLS baseline over `SslStream` (`tls.createSecureContext`, `tls.createServer`, `tls.connect`, `TLSSocket`) plus HTTPS client/server flows (`https.createServer`, `https.request`, `https.get`) reusing the existing streamed/chunked HTTP pipeline, with focused execution/generator coverage and explicit diagnostics for deferred advanced TLS options.
- node/crypto/tests/docs: complete issue #790 by adding `createHmac(...)` plus a practical `webcrypto.subtle` HMAC slice (`digest`, `importKey`, `sign`, `verify`), tightening Node/WebCrypto input validation, extending focused execution/generator coverage, and refreshing the Node crypto docs.
- node/fs/tests/docs: close issue #873 by adding a documented `fs.open(...)` / `fs.promises.open(...)` `FileHandle` baseline, `createReadStream(...)` / `createWriteStream(...)`, `appendFile(...)` / `rename(...)` / `unlink(...)`, Node-like Windows sharing and explicit-position parity fixes, focused execution/generator coverage, and refreshed Node fs tracking docs.
- node/child_process/hosting/tests/docs: harden the `fork(...)` baseline from issue #877 by authenticating the loopback IPC channel, stopping the listener after the intended child connects, emitting `disconnect` before `exit`/`close`, replacing the brittle fixed IPC connect timeout with child-exit-aware waiting, and throwing an explicit hosted-`JsEngine` `fork()` diagnostic while hosted support is tracked separately in issue #914.
- compiler/tests: optimize branch-only `typeof x === "function"` / `!== "function"` checks by lowering them to direct `isinst Delegate` tests in the LIR/IL pipeline, with focused BinaryOperator and normalization coverage for bound and CommonJS callable shapes.

## v0.9.3 - 2026-03-15

- node/stream/tests/docs: close issue #872 by expanding the stream baseline with `pause()` / `resume()`, UTF-8 `setEncoding()`, `destroy()` / `destroyed`, callback-oriented `pipeline(...)` / `finished(...)`, deterministic writable drain/finish teardown, and focused execution/generator coverage plus refreshed Node tracking.
- compiler/runtime/spec/tests/docs: close issue #858 by supporting getter/setter method definitions in object literals and classes, lowering complex object literals through descriptor-backed property helpers, dispatching class instance/static accessors through the existing runtime property surface with correct `this` binding, adding focused execution/generator/validator coverage, and refreshing ECMA-262 tracking.
- docs/samples/workflows: close issue #850 and umbrella #439 by aligning the hosting sample docs and release smoke workflows with the `Jroc.Runtime` package name and documenting the coordinated restore/build/post-publish validation matrix for the packaged hosting flow.
- node/esm/tests/docs: close issue #869 by resolving static ESM package requests with import-aware `package.json` `exports` / `imports` conditions, preserving require-mode package aliases for CommonJS parity, adding mixed CJS/ESM `node_modules` coverage plus resolver diagnostics, and refreshing the Node loader docs/triage snapshot (PR #888).

## v0.9.2 - 2026-03-14

- packaging/runtime/docs/tests: rename the runtime NuGet package from `JavaScriptRuntime` to `Jroc.Runtime`, add first-class package metadata/readme coverage for its package page, and update the SDK samples/docs/workflow/package tests to reference the new package ID while keeping the runtime assembly name `JavaScriptRuntime.dll`.

## v0.9.1 - 2026-03-14

- packaging/hosting/tests/docs: add the new `Jroc.SDK` NuGet package for issue #846, shipping an in-process MSBuild task plus `build\Jroc.SDK.props` / `.targets` so host projects can declare `JrocCompile` items, compile JavaScript during `dotnet build`, and consume the generated module assembly without shelling out to the `jroc` tool.
- packaging/hosting/samples/docs: migrate the hosting samples for issue #847 from the legacy `compiler\*.proj` shell-out flow onto direct `Jroc.SDK` usage, move packaged sample content from the `jroc` tool nupkg into the `Jroc.SDK` nupkg, and clarify when to use `jroc`, `Jroc.Core`, and `Jroc.SDK`.
- release/tooling/docs: close issue #848 by teaching tagged release builds to pack and publish coordinated `JavaScriptRuntime`, `jroc`, `Jroc.Core`, and `Jroc.SDK` packages, adding a `npm run release:validate` gate that pairs packed-tool canaries with local SDK package-consumption tests, and staging `samples/Directory.Build.props` so release commits preserve aligned package versions.
- packaging/nuget/docs/tests: close issue #849 by expanding the packaged `Jroc.Core` / `Jroc.SDK` README copy, cross-linking the `jroc`, `Jroc.Core`, and `Jroc.SDK` package pages, adding package-project URLs/tags for discoverability, and asserting the shipped nuspec/readme metadata in focused package tests before first publish.
- release/tooling: fix `scripts/bumpVersion.js` so release cuts still update `samples/Directory.Build.props` after the shared `JrocPackageVersion` property gained a `Condition=...` attribute.

## v0.9.0 - 2026-03-13

- packaging/compiler/layout: implement issue #845 by extracting the reusable compiler into `Jroc.Compiler.dll`, adding the referenceable `Jroc.Core` package, wiring the `jroc` tool to consume the new compiler assembly while staying self-contained, splitting the repo layout into `src\Compiler\` sources plus a thin `src\Cli\` tool project, and updating solution/tests/workflows/scripts/samples to follow the new paths.
- release/tooling/workflows: strengthen pre-release confidence for the repackaged `jroc` tool by teaching version bumps to update `Jroc.Core`, adding packaged-tool canary commands that pack and locally install `jroc` before running smoke suites, and wiring the release automation plus `canary-smoke` workflow to validate the actual nupkg instead of a source-run build.
- hosting/runtime/tests/docs: add issue #419 mutable CommonJS exports support by making typed and dynamic hosting exports proxies write through to `module.exports` on the owning script thread, adding focused hosting coverage for root-exports mutation, and updating the hosting docs to reflect the new read/write behavior.
- runtime/spec/tests/docs: complete the issue #728 bound-function baseline by tracking `Function.prototype.bind` metadata for `length` / `name`, routing `new` on bound constructors through target/new-target semantics and target prototypes, suppressing bound-function own `prototype`, adding focused Function execution/generator coverage, and refreshing ECMA-262 tracking.
- runtime/spec/tests/docs: make delegate-backed function `length`/`name` behave as descriptor-backed own properties for issue #727 so direct reads, `Object.getOwnPropertyDescriptor(...)`, and `Object.hasOwn(...)` share the same metadata path; add focused Function execution/generator coverage and refresh ECMA-262 §20.2 tracking.
- runtime/tests/docs: add the issue #791 Node ESM interop baseline by normalizing compiled-module `import.meta.url` to deterministic `file://` URLs, adding focused CommonJS/Import coverage for cross-module URL resolution, and refreshing the related Node and ECMA-262 tracking docs (PR #839).
- runtime/tests/docs: add the issue #792 networking baseline by implementing minimal `node:net` TCP loopback primitives (`createServer`, `connect`, `Socket`/`Server`) plus a narrow `node:http` HTTP/1.1 server/client slice (`createServer`, `request`, `get`, `IncomingMessage`, `ServerResponse`) with focused execution/generator coverage and explicit `node:https` / `node:tls` runtime diagnostics.
