# Node Gap Popularity Backlog

> **Last Updated**: 2026-03-16
> Purpose: Persist a holistic, popularity-weighted view of the highest-value remaining Node.js gaps so triage context is not lost between sessions.
> Scope: Node.js compatibility first, with adjacent ECMA and runtime work called out when they directly block common Node workloads.
> Active review item: PR [#905](https://github.com/tomacox74/js2il/pull/905) is implementing [#875](https://github.com/tomacox74/js2il/issues/875), so the next major remaining module gaps after that baseline are [#876](https://github.com/tomacox74/js2il/issues/876) and [#877](https://github.com/tomacox74/js2il/issues/877).

## Inputs Used

- Current support inventory: `docs/nodejs/Index.md`
- Stated runtime limitations: `docs/nodejs/NodeLimitations.json`
- Module-level coverage: `docs/nodejs/*.json`
- Runtime module footprint: `src/JavaScriptRuntime/Node/*` and `src/JavaScriptRuntime/CommonJS/*`
- Repo-local demand signals: `Js2IL.Tests/Node/**/*`, `Js2IL.Tests/CommonJS/**/*`, and `Js2IL.Tests/Import/**/*`

## Current Baseline (Snapshot)

- Node docs now track **18 modules** (**16 `partial`**, **2 `completed`**, **0 `not-supported`**) and **14 globals** (**14 `supported`**).
- Recent work already landed enough that these items no longer belong at the top of the remaining backlog:
  - Node ESM loader/package-resolution parity issue [#869](https://github.com/tomacox74/js2il/issues/869)
  - Stream lifecycle/helper parity issue [#872](https://github.com/tomacox74/js2il/issues/872)
  - Net socket parity and binary data handling issue [#874](https://github.com/tomacox74/js2il/issues/874)
  - HTTP parity issue [#871](https://github.com/tomacox74/js2il/issues/871)
  - TLS/HTTPS baseline issue [#870](https://github.com/tomacox74/js2il/issues/870)
  - Practical crypto expansion issue [#790](https://github.com/tomacox74/js2il/issues/790)
  - FileHandle and file-stream issue [#873](https://github.com/tomacox74/js2il/issues/873)
  - `timers/promises` is in active review for a documented Promise-based baseline via PR [#905](https://github.com/tomacox74/js2il/pull/905).
- The most important documented blockers now are:
  - `node:zlib` is still completely absent from the docs/runtime inventory.
  - `child_process` still lacks `fork()`, IPC channels, and richer process-control semantics.
  - The `timers/promises` baseline deliberately defers the async-iterator `setInterval(...)` contract and broader scheduler parity beyond the one-shot helpers.

## Repo-local Demand Signals

- Node test coverage remains heaviest around already-landed foundations: `fs`, `stream`, timers, `path`, `util`, and `process`.
- Because the biggest "basic" modules now have baseline support, the next backlog should optimize for **ecosystem unblock value** rather than revisiting already-landed minimum slices.

## Ranking Criteria

- Ecosystem unblock impact (how many modern packages or app patterns the feature opens up)
- Dependency leverage (whether it unlocks multiple later modules or runtimes)
- Current implementation gap size (how partial vs absent the current state is)
- Repo-local demand signals (existing tests and nearby runtime code)
- Ability to ship in slices with clear user-visible value

## Refreshed Remaining Backlog (Recommended Order)

| Rank | Feature family | Primary Node area | GitHub issue | Current status signal | Why it moved up now |
|---:|---|---|---|---|---|
| 1 | [Compression support](https://github.com/tomacox74/js2il/issues/876) | `zlib` | [#876](https://github.com/tomacox74/js2il/issues/876) | Not yet tracked in `docs/nodejs` | Compression is a practical missing piece for HTTP interoperability, packaging flows, and many real Node dependencies. |
| 2 | [Advanced child-process IPC and process-control parity](https://github.com/tomacox74/js2il/issues/877) | `child_process` | [#877](https://github.com/tomacox74/js2il/issues/877) | `partial` | The current spawn/exec baseline is useful, but many toolchains still need `fork()`, richer stdio semantics, IPC, and stronger signal/env behavior. |

## In-flight Review Item

## 1. Promise-based Timers and Abort-aware Timer Helpers ([#875](https://github.com/tomacox74/js2il/issues/875))

- Current signal:
  - PR [#905](https://github.com/tomacox74/js2il/pull/905) adds a practical `node:timers/promises` baseline for Promise-based `setTimeout(...)` / `setImmediate(...)`, abort-aware one-shot cancellation, and focused ordering coverage.
- Explicit deferred areas:
  - The async-iterator `setInterval(...)` contract remains intentionally deferred for now and should stay explicit in both diagnostics and docs.

## Linked Issue Briefs

## 2. Compression Support ([#876](https://github.com/tomacox74/js2il/issues/876))

- Current signal:
  - There is currently no tracked `zlib` surface even though HTTP and tooling scenarios will keep encountering compression requirements.
- Minimum acceptance:
  - Add a practical gzip/deflate baseline for common synchronous or streaming workflows.
  - Ensure the initial slice composes cleanly with the HTTP and stream work rather than living as a disconnected utility.

## 3. Advanced Child-process IPC and Process-control Parity ([#877](https://github.com/tomacox74/js2il/issues/877))

- Current signal:
  - `docs/nodejs/child_process.json` now covers `spawn`, `exec`, `execFile`, and sync variants, but not `fork()`, IPC, or richer stdio/process semantics.
- Minimum acceptance:
  - Add `fork()` and a basic parent/child message channel.
  - Improve stdio, signal, and environment behavior enough for common build tools and test-runner patterns.

## Recommended Sequencing

- **Finish the in-flight timer baseline first:** [#875](https://github.com/tomacox74/js2il/issues/875) via PR [#905](https://github.com/tomacox74/js2il/pull/905)
- **Then deliver compression:** [#876](https://github.com/tomacox74/js2il/issues/876)
- **Then deepen child-process parity:** [#877](https://github.com/tomacox74/js2il/issues/877)

This ordering keeps the highest-value missing module (`zlib`) ahead of the deeper process-control follow-on while preserving the current in-flight timers work.

## Gate for Each Delivered Item

- Add execution tests and generator tests where applicable.
- Update the relevant `docs/nodejs/*.json` source files.
- Regenerate the Node docs (`node scripts/generateNodeIndex.js` and `node scripts/generateNodeModuleDocs.js`, or the broader convenience script when the legacy split source is also being updated).
- Update `CHANGELOG.md` when behavior changes are user-visible.

## Risks / Caveats

- This ranking is deliberately heuristic. It reflects the current docs plus repo-local test demand, not external npm telemetry.
- Some items are feature families rather than single APIs. Each should still be delivered in explicit, documented slices.
- `https`, `http`, `net`, `stream`, and `zlib` are tightly coupled; avoid starting a higher layer without the minimum lower-layer behavior it needs.
