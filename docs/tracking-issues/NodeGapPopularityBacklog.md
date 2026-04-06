# Node Gap Popularity Backlog

> **Last Updated**: 2026-04-06
> Purpose: Persist a holistic, popularity-weighted view of the highest-value remaining Node.js gaps so triage context is not lost between sessions.
> Scope: Node.js compatibility first, with adjacent web/runtime work called out when it directly blocks common Node workloads.
> Active review item: none. This ranking assumes the recent child-process, networking, stream, fs, timers/promises, and `node:url` baselines are already on `master`.

## Inputs Used

- Current support inventory: `docs/nodejs/Index.md`
- Stated runtime limitations: `docs/nodejs/NodeLimitations.json`
- Module-level coverage: `docs/nodejs/*.json`
- Runtime module footprint: `src/JavaScriptRuntime/Node/*` and `src/JavaScriptRuntime/CommonJS/*`
- Repo-local demand signals: `Js2IL.Tests/Node/**/*`, `Js2IL.Tests/CommonJS/**/*`, and `Js2IL.Tests/Import/**/*`
- Current open Node/runtime follow-up issues: [#841](https://github.com/tomacox74/js2il/issues/841), [#946](https://github.com/tomacox74/js2il/issues/946), [#947](https://github.com/tomacox74/js2il/issues/947), [#949](https://github.com/tomacox74/js2il/issues/949), [#950](https://github.com/tomacox74/js2il/issues/950), [#951](https://github.com/tomacox74/js2il/issues/951), [#952](https://github.com/tomacox74/js2il/issues/952), [#953](https://github.com/tomacox74/js2il/issues/953), [#954](https://github.com/tomacox74/js2il/issues/954), [#955](https://github.com/tomacox74/js2il/issues/955), [#956](https://github.com/tomacox74/js2il/issues/956)

## Current Baseline (Snapshot)

- Node docs currently track **19 modules** (**17 `partial`**, **2 `completed`**) and **14 globals** (**14 `supported`**).
- Several high-value families have moved from "missing" to "follow-on" status on `master`:
  - `child_process` now has a documented `fork()` + JSON IPC baseline for compiled child modules.
  - `http`, `https`, `tls`, and `net` all have practical loopback/local baselines.
  - `stream` has lifecycle helpers (`pipeline`, `finished`, pause/resume, UTF-8 decoding, basic backpressure) instead of only raw class stubs.
  - `fs` / `fs/promises` now cover whole-file helpers, FileHandle open/read/write/close, and file stream basics.
  - `timers/promises` now supports the one-shot Promise helpers.
  - `node:url` now exposes a focused WHATWG URL module baseline.
- The biggest remaining popularity-weighted gaps are now concentrated in:
  - missing globals and web-adjacent surfaces (`URL`, `URLSearchParams`, `fetch`, `Headers`, `Request`, `Response`)
  - outbound client parity beyond local loopback baselines (`http`, `https`, `tls`)
  - deeper process-control and scheduler follow-ons (`child_process`, `timers/promises`)
  - mature toolchain surfaces that still have only pragmatic baselines (`fs`, `stream`, `crypto`, package loader/runtime probing)

## Repo-local Demand Signals

- Node coverage is now strongest around the landed foundations: `fs`, `fs/promises`, `http`, `https`, `tls`, `net`, `stream`, `timers/promises`, `child_process`, `url`, and `util`.
- Because those foundations exist, the next backlog should optimize for **ecosystem unblock value**: finishing the most commonly assumed follow-ons on top of those modules rather than starting lower-value new modules.

## Ranking Criteria

- Ecosystem unblock impact (how many modern packages or app patterns the feature opens up)
- Dependency leverage (whether it unlocks multiple later modules or runtimes)
- Current implementation gap size (pragmatic baseline vs still-missing follow-on)
- Repo-local demand signals (existing tests and nearby runtime code)
- Ability to ship in slices with clear user-visible value

## Current ranked backlog (recommended order)

| Rank | Feature family | Primary Node area | GitHub issue | Current status signal | Why it is top-10 now |
|---:|---|---|---|---|---|
| 1 | [Global WHATWG URL / URLSearchParams exposure](https://github.com/tomacox74/js2il/issues/946) | globals + `url` | [#946](https://github.com/tomacox74/js2il/issues/946) | `node:url` works, but bare globals are still missing and direct global usage is still a known gap | Common Node/browser-style code assumes `globalThis.URL`, and this already surfaced as a real repo-local tooling blocker. |
| 2 | [Outbound HTTP client parity for real tooling](https://github.com/tomacox74/js2il/issues/947) | `http` / `https` | [#947](https://github.com/tomacox74/js2il/issues/947) (related [#841](https://github.com/tomacox74/js2il/issues/841)) | Loopback HTTP/HTTPS baselines exist, but real network-mode workflows still need broader request/redirect/text-path parity | This is the clearest self-hosting/runtime follow-on after the recent extractor investigation and would unlock more than synthetic loopback demos. |
| 3 | [Global `fetch` / `Headers` / `Request` / `Response` baseline](https://github.com/tomacox74/js2il/issues/949) | globals + web platform | [#949](https://github.com/tomacox74/js2il/issues/949) | These globals are not listed in the current supported Node global inventory, even though the lower transport stack now exists in partial form | Modern Node 18+/22 packages increasingly assume fetch-style APIs first and only fall back to raw `node:http` in edge cases. |
| 4 | [Advanced child-process parity after the current fork baseline](https://github.com/tomacox74/js2il/issues/950) | `child_process` | [#950](https://github.com/tomacox74/js2il/issues/950) (hosted-fork sub-gap: [#914](https://github.com/tomacox74/js2il/issues/914)) | `spawn`/`exec`/`execFile`/`fork` basics now work, but detached children, handle passing, advanced serialization, and hosted-engine parity remain unsupported | Dev servers, worker orchestration, and IPC-heavy toolchains often need more than the current compiled-child JSON IPC slice. |
| 5 | [`timers/promises.setInterval(...)` async-iterator contract](https://github.com/tomacox74/js2il/issues/951) | `timers/promises` | [#951](https://github.com/tomacox74/js2il/issues/951) | The one-shot Promise helpers are supported, but `setInterval(...)` is still explicitly rejected | Polling, retry, and scheduler libraries use this exact modern timer surface and it is now the only clearly missing API in that module. |
| 6 | [Broader package loader and runtime-probing parity](https://github.com/tomacox74/js2il/issues/952) | `require()` / package resolution | [#952](https://github.com/tomacox74/js2il/issues/952) | Literal compile-time resolution works, but runtime probing, non-`./` package imports targets, and custom loaders/hooks remain unsupported | Plugin ecosystems and CLIs frequently depend on dynamic resolution patterns that the current compile-time-only slice cannot model. |
| 7 | [File watching, rich stats, and raw-fd parity](https://github.com/tomacox74/js2il/issues/953) | `fs` / `fs/promises` | [#953](https://github.com/tomacox74/js2il/issues/953) | Whole-file helpers, FileHandle basics, and file streams exist; watchers, richer stats/permissions, and raw numeric-fd workflows are still missing | Build tools and dev servers hit watch/stat/raw-fd gaps quickly even when the basic file I/O baseline is present. |
| 8 | [Practical crypto expansion beyond hashes/HMAC](https://github.com/tomacox74/js2il/issues/954) | `crypto` | [#954](https://github.com/tomacox74/js2il/issues/954) | Hash/HMAC/random/subtle-HMAC baselines exist, but pbkdf2Sync, ciphers, asymmetric keys, key import/export, and broader Web Crypto are still absent | Real auth, signing, and secure-config workloads still need more than the current digest-focused slice. |
| 9 | [Stream `objectMode` / `stream/promises` / AbortSignal completeness](https://github.com/tomacox74/js2il/issues/955) | `stream` | [#955](https://github.com/tomacox74/js2il/issues/955) | Callback `pipeline`/`finished` and basic Readable/Writable/Transform support exist, but object mode, promise helpers, AbortSignal, and richer buffering semantics remain out of scope | Many adapters and higher-level libraries assume these helpers instead of wiring raw `pipe()` + events manually. |
| 10 | [TLS trust, client-auth, and agent parity](https://github.com/tomacox74/js2il/issues/956) | `https` / `tls` | [#956](https://github.com/tomacox74/js2il/issues/956) | Local self-signed loopback flows work, but custom CA trust, client certificates, ALPN, and HTTPS agent pooling are still unsupported | Real outbound service integrations often fail here even after basic local HTTPS tests pass. |

## Notable next-tier gaps

- **Broader `net` parity beyond loopback IPv4**: `ref()` / `unref()`, non-UTF-8 encoding paths, `keepAlive` initialDelay, IPv6/non-loopback expectations, and other broader socket controls remain lower-level but still meaningful follow-ons.
- **`os` surface expansion**: the documented baseline is still only `tmpdir()` and `homedir()`, so a lot of common CLI/environment helpers remain absent.
- **`path.posix` / `path.win32` completeness**: the core `path` helpers are in good shape, but the namespaced `posix`/`win32` surfaces are still intentionally minimal.
- **`util` follow-ons**: `promisify`, `inherits`, `format`, and practical `types`/`inspect` slices exist, but broader utility parity is still incomplete.
- **Legacy low-priority gaps**: `querystring` helper extras and broader `perf_hooks` APIs are still partial, but they currently unblock fewer workloads than the ten items above.

## Recommended sequencing

- **Start with the current high-signal issue-backed items first:** [#946](https://github.com/tomacox74/js2il/issues/946) -> [#947](https://github.com/tomacox74/js2il/issues/947).
- **The previously untracked top-10 gaps now have dedicated issues:** [#949](https://github.com/tomacox74/js2il/issues/949), [#950](https://github.com/tomacox74/js2il/issues/950), [#951](https://github.com/tomacox74/js2il/issues/951), [#952](https://github.com/tomacox74/js2il/issues/952), [#953](https://github.com/tomacox74/js2il/issues/953), [#954](https://github.com/tomacox74/js2il/issues/954), [#955](https://github.com/tomacox74/js2il/issues/955), and [#956](https://github.com/tomacox74/js2il/issues/956).
- **Keep transport follow-ons layered:** do not start a higher-level convenience surface (`fetch`, advanced HTTPS, agent pooling) without the minimum lower-layer HTTP/TLS behavior it depends on.

## Gate for Each Delivered Item

- Add execution tests and generator tests where applicable.
- Update the relevant `docs/nodejs/*.json` source files.
- Regenerate the Node docs (`npm run generate:node-index` and `npm run generate:node-module-docs`, or `npm run generate:node-modules`).
- Update `CHANGELOG.md` when behavior changes are user-visible.

## Risks / Caveats

- This ranking is deliberately heuristic. It reflects the current docs plus repo-local demand signals, not external npm telemetry.
- Some items are feature families rather than single APIs. Each should still be delivered in explicit, documented slices.
- The global/web-platform items (`URL`, `fetch`, `Request`, `Response`, `Headers`) are included because they now directly affect common Node workloads, even when they are not packaged as core-module gaps.
