# Node Gap Popularity Backlog

> **Last Updated**: 2026-03-15
> Purpose: Persist a holistic, popularity-weighted view of the highest-value remaining Node.js gaps so triage context is not lost between sessions.
> Scope: Node.js compatibility first, with adjacent ECMA and runtime work called out when they directly block common Node workloads.
> Active review item: PR [#897](https://github.com/tomacox74/js2il/pull/897) is now implementing [#871](https://github.com/tomacox74/js2il/issues/871), so [#870](https://github.com/tomacox74/js2il/issues/870) is the next highest-value remaining networking follow-on.

## Inputs Used

- Current support inventory: `docs/nodejs/Index.md`
- Stated runtime limitations: `docs/nodejs/NodeLimitations.json`
- Module-level coverage: `docs/nodejs/*.json`
- Runtime module footprint: `src/JavaScriptRuntime/Node/*` and `src/JavaScriptRuntime/CommonJS/*`
- Repo-local demand signals: `Js2IL.Tests/Node/**/*`, `Js2IL.Tests/CommonJS/**/*`, and `Js2IL.Tests/Import/**/*`

## Current Baseline (Snapshot)

- Node docs currently track **17 modules** (**13 `partial`**, **2 `completed`**, **2 `not-supported`**) and **14 globals** (**14 `supported`**).
- Recent work already landed enough that these items no longer belong at the top of the remaining backlog:
  - Node ESM loader/package-resolution parity issue [#869](https://github.com/tomacox74/js2il/issues/869)
  - Stream lifecycle/helper parity issue [#872](https://github.com/tomacox74/js2il/issues/872)
  - Net socket parity and binary data handling issue [#874](https://github.com/tomacox74/js2il/issues/874)
  - `path`, `fs`, `util`, `child_process`, `url`, `querystring`, and loopback `net` / `http` baselines
- The most important documented blockers now are:
  - `https` and `tls` are explicit diagnostic-only stubs.
  - `http` now has streamed Buffer-based `IncomingMessage` bodies, chunked request/response framing, forwarded server `connection` events, and a sequential keep-alive `http.Agent` baseline, but it still lacks secure transport and advanced protocol features.
  - `net` now has Buffer-by-default socket reads, utf8 `setEncoding()`, idle `setTimeout()`, keepAlive enable/disable, and `allowHalfOpen` delayed-response behavior, but it remains IPv4 loopback-focused and still lacks broader socket-option parity.
  - `stream` now has pause()/resume(), UTF-8 setEncoding(), destroy()/destroyed, and callback-style pipeline()/finished(), but object mode, richer write callbacks, `node:stream/promises`, binary encodings, and broader teardown/buffering edge cases remain.
  - `crypto` is limited to hashing and secure-random helpers.

## Repo-local Demand Signals

- Node test coverage is now heaviest around foundations that are already in use: `fs` (**33** JS fixtures), `stream` (**15**), timers (**14**), `path` (**13**), `util` (**13**), and `process` (**11**).
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
| 1 | [TLS and HTTPS support](https://github.com/tomacox74/js2il/issues/870) | `tls`, `https` | [#870](https://github.com/tomacox74/js2il/issues/870) | `not-supported` | Secure networking is still completely absent, which blocks common SDKs, package clients, webhook consumers, and real-world service integrations. |
| 2 | [Practical crypto expansion](https://github.com/tomacox74/js2il/issues/790) | `crypto`, `webcrypto` | [#790](https://github.com/tomacox74/js2il/issues/790) | `partial` | Hashing and random bytes are no longer the main gap; modern auth, signing, and secure protocol code needs a broader cryptographic surface. |
| 3 | [Advanced file-system handles and stream APIs](https://github.com/tomacox74/js2il/issues/873) | `fs`, `fs/promises` | [#873](https://github.com/tomacox74/js2il/issues/873) | `partial` | Whole-file APIs exist, but package managers, bundlers, and tooling often need `open`, FileHandle, append/rename/unlink, and file streams. |
| 4 | [Promise-based timers and Abort-aware timer helpers](https://github.com/tomacox74/js2il/issues/875) | `timers/promises` | [#875](https://github.com/tomacox74/js2il/issues/875) | Not yet tracked in `docs/nodejs` | Timer globals are already solid and heavily exercised; the promise module is a high-value next layer for modern async Node code. |
| 5 | [Compression support](https://github.com/tomacox74/js2il/issues/876) | `zlib` | [#876](https://github.com/tomacox74/js2il/issues/876) | Not yet tracked in `docs/nodejs` | Compression is a practical missing piece for HTTP interoperability, packaging flows, and many real Node dependencies. |
| 6 | [Advanced child-process IPC and process-control parity](https://github.com/tomacox74/js2il/issues/877) | `child_process` | [#877](https://github.com/tomacox74/js2il/issues/877) | `partial` | The current spawn/exec baseline is useful, but many toolchains still need `fork()`, richer stdio semantics, IPC, and stronger signal/env behavior. |

## Linked Issue Briefs

## 1. HTTP Parity Beyond the Current Loopback Baseline ([#871](https://github.com/tomacox74/js2il/issues/871))

- Current signal:
  - PR [#897](https://github.com/tomacox74/js2il/pull/897) now implements streamed Buffer-based `IncomingMessage` bodies, chunked transfer encoding/decoding, forwarded server `connection` events, and a sequential keep-alive `http.Agent` baseline.
- Remaining follow-on after this slice:
  - Move to [#870](https://github.com/tomacox74/js2il/issues/870) for secure transport (`https` / `tls`) and keep advanced HTTP features such as pipelining, upgrades, and HTTP/2 explicitly out of scope.

## 2. TLS and HTTPS Support ([#870](https://github.com/tomacox74/js2il/issues/870))

- Current signal:
  - `docs/nodejs/https.json` and `docs/nodejs/tls.json` are explicit throw-only stubs today.
- Minimum acceptance:
  - Add practical client and loopback-server baselines for `https.request()`, `https.get()`, and TLS-backed sockets.
  - Support common certificate/key option shapes and return explicit diagnostics for deferred advanced TLS features.

## 3. Practical Crypto Expansion ([#790](https://github.com/tomacox74/js2il/issues/790))

- Current signal:
  - `docs/nodejs/crypto.json` supports `createHash`, `randomBytes`, and `getRandomValues`, but not the broader crypto surface real apps use.
- Minimum acceptance:
  - Add a practical next slice such as `createHmac`, key-derivation APIs, signing/verification, or an initial `webcrypto.subtle` digest/import/sign subset.
  - Keep the slice explicitly documented and test-backed rather than exposing broad placeholders with silent gaps.

## 4. Advanced File-system Handles and Stream APIs ([#873](https://github.com/tomacox74/js2il/issues/873))

- Current signal:
  - `fs` and `fs/promises` are good for whole-file workflows, but there is still no broad file-descriptor, FileHandle, or file-stream story.
- Minimum acceptance:
  - Add `open()` / FileHandle baselines and `createReadStream()` / `createWriteStream()` so package tooling can work incrementally.
  - Expand practical mutation APIs such as `appendFile`, `rename`, `unlink`, and basic link support with Node-like error behavior.

## 5. Promise-based Timers and Abort-aware Timer Helpers ([#875](https://github.com/tomacox74/js2il/issues/875))

- Current signal:
  - Timer globals are supported and well-tested, but the `timers/promises` module is not part of the current Node docs inventory.
- Minimum acceptance:
  - Add `setTimeout`, `setImmediate`, and `setInterval` promise helpers with the cancellation semantics Node developers expect.
  - Cover ordering and interaction with `process.nextTick`, Promise microtasks, and existing timer globals.

## 6. Compression Support ([#876](https://github.com/tomacox74/js2il/issues/876))

- Current signal:
  - There is currently no tracked `zlib` surface even though HTTP and tooling scenarios will keep encountering compression requirements.
- Minimum acceptance:
  - Add a practical gzip/deflate baseline for common synchronous or streaming workflows.
  - Ensure the initial slice composes cleanly with the HTTP and stream work rather than living as a disconnected utility.

## 7. Advanced Child-process IPC and Process-control Parity ([#877](https://github.com/tomacox74/js2il/issues/877))

- Current signal:
  - `docs/nodejs/child_process.json` now covers `spawn`, `exec`, `execFile`, and sync variants, but not `fork()`, IPC, or richer stdio/process semantics.
- Minimum acceptance:
  - Add `fork()` and a basic parent/child message channel.
  - Improve stdio, signal, and environment behavior enough for common build tools and test-runner patterns.

## Recommended Sequencing

- **Track A (application networking):** [#871](https://github.com/tomacox74/js2il/issues/871) (active review via PR [#897](https://github.com/tomacox74/js2il/pull/897)) -> [#870](https://github.com/tomacox74/js2il/issues/870)
- **Track B (I/O and file primitives):** [#873](https://github.com/tomacox74/js2il/issues/873)
- **Track C (platform APIs):** [#790](https://github.com/tomacox74/js2il/issues/790), [#875](https://github.com/tomacox74/js2il/issues/875), [#876](https://github.com/tomacox74/js2il/issues/876), and [#877](https://github.com/tomacox74/js2il/issues/877)

This ordering keeps shared infrastructure (`stream`, `net`) ahead of the higher-level modules that depend on it.

## Gate for Each Delivered Item

- Add execution tests and generator tests where applicable.
- Update the relevant `docs/nodejs/*.json` source files.
- Regenerate the Node docs (`node scripts/generateNodeIndex.js` and `node scripts/generateNodeModuleDocs.js`, or the broader convenience script when the legacy split source is also being updated).
- Update `CHANGELOG.md` when behavior changes are user-visible.

## Risks / Caveats

- This ranking is deliberately heuristic. It reflects the current docs plus repo-local test demand, not external npm telemetry.
- Some items are feature families rather than single APIs. Each should still be delivered in explicit, documented slices.
- `https`, `http`, `net`, `stream`, and `zlib` are tightly coupled; avoid starting a higher layer without the minimum lower-layer behavior it needs.
