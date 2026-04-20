# Issue triage snapshot (refreshed 2026-04-20)

This file captures a point-in-time recommended ordering for all currently open GitHub issues.

Synced to:
- Repo: `origin/master` @ `163cb816`
- Latest tagged release on `master`: `v0.9.7` via PR [#966](https://github.com/tomacox74/js2il/pull/966)
- Recent merged PRs since the previous snapshot: [#974](https://github.com/tomacox74/js2il/pull/974), [#975](https://github.com/tomacox74/js2il/pull/975)
- GitHub: open issue / PR state as of 2026-04-20
- Open issues: **16**
- Open PRs: **0**

## What changed since the previous snapshot

- PR [#975](https://github.com/tomacox74/js2il/pull/975) landed on `master`, closing [#931](https://github.com/tomacox74/js2il/issues/931) and adding classified MVP results plus a machine-readable `summary.json` baseline artifact for the current `test262` slice.
- The open Node/runtime queue is unchanged and still led by [#949](https://github.com/tomacox74/js2il/issues/949) and [#956](https://github.com/tomacox74/js2il/issues/956).
- The `test262` program is now down to umbrella [#927](https://github.com/tomacox74/js2il/issues/927) plus the remaining follow-ons [#932](https://github.com/tomacox74/js2il/issues/932)-[#934](https://github.com/tomacox74/js2il/issues/934); the bootstrap/parser/MVP/classification foundation [#928](https://github.com/tomacox74/js2il/issues/928)-[#931](https://github.com/tomacox74/js2il/issues/931) is closed.
- Open issue count dropped from **17** to **16** while open PR count stayed at **0**.

## Ranking method

- Use [TriageScoreboard.md](./TriageScoreboard.md) as the strategic source of truth.
- Prefer concrete, user-visible compatibility slices before research and benchmark work.
- Treat [#927](https://github.com/tomacox74/js2il/issues/927) as the umbrella tracker for the `test262` program, and rank the concrete child issues by execution order.
- Keep performance behind compatibility and conformance unless it directly unblocks those lanes.

## Recommended next picks

- **Primary next item:** [#949](https://github.com/tomacox74/js2il/issues/949) `node: add global fetch / Request / Response / Headers baseline`
- **Best next production-network follow-on:** [#956](https://github.com/tomacox74/js2il/issues/956) `node: expand TLS/HTTPS trust, client-auth, and agent parity`
- **Best next `test262` automation slice:** [#932](https://github.com/tomacox74/js2il/issues/932) `test262: add CI workflow and machine-readable reporting`
- **Best next `test262` docs/backlog slice:** [#933](https://github.com/tomacox74/js2il/issues/933) `test262: connect conformance results to ECMA-262 docs and backlog`
- **Umbrella only, not the next direct code pick:** [#927](https://github.com/tomacox74/js2il/issues/927) stays open as the parent issue for the remaining `test262` lane.

## Recommended order

### Tier 1 - Immediate compatibility wins

1. **[#949](https://github.com/tomacox74/js2il/issues/949)** `node: add global fetch / Request / Response / Headers baseline` - The highest-leverage remaining platform gap after the recent URL/HTTP/TLS/stream/fs/crypto tranche; modern packages frequently assume `fetch`-style globals first.
2. **[#956](https://github.com/tomacox74/js2il/issues/956)** `node: expand TLS/HTTPS trust, client-auth, and agent parity` - The current TLS/HTTPS baseline is good enough for local/self-signed flows, but real outbound integrations still need trust-store, client-cert, and richer agent behavior.

### Tier 2 - `test262` follow-ons after the classification slice landed

3. **[#927](https://github.com/tomacox74/js2il/issues/927)** `test262: create a phased conformance program for js2il` - Keep this open as the sequencing umbrella and reporting parent while the remaining child issues land.
4. **[#932](https://github.com/tomacox74/js2il/issues/932)** `test262: add CI workflow and machine-readable reporting` - The local classification and baseline artifact now exist, so the next concrete step is turning that into repeatable CI and repo-consumable reporting.
5. **[#933](https://github.com/tomacox74/js2il/issues/933)** `test262: connect conformance results to ECMA-262 docs and backlog` - More actionable now that the MVP runner produces stable verdicts and a summary artifact instead of only ad-hoc console output.
6. **[#934](https://github.com/tomacox74/js2il/issues/934)** `test262: expand beyond the MVP to modules, async, and harness-heavy suites` - Important, but still deliberately later than stabilizing CI/reporting and wiring the current slice back into docs.

### Tier 3 - Deferred performance queue

7. **[#451](https://github.com/tomacox74/js2il/issues/451)** `perf(il): expand typed temps/locals to reduce casts/boxing` - Best broad performance enabler once the current compatibility and conformance priorities relax.
8. **[#737](https://github.com/tomacox74/js2il/issues/737)** `perf: callsite-based typed parameter specialization for non-exported functions` - Builds on the same typed fast-path direction as [#451](https://github.com/tomacox74/js2il/issues/451).
9. **[#738](https://github.com/tomacox74/js2il/issues/738)** `perf(prime): close PrimeJavaScript gap with spec-safe hot-path optimizations` - Still the umbrella Prime performance issue, but now clearly secondary to the remaining Node and `test262` work.
10. **[#742](https://github.com/tomacox74/js2il/issues/742)** `perf(prime): trim timing/config coercion overhead in main path` - Scoped child under [#738](https://github.com/tomacox74/js2il/issues/738).
11. **[#743](https://github.com/tomacox74/js2il/issues/743)** `perf(prime): add Prime perf acceptance gate and reporting` - Best once Prime tuning resumes.
12. **[#746](https://github.com/tomacox74/js2il/issues/746)** `perf: make dromaeo-object-regexp faster than Jint prepared` - Valuable benchmark target, but still behind compatibility and tooling work.
13. **[#747](https://github.com/tomacox74/js2il/issues/747)** `perf(regexp): cache Regex instances by source+flags` - Child optimization under [#746](https://github.com/tomacox74/js2il/issues/746).
14. **[#748](https://github.com/tomacox74/js2il/issues/748)** `perf(dispatch): add RegExp fast paths in Object.CallMember1/2` - Another child optimization under [#746](https://github.com/tomacox74/js2il/issues/746).
15. **[#768](https://github.com/tomacox74/js2il/issues/768)** `Perf: devirtualize calls to const/arrow function bindings (dromaeo-object-regexp-modern)` - Still scenario-specific rather than a broad ecosystem unblocker.
16. **[#837](https://github.com/tomacox74/js2il/issues/837)** `perf(runtime): investigate DLR-backed CallMember fast path` - Remains research-heavy rather than a near-term implementation slice.

## Execution notes

- **No active review queue remains:** there are currently no open PRs, so the next work can start directly instead of finishing review branches first.
- **Immediate top-of-stack Node track:** [#949](https://github.com/tomacox74/js2il/issues/949) -> [#956](https://github.com/tomacox74/js2il/issues/956).
- **`test262` track:** keep [#927](https://github.com/tomacox74/js2il/issues/927) as the umbrella, then execute [#932](https://github.com/tomacox74/js2il/issues/932) -> [#933](https://github.com/tomacox74/js2il/issues/933), leaving [#934](https://github.com/tomacox74/js2il/issues/934) as the deliberate post-MVP expansion bucket. The bootstrap/parser/MVP/classification foundation [#928](https://github.com/tomacox74/js2il/issues/928)-[#931](https://github.com/tomacox74/js2il/issues/931) is already landed via PRs [#971](https://github.com/tomacox74/js2il/pull/971)-[#973](https://github.com/tomacox74/js2il/pull/973) and [#975](https://github.com/tomacox74/js2il/pull/975).
- **Architecture note:** closed issue [#841](https://github.com/tomacox74/js2il/issues/841) is now reference material for the remaining network work, not a current queue item.
- **Performance track:** [#451](https://github.com/tomacox74/js2il/issues/451) -> [#737](https://github.com/tomacox74/js2il/issues/737) remains the best general optimization path; the Prime and regexp benchmark sub-queues stay explicitly secondary.

## Metadata gaps

- Open-issue labeling still lags the actual queue: **2/16** open issues currently carry a `priority:*` label, **0/16** carry a `lane:*` label, and **9/16** have no labels at all.
- With no open PRs and a smaller issue set after [#975](https://github.com/tomacox74/js2il/pull/975), this triage document plus [NodeGapPopularityBacklog.md](./NodeGapPopularityBacklog.md) remain the clearest current ordering signals until issue metadata catches up.
