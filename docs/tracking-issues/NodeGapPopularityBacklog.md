# Node Gap Popularity Backlog

> **Last Updated**: 2026-04-13
> Purpose: Persist a holistic, popularity-weighted view of the highest-value remaining Node.js gaps so triage context is not lost between sessions.
> Scope: Node.js compatibility first, with adjacent web/runtime work called out when it directly blocks common Node workloads.
> Active review item: none.

## Inputs Used

- Current support inventory: `docs/nodejs/Index.md`
- Stated runtime limitations: `docs/nodejs/NodeLimitations.json`
- Module-level coverage: `docs/nodejs/*.json`
- Runtime module footprint: `src/JavaScriptRuntime/Node/*` and `src/JavaScriptRuntime/CommonJS/*`
- Repo-local demand signals: `tests/Js2IL.Tests/Node/**/*`, `tests/Js2IL.Tests/CommonJS/**/*`, and `tests/Js2IL.Tests/Import/**/*`
- Current open Node/runtime follow-up issues: [#949](https://github.com/tomacox74/js2il/issues/949), [#956](https://github.com/tomacox74/js2il/issues/956)
- Recently closed Node/runtime issues that changed the backlog shape: [#946](https://github.com/tomacox74/js2il/issues/946), [#947](https://github.com/tomacox74/js2il/issues/947), [#950](https://github.com/tomacox74/js2il/issues/950)-[#955](https://github.com/tomacox74/js2il/issues/955), and the architecture investigation [#841](https://github.com/tomacox74/js2il/issues/841)

## Current Baseline (Snapshot)

- Node docs currently track **19 modules** (**16 `partial`**, **3 `completed`**) and **16 globals** (**15 `supported`**, **1 `partial`**).
- Several previously top-ranked gaps are now shipped on `master`:
  - `globalThis.URL` is supported and shares constructor identity with `node:url`.
  - The extractor network-mode follow-on landed, so JS2IL can now run the real `scripts/ECMA262` networking path instead of being limited to offline/manual fetch flows.
  - `child_process`, `timers/promises`, loader/runtime probing, `fs`, `crypto`, and `stream` all moved forward enough that their previous issue-backed follow-ons are now closed.
- The biggest remaining popularity-weighted gaps are now concentrated in:
  - missing high-level web-style globals (`fetch`, `Headers`, `Request`, `Response`)
  - production-grade TLS/HTTPS parity beyond the current local/self-signed baseline
  - still-thin but commonly assumed module surfaces (`os`, `path.posix` / `path.win32`, `util`, `URLSearchParams`)
  - deeper networking/runtime polish (`net`, broader outbound client behavior)

## Repo-local Demand Signals

- Node coverage is strongest around the recently delivered foundations: `child_process`, `crypto`, `fs`, `http`, `https`, `net`, `stream`, `timers/promises`, `url`, `util`, and `zlib`.
- Because those foundations now exist, the next backlog should optimize for **ecosystem unblock value**: finish the most commonly assumed follow-ons on top of those modules instead of starting low-value new modules.

## Ranking Criteria

- Ecosystem unblock impact (how many modern packages or app patterns the feature opens up)
- Dependency leverage (whether it unlocks multiple later modules or runtimes)
- Current implementation gap size (pragmatic baseline vs still-missing follow-on)
- Repo-local demand signals (existing tests and nearby runtime code)
- Ability to ship in slices with clear user-visible value

## Current ranked backlog (recommended order)

Only [#949](https://github.com/tomacox74/js2il/issues/949) and [#956](https://github.com/tomacox74/js2il/issues/956) remain open from the previous issue-backed top 10. Ranks 3-10 below are the best current **issue-creation candidates** if they remain high-priority after those two land.

| Rank | Feature family | Primary Node area | GitHub issue | Current status signal | Why it is top-10 now |
|---:|---|---|---|---|---|
| 1 | [Global `fetch` / `Headers` / `Request` / `Response` baseline](https://github.com/tomacox74/js2il/issues/949) | globals + web platform | [#949](https://github.com/tomacox74/js2il/issues/949) | These globals are still absent from the supported global inventory even though the lower transport stack now exists in partial form | Modern Node 18+/22 packages increasingly assume fetch-style APIs first and only fall back to raw `node:http` in edge cases. |
| 2 | [TLS trust, client-auth, and agent parity](https://github.com/tomacox74/js2il/issues/956) | `https` / `tls` | [#956](https://github.com/tomacox74/js2il/issues/956) | Local self-signed loopback flows work, but custom CA trust, client certificates, ALPN, and richer agent behavior remain unsupported | Real outbound service integrations still fail here even after the recent HTTP/TLS baseline wins. |
| 3 | `os` surface expansion | `os` | No dedicated issue yet | The documented baseline is still only `tmpdir()` and `homedir()` | CLI and tooling code frequently expects many more environment and platform helpers than the current tiny slice exposes. |
| 4 | `URLSearchParams` completion and legacy `url` follow-ons | globals + `url` | No dedicated issue yet | `URL` is now supported, but `URLSearchParams` is still `partial`, and legacy `url.parse` / `url.format` remain unimplemented | URL support is much more useful now, which makes the remaining gaps more visible to real workloads. |
| 5 | `path.posix` / `path.win32` completeness | `path` | No dedicated issue yet | Core `path` helpers are in good shape, but the namespaced `posix` / `win32` surfaces are intentionally minimal | Bundlers, build tools, and cross-platform fixtures often rely on the namespaced helpers rather than the host-default surface. |
| 6 | Broader `net` parity beyond loopback IPv4 | `net` | No dedicated issue yet | `ref()` / `unref()`, non-UTF-8 encoding paths, `keepAlive` initialDelay, and broader IPv6/non-loopback expectations remain unsupported | Lower-level networking stacks, dev servers, and adapters hit these controls quickly once basic TCP already works. |
| 7 | Broader outbound `http` / `https` client polish after the extractor baseline | `http` / `https` | No dedicated issue yet | The extractor-specific network mode landed, but the public client surfaces still expose only a pragmatic subset of broader outbound behavior | There is still a meaningful gap between "works for the checked-in extractor path" and "safe for arbitrary Node HTTP clients". |
| 8 | `util` follow-ons | `util` | No dedicated issue yet | `promisify`, `inherits`, `format`, and practical `types` / `inspect` slices exist, but the module remains `partial` | Utility shims get pulled in by a wide range of packages, so missing edges here create diffuse ecosystem friction. |
| 9 | Practical compression expansion beyond the current gzip slice | `zlib` | No dedicated issue yet | `gzipSync`, `gunzipSync`, `createGzip`, and `createGunzip` exist, but deflate/inflate/brotli and richer streaming/tuning remain unsupported | Compression support is increasingly relevant once HTTP/stream baselines exist and packages start expecting richer content-encoding coverage. |
| 10 | `perf_hooks` baseline expansion | `perf_hooks` | No dedicated issue yet | Only `performance` and `performance.now()` are documented today | Lower impact than the items above, but still a common helper surface for modern tooling and metrics libraries. |

## Notable next-tier gaps

- **Further `child_process` polish**: the module is much stronger now, but detached launches, handle passing, and advanced serialization remain unsupported by design in the delivered slice.
- **`querystring` helper extras**: lower priority now that WHATWG URL support is present, but still incomplete.
- **More `zlib` streaming fidelity**: today the delivered transform helpers buffer the full payload and emit a single output chunk on `end()`.

## Recommended sequencing

- **Finish the remaining issue-backed Node work first:** [#949](https://github.com/tomacox74/js2il/issues/949) -> [#956](https://github.com/tomacox74/js2il/issues/956).
- **Then cut the next issue from the highest remaining untracked gap:** ranks 3-5 above are the strongest current candidates.
- **Keep transport follow-ons layered:** do not start a higher-level convenience surface (`fetch`, advanced HTTPS, agent pooling) without the minimum lower-layer HTTP/TLS behavior it depends on.

## Gate for Each Delivered Item

- Add execution tests and generator tests where applicable.
- Update the relevant `docs/nodejs/*.json` source files.
- Regenerate the Node docs (`npm run generate:node-index` and `npm run generate:node-module-docs`, or `npm run generate:node-modules`).
- Update `CHANGELOG.md` when behavior changes are user-visible.

## Risks / Caveats

- This ranking is deliberately heuristic. It reflects the current docs plus repo-local demand signals, not external npm telemetry.
- Some items are feature families rather than single APIs. Each should still be delivered in explicit, documented slices.
- The global/web-platform items are included because they now directly affect common Node workloads, even when they are not packaged as classic core-module gaps.

## Issue #841 investigation addendum (closed 2026-04-09)

This addendum is the concrete output for [#841](https://github.com/tomacox74/js2il/issues/841). It remains useful reference material for the remaining network work; it is no longer an open queue item.

### Current-state inventory

- `src/JavaScriptRuntime/Node/Net.cs` owns the transport/event-loop model through `TcpClient` / `TcpListener`, `IIOScheduler`, `NodeSchedulerState`, incremental reads/writes, and Node-shaped socket lifecycle behavior.
- `src/JavaScriptRuntime/Node/Http.cs` owns the custom HTTP/1.1 request/response model: `HttpClientRequest`, `HttpServer`, `IncomingMessage`, `ServerResponse`, custom parsing/decoding, chunked framing, sequential keep-alive reuse, and explicit unsupported handling for CONNECT, Upgrade, Expect/100-continue, and pipelining.
- `src/JavaScriptRuntime/Node/Https.cs` layers TLS on top of that same transport through `SslStream`, with a documented local/self-signed baseline and explicit omissions around custom CA trust, client certificates, ALPN, and advanced `https.Agent` behavior.
- Because JS2IL already exposes Node-shaped request/response/socket/event surfaces, any .NET-backed reuse has to sit behind those existing objects instead of replacing them wholesale.

### Options comparison

| Option | Best fit | Advantages | Main risks / mismatches | Recommendation |
|---|---|---|---|---|
| Current custom TCP + parser path | Existing server and client baseline | Full control over socket visibility, unsupported-feature gating, event-loop scheduling, and Node-shaped objects | More incremental work is still needed to grow outbound parity and TLS follow-ons | **Keep as the server-side and transport foundation** |
| `HttpClient` / `SocketsHttpHandler` | Outbound client follow-ons only | Mature cross-platform HTTP/TLS behavior, strong handler knobs, and headers-first response streaming | Raw socket visibility is limited, default auto-behaviors must be disabled, agent semantics are not 1:1, and abort/error timing still needs a Node adapter | **Investigate selectively for outbound-only flows** |
| `HttpListener` | Narrow server experiments at most | Built-in server API with low immediate coding cost | Older API, host/OS friction, and a poor fit for Node's stream/event/socket model | **Do not adopt for core `node:http` server work** |
| Kestrel / ASP.NET Core | Heavyweight hosting only | Rich HTTP/TLS infrastructure and a well-tested server stack | Large dependency/runtime weight, request-pipeline model differs from Node, and it does not naturally expose Node-like socket/request lifecycles | **Possible for a hosting-specific integration layer, but not recommended for the baseline Node runtime** |

### Recommendation

- Keep the current custom `node:net` + HTTP server/parser stack as the long-term foundation for server-side flows and any API that depends on direct socket ownership.
- Only consider selective reuse of .NET HTTP primitives through an outbound-only adapter over `HttpClient` / `SocketsHttpHandler`, and only if it stays behind the existing `HttpClientRequest`, `IncomingMessage`, and `http.Agent` semantics.
- If that outbound adapter is spiked, require all of the following up front:
  - `AllowAutoRedirect = false` so Node redirect semantics stay explicit.
  - Headers-first streaming (`ResponseHeadersRead`) so response bodies can still surface incrementally.
  - No hidden cookies or automatic decompression unless JS2IL explicitly models those behaviors.
  - Explicit mapping for abort/destroy/error timing and clear preservation of supported keep-alive / agent behavior.
  - A deliberate answer for any supported `request.socket` / `response.socket` expectations before widening the public surface.
- Do not pursue `HttpListener` or Kestrel as the baseline `node:http` server implementation; both fight the Node evented stream model harder than the current parser path helps.

### Follow-on map

- [#947](https://github.com/tomacox74/js2il/issues/947) is now closed and should be treated as the first worked example of this recommendation being applied to a real tooling workload.
- [#949](https://github.com/tomacox74/js2il/issues/949) should reuse the same transport guidance when layering fetch-style globals over the current HTTP/HTTPS baseline.
- [#956](https://github.com/tomacox74/js2il/issues/956) should only build on a `HttpClient` path if the spike proves we can map CA trust, client certificates, ALPN, and agent behavior cleanly; otherwise it should continue extending the current `SslStream` + `NetSocket` / `https` stack.
- Non-goals for the next transport follow-ons: server rewrite, HTTP/2, WebSocket/Upgrade, CONNECT tunneling, or replacing `node:net`.
