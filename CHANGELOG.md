# Changelog

All notable changes to this project are documented here.

For older release lines, browse [`docs/archive/changelog/Index.md`](docs/archive/changelog/Index.md). `CHANGELOG.md` remains the active changelog file.

## Unreleased

_Nothing yet._

## v0.9.18 - 2026-05-16

- tooling/perf/docs: expand the BenchmarkDotNet scenario catalog, make phased js2il benchmark failures surface explicitly instead of being skipped, and make benchmark runs exit non-zero when any benchmark case fails so broken perf scripts are not misreported as successful timings.
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

- compiler/cli/sdk/tests/docs: remove the top-level `"use strict"` requirement and its configurable warning/error flag so JS2IL accepts both strict and non-strict scripts by default.
- tests/docs: port 400 additional crosscutting `test262` cases into `tests/Js2IL.Test262.Tests`, keep currently unsupported/failing cases checked in as skipped coverage, sync the linked ECMA-262 evidence docs for the newly strengthened areas, and remove stray PR-1010 function snapshot files that should never have been added.
- compiler/runtime/tests/test262: follow PR #1043 by fixing and unskipping 138 previously skipped `test262` ports across arrays/functions, JSON/string/object built-ins, control-flow/binding semantics, and class/expression semantics, reducing the remaining checked-in skip count to 135.

## v0.9.13 - 2026-05-04

- compiler/tests: infer stable primitive parameter types (`number`, `boolean`, and `string`) for direct class method calls when all visible call sites agree, allowing generated IL to use typed parameters and direct primitive operations while keeping delegate-backed callables on the object ABI.

## v0.9.12 - 2026-05-04

- compiler/runtime/tests: fix the prime benchmark regression by restoring direct `Int32Array` numeric-index hot paths (including boxed numeric indexes) instead of falling back to generic item helpers, repairing the corresponding inline typed-array IL emission for verifier-safe direct reads, and adding a boxed-double `TypeUtilities.ToNumber(...)` fast path to reduce coercion overhead in hot loops.

## v0.9.11 - 2026-05-03

- compiler/runtime/tests/test262: remove the remaining skipped representative `test262` execution ports by fixing block/function scope materialization, hosted global service resolution, sloppy assignment and unary lowering edge cases, and several built-in/runtime semantics gaps (including `Object.values`, `Number.isInteger`, `parseInt`, `isNaN`, `isFinite`, `Array.isArray`, loose BigInt equality, and non-constructible intrinsics), bringing the checked-in `Js2IL.Test262.Tests` slice to 326 passing tests with 0 skips.

## v0.9.10 - 2026-04-30

- compiler/runtime/node/tests/docs: remove the remaining skipped `Js2IL.Test262.Tests` ports by fixing block/switch lexical-scope materialization and block-scope type visibility, constructor `_scopes` initialization/insertion rules, sloppy assignment `ThrowOnError` behavior, Number wrapper / `RegExp.prototype.toString` gaps, and hosted `child_process.fork()` IPC ordering; refresh the affected generator snapshots, add an on-demand Copilot skill for porting individual `test262` cases, and bring the checked-in `test262` slice to 121 passing tests with 0 skips.

## v0.9.9 - 2026-04-27

- compiler/packaging/tests: fix the v0.9.8 Linux and Windows release smoke regression by capturing module-local bindings that shadow runtime global intrinsic names (for example `URL`) when nested CommonJS function expressions close over them, restoring the packaged Hosting.Domino sample build.

## v0.9.8 - 2026-04-27

- runtime/spec/tests: align arrow-function restricted `caller` / `arguments` access by exposing `TypeError` globally and wiring function prototypes.
- tooling/tests/docs: close issue #934 and umbrella #927 by replacing the single post-MVP `test262` expansion bucket with a checked-in rollout plan plus ADR, creating dedicated follow-on issues for modules, async/Promise, raw+harness-heavy, agent/CanBlock, and Intl/environment-sensitive suites so future conformance work lands in bounded slices instead of one vague umbrella.
- tooling/tests/docs: close issue #933 by mapping bounded test262 MVP results back to ECMA-262 support docs and backlog ownership through a checked-in linkage manifest, annotating `summary.json` with clause/doc linkage, surfacing test262 evidence in the relevant ECMA-262 section docs, and documenting when linked failures should update docs vs attach to or create issues.
- tooling/tests/docs: close issue #932 by adding named bounded test262 MVP suites for PR vs nightly runs, exposing local npm entrypoints for those suites, publishing `summary.json` artifacts from a dedicated GitHub Actions workflow, and reusing the repository's scheduled-failure issue pattern for nightly regressions while extending focused runner integration coverage.
- tooling/tests/docs: close issue #931 by formalizing test262 MVP result kinds/verdicts, classifying parse/runtime negatives plus policy skips vs unsupported requirements, emitting a stable `summary.json` baseline artifact for each runner invocation, documenting the contract in a new ADR, and extending focused runner integration coverage.
- tooling/tests: close issue #930 by adding a plain synchronous script MVP test262 runner that resolves the pinned checkout, filters unsupported/module/async/agent cases, executes strict/non-strict variants plus parse/runtime negatives through js2il, emits per-test outcomes with repro commands, and adds focused runner integration coverage.
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

