# Changelog

All notable changes to this project are documented here.

For older release lines, browse [`docs/archive/changelog/Index.md`](docs/archive/changelog/Index.md). `CHANGELOG.md` remains the active changelog file.

## Unreleased

- runtime: complete covered `String.prototype.padStart` and `String.prototype.padEnd` behavior, including Symbol fill-string rejection. Ports twenty upstream test262 cases for padding behavior and built-in metadata.
- runtime: add standards-compatible `Map.groupBy` and complete covered `Object.groupBy` behavior for null-prototype results, iterator closing, property-key coercion, and built-in metadata. Ports twenty upstream test262 cases.
- runtime: add standards-compatible `RegExp.escape` for primitive strings, including leading alphanumeric, syntax, solidus, control, whitespace, and surrogate escaping. Ports ten upstream test262 cases.
- runtime: add standards-compatible Map/Set iterator prototypes and `Symbol.toStringTag` metadata for Map, Set, WeakMap, and WeakSet. Ports ten upstream test262 cases covering descriptors and `Object.prototype.toString` behavior.
- runtime: add `String.prototype.normalize` with NFC defaulting, all four ECMA-262 normalization forms, form coercion, Symbol rejection, and invalid-form `RangeError`s. Ports ten upstream test262 cases.
- runtime: add `String.prototype.concat` with generic receiver coercion and ordered argument conversion. Ports ten upstream test262 cases for built-in metadata, invalid receivers, and primitive arguments.
- runtime: add standards-compatible `Uint8Array.fromHex` and `Uint8Array.prototype.toHex`, including strict input validation, lowercase hexadecimal output, non-constructible built-in metadata, property descriptors, and typed receiver validation. Ports ten test262 cases for hexadecimal Uint8Array conversion.

## v0.11.35 - 2026-07-21

- perf/compiler: elide block lexical-environment allocations when every binding is stored in an IL local, while retaining runtime scope instances for captured bindings and block-level functions.

## v0.11.34 - 2026-07-21

- perf/compiler: preserve proven generated-class receiver types through `RequireObjectCoercible<T>`, including single-definition locals initialized by `new`, eliminating redundant boxing, casts, and guarded dynamic-call fallbacks while retaining object fallback for reassigned bindings.
- perf/compiler: infer class instance field CLR types from stable constructor and method parameter bindings, allowing assignments such as `this.sieveSize = sieveSize` to retain unboxed `double` storage.

## v0.11.33 - 2026-07-20

- perf/compiler: infer stable primitive class-constructor parameters from closed-world direct `new ClassName(...)` call sites. Parameter inference can now retain safe leading identifier parameters when a callable has later defaulted parameters, allowing numeric evidence to propagate through `main` and `runSieveBatch` into the `PrimeSieve` constructor.
- compiler: replace the manually repeated type-inference sequence with convergence-driven fixed-point iteration across variables, definite initialization, class fields, callable parameters/returns, and object-literal shapes. Long dependency chains now infer to completion without a pass-count limit, while repeated-state detection prevents non-converging cycles.
- perf/compiler: keep eligible object literals specialized when passed to closed-world callables with simple object-destructuring parameters, and infer stable primitive CLR types for extracted bindings when every call site agrees on the corresponding member type.

