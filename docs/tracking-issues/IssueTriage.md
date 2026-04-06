# Issue triage snapshot (refreshed 2026-04-06)

This file captures a point-in-time recommended ordering for all currently open GitHub issues.

Synced to:
- Repo: `master` @ `25ed490d`
- Latest release on `master`: `v0.9.6` via PR [#937](https://github.com/tomacox74/js2il/pull/937)
- Active review branches at latest update: none
- GitHub: open issues / PR state as of 2026-04-06 after merging PR [#948](https://github.com/tomacox74/js2il/pull/948) and creating the new Node follow-up issues [#949](https://github.com/tomacox74/js2il/issues/949)-[#956](https://github.com/tomacox74/js2il/issues/956)
- Open issues: 29
- Open PRs: 0

## What changed since the previous snapshot

- PR [#948](https://github.com/tomacox74/js2il/pull/948) merged as commit `25ed490d` and refreshed the earlier post-[#945](https://github.com/tomacox74/js2il/pull/945) issue snapshot onto `master`.
- The Node backlog review then created eight new follow-up issues [#949](https://github.com/tomacox74/js2il/issues/949)-[#956](https://github.com/tomacox74/js2il/issues/956), so the top unresolved Node compatibility items now all have explicit GitHub trackers instead of living only in `NodeGapPopularityBacklog.md`.
- The open issue count rose from 21 to 29 while the open PR count stayed at 0. That increase is new planning signal, not new review debt: the queue is simply better issue-backed than it was before.
- The remaining backlog is now split into:
  - 11 Node/runtime follow-ons ([#841](https://github.com/tomacox74/js2il/issues/841), [#946](https://github.com/tomacox74/js2il/issues/946), [#947](https://github.com/tomacox74/js2il/issues/947), [#949](https://github.com/tomacox74/js2il/issues/949)-[#956](https://github.com/tomacox74/js2il/issues/956))
  - 8 `test262` adoption issues ([#927](https://github.com/tomacox74/js2il/issues/927)-[#934](https://github.com/tomacox74/js2il/issues/934))
  - 10 deferred performance issues ([#451](https://github.com/tomacox74/js2il/issues/451), [#737](https://github.com/tomacox74/js2il/issues/737), [#738](https://github.com/tomacox74/js2il/issues/738), [#742](https://github.com/tomacox74/js2il/issues/742), [#743](https://github.com/tomacox74/js2il/issues/743), [#746](https://github.com/tomacox74/js2il/issues/746), [#747](https://github.com/tomacox74/js2il/issues/747), [#748](https://github.com/tomacox74/js2il/issues/748), [#768](https://github.com/tomacox74/js2il/issues/768), [#837](https://github.com/tomacox74/js2il/issues/837))

## Ranking method

- Use [TriageScoreboard.md](./TriageScoreboard.md) as the strategic source of truth, and let [NodeGapPopularityBacklog.md](./NodeGapPopularityBacklog.md) drive the Node lane order now that its top remaining items are explicitly issue-backed.
- Prefer concrete, issue-backed Node compatibility slices with clear user-visible unblock value first; then keep the `test262` program in dependency order; keep research behind shipping slices; and keep performance behind correctness/tooling unless it directly unblocks those tracks.
- Treat [#927](https://github.com/tomacox74/js2il/issues/927) as the umbrella tracker for the `test262` program, and rank the child issues by execution order rather than by issue number alone.

## Recommended next picks

- **Primary next item:** [#946](https://github.com/tomacox74/js2il/issues/946) `runtime: add full global URL support`
- **Best tooling / self-hosting follow-on:** [#947](https://github.com/tomacox74/js2il/issues/947) `scripts/ECMA262: enable network mode under js2il via HTTP client fixes`
- **Best next Node platform follow-on:** [#949](https://github.com/tomacox74/js2il/issues/949) `node: add global fetch / Request / Response / Headers baseline`
- **Best research / architecture follow-on:** [#841](https://github.com/tomacox74/js2il/issues/841) `Investigate using existing .NET HTTP primitives for future Node HTTP work`
- **Strategic next program:** [#927](https://github.com/tomacox74/js2il/issues/927) `test262: create a phased conformance program for js2il`, starting with [#928](https://github.com/tomacox74/js2il/issues/928) -> [#929](https://github.com/tomacox74/js2il/issues/929) -> [#930](https://github.com/tomacox74/js2il/issues/930) -> [#931](https://github.com/tomacox74/js2il/issues/931) -> [#932](https://github.com/tomacox74/js2il/issues/932) -> [#933](https://github.com/tomacox74/js2il/issues/933), while keeping [#934](https://github.com/tomacox74/js2il/issues/934) explicitly deferred until the MVP exists.

## Recommended order

### Tier 1 - Immediate Node compatibility wins

1. **[#946](https://github.com/tomacox74/js2il/issues/946)** `runtime: add full global URL support` - This remains the clearest small runtime/tooling follow-on: it removes the bare global `URL` / `URLSearchParams` validator/runtime gap without waiting on broader HTTP networking work.
2. **[#947](https://github.com/tomacox74/js2il/issues/947)** `scripts/ECMA262: enable network mode under js2il via HTTP client fixes` - This is still the best self-hosting/tooling issue because it keeps a checked-in repo script as the driving repro and pushes the HTTP client path beyond loopback-only success.
3. **[#949](https://github.com/tomacox74/js2il/issues/949)** `node: add global fetch / Request / Response / Headers baseline` - This is the next high-leverage platform surface after [#946](https://github.com/tomacox74/js2il/issues/946): many modern Node packages assume `fetch`-style globals before they ever touch raw `node:http`.
4. **[#950](https://github.com/tomacox74/js2il/issues/950)** `node: expand child_process beyond the current fork/IPC baseline` - The current `child_process` baseline is useful, but many toolchains still need the next explicit slice: detached lifecycle behavior, richer stdio/IPC follow-ons, or better hosted-fork parity.
5. **[#951](https://github.com/tomacox74/js2il/issues/951)** `node: complete timers/promises with setInterval async-iterator support` - This is now the one obvious missing API in `timers/promises`, which makes it both bounded and disproportionately valuable for scheduler/polling workloads.
6. **[#952](https://github.com/tomacox74/js2il/issues/952)** `node: expand loader/runtime probing beyond the current compile-time slice` - Package graphs now work well in the documented literal/compile-time slice, but plugin ecosystems and CLIs still hit the remaining runtime-probing and broader loader gaps quickly.
7. **[#953](https://github.com/tomacox74/js2il/issues/953)** `node: expand fs/fs.promises with watch, rich stats, and raw-fd follow-ons` - The file I/O baseline is practical now, which makes file watching, richer stats, and raw-fd follow-ons the next meaningful filesystem unblockers for build tools and dev loops.
8. **[#954](https://github.com/tomacox74/js2il/issues/954)** `node: expand practical crypto surface beyond hashes and HMAC` - Real auth, signing, and secure-config workflows still need the next pragmatic crypto layer after the current digest/HMAC baseline.
9. **[#955](https://github.com/tomacox74/js2il/issues/955)** `node: expand stream with objectMode, promise helpers, and AbortSignal support` - The callback-oriented stream baseline is useful, but many adapters and higher-level libraries now assume object-mode streams, promise helpers, and cancellation.
10. **[#956](https://github.com/tomacox74/js2il/issues/956)** `node: expand TLS/HTTPS trust, client-auth, and agent parity` - Local TLS/HTTPS loopback success is no longer the main blocker; real outbound integrations now need trust and agent parity beyond that baseline.

### Tier 2 - `test262` program bootstrapping

11. **[#927](https://github.com/tomacox74/js2il/issues/927)** `test262: create a phased conformance program for js2il` - Keep this as the umbrella issue that defines the scope, sequencing, and reporting model for the child issues below.
12. **[#928](https://github.com/tomacox74/js2il/issues/928)** `test262: decide upstream intake and sync model` - This is the first concrete decision point because acquisition, pinning, and licensing expectations constrain every downstream runner and CI decision.
13. **[#929](https://github.com/tomacox74/js2il/issues/929)** `test262: implement metadata/frontmatter parser` - Once intake is decided, metadata parsing is the next hard dependency; the runner cannot classify includes, flags, features, or negative tests without it.
14. **[#930](https://github.com/tomacox74/js2il/issues/930)** `test262: create MVP runner for plain synchronous script tests` - This is the first runnable delivery slice and should stay intentionally narrow so failures remain actionable.
15. **[#931](https://github.com/tomacox74/js2il/issues/931)** `test262: classify negative tests, exclusions, and baselines` - This can partially evolve alongside the MVP runner, but it should land before broadening the slice so output stays reproducible and triageable.
16. **[#932](https://github.com/tomacox74/js2il/issues/932)** `test262: add CI workflow and machine-readable reporting` - CI and summary output should follow a working local slice rather than arrive before the runner semantics are stable.
17. **[#933](https://github.com/tomacox74/js2il/issues/933)** `test262: connect conformance results to ECMA-262 docs and backlog` - This becomes high value once there is real output to map back into the docs and issue system.
18. **[#934](https://github.com/tomacox74/js2il/issues/934)** `test262: expand beyond the MVP to modules, async, and harness-heavy suites` - Important, but explicitly later: this should remain deferred until the narrow MVP runner and reporting loop are already working.

### Tier 3 - Research / architecture follow-on

19. **[#841](https://github.com/tomacox74/js2il/issues/841)** `Investigate using existing .NET HTTP primitives for future Node HTTP work` - Still useful architectural input and likely relevant to [#947](https://github.com/tomacox74/js2il/issues/947) and [#956](https://github.com/tomacox74/js2il/issues/956), but it remains research rather than the next direct shipping slice.

### Tier 4 - Deferred performance queue

20. **[#451](https://github.com/tomacox74/js2il/issues/451)** `perf(il): expand typed temps/locals to reduce casts/boxing` - Best broad performance enabler once the current Node and conformance priorities relax.
21. **[#737](https://github.com/tomacox74/js2il/issues/737)** `perf: callsite-based typed parameter specialization for non-exported functions` - Builds on the same typed-fast-path direction as [#451](https://github.com/tomacox74/js2il/issues/451).
22. **[#738](https://github.com/tomacox74/js2il/issues/738)** `perf(prime): close PrimeJavaScript gap with spec-safe hot-path optimizations` - Still the umbrella Prime performance issue, but now clearly a secondary lane behind the expanded Node and `test262` work.
23. **[#742](https://github.com/tomacox74/js2il/issues/742)** `perf(prime): trim timing/config coercion overhead in main path` - Scoped child under [#738](https://github.com/tomacox74/js2il/issues/738).
24. **[#743](https://github.com/tomacox74/js2il/issues/743)** `perf(prime): add Prime perf acceptance gate and reporting` - Best once the next round of Prime tuning resumes.
25. **[#746](https://github.com/tomacox74/js2il/issues/746)** `perf: make dromaeo-object-regexp faster than Jint prepared` - Valuable benchmark target, but still behind compatibility and tooling work.
26. **[#747](https://github.com/tomacox74/js2il/issues/747)** `perf(regexp): cache Regex instances by source+flags` - Child optimization under [#746](https://github.com/tomacox74/js2il/issues/746).
27. **[#748](https://github.com/tomacox74/js2il/issues/748)** `perf(dispatch): add RegExp fast paths in Object.CallMember1/2` - Another child optimization under [#746](https://github.com/tomacox74/js2il/issues/746).
28. **[#768](https://github.com/tomacox74/js2il/issues/768)** `Perf: devirtualize calls to const/arrow function bindings (dromaeo-object-regexp-modern)` - Still scenario-specific rather than a broad ecosystem unblocker.
29. **[#837](https://github.com/tomacox74/js2il/issues/837)** `perf(runtime): investigate DLR-backed CallMember fast path` - Remains a research-heavy idea rather than a near-term implementation slice.

## Execution notes

- **No active review queue remains:** there are currently no open PRs on `master`, so the next work can start directly instead of finishing review branches first.
- **Immediate top-of-stack track:** [#946](https://github.com/tomacox74/js2il/issues/946) -> [#947](https://github.com/tomacox74/js2il/issues/947) -> [#949](https://github.com/tomacox74/js2il/issues/949) -> [#950](https://github.com/tomacox74/js2il/issues/950) -> [#951](https://github.com/tomacox74/js2il/issues/951) -> [#952](https://github.com/tomacox74/js2il/issues/952) -> [#953](https://github.com/tomacox74/js2il/issues/953) -> [#954](https://github.com/tomacox74/js2il/issues/954) -> [#955](https://github.com/tomacox74/js2il/issues/955) -> [#956](https://github.com/tomacox74/js2il/issues/956). [#841](https://github.com/tomacox74/js2il/issues/841) remains the next research-oriented follow-on after that concrete Node queue.
- **`test262` track:** use [#927](https://github.com/tomacox74/js2il/issues/927) as the parent, then deliver [#928](https://github.com/tomacox74/js2il/issues/928) -> [#929](https://github.com/tomacox74/js2il/issues/929) -> [#930](https://github.com/tomacox74/js2il/issues/930) -> [#931](https://github.com/tomacox74/js2il/issues/931) -> [#932](https://github.com/tomacox74/js2il/issues/932) -> [#933](https://github.com/tomacox74/js2il/issues/933), leaving [#934](https://github.com/tomacox74/js2il/issues/934) as the deliberate post-MVP expansion bucket.
- **Performance track:** [#451](https://github.com/tomacox74/js2il/issues/451) -> [#737](https://github.com/tomacox74/js2il/issues/737) remains the best general optimization path; [#738](https://github.com/tomacox74/js2il/issues/738) stays the Prime umbrella with [#742](https://github.com/tomacox74/js2il/issues/742) / [#743](https://github.com/tomacox74/js2il/issues/743) as child slices, while [#746](https://github.com/tomacox74/js2il/issues/746) -> [#747](https://github.com/tomacox74/js2il/issues/747) / [#748](https://github.com/tomacox74/js2il/issues/748) and [#768](https://github.com/tomacox74/js2il/issues/768) / [#837](https://github.com/tomacox74/js2il/issues/837) remain lower-priority benchmark work.

## Metadata gaps

- Open-issue labeling still lags the actual queue: `10/29` open issues currently carry a `priority:*` label, `0/29` carry a `lane:*` label, and `14/29` have no labels at all.
- The new Node backlog items are now issue-backed and consistently labeled, but the older test262/performance queue still lacks lane labeling and many issues remain unlabeled.
- With no open PRs, this triage document plus `NodeGapPopularityBacklog.md` are currently more reliable ordering signals than any active review-queue metadata.