- packaging/hosting/samples/tests: support npm/module-id entrypoints in `Js2IL.SDK` with Node-style package resolution, keep `ModuleResolutionBaseDirectory` / `Js2ILModuleResolutionBaseDirectory` available only for non-default layouts, and simplify `Hosting.Domino` so the host project restores domino in place without a separate `compiler/` directory.
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
- docs/samples/workflows: close issue #850 and umbrella #439 by aligning the hosting sample docs and release smoke workflows with the `Js2IL.Runtime` package name and documenting the coordinated restore/build/post-publish validation matrix for the packaged hosting flow.
- node/esm/tests/docs: close issue #869 by resolving static ESM package requests with import-aware `package.json` `exports` / `imports` conditions, preserving require-mode package aliases for CommonJS parity, adding mixed CJS/ESM `node_modules` coverage plus resolver diagnostics, and refreshing the Node loader docs/triage snapshot (PR #888).

## v0.9.2 - 2026-03-14

- packaging/runtime/docs/tests: rename the runtime NuGet package from `JavaScriptRuntime` to `Js2IL.Runtime`, add first-class package metadata/readme coverage for its package page, and update the SDK samples/docs/workflow/package tests to reference the new package ID while keeping the runtime assembly name `JavaScriptRuntime.dll`.

## v0.9.1 - 2026-03-14

- packaging/hosting/tests/docs: add the new `Js2IL.SDK` NuGet package for issue #846, shipping an in-process MSBuild task plus `build\Js2IL.SDK.props` / `.targets` so host projects can declare `Js2ILCompile` items, compile JavaScript during `dotnet build`, and consume the generated module assembly without shelling out to the `js2il` tool.
- packaging/hosting/samples/docs: migrate the hosting samples for issue #847 from the legacy `compiler\*.proj` shell-out flow onto direct `Js2IL.SDK` usage, move packaged sample content from the `js2il` tool nupkg into the `Js2IL.SDK` nupkg, and clarify when to use `js2il`, `Js2IL.Core`, and `Js2IL.SDK`.
- release/tooling/docs: close issue #848 by teaching tagged release builds to pack and publish coordinated `JavaScriptRuntime`, `js2il`, `Js2IL.Core`, and `Js2IL.SDK` packages, adding a `npm run release:validate` gate that pairs packed-tool canaries with local SDK package-consumption tests, and staging `samples/Directory.Build.props` so release commits preserve aligned package versions.
- packaging/nuget/docs/tests: close issue #849 by expanding the packaged `Js2IL.Core` / `Js2IL.SDK` README copy, cross-linking the `js2il`, `Js2IL.Core`, and `Js2IL.SDK` package pages, adding package-project URLs/tags for discoverability, and asserting the shipped nuspec/readme metadata in focused package tests before first publish.
- release/tooling: fix `scripts/bumpVersion.js` so release cuts still update `samples/Directory.Build.props` after the shared `Js2ILPackageVersion` property gained a `Condition=...` attribute.

## v0.9.0 - 2026-03-13

- packaging/compiler/layout: implement issue #845 by extracting the reusable compiler into `Js2IL.Compiler.dll`, adding the referenceable `Js2IL.Core` package, wiring the `js2il` tool to consume the new compiler assembly while staying self-contained, splitting the repo layout into `src\Compiler\` sources plus a thin `src\Cli\` tool project, and updating solution/tests/workflows/scripts/samples to follow the new paths.
- release/tooling/workflows: strengthen pre-release confidence for the repackaged `js2il` tool by teaching version bumps to update `Js2IL.Core`, adding packaged-tool canary commands that pack and locally install `js2il` before running smoke suites, and wiring the release automation plus `canary-smoke` workflow to validate the actual nupkg instead of a source-run build.
- hosting/runtime/tests/docs: add issue #419 mutable CommonJS exports support by making typed and dynamic hosting exports proxies write through to `module.exports` on the owning script thread, adding focused hosting coverage for root-exports mutation, and updating the hosting docs to reflect the new read/write behavior.
- runtime/spec/tests/docs: complete the issue #728 bound-function baseline by tracking `Function.prototype.bind` metadata for `length` / `name`, routing `new` on bound constructors through target/new-target semantics and target prototypes, suppressing bound-function own `prototype`, adding focused Function execution/generator coverage, and refreshing ECMA-262 tracking.
- runtime/spec/tests/docs: make delegate-backed function `length`/`name` behave as descriptor-backed own properties for issue #727 so direct reads, `Object.getOwnPropertyDescriptor(...)`, and `Object.hasOwn(...)` share the same metadata path; add focused Function execution/generator coverage and refresh ECMA-262 §20.2 tracking.
- runtime/tests/docs: add the issue #791 Node ESM interop baseline by normalizing compiled-module `import.meta.url` to deterministic `file://` URLs, adding focused CommonJS/Import coverage for cross-module URL resolution, and refreshing the related Node and ECMA-262 tracking docs (PR #839).
- runtime/tests/docs: add the issue #792 networking baseline by implementing minimal `node:net` TCP loopback primitives (`createServer`, `connect`, `Socket`/`Server`) plus a narrow `node:http` HTTP/1.1 server/client slice (`createServer`, `request`, `get`, `IncomingMessage`, `ServerResponse`) with focused execution/generator coverage and explicit `node:https` / `node:tls` runtime diagnostics.
