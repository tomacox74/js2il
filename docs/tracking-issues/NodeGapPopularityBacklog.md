# Node Gap Popularity Backlog

> **Last Updated**: 2026-03-14
> Purpose: Persist a holistic, popularity-weighted view of the highest-value remaining Node.js gaps so triage context is not lost between sessions.
> Scope: Node.js compatibility first, with adjacent ECMA and runtime work called out when they directly block common Node workloads.
> Active review item: [#872](https://github.com/tomacox74/js2il/issues/872) is now in review via PR [#890](https://github.com/tomacox74/js2il/pull/890); this backlog describes the remaining follow-on queue once that stream slice lands.

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
  - Stream lifecycle/helper parity issue [#872](https://github.com/tomacox74/js2il/issues/872) once PR [#890](https://github.com/tomacox74/js2il/pull/890) merges
  - `path`, `fs`, `util`, `child_process`, `url`, `querystring`, and loopback `net` / `http` baselines
- The most important documented blockers now are:
  - `https` and `tls` are explicit diagnostic-only stubs.
  - `http` and `net` are still intentionally narrow baselines, not broad Node parity.
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
| 1 | [Net socket parity and binary data handling](https://github.com/tomacox74/js2il/issues/874) | `net` | [#874](https://github.com/tomacox74/js2il/issues/874) | `partial` | The new stream lifecycle/helper slice makes binary sockets, timeouts, and stronger lifecycle control the highest-value next I/O follow-on. |
| 2 | [HTTP parity beyond the current loopback baseline](https://github.com/tomacox74/js2il/issues/871) | `http` | [#871](https://github.com/tomacox74/js2il/issues/871) | `partial` | Current HTTP support is buffered, Content-Length-focused, and connection-close-based; many real clients need chunked transfer, keep-alive, and streaming bodies. |
| 3 | [TLS and HTTPS support](https://github.com/tomacox74/js2il/issues/870) | `tls`, `https` | [#870](https://github.com/tomacox74/js2il/issues/870) | `not-supported` | Secure networking is still completely absent, which blocks common SDKs, package clients, webhook consumers, and real-world service integrations. |
| 4 | [Practical crypto expansion](https://github.com/tomacox74/js2il/issues/790) | `crypto`, `webcrypto` | [#790](https://github.com/tomacox74/js2il/issues/790) | `partial` | Hashing and random bytes are no longer the main gap; modern auth, signing, and secure protocol code needs a broader cryptographic surface. |
| 5 | [Advanced file-system handles and stream APIs](https://github.com/tomacox74/js2il/issues/873) | `fs`, `fs/promises` | [#873](https://github.com/tomacox74/js2il/issues/873) | `partial` | Whole-file APIs exist, but package managers, bundlers, and tooling often need `open`, FileHandle, append/rename/unlink, and file streams. |
| 6 | [Promise-based timers and Abort-aware timer helpers](https://github.com/tomacox74/js2il/issues/875) | `timers/promises` | [#875](https://github.com/tomacox74/js2il/issues/875) | Not yet tracked in `docs/nodejs` | Timer globals are already solid and heavily exercised; the promise module is a high-value next layer for modern async Node code. |
| 7 | [Compression support](https://github.com/tomacox74/js2il/issues/876) | `zlib` | [#876](https://github.com/tomacox74/js2il/issues/876) | Not yet tracked in `docs/nodejs` | Compression is a practical missing piece for HTTP interoperability, packaging flows, and many real Node dependencies. |
| 8 | [Advanced child-process IPC and process-control parity](https://github.com/tomacox74/js2il/issues/877) | `child_process` | [#877](https://github.com/tomacox74/js2il/issues/877) | `partial` | The current spawn/exec baseline is useful, but many toolchains still need `fork()`, richer stdio semantics, IPC, and stronger signal/env behavior. |

## Linked Issue Briefs

## 1. Net Socket Parity and Binary Data Handling ([#874](https://github.com/tomacox74/js2il/issues/874))

- Current signal:
  - `docs/nodejs/net.json` still notes UTF-8 text chunks only plus missing broader lifecycle controls, timeouts, keep-alive, and binary framing.
- Minimum acceptance:
  - Deliver Buffer-based socket reads and the ability to opt into text decoding with `setEncoding()`.
  - Add timeout, keep-alive, and half-open lifecycle behavior needed by higher-level protocol stacks.

## 2. HTTP Parity Beyond the Current Loopback Baseline ([#871](https://github.com/tomacox74/js2il/issues/871))

- Current signal:
  - `docs/nodejs/http.json` still documents Content-Length framing, buffered bodies, and connection-close completion as the current slice.
- Minimum acceptance:
  - Stream request and response bodies incrementally instead of forcing a buffered-only path.
  - Add chunked transfer, keep-alive or `Agent` baseline behavior, and broader header/method handling expected by common clients.

## 3. TLS and HTTPS Support ([#870](https://github.com/tomacox74/js2il/issues/870))

- Current signal:
  - `docs/nodejs/https.json` and `docs/nodejs/tls.json` are explicit throw-only stubs today.
- Minimum acceptance:
  - Add practical client and loopback-server baselines for `https.request()`, `https.get()`, and TLS-backed sockets.
  - Support common certificate/key option shapes and return explicit diagnostics for deferred advanced TLS features.

## 4. Practical Crypto Expansion ([#790](https://github.com/tomacox74/js2il/issues/790))

- Current signal:
  - `docs/nodejs/crypto.json` supports `createHash`, `randomBytes`, and `getRandomValues`, but not the broader crypto surface real apps use.
- Minimum acceptance:
  - Add a practical next slice such as `createHmac`, key-derivation APIs, signing/verification, or an initial `webcrypto.subtle` digest/import/sign subset.
  - Keep the slice explicitly documented and test-backed rather than exposing broad placeholders with silent gaps.

## 5. Advanced File-system Handles and Stream APIs ([#873](https://github.com/tomacox74/js2il/issues/873))

- Current signal:
  - `fs` and `fs/promises` are good for whole-file workflows, but there is still no broad file-descriptor, FileHandle, or file-stream story.
- Minimum acceptance:
  - Add `open()` / FileHandle baselines and `createReadStream()` / `createWriteStream()` so package tooling can work incrementally.
  - Expand practical mutation APIs such as `appendFile`, `rename`, `unlink`, and basic link support with Node-like error behavior.

## 6. Promise-based Timers and Abort-aware Timer Helpers ([#875](https://github.com/tomacox74/js2il/issues/875))

- Current signal:
  - Timer globals are supported and well-tested, but the `timers/promises` module is not part of the current Node docs inventory.
- Minimum acceptance:
  - Add `setTimeout`, `setImmediate`, and `setInterval` promise helpers with the cancellation semantics Node developers expect.
  - Cover ordering and interaction with `process.nextTick`, Promise microtasks, and existing timer globals.

## 7. Compression Support ([#876](https://github.com/tomacox74/js2il/issues/876))

- Current signal:
  - There is currently no tracked `zlib` surface even though HTTP and tooling scenarios will keep encountering compression requirements.
- Minimum acceptance:
  - Add a practical gzip/deflate baseline for common synchronous or streaming workflows.
  - Ensure the initial slice composes cleanly with the HTTP and stream work rather than living as a disconnected utility.

## 8. Advanced Child-process IPC and Process-control Parity ([#877](https://github.com/tomacox74/js2il/issues/877))

- Current signal:
  - `docs/nodejs/child_process.json` now covers `spawn`, `exec`, `execFile`, and sync variants, but not `fork()`, IPC, or richer stdio/process semantics.
- Minimum acceptance:
  - Add `fork()` and a basic parent/child message channel.
  - Improve stdio, signal, and environment behavior enough for common build tools and test-runner patterns.

## Recommended Sequencing

- **Track A (I/O substrate):** Items 1 and 5
- **Track B (application networking):** Items 2 and 3
- **Track C (platform APIs):** Items 4, 6, 7, and 8

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
