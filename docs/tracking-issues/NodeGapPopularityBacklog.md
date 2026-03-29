# Node Gap Popularity Backlog

> **Last Updated**: 2026-03-16
> Purpose: Persist a holistic, popularity-weighted view of the highest-value remaining Node.js gaps so triage context is not lost between sessions.
> Scope: Node.js compatibility first, with adjacent ECMA and runtime work called out when they directly block common Node workloads.
> Active review item: PR [#907](https://github.com/tomacox74/js2il/pull/907) is implementing [#877](https://github.com/tomacox74/js2il/issues/877), so the next major process-control gap after the recent `zlib` landing is the remaining deferred `child_process` IPC/detached/handle-passing work beyond that supported slice.

## Inputs Used

- Current support inventory: `docs/nodejs/Index.md`
- Stated runtime limitations: `docs/nodejs/NodeLimitations.json`
- Module-level coverage: `docs/nodejs/*.json`
- Runtime module footprint: `src/JavaScriptRuntime/Node/*` and `src/JavaScriptRuntime/CommonJS/*`
- Repo-local demand signals: `Js2IL.Tests/Node/**/*`, `Js2IL.Tests/CommonJS/**/*`, and `Js2IL.Tests/Import/**/*`

## Current Baseline (Snapshot)

- Node docs now track **19 modules** (**17 `partial`**, **2 `completed`**, **0 `not-supported`**) and **14 globals** (**14 `supported`**).
- Recent work already landed enough that these items no longer belong at the top of the remaining backlog:
  - Node ESM loader/package-resolution parity issue [#869](https://github.com/tomacox74/js2il/issues/869)
  - Stream lifecycle/helper parity issue [#872](https://github.com/tomacox74/js2il/issues/872)
  - Net socket parity and binary data handling issue [#874](https://github.com/tomacox74/js2il/issues/874)
  - HTTP parity issue [#871](https://github.com/tomacox74/js2il/issues/871)
  - TLS/HTTPS baseline issue [#870](https://github.com/tomacox74/js2il/issues/870)
  - Practical crypto expansion issue [#790](https://github.com/tomacox74/js2il/issues/790)
  - FileHandle and file-stream issue [#873](https://github.com/tomacox74/js2il/issues/873)
  - Promise-based timers issue [#875](https://github.com/tomacox74/js2il/issues/875)
  - Compression support issue [#876](https://github.com/tomacox74/js2il/issues/876)
- The most important documented blockers now are:
  - `child_process` on `master` still lacks the new fork/IPC/process-control baseline, although PR [#907](https://github.com/tomacox74/js2il/pull/907) is actively closing that gap.
  - The new `timers/promises` baseline still deliberately defers the async-iterator `setInterval(...)` contract and broader scheduler parity beyond the one-shot helpers.
  - Even after PR [#907](https://github.com/tomacox74/js2il/pull/907), deeper `child_process` parity such as detached lifecycle management, handle passing, advanced serialization, and Node-internal IPC semantics will remain explicit follow-on work.

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
| 1 | [Advanced child-process IPC and process-control parity](https://github.com/tomacox74/js2il/issues/877) | `child_process` | [#877](https://github.com/tomacox74/js2il/issues/877) | In active review via PR [#907](https://github.com/tomacox74/js2il/pull/907) | The current spawn/exec baseline is useful, but many toolchains still need `fork()`, richer stdio semantics, IPC, and stronger signal/env behavior. |

## In-flight Review Item

## 1. Advanced Child-process IPC and Process-control Parity ([#877](https://github.com/tomacox74/js2il/issues/877))

- Current signal:
  - PR [#907](https://github.com/tomacox74/js2il/pull/907) adds a documented `fork()` baseline for compiled child modules plus JSON-only IPC, environment overlays, supported stdio shapes, and clearer kill/exit reporting.
- Explicit deferred areas:
  - Detached child lifecycle management, handle passing, advanced/non-JSON serialization, cluster-style integration, and deeper Node-internal IPC semantics should remain explicitly documented as follow-on work.

## Linked Issue Briefs

## Recommended Sequencing

- **Finish the in-flight child-process baseline first:** [#877](https://github.com/tomacox74/js2il/issues/877) via PR [#907](https://github.com/tomacox74/js2il/pull/907)
- **Then revisit deferred scheduler / IPC follow-ons:** especially the `timers/promises.setInterval(...)` async-iterator contract and deeper `child_process` detached / handle-passing work.

This ordering keeps the newly in-flight process-control work visible after the `zlib` landing while preserving explicit visibility into the deferred `timers/promises.setInterval(...)` async-iterator gap and deeper child-process IPC follow-ons.

## Gate for Each Delivered Item

- Add execution tests and generator tests where applicable.
- Update the relevant `docs/nodejs/*.json` source files.
- Regenerate the Node docs (`node scripts/generateNodeIndex.js` and `node scripts/generateNodeModuleDocs.js`, or the broader convenience script when the legacy split source is also being updated).
- Update `CHANGELOG.md` when behavior changes are user-visible.

## Risks / Caveats

- This ranking is deliberately heuristic. It reflects the current docs plus repo-local test demand, not external npm telemetry.
- Some items are feature families rather than single APIs. Each should still be delivered in explicit, documented slices.
- `https`, `http`, `net`, `stream`, and `zlib` are tightly coupled; avoid starting a higher layer without the minimum lower-layer behavior it needs.