## v0.11.32 - 2026-07-20
- perf/compiler/runtime: keep packed dense numeric Arrays in unboxed `double` storage and transition once to generic object storage for mixed values, holes, or descriptor semantics. Numeric literals now populate typed storage directly, predictable lengths use bounded capacity hints without materializing holes, and guarded numeric reads/writes avoid result, index, and discarded-assignment boxing while preserving prototype, descriptor, sparse, integrity, and typed-array coercion behavior. Hosted comparisons reduced allocation by 11.07% for `audio-beat-detection`, 11.54% for `audio-fft`, and 10.64% for `array-stress`, while execution improved 7.95%, 15.07%, and 9.36% respectively. Both cube scenarios ran 4-7% faster with allocation effectively unchanged (+0.17-0.19%). (#1320, #1321, #1323, #1535)
- perf/runtime: replace per-shape `FrozenDictionary` slot storage with canonical ordered property-name arrays, using allocation-free linear lookup through two properties and promoting to an ordinal dictionary above that hosted-measured crossover. Ordered-name enumeration no longer allocates or reconstructs slot order, uncached leaves do not retain intermediate shapes, and deletion compacts/demotes the representation. Hosted shape microbenchmarks reduced construction allocation from 536 B to 112 B at one property, 1,016 B to 192 B at two, 2,264 B to 1,088 B at four, and 27,304 B to 9,592 B at sixteen; enumeration fell from 32-152 B to 0 B. `dromaeo-object-array` improved 2.16% with allocation effectively unchanged, while `dromaeo-object-regexp` allocation fell by 817,659 B/op (-0.548%) with timing effectively flat (+0.21%); `ai-astar` allocation was unchanged with a +0.82% timing difference. (#1533, #1534)
- perf/runtime: capture ordered shape names once per `GetOwnProperties` and dictionary enumeration, avoiding repeated per-property reconstruction while preserving mutation-safe snapshots. Public value snapshots and property deletion also avoid LINQ iterator allocations. (#1534)
- perf/runtime: lazily allocate `JsShape` transition dictionaries so leaf shapes and uncached transitions avoid an empty dictionary allocation while cached property sequences still reuse live child shapes. (#1531)

## v0.11.31 - 2026-07-19

- perf/runtime: restore geometric growth for dense Array backing storage instead of reallocating `Object[]` to the exact next size on every sequential indexed write. Focused allocation coverage now guards 1,024 writes below 512 KB versus 4,253,216 bytes before the fix while preserving holes, sparse jumps, truncation, descriptors, prototypes, and integrity constraints. Hosted comparisons reduced `audio-beat-detection` allocation from 14,205,274,808 to 1,568,118,488 bytes/op (-88.96%) and execution time from 6.491 s to 4.715 s (-27.36%); `audio-fft` allocation fell from 14,006,722,200 to 1,387,380,800 bytes/op (-90.09%) and time from 5.908 s to 4.059 s (-31.30%). Two `array-stress` comparisons kept allocation effectively unchanged (+124 and +215 bytes/op), with timing differences narrowing from +5.39% to +1.57% on rerun. (#1535)
- perf/runtime: store `JsPropertyDescriptor` as a value type so descriptor snapshots and synthesized descriptors no longer allocate a separate descriptor object. Descriptor reads return independent copies while snapshot publication, accessor identity, and runtime-local overrides preserve existing semantics. Field ordering keeps the descriptor at 32 bytes on 64-bit runtimes. Repeated hosted comparisons found no repeatable execution-time regression: `ai-astar` consistently allocated 4,361,400 fewer bytes/op (-0.35%), `array-stress` allocated about 5.8 KB more/op (+0.35%), and `audio-beat-detection` allocated 1,008,000 fewer bytes/op (-0.007%). Compact-layout reruns produced the same allocation results as the original 40-byte struct layout.

## v0.11.30 - 2026-07-18

- perf/runtime: separate exotic-object value reads from descriptor introspection. Array `length`, dense indices, and named values now use backing storage without synthesizing `JsPropertyDescriptor` instances, while stored accessors, data overrides, tombstones, and descriptor APIs retain their existing semantics. A same-runner `ai-astar` comparison reduced mean execution time from 11.663 s to 8.119 s (30.4%) and allocation from 3326.39 MB to 1172.04 MB (64.8%). (#1523)
- runtime: centralize `JsObject` own-property reads behind the virtual `TryGetBoxedValue` contract. Descriptor/accessor handling, delete tombstones, lazy class methods, and exotic subclass dispatch now stay within `JsObject`, while `Object.GetProperty` retains proxy, primitive, and prototype traversal. This behavior-preserving refactor prepares per-instance read optimizations without intentionally changing performance. (#1522)

## v0.11.29 - 2026-07-17

- ci/benchmarks: run the BenchmarkDotNet suite on three parallel hosted runners, independently executing `KrackenExecutionBenchmarks`, `JrocPhasedBenchmarks`, and `JavaScriptRuntimeBenchmarks`.

## v0.11.28 - 2026-07-17

- runtime: complete the `Array : JsObject` migration. `Array.prototype`, its runtime-local overlay, and `Array.prototype[Symbol.unscopables]` now use ordinary `JsObject` storage, removing the final runtime `ExpandoObject` representation while preserving prototype isolation, descriptors, subclass construction, compiler array intrinsics, reflection, Node inspection, JSON, and dense/sparse array semantics. Runtime architecture documentation now defines `JsObject` as the shared ordinary/exotic object substrate. Hosted comparisons measured precompiled `array-stress` execution 3.6% faster with 1.0% lower allocation and `dromaeo-object-array` execution 0.7% faster with 0.7% higher allocation. Compilation timing was 8.8% and 6.9% slower respectively despite no compiler/codegen changes; compiler allocation changed by less than 1%. (#1448, closes #1443)
- perf/compiler: specialize provably initialized, uncaptured top-level numeric `var` bindings into unboxed `double` locals. Module-loop arithmetic and comparisons now avoid repeated numeric coercion and boxing, while hoisted reads, incompatible writes, captures, uninitialized declarations, and any `globalThis` observation retain object storage. (#1511, parent #1322)
- runtime: consolidate generic `JsObject` get/set/has/delete/descriptor/own-key dispatch through virtual internal-operation hooks, removing obsolete parallel `Array` branches while retaining allocation-free numeric index paths. Own-key merging is centralized for ordinary and exotic objects, Array reflection/proxy/`for-in` behavior shares that path, and `Reflect.ownKeys` now exposes ordered string and symbol keys. (#1447, parent #1443)
- runtime: complete Array exotic index and `length` semantics. Canonical indices now preserve data/accessor descriptors through dense-to-sparse promotion, grow `length` through `2^32 - 2`, and keep `2^32 - 1` as an ordinary key. `ArraySetLength` now enforces UInt32 validation, non-writable length, highest-to-lowest truncation with partial failure at non-configurable elements, and strict/sloppy assignment behavior. Array writes and mutating methods honor prototypes, extensibility, sealing, and freezing, while `Reflect.defineProperty` returns `false` for rejected definitions. (#1446, parent #1443)
- runtime: make `Array` inherit `JsObject` and opt into the exotic internal-operation contract. Array instances now keep ordinary named and symbol properties in inherited shape/slot storage while canonical indices remain in dedicated element storage and `length` remains specialized. Descriptor, prototype, proxy, integrity, weak-map identity, sparse/hole behavior, derived-array construction, JSON, console inspection, and ordered own-key behavior continue to operate on the single array identity. Same-machine short-run microbenchmarks measured dense indexed reads at 11.547 us versus 13.417 us on `master`, with 0 B allocated in both cases; construction measured 5.343 us versus 5.642 us, with the inherited object state adding 24 B per 256-element array (2,218 B versus 2,194 B). (#1445, parent #1443)
- runtime: add an explicit, opt-in internal object-operation contract for exotic `JsObject` subclasses, covering own descriptors, values, definitions, writes, deletion, and complete ordered keys while preserving direct hot paths for ordinary/generated objects. Descriptor-store reads retain their shared no-clone contract, canonical array-index parsing is centralized, and focused ordinary-object read/write benchmarks plus an exotic-storage test double prepare the runtime for the phased `Array : JsObject` migration. (#1444, parent #1443)
- modules: add the Node `util/types` / `node:util/types` subpath as an alias of `util.types`, including Undici-required `isUint8Array` and `isArrayBuffer` predicates for Fetch byte validation and WebSocket ArrayBuffer handling. (#1496)
- perf/benchmarks: add Jint and YantraJS execution measurements to the Kraken 1.1 `ai-astar` suite. The data and test scripts load during benchmark setup, so each runtime measures only the prepared `runTest()` invocation alongside the existing jroc and Okojo cases.
- perf/benchmarks: add Kraken 1.1 audio beat-detection and FFT scenarios, using a shared callback-registration harness so each engine measures the registered workload alone. Kraken runs now honor `--scenario`, including branch-comparison workflow dispatches.

## v0.11.27 - 2026-07-16

- modules: add the Node `timers` / `node:timers` module with `setTimeout` and `clearTimeout` exports. One-shot timeout handles now support Node-compatible `refresh()` rescheduling, enabling Undici's snapshot-recorder auto-flush path. (#1495)
- modules: add Node-compatible `buffer` / `node:buffer` module resolution. The module exports the existing `Buffer` constructor plus `isUtf8` and `resolveObjectURL`, enabling Undici's llhttp WASM decoding, SOCKS5 binary protocol handling, and WebSocket UTF-8 validation paths. Buffer views now expose `buffer`, `byteOffset`, and `byteLength` metadata. (#1487)
- modules: add the Node `console` / `node:console` module with a constructible `Console` export that routes `log`, `warn`, `error`, and `table` output to JROC writable streams. This covers Undici's pending-interceptor table formatter. (#1490)
- modules: support Node-compatible CommonJS `require()` of JSON modules, including explicit and extensionless `.json` paths plus package `main` entries. JSON source is validated at compile time and initialized through `JSON.parse` so ordinary JSON object semantics and CommonJS module caching are retained. (#1486)
- compiler/runtime: support lexical `super` property access and constructor calls from nested arrow functions, including derived-constructor `this` initialization and repeated-`super()` errors. Ports four upstream test262 scenarios for lexical `super` in arrows. (#1498)
- perf/compiler: specialize proven numeric function-local `var` bindings and loop induction variables into unboxed `double` locals after a flow-sensitive initialization/write proof. Hoisted reads, conditional and optional writes, mixed types, captures, shadowing, destructuring, and abrupt loop control retain object storage. Legacy `dromaeo-3d-cube` generated IL now has 44 fewer double boxes, 38 fewer `TypeUtilities.ToNumber` calls, and 4 fewer dynamic `Operators.*` calls; a same-runner comparison improved precompiled execution from 12.151 ms to 11.593 ms (-4.6%) and runtime allocation from 5,137.69 KB to 4,850.26 KB (-5.6%). Compile time changed from 79.525 ms to 75.792 ms (-4.7%) while compiler allocation increased from 12,596.37 KB to 13,060.09 KB (+3.7%). The modern lexical-declaration control remained allocation-neutral; its timing varied +3.7% in this run. (#1476, parent #1322)

## v0.11.26 - 2026-07-14

- perf/compiler: replace allocating LIR temp-use iterator traversal with a struct-visitor API across local allocation, stackification, normalization, branch analysis, and IL emission. A warmed seven-sample `dromaeo-3d-cube` compiler measurement reduced median allocated bytes from 84.06 MB to 16.16 MB (-80.8%). (#1481)

## v0.11.25 - 2026-07-14

- perf/benchmarks: add YantraJS to the standard BenchmarkDotNet cross-runtime suite. YantraJS evaluates each scenario in a fresh `JSContext`, so its reported result covers the same cold runtime creation and source evaluation path as the existing Jint, Okojo, ClearScript, and `jroc (compile+execute)` comparisons.

## v0.11.24 - 2026-07-13




## v0.11.23 - 2026-07-13

- perf/compiler: elide runtime lexical-environment creation for empty ordinary and `switch` block scopes while retaining scope instances for blocks with direct bindings. This removes unnecessary per-execution allocations from hot control-flow bodies such as Kraken `ai-astar`'s `findGraphNode` loop and conditional blocks. A same-runner `ai-astar` comparison improved jroc execution from 8.752 s to 8.007 s (-8.5%), while measured allocation remained effectively unchanged (2,242.94 MB to 2,246.64 MB).

## v0.11.22 - 2026-07-13

- runtime: complete the ordinary-object `ExpandoObject` migration (issue #1456, parent #1426). Core dispatch now recognizes only `JsObject` as the runtime-owned ordinary representation; compiler metadata, import metadata, proxy revocation records, bound class constructors, runtime descriptor records, test262 host records, and test helpers no longer construct or rely on `ExpandoObject`. External CLR dictionaries remain host objects. The only remaining runtime `ExpandoObject` uses are Array exotic-prototype storage, explicitly tracked by #1443; DLR isolation from core `JsObject` remains tracked by #1461.
  - Two full phased `dromaeo-3d-cube` BenchmarkDotNet runs on this machine measured precompiled execution at 60.421 ms before and 38.204 ms after (-36.8%), with allocation effectively unchanged (5,143 KB to 5,149 KB). Compile time improved from 700.412 ms to 600.509 ms for the legacy scenario, while the modern scenario varied in the opposite direction (591.132 ms to 660.353 ms), so no general compile-time claim is made.
- runtime: migrate process version/environment snapshots, synchronous child-process result records, and network address records from `ExpandoObject` to `JsObject` (issue #1455, parent #1426). Process environment snapshots use uncached shape transitions so host-controlled environment variable names are neither globally interned nor retained by shared shape caches; child-process and network option reads now use the shared property API, preserving accessor-backed options.
- runtime: migrate lower-coupling Node.js path, query-string, util, timers/promises, and filesystem ordinary result/helper objects from `ExpandoObject` to `JsObject` (issue #1454, parent #1426). Path and query-string result ordering, `util.types` and synthesized inheritance prototypes, `fs.constants`, promise-based file-handle read/write records, and `fs`/timer option reads retain their existing Node-visible behavior through shared property APIs. Higher-risk process, child-process, network, stream, and IPC objects remain deferred.
- runtime: migrate CommonJS default `module.exports` plus ESM dynamic-import namespace, primitive-namespace, and namespace-cache descriptor records from `ExpandoObject` to `JsObject` (issue #1453, parent #1426). CommonJS export aliasing/replacement and circular-require identity, ESM accessor-backed live bindings, namespace cache identity (including non-extensible exports), and hosted `JsEngine.LoadModule` export resolution remain preserved.
- runtime: migrate the per-realm global object and the shared `String`, string-iterator, `RegExp`, `Map`, and `Set` intrinsic prototypes from `ExpandoObject` to `JsObject` (issue #1452, parent #1426). Constructor/prototype identity cycles, intrinsic descriptors, prototype hierarchies, symbol properties, and runtime-store mutation isolation are preserved. `JsObject` now supplies DynamicObject member access and complete dictionary collection copy/removal behavior so the global-object migration retains its existing hosted DLR and collection surface. Array prototype representation remains unchanged under #1443.
- runtime: migrate built-in-created ordinary result/helper records to `JsObject` (issue #1451, parent #1426). `JSON.parse` now creates parsed objects and reviver root holders as ordinary `JsObject` instances with descriptor-free shape slots; reviver traversal preserves child-holder `this`, deletion, replacement, and property-order semantics. RegExp exec/String match/matchAll results now expose null-prototype `JsObject` records for named `groups` and `indices.groups`, preserve source-order keys and present-`undefined` unmatched captures, and create `groups`, `index`, `input`, and `indices` as default data properties without invoking inherited setters. Unmatched entries in the indices array are now JavaScript `undefined` rather than `null`.
  - `IteratorResultObject` already avoided `ExpandoObject`, so iterator methods continue returning it directly; the earlier POCO-to-`JsObject` conversion and its extra allocation were removed. Map/Set iterator records likewise required no representation migration. Remaining `ExpandoObject` sites in these files are excluded global prototypes or `import.meta` module state, not short-lived ordinary result/helper records.
  - Correct `Object.getOwnPropertyDescriptor` result key order to `value`/`writable`/`enumerable`/`configurable` (or `get`/`set` first), and include enumerable descriptor-backed custom Array properties in `for-in`, as required by RegExp result metadata.
  - Short-run precompiled `dromaeo-object-regexp` measurements found no material allocation change versus `master` (within 0.5% across the legacy and modern scenarios) and no execution-time regression; timing results were favorable but too variable for a stronger performance claim.
- perf/runtime: normalize core ordinary-object dispatch around direct `JsObject`-first backing operations in `ObjectRuntime`, isolating transitional `ExpandoObject` compatibility across property reads/writes/deletes, descriptors, ordered keys, prototype checks, `for-in`, spread, iterator close, `in`, and proxy fallback. Deleted intrinsic properties now remain masked consistently even when their compatibility backing slot is retained. Adds representation-parity regressions and a plain-object read benchmark; local precompiled execution improved from 20.32 ms to 19.85 ms for literals and from 38.36 ms to 35.05 ms for constructed objects, with effectively unchanged allocations. (#1450, parent #1426)
- perf/runtime: ordinary receivers allocated by `new Fn()` and the shared ordinary `Function.prototype` / restricted-function prototype objects now use shape-backed `JsObject` storage instead of `ExpandoObject`. Constructor assignments therefore use the descriptor-free own-read path while preserving prototype lookup, bound/dynamic constructors, constructor return overrides, restricted `caller` / `arguments` thrower identity, and mixed-runtime `ExpandoObject` compatibility. A constructed-object benchmark (1,000 instances, 100 repeated read passes) improved precompiled execution from 38.6 ms to 25.8 ms (~33%); current `JsObject` setup/storage increased allocations from 6.95 MB to 11.51 MB, leaving allocation reduction for the core-dispatch/storage follow-up (#1450). (#1449, parent #1426)

## v0.11.21 - 2026-07-11

- compiler/analysis+codegen: object literal inference phase 6 (issue #1434, parent #1428). Specialized object-literal shapes now propagate across call boundaries: a literal passed into a closed-world callable lets the receiving parameter reuse the generated specialized type, so member reads and same-type member writes/updates on the parameter (e.g. `a.b` in `function c(o){console.log(o.b);} c(a);`) lower through the inferred-member field/accessor path instead of dynamic `ObjectRuntime.GetItem`/`SetItem`. Propagation is a coupled, monotone fixed point with cascading invalidation and no iteration cap, so arbitrarily long multi-hop chains and mutual recursion converge deterministically; structurally identical literals canonicalize onto one shared generated CLR type (the structural signature is normalized by member name, so reordered same-shape literals join) and can join at a parameter. A same-type parameter member write is early-bound through the generated setter; conflicting-type writes, method calls through the parameter, conflicting shapes/member types, spread or missing arguments, escaping/aliased callables, and any other unsafe callee use (returns, stores, aliasing, computed/`delete`, unknown calls) conservatively fall back to the generic path.
- compiler/validation+docs: object literal inference phase 5 (issue #1433, parent #1428). Adds compatibility parity coverage for enumeration order, `Object.keys/values/entries`, integer-key ordering, `JSON.stringify`, descriptor-observing APIs (`getOwnPropertyDescriptor`/`defineProperty`), `delete`/`freeze`/`seal`/`in` fallback behavior, and closure-capture/aliasing scenarios (closure capture keeps early binding; aliasing and escapes fall back with identical behavior). Documents the feature, eligibility rules, and remaining conservative exclusions in `docs/compiler/ObjectLiteralTypeInference.md`. Measured locally: literal-heavy read loops ~5× faster with ~44% fewer allocations versus the dynamic path (mirror elision for fully early-bound shapes tracked by #1439).
- compiler/codegen: early-bound object-literal property access (issue #1432, parent #1428). Property reads and writes on eligible object-literal bindings now compile to direct `callvirt` calls on the generated type's typed accessors (`get_<name>`/`set_<name>`) instead of dynamic `ObjectRuntime.GetItem`/`SetItem`, covering plain reads, simple assignments, compound assignments, `??=`, and `++`/`--` update expressions; the accessor setters keep the `JsObject` mirror in sync, so any remaining dynamic path stays correct. Shape analysis now also disqualifies literals whose members are used as destructuring or `for-in`/`for-of` assignment targets so those writes never bypass the typed backing fields.
- compiler/codegen: emit specialized construction for eligible object literals (issue #1431, parent #1428). The LIR lowering now creates generated object-literal instances directly, populates typed CLR fields, and mirrors those values back into `JsObject` storage so existing property reads, descriptors, and enumeration behavior remain compatible; ineligible literals continue to use the existing `CreateObjectLiteral()` path.
- compiler/metadata: add object literal type-generation groundwork (issue #1430, parent #1428). Eligible `ObjectLiteralShapeInfo` instances now get deterministic generated CLR helper TypeDefs nested inside their module type under an `ObjectLiterals` container type (a sibling of `Scope`), with public fields registered in `VariableRegistry` for later construction/direct-access phases; `JsObject` is unsealed so generated literal types can derive from it while preserving dictionary, descriptor, enumeration, and JSON behavior through base-storage mirroring.
- compiler/analysis: add object literal shape eligibility analysis (issue #1429, parent #1428). `SymbolTableBuilder` now records an `ObjectLiteralShapeInfo` on bindings declared with object literal initializers, conservatively proving whether every use of the binding is safe for future early-bound, strongly-typed CLR member access (any escape, reassignment, `delete`, computed access, spread, enumeration, export, or unresolvable member operation disqualifies). Analysis only; no codegen consumes the result yet.
- compiler/fix: `AstWalker` now traverses into `SpreadElement` arguments (previously identifiers inside `...expr` in object/array literals and call arguments were invisible to AST analyses).

## v0.11.20 - 2026-07-10

- perf/runtime: skip the descriptor-store probe (`ConditionalWeakTable` lookup) entirely for plain object literal reads; `JsObject` now tracks a sticky `HasNonDataDescriptors` flag (set by accessors, `delete` tombstones, non-default `defineProperty` attributes, `seal`/`freeze`, intrinsic descriptors, and class-prototype mirror writes), and while it is clear `Object.GetProperty` answers own reads directly from the object's shape/slot dictionary. Object-literal property read loops run ~32% faster locally (10.9s → 7.4s microbenchmark). (#1418 follow-up)

## v0.11.19 - 2026-07-10

- perf/runtime: add fast property lookup dispatch to `Object.GetProperty`; plain JS objects (`JsObject` literals and function-constructed `ExpandoObject` instances) now resolve own reads with a single unified descriptor-store probe (`PropertyDescriptorStore.GetOwnLookup` answering deleted/descriptor/none at once) before the full dispatch ladder, and `TryGetOwnPropertyValue`/`TryGetFastDictionaryOwnValue` use the same single probe instead of separate `IsDeleted` + `TryGetOwn` calls. Kraken `ai-astar` warmed execution drops ~23% (71.5s → 55.1s locally). (#1418)
- perf/runtime: make `PropertyDescriptorStore` reads lock-free; descriptor and override slots now publish immutable copy-on-write snapshots that hot read paths (`TryGetOwn`, `HasAny`, `TryGetOverride`, own-key enumeration) access via a volatile read while writers serialize on a per-slot write lock, removing `Monitor.Enter_Slowpath` as the dominant remaining hotspot in the Kraken `ai-astar` execution trace. (#1417)

## v0.11.18 - 2026-07-09

- perf/runtime: stop cloning property descriptors on every property read (`PropertyDescriptorStore.TryGetOwn` intrinsic + runtime-store paths); descriptors are still cloned on write and by the few callers that mutate a read descriptor (`Object.seal`, `Object.freeze`, `SetProperty` accessor/data update). Cuts Kraken `ai-astar` execution time roughly in half (136s → 69s locally). (#1415)
- perf/compiler: replace repeated O(N) LIR instruction scans with cached def-instruction/use lookups in `LIRToILCompiler` (`TryFindDefInstruction`, `HasAnyUses`) and build the def map once per method in `TempLocalAllocator`; compiling the Kraken `ai-astar` scenario (50k-line data literal) drops from ~65s to ~9.5s locally. (#1415)

## v0.11.17 - 2026-07-09




## v0.11.16 - 2026-07-08

- runtime/hosting/tests: fix ES module named exports (e.g. `export function runTest() {}`) resolving to `null` through the hosting exports layer (`JsEngine.LoadModule` dynamic/typed proxies); `ExportMemberResolver` now reads exports through the runtime property path so ESM export getters are evaluated instead of returning the raw backing slot.
- compiler/tests: fix `InvalidProgramException` when a deeply nested array/object literal is stored into a scope field (e.g. Kraken ai-astar's top-level `var g1 = [[{...}]]` data table); the maxstack estimator now accounts for the receiver slot(s) pushed by scope/parameter/user-class field stores before the inlined value is constructed.
- runtime/tests: pass RegExp capture arguments to `String.prototype.replace` callback replacers, fixing captured replacement callbacks such as Dromaeo's regexp benchmark `capture.toUpperCase()` path.
- perf/benchmarks: report benchmark failures with the underlying exception details (root-cause `TypeError` etc.) in the final stdout summary, emit GitHub Actions error annotations, and stop printing "Benchmark execution complete!" after failed runs, so CI failures like the dromaeo-object-regexp crash are diagnosable without scanning the full log.

## v0.11.15 - 2026-07-08

- compiler/tests: stop early-binding arbitrary method names on typed `JavaScriptRuntime.Array` and `JavaScriptRuntime.Console` receivers; unknown members like custom `Array.prototype.removeGraphNode(...)` now stay on the normal JS member-dispatch path instead of crashing compilation.
- perf/benchmarks: drain pending collectible in-memory load-context unloads between `jroc (compile+execute)` BenchmarkDotNet iterations so the published-package regexp scenarios stop failing mid-run; benchmark adapter failures now also preserve the full exception text for diagnosis.

## v0.11.14 - 2026-07-07

- docs/ecma262: reclassify ECMA-262 Section 8.6 Miscellaneous to `Supported with Limitations`, documenting the current function-instantiation, binding-initialization, iterator-binding, assignment-target, and property-name evaluation coverage and rolling Section 8 up to `Supported with Limitations`.
- docs/ecma262: reclassify ECMA-262 Sections 8.2 Scope Analysis and 8.4 Function Name Inference to `Supported with Limitations`, documenting the current scope-building, hoisting, lexical-environment, and observable SetFunctionName / NamedEvaluation coverage against existing compiler/runtime behavior and focused evidence.
- docs/ecma262: reclassify ECMA-262 Section 8.1 Runtime Semantics: Evaluation to `Supported with Limitations`, documenting the current HIR/LIR/IL evaluation pipeline coverage across expressions, control flow, generators, async evaluation, and evaluation-order semantics while keeping general `eval` unsupported by design.
- docs/ecma262: reclassify ECMA-262 Section 7.4 Operations on Iterator Objects to `Supported with Limitations`, documenting the current sync/async iterator acquisition, step/value, close-on-abrupt-completion, helper cleanup, and iterator-to-list style materialization coverage against existing runtime behavior and focused evidence.
- docs/ecma262: reclassify ECMA-262 Section 7.3 Operations on Objects to `Supported with Limitations`, documenting the current integrity-level, apply/construct argument normalization, species accessor, private method/accessor, grouping, and class-field own-property coverage against existing runtime behavior and targeted evidence.
- docs/ecma262: reclassify ECMA-262 Section 7.2 Testing and Comparison Operations to `Supported with Limitations`, documenting the current `RequireObjectCoercible`, `IsExtensible`, `isWellFormed`, identity, and comparison coverage against existing runtime behavior and targeted evidence.
- runtime/tests/docs/test262: reclassify ECMA-262 Section 7.1 Type Conversion to `Supported with Limitations`, add shared `ToInt8` / `ToInt16` / `ToUint8` / `ToUint16` / `ToUint8Clamp` runtime helpers, implement `Uint8ClampedArray`, port new `for..of` test262 coverage for `Uint8ClampedArray`, and refresh the Section 7.1 support notes.
- docs/ecma262: reclassify Section 6.2 ECMAScript Specification Types so every clause/subclause is now tracked as `Supported with Limitations` or `N/A (informational)`, documenting environment records, abstract closures, data blocks, and class static block records against current implementation evidence and rolling Section 6 up to `Supported with Limitations`.
- runtime/tests/docs/test262: raise the Section 6.1.7 object-model coverage floor by fixing strict `NaN` equality for boxed doubles, preserving NaN across repeated ordinary-property updates, and enforcing the newly ported Proxy `[[GetOwnProperty]]` forwarding/invariant cases; refresh the Section 6.1 docs and roll-ups.
- runtime/tests/docs/test262: expand ECMA-262 Section 21.2 BigInt coverage with newly ported prototype, prototype-method, branding, and wrapper-object test262 cases; add the covered `BigInt.prototype` surface (`toString`, `toLocaleString`, `valueOf`, `@@toStringTag`, and wrapper integration) and refresh the Section 21.2 support docs.
- runtime/tests/docs/test262: expand ECMA-262 Section 21.3 Math coverage with new test262 ports across the previously untracked Math intrinsics, add ES2025 `Math.f16round` / `Math.sumPrecise` callable surface, fix `Math.pow(..., NaN)`, preserve signed zero in `Math.expm1` / `Math.log1p`, correct `ToUint32`-driven `Math.clz32` behavior for large negative inputs, and refresh the Section 21.3 support docs.

## v0.11.13 - 2026-07-07

- perf/compiler: infer stable parameter CLR types from statically-resolved constructor calls (`new Identifier(...)`) and from numeric/boolean/typeof unary arguments (e.g. `-CubeSize`), so constructors like `CreateP(X, Y, Z)` in dromaeo-3d-cube compile with typed float64 parameters; also fixed AST walker traversal to descend into `new` expression callees and arguments so escaped function references in constructor arguments are detected (fixes #1334).
- perf/compiler: extend stable CLR type inference to block scopes at module top level (for-loop head scopes and block statements), so top-level `let`/`const` loop variables like `x`/`y` in `stopwatch-modern.js` compile to unboxed float64 locals instead of boxed objects.
- tests/docs/test262: expand ECMA-262 Section 21.1 Number coverage with newly ported constructor, constructor-property, prototype, prototype-method, and instance-shape test262 cases; implement the covered Number prototype method surface and refresh the Section 21.1 support docs.

## v0.11.12 - 2026-07-06

- perf/runtime: cut jroc-execute allocations for array/property hot paths (dromaeo-3d-cube: ~12.9 MB → ~4.9 MB per op, ~40% faster steady-state): allocation-free `PropertyDescriptorStore.HasAny`, cached small-integer number-to-string conversions, numeric fast path in `ObjectRuntime.GetItem(object, object)`, lazy index-key materialization in the `Array` double indexer, and allocation-free prototype-chain cycle detection in property stores.
- test262: migrated `tests/Jroc.Test262.Tests` execution fixtures to the shared in-memory assert harness, removed copied per-fixture assert/helper blocks across the suite, and restored fixture files toward their upstream `test262` sources.

## v0.11.11 - 2026-07-05

- perf/compiler/tests: support partial stable parameter type inference so unknown or conflicting evidence disables only the affected parameter instead of the whole callable, allowing Dromaeo 3D cube helpers like `Translate(M, Dx, Dy, Dz)` and downstream `MMulti(...)` calls to keep stable array parameter signatures.
- runtime/tests/docs/nodejs: add `fs.unlinkSync(path)` support so compiled tooling like `scripts/release.js` can delete temporary `pr-body.md` / `release-notes.md` artifacts after `gh pr create` and `gh release create`; cover the release-style cleanup flow with focused Node `fs` execution/generator tests and document the new API in the Node module docs.

## v0.11.10 - 2026-07-05

- perf/compiler/tests: restore direct `JavaScriptRuntime.Array` call lowering for proven array receivers in hot paths, including `push`, `pop`, `shift`, `unshift`, `slice`, and `splice`; limit `with`-binding fallback emission to callables created inside `with` bodies so unrelated scope-field reads keep their concrete receiver types.
- runtime/tests/docs/test262: expand ECMA-262 Section 21.4 Date coverage with newly ported test262 cases, implement the covered Date constructor/prototype/static surface, move Date intrinsic descriptor setup into `Date`, normalize local timezone edge cases, and refresh the Section 21.4 support docs.

## v0.11.9 - 2026-07-04

- runtime/tests/docs/nodejs: add minimal `SharedArrayBuffer` and `Atomics.wait(...)` global support so the compiled `scripts/release.js` canary can execute unchanged; cover the canary path with focused typed-array execution/generator tests and document the intentionally partial global support.
- compiler/tests/docs/test262: support named class expression lexical bindings so heritage-created closures and class methods observe the immutable internal class name while same-named outer bindings remain unchanged.
- compiler/runtime/tests/test262: fix the newly ported language test262 cases by aligning strict/bound `this`, destructuring default function-name inference, class field data-property descriptors, computed/private class element handling, derived constructor primitive returns, and generator/async-generator parameter initialization timing.
- compiler/tests/docs/test262: fix assignment-expression evaluation order for computed member targets, add explicit strict-mode prologues for the strict-only assignment ports that rely on read-only-property TypeErrors, and refresh the ECMA-262 assignment-operator coverage notes.
- runtime/tests/docs/test262: align `Array.prototype.at` with spec `ToIntegerOrInfinity` coercion and builtin callable metadata so object indexes coerce via `valueOf()`, `Symbol()` indexes throw `TypeError`, and the method exposes the expected `name` descriptor.
- compiler/tests/docs/test262: preserve strict bare-call `this` semantics by routing strict direct function calls through the normal function-value invocation path, fixing the newly split `language/function-code` strict nested function cases.

## v0.11.8 - 2026-07-02

- runtime/tooling/tests: fix Windows `child_process.execSync` shell execution to preserve quoted arguments when running through `cmd.exe`, add `child_process.execSync` quoted-command regression coverage, and add a `scripts/release.js` Node-only escape hatch (`--node-only` / `JROC_RELEASE_NODE_ONLY=1`) so releases can proceed even if compiled-script execution regresses.
- perf/benchmarks/tooling: add `scripts/runCubePhasedGuardrails.js` plus npm scripts (`perf:phased:cube*`) to run only the Dromaeo cube phased scenarios and report `jroc-execute` vs `jint-execute-prepared` vs `okojo-execute` time/allocation counters, with optional generated-IL smell counting for allocation/codegen guardrails; add phased benchmark `--scenario` filtering so script selection happens in benchmark setup instead of via BenchmarkDotNet `--filter`.
- perf/compiler/runtime: add numeric fast paths for `Math.abs`, `Math.ceil`, `Math.round`, `Math.sqrt`, `Math.sin`, and `Math.cos` when arguments are already unboxed doubles, plus safe `Math.PI` constant lowering (`Math.PI / 180` fold) when the global `Math` binding has not been written; preserve dynamic dispatch when `Math` is written/replaced.
- perf/compiler/tests: infer stable `JavaScriptRuntime.Array` parameter types for direct function and arrow callsites, including the Dromaeo cube `RotateX`/`RotateY`/`RotateZ` helpers, while keeping escaped callables on the object ABI.

## v0.11.7 - 2026-06-30

- perf/benchmarks: add Okojo execution-only coverage to phased BenchmarkDotNet runs with a dedicated `okojo-execute` counter.

## v0.11.6 - 2026-06-30

- compiler/runtime/tests/docs/nodejs: fix npm-run-all2 module-id compilation by materializing regular `for (let/const ...)` loop-head scopes for nested closures, resolving nested arrow callable IDs from loop-body scopes, and adding legacy `new Buffer(...)` constructor support required by transitive dependencies such as `memorystream`; update Buffer docs.
- runtime/tests/docs/nodejs: add `process.stdout` and `process.stderr` writable stream support (including `write(...)`) so CLI-style scripts can report errors through process stdio; add Node Process regressions and update process docs.

## v0.11.5 - 2026-06-29

- runtime/tests/docs/nodejs: add `child_process.execFileSync(file[, args][, options])` support — runs a file directly without a shell, returns stdout as a string, and throws a `ChildProcessError` (with `status`/`code`/`stdout`/`stderr` properties) on non-zero exit; update Node docs.
- runtime/tests/docs/nodejs: add `string_decoder` support with utf8/utf-8 decoding, `node:` prefix resolution, and a stream-style decoder test; update the node support docs/index for the new module.
- sdk/samples/ci: add `NpmRunAll2` sample — compiles `npm-run-all2` and its transitive dependencies into a .NET assembly via `Jroc.SDK`, then calls `createHeader` (task-header formatting) and a `filterTasks` glob helper from C#; add smoke-test steps to `windows-smoke.yml` and `linux-smoke.yml`.
- sdk/samples/ci: add `Picocolors` sample — compiles the `picocolors` npm package to a .NET assembly via `Jroc.SDK` and calls color/style functions from C#; add smoke-test steps to `windows-smoke.yml` and `linux-smoke.yml`.
- sdk/samples/ci: rename all samples to drop the `Hosting.` prefix (`Hosting.Basic` → `Basic`, `Hosting.Domino` → `Domino`, `Hosting.Picocolors` → `Picocolors`, `Hosting.Typed` → `Typed`); update CI workflows, test assertions, and docs.
- runtime/tests/docs/test262: implement `Array.prototype.some` using the existing array-like callback method pipeline, and port 10 failing `built-ins/Array/prototype/some` test262 cases covering primitive wrappers, built-in objects, and arguments-object receivers.

## v0.11.4 - 2026-06-29

- sdk/samples/tests/ci: flatten `samples/Hosting.Domino` by moving host assets out of the nested `host/` folder, update smoke/package validation paths, and derive module-id compile outputs using normalized module names (for example `@mixmark-io/domino` -> `mixmark-io.domino.dll`).
- tests/docs/test262: port 10 RegExp `regexp-v-flag` negative parse cases under `built-ins/RegExp/prototype/unicodeSets`, wiring them as compilation-failure tests so `$DONOTEVALUATE()` is never evaluated, and document the current UnicodeSets limitation.
- compiler/tests: keep stable no-scope zero-arg numeric function returns unboxed (including `NumericInference_ExponentiationLoop_NoBoxing`), threading callable return-type metadata through direct-call lowering and only boxing at object call boundaries.

## v0.11.3 - 2026-06-28

- runtime/tests/docs/test262: add `Map[Symbol.species]` and `Set[Symbol.species]` accessor support with spec-shaped getter metadata (`name`/`length` + configurable/non-enumerable descriptor), and port 5 new test262 cases under `built-ins/Map/Symbol.species` and `built-ins/Set/Symbol.species`.
- runtime/tests/docs/test262: add BigInt constructor static methods `BigInt.asIntN` / `BigInt.asUintN` (including global exposure and callable metadata), and port 10 new `built-ins/BigInt/asIntN` + `built-ins/BigInt/asUintN` test262 cases.

## v0.11.2 - 2026-06-27

- compiler/tests: reduce redundant object materialization when typed numeric temps flow into user-class method calls, trimming boxing in prime/class-method generator snapshots.

## v0.11.1 - 2026-06-27

- runtime/tests/docs/test262: implement `Array.prototype[Symbol.unscopables]` with the covered baseline plus `findLast`/`findLastIndex` and change-array-by-copy entries, mark `Array.prototype[Symbol.iterator]` as non-constructible, and port 5 new built-ins/Array prototype test262 cases.
- runtime/tests/docs/test262: align `String.prototype` legacy-object behavior and `String.prototype[Symbol.iterator]` callable metadata (`name` + non-constructibility), and port 5 new built-ins/String prototype test262 cases (`S15.5.4_A1`, `S15.5.4_A2`, `S15.5.4_A3`, `Symbol.iterator/name`, `Symbol.iterator/not-a-constructor`).

## v0.11.0 - 2026-06-26

- compiler/tests: extract a reusable compiled-assembly artifact for in-memory PE/PDB emission, keep file-backed output as a consumer of that artifact, and add regression coverage for artifact-only compilation without writing generated output files.
- compiler/tests: add a public `Jroc.Core` compile-to-memory API that creates a fresh compiler service provider per request, supports source-text or file-backed input, and returns `JrocCompiledAssemblyArtifact` with focused success/failure coverage.
- hosting/tests: add a collectible in-memory assembly loader for compiled artifacts, loading PE/PDB bytes through `AssemblyLoadContext.LoadFromStream(...)`, sharing the already-loaded runtime assembly, and covering deterministic unload boundaries.
- hosting/tests: add `CompileAndLoadModule(...)` in-memory hosting APIs for typed and dynamic exports, returning disposable module handles that own both the hosted runtime proxy and the collectible assembly-load boundary.
- hosting/tests: define the path-dependent in-memory hosting contract by keeping `Assembly.Location` empty, documenting that pure in-memory hosting never writes files implicitly, and covering the explicit `child_process.fork` configuration error when no launchable compiled assembly path is supplied.
- hosting/tests: expand in-memory coverage to include an `EmitPdb=true` compile-and-load path so the first-cut optional PDB behavior stays covered end-to-end.
- runtime/hosting/tests: unregister module require delegates during runtime shutdown so in-memory generated assemblies and full compile-and-load modules can release their collectible load contexts after disposal.
- runtime/tests: keep the Array prototype built-ins on an immutable singleton template while using per-thread prototype overrides for engine mutations, with regression coverage for parallel engine-thread isolation and serial in-memory descriptor isolation.
- docs/sdk: rename the user-facing Hosting documentation to SDK documentation, keep the API/tutorial structure, and add coverage for the `JrocCompile` MSBuild task plus the in-memory compile-and-run APIs.
- test262/tests: switch the built-ins Array test slice to in-memory compilation/execution, including a collectible load-context unload regression for representative Array fixtures.
- runtime/tests/docs/test262: fix Object/String/Date compatibility gaps for Object constructor prototype relationships, Date `getFullYear`/`getMonth`, String coercion of arrays and `-0`, and string `NaN` index reads; add 5 new built-ins/Object and built-ins/String test262 ports.

## v0.10.1 - 2026-06-23

- runtime/tests/docs/test262: implement `Array.prototype.at` method for accessing array elements by index with support for negative indices, and add 10 new non-`eval` test262 ports covering basic usage, negative indices, parameter coercion, descriptor validation, and error handling.
- runtime/tests/docs/test262: harden `Object.fromEntries` iterator-closing semantics so abrupt entry/key/value failures close iterators correctly without overwriting the original throw completion, and add 5 new non-`eval` Object.fromEntries iterator-edge test262 ports.
- runtime/tests/docs/test262: align `Object()` / `Object(null)` / `Object(undefined)` callable semantics with ordinary-object creation so the new built-ins/Object constructor ports inherit `Object.prototype` methods and match `new Object(...)`.
- runtime/tests/docs/test262: unskip the remaining non-`eval` global-object/value-property ports, mirroring top-level `var` bindings onto `globalThis` and enforcing strict assignment errors for `NaN`/`undefined`.
- runtime/tests/docs/test262: implement `Number.MAX_SAFE_INTEGER` and `Number.MIN_SAFE_INTEGER` constructor properties and unskip the new non-`eval` Number constructor/value-property ports (`S15.7.1.1_A1`, `MAX_SAFE_INTEGER`, `MIN_SAFE_INTEGER`).
- runtime/tests/docs/test262: align Math value-property descriptors by exposing constant data properties (`E`, `LN10`, `LN2`, `LOG10E`, `LOG2E`, `PI`, `SQRT1_2`, `SQRT2`) and add the new non-`eval` `prop-desc` ports for `E`, `LN10`, and `LN2`.
- runtime/tests/docs/test262: align non-writable `@@toStringTag` descriptors for Math/JSON/Reflect and add the new non-`eval` `Symbol.toStringTag` ports for those built-ins.

## v0.10.0 - 2026-06-21




## v0.9.32 - 2026-06-20

- runtime/tests/docs: add `Promise.try` support, including constructor-aware capability creation, callback invocation with forwarded arguments, and Promise resolve/reject wiring; enable the new non-`eval` test262 `Promise.try` metadata ports and refresh ECMA-262 Promise docs.
- runtime/tests/docs/test262: improve `JSON.stringify` support for string `space` gap formatting plus replacer/toJSON return-value serialization, enabling the newly ported non-`eval` JSON.stringify test262 cases and refreshing ECMA-262 JSON docs.

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
