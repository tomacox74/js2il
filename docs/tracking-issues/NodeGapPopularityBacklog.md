# Node Gap Popularity Backlog

> **Last Updated**: 2026-03-12  
> Purpose: Persist a holistic, popularity-weighted view of missing functionality so triage context is not lost between sessions.
> Scope: Node.js compatibility first, with adjacent ECMA impacts called out where they block common Node workloads.

## Inputs Used

- Current support inventory: `docs/nodejs/Index.md`
- Stated limitations: `docs/nodejs/NodeLimitations.json`
- Module-level coverage: `docs/nodejs/*.json`
- Runtime module footprint: `JavaScriptRuntime/Node/*`
- Internal demand signals: `require(...)` usage across `Js2IL.Tests/**/*.js` and `samples/**/*.js`
- ECMA tracking breadth: `docs/ECMA262/Index.md`

## Current Baseline (Snapshot)

- Node docs track **17 modules** (**13 `partial`**, **2 `completed`**, **2 `not-supported`**) and **14 globals** (**14 `supported`**, **0 `partial`**).
- Major blockers currently documented:
  - CommonJS `require(id)` supports compile-time `node_modules` package resolution via `package.json` `main` and a minimal `exports` subset, but the runtime does not probe the filesystem (packages must be discovered at compile time).
  - Node ESM loader semantics remain partial (`.mjs`, `type: module`, and broader package `exports` / `imports` handling), although `import.meta.url` is now available for compiled modules as a deterministic `file://` URL.

## Ranking Criteria

- Ecosystem unblock impact (transitive dependency reach)
- Local demand signals (tests/samples)
- Dependency leverage (enables multiple later modules)
- Testability / ability to ship in slices

## Top 10 Backlog (Recommended Order)

