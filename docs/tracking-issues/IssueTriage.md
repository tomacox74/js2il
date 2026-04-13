# Issue triage snapshot (refreshed 2026-04-13)

This file captures a point-in-time recommended ordering for all currently open GitHub issues.

Synced to:
- Repo: `origin/master` @ `731a54f4`
- Latest tagged release on `master`: `v0.9.7` via PR [#966](https://github.com/tomacox74/js2il/pull/966)
- Recent merged PRs since the previous snapshot: [#969](https://github.com/tomacox74/js2il/pull/969), [#970](https://github.com/tomacox74/js2il/pull/970), [#971](https://github.com/tomacox74/js2il/pull/971), [#972](https://github.com/tomacox74/js2il/pull/972), [#973](https://github.com/tomacox74/js2il/pull/973)
- GitHub: open issue / PR state as of 2026-04-13
- Open issues: **17**
- Open PRs: **0**

## What changed since the previous snapshot

- PRs [#969](https://github.com/tomacox74/js2il/pull/969)-[#973](https://github.com/tomacox74/js2il/pull/973) landed on `master`, closing [#946](https://github.com/tomacox74/js2il/issues/946), [#947](https://github.com/tomacox74/js2il/issues/947), [#950](https://github.com/tomacox74/js2il/issues/950)-[#955](https://github.com/tomacox74/js2il/issues/955), and the first three concrete `test262` child issues [#928](https://github.com/tomacox74/js2il/issues/928)-[#930](https://github.com/tomacox74/js2il/issues/930).
- The open Node/runtime queue dropped from four follow-ons to two: [#949](https://github.com/tomacox74/js2il/issues/949) and [#956](https://github.com/tomacox74/js2il/issues/956).
- The `test262` program moved out of intake/bootstrap setup and into post-MVP follow-ons: [#927](https://github.com/tomacox74/js2il/issues/927) remains the umbrella issue; [#931](https://github.com/tomacox74/js2il/issues/931)-[#934](https://github.com/tomacox74/js2il/issues/934) remain open; [#928](https://github.com/tomacox74/js2il/issues/928)-[#930](https://github.com/tomacox74/js2il/issues/930) are now closed.
- Open issue count dropped from **22** to **17** while open PR count stayed at **0**.

## Ranking method

- Use [TriageScoreboard.md](./TriageScoreboard.md) as the strategic source of truth.
- Prefer concrete, user-visible compatibility slices before research and benchmark work.
- Treat [#927](https://github.com/tomacox74/js2il/issues/927) as the umbrella tracker for the `test262` program, and rank the concrete child issues by execution order.
- Keep performance behind compatibility and conformance unless it directly unblocks those lanes.

## Recommended next picks

- **Primary next item:** [#949](https://github.com/tomacox74/js2il/issues/949) `node: add global fetch / Request / Response / Headers baseline`
- **Best next production-network follow-on:** [#956](https://github.com/tomacox74/js2il/issues/956) `node: expand TLS/HTTPS trust, client-auth, and agent parity`
- **Best next `test262` implementation slice:** [#931](https://github.com/tomacox74/js2il/issues/931) `test262: classify negative tests, exclusions, and baselines`
- **Best next `test262` reporting slice:** [#932](https://github.com/tomacox74/js2il/issues/932) `test262: add CI workflow and machine-readable reporting`
- **Umbrella only, not the next direct code pick:** [#927](https://github.com/tomacox74/js2il/issues/927) stays open as the parent issue for the remaining `test262` lane.

## Recommended order

### Tier 1 - Immediate compatibility wins

1. **[#949](https://github.com/tomacox74/js2il/issues/949)** `node: add global fetch / Request / Response / Headers baseline` - The highest-leverage remaining platform gap after the recent URL/HTTP/TLS/stream/fs/crypto tranche; modern packages frequently assume `fetch`-style globals first.
2. **[#956](https://github.com/tomacox74/js2il/issues/956)** `node: expand TLS/HTTPS trust, client-auth, and agent parity` - The current TLS/HTTPS baseline is good enough for local/self-signed flows, but real outbound integrations still need trust-store, client-cert, and richer agent behavior.

### Tier 2 - `test262` follow-ons after the MVP landed

3. **[#927](https://github.com/tomacox74/js2il/issues/927)** `test262: create a phased conformance program for js2il` - Keep this open as the sequencing umbrella and reporting parent while the remaining child issues land.
4. **[#931](https://github.com/tomacox74/js2il/issues/931)** `test262: classify negative tests, exclusions, and baselines` - The right next concrete slice after the MVP runner because reproducible classification is the prerequisite for meaningful results.
5. **[#932](https://github.com/tomacox74/js2il/issues/932)** `test262: add CI workflow and machine-readable reporting` - CI and machine-readable output should follow once the local classification and baseline story is stable enough to automate.
6. **[#933](https://github.com/tomacox74/js2il/issues/933)** `test262: connect conformance results to ECMA-262 docs and backlog` - High value once the runner emits stable, triageable output rather than just the initial narrow MVP slice.
7. **[#934](https://github.com/tomacox74/js2il/issues/934)** `test262: expand beyond the MVP to modules, async, and harness-heavy suites` - Important, but still deliberately later than getting the narrow slice classified, reported, and wired back into docs.

### Tier 3 - Deferred performance queue

8. **[#451](https://github.com/tomacox74/js2il/issues/451)** `perf(il): expand typed temps/locals to reduce casts/boxing` - Best broad performance enabler once the current compatibility and conformance priorities relax.
9. **[#737](https://github.com/tomacox74/js2il/issues/737)** `perf: callsite-based typed parameter specialization for non-exported functions` - Builds on the same typed fast-path direction as [#451](https://github.com/tomacox74/js2il/issues/451).
10. **[#738](https://github.com/tomacox74/js2il/issues/738)** `perf(prime): close PrimeJavaScript gap with spec-safe hot-path optimizations` - Still the umbrella Prime performance issue, but now clearly secondary to the remaining Node and `test262` work.
11. **[#742](https://github.com/tomacox74/js2il/issues/742)** `perf(prime): trim timing/config coercion overhead in main path` - Scoped child under [#738](https://github.com/tomacox74/js2il/issues/738).
12. **[#743](https://github.com/tomacox74/js2il/issues/743)** `perf(prime): add Prime perf acceptance gate and reporting` - Best once Prime tuning resumes.
13. **[#746](https://github.com/tomacox74/js2il/issues/746)** `perf: make dromaeo-object-regexp faster than Jint prepared` - Valuable benchmark target, but still behind compatibility and tooling work.
14. **[#747](https://github.com/tomacox74/js2il/issues/747)** `perf(regexp): cache Regex instances by source+flags` - Child optimization under [#746](https://github.com/tomacox74/js2il/issues/746).
15. **[#748](https://github.com/tomacox74/js2il/issues/748)** `perf(dispatch): add RegExp fast paths in Object.CallMember1/2` - Another child optimization under [#746](https://github.com/tomacox74/js2il/issues/746).
16. **[#768](https://github.com/tomacox74/js2il/issues/768)** `Perf: devirtualize calls to const/arrow function bindings (dromaeo-object-regexp-modern)` - Still scenario-specific rather than a broad ecosystem unblocker.
17. **[#837](https://github.com/tomacox74/js2il/issues/837)** `perf(runtime): investigate DLR-backed CallMember fast path` - Remains research-heavy rather than a near-term implementation slice.

## Execution notes

- **No active review queue remains:** there are currently no open PRs, so the next work can start directly instead of finishing review branches first.
- **Immediate top-of-stack Node track:** [#949](https://github.com/tomacox74/js2il/issues/949) -> [#956](https://github.com/tomacox74/js2il/issues/956).
- **`test262` track:** keep [#927](https://github.com/tomacox74/js2il/issues/927) as the umbrella, then execute [#931](https://github.com/tomacox74/js2il/issues/931) -> [#932](https://github.com/tomacox74/js2il/issues/932) -> [#933](https://github.com/tomacox74/js2il/issues/933), leaving [#934](https://github.com/tomacox74/js2il/issues/934) as the deliberate post-MVP expansion bucket. The bootstrap/parser/MVP foundation [#928](https://github.com/tomacox74/js2il/issues/928)-[#930](https://github.com/tomacox74/js2il/issues/930) is already landed via PRs [#971](https://github.com/tomacox74/js2il/pull/971)-[#973](https://github.com/tomacox74/js2il/pull/973).
- **Architecture note:** closed issue [#841](https://github.com/tomacox74/js2il/issues/841) is now reference material for the remaining network work, not a current queue item.
- **Performance track:** [#451](https://github.com/tomacox74/js2il/issues/451) -> [#737](https://github.com/tomacox74/js2il/issues/737) remains the best general optimization path; the Prime and regexp benchmark sub-queues stay explicitly secondary.

## Metadata gaps

- Open-issue labeling still lags the actual queue: **2/17** open issues currently carry a `priority:*` label, **0/17** carry a `lane:*` label, and **10/17** have no labels at all.
- With no open PRs and a smaller issue set after the recent merges, this triage document plus [NodeGapPopularityBacklog.md](./NodeGapPopularityBacklog.md) remain the clearest current ordering signals until issue metadata catches up.
