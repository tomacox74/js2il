# Issue triage snapshot (refreshed 2026-04-09)

This file captures a point-in-time recommended ordering for all currently open GitHub issues.

Synced to:
- Repo: `master` @ `9f2abca1`
- Latest release on `master`: `v0.9.7` via PR [#966](https://github.com/tomacox74/js2il/pull/966)
- Active review branches at latest update: none
- GitHub: open issues / PR state as of 2026-04-09 after merging PR [#966](https://github.com/tomacox74/js2il/pull/966) and publishing `v0.9.7`
- Open issues: 22
- Open PRs: 0

## What changed since the previous snapshot

- PR [#966](https://github.com/tomacox74/js2il/pull/966) merged as commit `9f2abca1` and published patch release `v0.9.7`, so the earlier merge/release queue is now fully landed on `master`.
- The previously top-ranked Node tranche is mostly complete now: issues [#946](https://github.com/tomacox74/js2il/issues/946) and [#950](https://github.com/tomacox74/js2il/issues/950)-[#955](https://github.com/tomacox74/js2il/issues/955) were all closed by the merged PR queue and the release follow-up fix.
- The open issue count dropped from 29 to 22 while the open PR count stayed at 0. The remaining queue is materially smaller, not just reorganized.
- The remaining backlog is now split into:
  - 4 Node/runtime follow-ons ([#841](https://github.com/tomacox74/js2il/issues/841), [#947](https://github.com/tomacox74/js2il/issues/947), [#949](https://github.com/tomacox74/js2il/issues/949), [#956](https://github.com/tomacox74/js2il/issues/956))
  - 8 `test262` adoption issues ([#927](https://github.com/tomacox74/js2il/issues/927)-[#934](https://github.com/tomacox74/js2il/issues/934))
  - 10 deferred performance issues ([#451](https://github.com/tomacox74/js2il/issues/451), [#737](https://github.com/tomacox74/js2il/issues/737), [#738](https://github.com/tomacox74/js2il/issues/738), [#742](https://github.com/tomacox74/js2il/issues/742), [#743](https://github.com/tomacox74/js2il/issues/743), [#746](https://github.com/tomacox74/js2il/issues/746), [#747](https://github.com/tomacox74/js2il/issues/747), [#748](https://github.com/tomacox74/js2il/issues/748), [#768](https://github.com/tomacox74/js2il/issues/768), [#837](https://github.com/tomacox74/js2il/issues/837))

## Ranking method

- Use [TriageScoreboard.md](./TriageScoreboard.md) as the strategic source of truth, and let [NodeGapPopularityBacklog.md](./NodeGapPopularityBacklog.md) drive the remaining Node lane order now that most of its earlier top items are already issue-backed and closed.
- Prefer concrete, repo-driven compatibility/tooling slices first; then keep the `test262` program in dependency order; keep research behind shipping slices; and keep performance behind correctness/tooling unless it directly unblocks those tracks.
- Treat [#927](https://github.com/tomacox74/js2il/issues/927) as the umbrella tracker for the `test262` program, and rank the child issues by execution order rather than by issue number alone.

## Recommended next picks

- **Primary next item:** [#947](https://github.com/tomacox74/js2il/issues/947) `scripts/ECMA262: enable network mode under js2il via HTTP client fixes`
- **Best next Node platform follow-on:** [#949](https://github.com/tomacox74/js2il/issues/949) `node: add global fetch / Request / Response / Headers baseline`
- **Best next production-network follow-on:** [#956](https://github.com/tomacox74/js2il/issues/956) `node: expand TLS/HTTPS trust, client-auth, and agent parity`
- **Best research / architecture follow-on:** [#841](https://github.com/tomacox74/js2il/issues/841) `Investigate using existing .NET HTTP primitives for future Node HTTP work`
- **Strategic next program:** [#927](https://github.com/tomacox74/js2il/issues/927) `test262: create a phased conformance program for js2il`, starting with [#928](https://github.com/tomacox74/js2il/issues/928) -> [#929](https://github.com/tomacox74/js2il/issues/929) -> [#930](https://github.com/tomacox74/js2il/issues/930) -> [#931](https://github.com/tomacox74/js2il/issues/931) -> [#932](https://github.com/tomacox74/js2il/issues/932) -> [#933](https://github.com/tomacox74/js2il/issues/933), while keeping [#934](https://github.com/tomacox74/js2il/issues/934) explicitly deferred until the MVP exists.

## Recommended order

### Tier 1 - Immediate compatibility and tooling wins

1. **[#947](https://github.com/tomacox74/js2il/issues/947)** `scripts/ECMA262: enable network mode under js2il via HTTP client fixes` - This is now the clearest top-of-stack item because it is a checked-in self-hosting/tooling repro with direct user value and obvious dependency overlap with the remaining network-oriented Node work.
2. **[#949](https://github.com/tomacox74/js2il/issues/949)** `node: add global fetch / Request / Response / Headers baseline` - This remains the highest-leverage missing platform surface after the recent URL/stream/crypto/fs/timers wins; modern packages assume `fetch`-style globals early.
3. **[#956](https://github.com/tomacox74/js2il/issues/956)** `node: expand TLS/HTTPS trust, client-auth, and agent parity` - With the local HTTPS/TLS baseline already in place, the next real ecosystem blocker is production-grade trust and connection-management parity for outbound integrations.

### Tier 2 - `test262` program bootstrapping

4. **[#927](https://github.com/tomacox74/js2il/issues/927)** `test262: create a phased conformance program for js2il` - Keep this as the umbrella issue that defines the scope, sequencing, and reporting model for the child issues below.
5. **[#928](https://github.com/tomacox74/js2il/issues/928)** `test262: decide upstream intake and sync model` - This remains the first concrete decision point because acquisition, pinning, and licensing expectations constrain every downstream runner and CI decision.
6. **[#929](https://github.com/tomacox74/js2il/issues/929)** `test262: implement metadata/frontmatter parser` - Once intake is decided, metadata parsing is the next hard dependency; the runner cannot classify includes, flags, features, or negative tests without it.
7. **[#930](https://github.com/tomacox74/js2il/issues/930)** `test262: create MVP runner for plain synchronous script tests` - This is still the first runnable delivery slice and should stay intentionally narrow so failures remain actionable.
8. **[#931](https://github.com/tomacox74/js2il/issues/931)** `test262: classify negative tests, exclusions, and baselines` - This can partially evolve alongside the MVP runner, but it should land before broadening the slice so output stays reproducible and triageable.
9. **[#932](https://github.com/tomacox74/js2il/issues/932)** `test262: add CI workflow and machine-readable reporting` - CI and summary output should follow a working local slice rather than arrive before the runner semantics are stable.
10. **[#933](https://github.com/tomacox74/js2il/issues/933)** `test262: connect conformance results to ECMA-262 docs and backlog` - This becomes high value once there is real output to map back into the docs and issue system.
11. **[#934](https://github.com/tomacox74/js2il/issues/934)** `test262: expand beyond the MVP to modules, async, and harness-heavy suites` - Important, but explicitly later: this should remain deferred until the narrow MVP runner and reporting loop are already working.

### Tier 3 - Research / architecture follow-on

12. **[#841](https://github.com/tomacox74/js2il/issues/841)** `Investigate using existing .NET HTTP primitives for future Node HTTP work` - Still useful architectural input and likely relevant to both [#947](https://github.com/tomacox74/js2il/issues/947) and [#956](https://github.com/tomacox74/js2il/issues/956), but it remains research rather than the next direct shipping slice.

### Tier 4 - Deferred performance queue

13. **[#451](https://github.com/tomacox74/js2il/issues/451)** `perf(il): expand typed temps/locals to reduce casts/boxing` - Best broad performance enabler once the current compatibility and conformance priorities relax.
14. **[#737](https://github.com/tomacox74/js2il/issues/737)** `perf: callsite-based typed parameter specialization for non-exported functions` - Builds on the same typed-fast-path direction as [#451](https://github.com/tomacox74/js2il/issues/451).
15. **[#738](https://github.com/tomacox74/js2il/issues/738)** `perf(prime): close PrimeJavaScript gap with spec-safe hot-path optimizations` - Still the umbrella Prime performance issue, but now clearly a secondary lane behind the remaining Node and `test262` work.
16. **[#742](https://github.com/tomacox74/js2il/issues/742)** `perf(prime): trim timing/config coercion overhead in main path` - Scoped child under [#738](https://github.com/tomacox74/js2il/issues/738).
17. **[#743](https://github.com/tomacox74/js2il/issues/743)** `perf(prime): add Prime perf acceptance gate and reporting` - Best once the next round of Prime tuning resumes.
18. **[#746](https://github.com/tomacox74/js2il/issues/746)** `perf: make dromaeo-object-regexp faster than Jint prepared` - Valuable benchmark target, but still behind compatibility and tooling work.
19. **[#747](https://github.com/tomacox74/js2il/issues/747)** `perf(regexp): cache Regex instances by source+flags` - Child optimization under [#746](https://github.com/tomacox74/js2il/issues/746).
20. **[#748](https://github.com/tomacox74/js2il/issues/748)** `perf(dispatch): add RegExp fast paths in Object.CallMember1/2` - Another child optimization under [#746](https://github.com/tomacox74/js2il/issues/746).
21. **[#768](https://github.com/tomacox74/js2il/issues/768)** `Perf: devirtualize calls to const/arrow function bindings (dromaeo-object-regexp-modern)` - Still scenario-specific rather than a broad ecosystem unblocker.
22. **[#837](https://github.com/tomacox74/js2il/issues/837)** `perf(runtime): investigate DLR-backed CallMember fast path` - Remains a research-heavy idea rather than a near-term implementation slice.

## Execution notes

- **No active review queue remains:** there are currently no open PRs on `master`, so the next work can start directly instead of finishing review branches first.
- **Immediate top-of-stack track:** [#947](https://github.com/tomacox74/js2il/issues/947) -> [#949](https://github.com/tomacox74/js2il/issues/949) -> [#956](https://github.com/tomacox74/js2il/issues/956). [#841](https://github.com/tomacox74/js2il/issues/841) remains the next research-oriented follow-on after that concrete compatibility queue.
- **`test262` track:** use [#927](https://github.com/tomacox74/js2il/issues/927) as the parent, then deliver [#928](https://github.com/tomacox74/js2il/issues/928) -> [#929](https://github.com/tomacox74/js2il/issues/929) -> [#930](https://github.com/tomacox74/js2il/issues/930) -> [#931](https://github.com/tomacox74/js2il/issues/931) -> [#932](https://github.com/tomacox74/js2il/issues/932) -> [#933](https://github.com/tomacox74/js2il/issues/933), leaving [#934](https://github.com/tomacox74/js2il/issues/934) as the deliberate post-MVP expansion bucket.
- **Performance track:** [#451](https://github.com/tomacox74/js2il/issues/451) -> [#737](https://github.com/tomacox74/js2il/issues/737) remains the best general optimization path; [#738](https://github.com/tomacox74/js2il/issues/738) stays the Prime umbrella with [#742](https://github.com/tomacox74/js2il/issues/742) / [#743](https://github.com/tomacox74/js2il/issues/743) as child slices, while [#746](https://github.com/tomacox74/js2il/issues/746) -> [#747](https://github.com/tomacox74/js2il/issues/747) / [#748](https://github.com/tomacox74/js2il/issues/748) and [#768](https://github.com/tomacox74/js2il/issues/768) / [#837](https://github.com/tomacox74/js2il/issues/837) remain lower-priority benchmark work.

## Metadata gaps

- Open-issue labeling still lags the actual queue: `0/22` open issues currently carry a `priority:*` label, `0/22` carry a `lane:*` label, and `14/22` have no labels at all.
- The queue is materially smaller after the recent merges and release, but the remaining Node, `test262`, and performance issues still are not consistently labeled for priority/lane filtering.
- With no open PRs and the release queue now fully landed, this triage document plus `NodeGapPopularityBacklog.md` are currently more reliable ordering signals than any active review-queue metadata.