| Rank | Backlog item | Primary Node area | Current status signal |
|---:|---|---|---|
| 1 | [Expand CommonJS `require()` resolution (`node_modules`, `package.json` `main`/`exports`)](https://github.com/tomacox74/js2il/issues/783) | CommonJS loader | Merged (PR #798) |
| 2 | [Expand `path` module parity (normalize/parse/format/extname/isAbsolute, posix/win32)](https://github.com/tomacox74/js2il/issues/784) | `path` | Merged (PR #797) |
| 3 | [Expand `fs` module parity (callbacks, buffers, mkdir/copyFile/readFile/writeFile)](https://github.com/tomacox74/js2il/issues/785) | `fs` | Merged (PR #800) |
| 4 | [Expand `stream` module (Duplex/Transform/PassThrough + basic backpressure)](https://github.com/tomacox74/js2il/issues/786) | `stream` | In review (PR #803) |
| 5 | [Expand `util` essentials (format, inspect parity, util.types breadth)](https://github.com/tomacox74/js2il/issues/787) | `util` | Merged (PR #826) |
| 6 | [Expand `child_process` beyond sync (spawn/exec/execFile, stdio pipes)](https://github.com/tomacox74/js2il/issues/788) | `child_process` | Merged (PR #827) |
| 7 | [Implement `url`/`querystring` baseline (URL, URLSearchParams, parse/stringify)](https://github.com/tomacox74/js2il/issues/789) | `url`, `querystring` | Merged (PR #828) |
| 8 | [Implement `crypto` minimum practical subset (createHash, randomBytes, webcrypto bridge)](https://github.com/tomacox74/js2il/issues/790) | `crypto` | In progress on `copilot/gh-790-crypto-minimum-subset` |
| 9 | [Add ESM interop baseline (`import.meta.url` + Node-style ESM resolution plan)](https://github.com/tomacox74/js2il/issues/791) | ESM loader/interop | In review (PR #839) |
| 10 | [Add `http`/`https`/`net`/`tls` baseline plan (client/server skeleton)](https://github.com/tomacox74/js2il/issues/792) | networking | In review (PR #840) |

## Issue-ready Backlog Stubs

## Issue 1: Expand CommonJS `require()` Resolution ([#783](https://github.com/tomacox74/js2il/issues/783))
- Suggested labels: `enhancement`, `modules`, `commonjs`, `priority:high`
- Status: merged (PR #798): https://github.com/tomacox74/js2il/pull/798
- Minimum acceptance:
  - `require('pkg')` resolves via `node_modules` discovery and `package.json` (`main` + minimal `exports` subset)
  - Deterministic caching and Node-like diagnostics for missing packages

## Issue 2: Expand `path` Module Parity ([#784](https://github.com/tomacox74/js2il/issues/784))
- Suggested labels: `enhancement`, `modules`, `priority:high`
- Status: merged (PR #797): https://github.com/tomacox74/js2il/pull/797
- Minimum acceptance:
  - Add normalize/parse/format/extname/isAbsolute + basic posix/win32 shape
  - Execution + generator tests for common edge cases

## Issue 3: Expand `fs` Module Parity ([#785](https://github.com/tomacox74/js2il/issues/785))
- Suggested labels: `enhancement`, `modules`, `priority:high`
- Status: merged (PR #800): https://github.com/tomacox74/js2il/pull/800
  - callback-style async APIs + execution coverage: `readFile`, `writeFile`, `copyFile`, `readdir`, `mkdir`, `stat`, `rm`, `access`, `realpath`
- Minimum acceptance:
  - Buffer + callback baselines for read/write workflows
  - Tests + docs updates for newly supported APIs

## Issue 4: Expand `stream` Module Core Classes ([#786](https://github.com/tomacox74/js2il/issues/786))
- Suggested labels: `enhancement`, `modules`, `priority:high`
- Status: in review (PR #803): https://github.com/tomacox74/js2il/pull/803
- Minimum acceptance:
  - Minimal Duplex/Transform/PassThrough and basic backpressure signaling
  - Tests covering Readable->Transform->Writable pipelines

## Issue 5: Expand `util` Essentials ([#787](https://github.com/tomacox74/js2il/issues/787))
- Suggested labels: `enhancement`, `modules`, `priority:medium`
- Status: merged (PR #826): https://github.com/tomacox74/js2il/pull/826
- Minimum acceptance:
  - `util.format` baseline + improved `inspect` parity in common cases
  - Expand `util.types` checks as runtime support allows

## Issue 6: Expand `child_process` Beyond Sync ([#788](https://github.com/tomacox74/js2il/issues/788))
- Suggested labels: `enhancement`, `modules`, `priority:medium`
- Status: merged (PR #827): https://github.com/tomacox74/js2il/pull/827
- Minimum acceptance:
  - Minimal async spawn with deterministic stdout capture

## Issue 7: Implement `url` / `querystring` Baseline ([#789](https://github.com/tomacox74/js2il/issues/789))
- Suggested labels: `enhancement`, `modules`, `priority:medium`
- Status: merged (PR #828): https://github.com/tomacox74/js2il/pull/828
- Minimum acceptance:
  - URL parsing + URLSearchParams basics
  - querystring parse/stringify basics

## Issue 8: Implement `crypto` Minimum Subset ([#790](https://github.com/tomacox74/js2il/issues/790))
- Suggested labels: `enhancement`, `modules`, `priority:medium`
- Status: in progress on `copilot/gh-790-crypto-minimum-subset`
- Minimum acceptance:
  - createHash('sha256') + randomBytes returning Buffer
  - minimal `crypto.webcrypto.getRandomValues(...)` bridge for supported buffer / typed-array shapes

## Issue 9: Add Node ESM Interop Baseline ([#791](https://github.com/tomacox74/js2il/issues/791))
- Suggested labels: `enhancement`, `modules`, `commonjs`, `priority:low`
- Status: in review (PR #839): https://github.com/tomacox74/js2il/pull/839
- Minimum acceptance:
  - `import.meta.url` exists for compiled modules with deterministic `file://` semantics and clear, documented constraints
  - Note: deeper ECMA-262 ESM semantics are tracked separately (see [#772](https://github.com/tomacox74/js2il/issues/772))

## Issue 10: Add HTTP Stack Baseline Plan ([#792](https://github.com/tomacox74/js2il/issues/792))
- Suggested labels: `enhancement`, `modules`, `priority:low`
- Status: in review (PR #840): https://github.com/tomacox74/js2il/pull/840
- Minimum acceptance:
  - Minimal `node:net` loopback support via `createServer`, `connect`/`createConnection`, and EventEmitter-backed `Server` / `Socket` lifecycles
  - Minimal `node:http` loopback support via `createServer`, `request`, `get`, `IncomingMessage`, and `ServerResponse`
  - Explicit `node:https` / `node:tls` diagnostics and docs for deferred TLS functionality

## Internal Demand Signals (Repo-local)

Observed frequently in tests/samples and therefore good near-term ROI:

- `path`, `fs`, `process`, timers, `perf_hooks`, and `events` are actively exercised.
- Integration tests reference `node:child_process`, `node:fs`, `node:path`, `node:os`.

## Recommended Sequencing (Execution)

- **Slice 1:** Issue 1 (require resolution) + Issue 2 (path)
- **Slice 2:** Issue 3 (fs) + Issue 4 (stream)

### Gate for each delivered item

- Add execution tests (+ generator tests where applicable)
- Update `docs/nodejs/*.json`
- Regenerate node markdown/index artifacts
- Note user-visible behavior in changelog when appropriate

## Risks / Caveats

- Popularity ranking combines ecosystem heuristics with local demand signals; refine weekly with fresh failing-fixture data.
- Large subsystems (`http`) should not start before stream + Buffer primitives stabilize.

## Ownership Handoff Notes

When resuming work, start from this file plus `TriageScoreboard.md`, then pick the highest unfinished item.
