# Issue triage snapshot (refreshed 2026-04-20)

This file captures a point-in-time recommended ordering for all currently open GitHub issues.

Synced to:
- Repo: `origin/master` @ `145d7a72`
- Latest tagged release on `master`: `v0.9.7` via PR [#966](https://github.com/tomacox74/js2il/pull/966)
- Recent merged PRs since the previous snapshot: [#974](https://github.com/tomacox74/js2il/pull/974), [#975](https://github.com/tomacox74/js2il/pull/975), [#977](https://github.com/tomacox74/js2il/pull/977), [#978](https://github.com/tomacox74/js2il/pull/978)
- GitHub: open issue / PR state as of 2026-04-20
- Open issues: **20**
- Open PRs: **1**

## What changed since the previous snapshot

- PR [#977](https://github.com/tomacox74/js2il/pull/977) landed on `master`, closing [#932](https://github.com/tomacox74/js2il/issues/932) and adding bounded CI/reporting for the current `test262` MVP suites.
- PR [#978](https://github.com/tomacox74/js2il/pull/978) landed on `master`, closing [#933](https://github.com/tomacox74/js2il/issues/933) and linking bounded `test262` evidence back into `docs/ECMA262` plus backlog ownership.
- The old single post-MVP `test262` bucket is now decomposed into concrete follow-ons [#981](https://github.com/tomacox74/js2il/issues/981)-[#985](https://github.com/tomacox74/js2il/issues/985), leaving [#934](https://github.com/tomacox74/js2il/issues/934) and umbrella [#927](https://github.com/tomacox74/js2il/issues/927) as closure/bookkeeping items rather than the next implementation picks.
- Open issue count is now **20** and there is **1** open PR elsewhere in the repo.

## Ranking method

- Use [TriageScoreboard.md](./TriageScoreboard.md) as the strategic source of truth.
- Prefer concrete, user-visible compatibility slices before research and benchmark work.
- Treat [#934](https://github.com/tomacox74/js2il/issues/934) and [#927](https://github.com/tomacox74/js2il/issues/927) as closure/bookkeeping items for the roadmap split, and rank the concrete post-MVP follow-ons [#981](https://github.com/tomacox74/js2il/issues/981)-[#985](https://github.com/tomacox74/js2il/issues/985) by execution order.
- Keep performance behind compatibility and conformance unless it directly unblocks those lanes.

## Recommended next picks

- **Primary next item:** [#949](https://github.com/tomacox74/js2il/issues/949) `node: add global fetch / Request / Response / Headers baseline`
- **Best next production-network follow-on:** [#956](https://github.com/tomacox74/js2il/issues/956) `node: expand TLS/HTTPS trust, client-auth, and agent parity`
- **Best next `test262` expansion slice:** [#981](https://github.com/tomacox74/js2il/issues/981) `test262: add module-mode conformance slice`
- **Best next docs/tooling hygiene slice:** [#979](https://github.com/tomacox74/js2il/issues/979) `docs(changelog): archive older release lines and add browse-only archive index`
- **Bookkeeping only, not the next direct code pick:** [#934](https://github.com/tomacox74/js2il/issues/934) and [#927](https://github.com/tomacox74/js2il/issues/927) should close once the roadmap-split PR lands.

## Recommended order

### Tier 1 - Immediate compatibility wins

1. **[#949](https://github.com/tomacox74/js2il/issues/949)** `node: add global fetch / Request / Response / Headers baseline` - The highest-leverage remaining platform gap after the recent URL/HTTP/TLS/stream/fs/crypto tranche; modern packages frequently assume `fetch`-style globals first.
2. **[#956](https://github.com/tomacox74/js2il/issues/956)** `node: expand TLS/HTTPS trust, client-auth, and agent parity` - The current TLS/HTTPS baseline is good enough for local/self-signed flows, but real outbound integrations still need trust-store, client-cert, and richer agent behavior.

### Tier 2 - `test262` post-MVP follow-ons after the MVP foundation landed

3. **[#981](https://github.com/tomacox74/js2il/issues/981)** `test262: add module-mode conformance slice` - Highest-leverage post-MVP follow-on because JS2IL already has meaningful loader/module surface area and module conformance gaps are immediately user-visible.
4. **[#982](https://github.com/tomacox74/js2il/issues/982)** `test262: add async and Promise conformance slice` - Async and Promise semantics are central to real-world compatibility, but they need an explicit completion/microtask contract rather than ad hoc harness growth.
5. **[#983](https://github.com/tomacox74/js2il/issues/983)** `test262: add raw and harness-heavy conformance slice` - The next best expansion once modules/async hosting rules start solidifying, because it unlocks more corpus breadth without pretending the entire suite is ready.
6. **[#985](https://github.com/tomacox74/js2il/issues/985)** `test262: add Intl and environment-sensitive suite strategy` - Important for turning current path exclusions into intentional, deterministic policy instead of a blanket skip forever.
7. **[#984](https://github.com/tomacox74/js2il/issues/984)** `test262: add agent and CanBlock conformance slice` - Important long-term, but still the lowest near-term pick because it needs a substantially different host/runtime model than the current single-process runner.
8. **[#934](https://github.com/tomacox74/js2il/issues/934)** `test262: expand beyond the MVP to modules, async, and harness-heavy suites` - Closure/bookkeeping only now that the work has been decomposed into [#981](https://github.com/tomacox74/js2il/issues/981)-[#985](https://github.com/tomacox74/js2il/issues/985).
9. **[#927](https://github.com/tomacox74/js2il/issues/927)** `test262: create a phased conformance program for js2il` - The original umbrella no longer owns unique implementation work once [#934](https://github.com/tomacox74/js2il/issues/934) closes.

### Tier 3 - Docs/tooling hygiene

10. **[#979](https://github.com/tomacox74/js2il/issues/979)** `docs(changelog): archive older release lines and add browse-only archive index` - Important repository hygiene, but still behind compatibility and conformance slices with direct runtime impact.

### Tier 4 - Deferred performance queue

11. **[#451](https://github.com/tomacox74/js2il/issues/451)** `perf(il): expand typed temps/locals to reduce casts/boxing` - Best broad performance enabler once the current compatibility and conformance priorities relax.
12. **[#737](https://github.com/tomacox74/js2il/issues/737)** `perf: callsite-based typed parameter specialization for non-exported functions` - Builds on the same typed fast-path direction as [#451](https://github.com/tomacox74/js2il/issues/451).
13. **[#738](https://github.com/tomacox74/js2il/issues/738)** `perf(prime): close PrimeJavaScript gap with spec-safe hot-path optimizations` - Still the umbrella Prime performance issue, but now clearly secondary to the remaining Node and `test262` work.
14. **[#742](https://github.com/tomacox74/js2il/issues/742)** `perf(prime): trim timing/config coercion overhead in main path` - Scoped child under [#738](https://github.com/tomacox74/js2il/issues/738).
15. **[#743](https://github.com/tomacox74/js2il/issues/743)** `perf(prime): add Prime perf acceptance gate and reporting` - Best once Prime tuning resumes.
16. **[#746](https://github.com/tomacox74/js2il/issues/746)** `perf: make dromaeo-object-regexp faster than Jint prepared` - Valuable benchmark target, but still behind compatibility and tooling work.
17. **[#747](https://github.com/tomacox74/js2il/issues/747)** `perf(regexp): cache Regex instances by source+flags` - Child optimization under [#746](https://github.com/tomacox74/js2il/issues/746).
18. **[#748](https://github.com/tomacox74/js2il/issues/748)** `perf(dispatch): add RegExp fast paths in Object.CallMember1/2` - Another child optimization under [#746](https://github.com/tomacox74/js2il/issues/746).
19. **[#768](https://github.com/tomacox74/js2il/issues/768)** `Perf: devirtualize calls to const/arrow function bindings (dromaeo-object-regexp-modern)` - Still scenario-specific rather than a broad ecosystem unblocker.
20. **[#837](https://github.com/tomacox74/js2il/issues/837)** `perf(runtime): investigate DLR-backed CallMember fast path` - Remains research-heavy rather than a near-term implementation slice.

## Execution notes

- **There is an active review queue elsewhere:** there is currently 1 open PR in the repo, so new work should not assume the review lane is empty.
- **Immediate top-of-stack Node track:** [#949](https://github.com/tomacox74/js2il/issues/949) -> [#956](https://github.com/tomacox74/js2il/issues/956).
- **`test262` track:** the MVP foundation [#928](https://github.com/tomacox74/js2il/issues/928)-[#933](https://github.com/tomacox74/js2il/issues/933) is already landed via PRs [#971](https://github.com/tomacox74/js2il/pull/971)-[#973](https://github.com/tomacox74/js2il/pull/973), [#975](https://github.com/tomacox74/js2il/pull/975), [#977](https://github.com/tomacox74/js2il/pull/977), and [#978](https://github.com/tomacox74/js2il/pull/978); the concrete post-MVP queue is now [#981](https://github.com/tomacox74/js2il/issues/981) -> [#982](https://github.com/tomacox74/js2il/issues/982) -> [#983](https://github.com/tomacox74/js2il/issues/983) -> [#985](https://github.com/tomacox74/js2il/issues/985) -> [#984](https://github.com/tomacox74/js2il/issues/984), with [#934](https://github.com/tomacox74/js2il/issues/934) and [#927](https://github.com/tomacox74/js2il/issues/927) pending closure once the roadmap split lands.
- **Docs/tooling hygiene:** [#979](https://github.com/tomacox74/js2il/issues/979) is the one active non-runtime follow-on and can stay behind the compatibility/conformance lanes.
- **Architecture note:** closed issue [#841](https://github.com/tomacox74/js2il/issues/841) is now reference material for the remaining network work, not a current queue item.
- **Performance track:** [#451](https://github.com/tomacox74/js2il/issues/451) -> [#737](https://github.com/tomacox74/js2il/issues/737) remains the best general optimization path; the Prime and regexp benchmark sub-queues stay explicitly secondary.

## Metadata gaps

- Open-issue labeling still lags the actual queue: **2/20** open issues currently carry a `priority:*` label, **0/20** carry a `lane:*` label, and **13/20** have no labels at all.
- With the issue queue expanded by the explicit `test262` follow-ons [#981](https://github.com/tomacox74/js2il/issues/981)-[#985](https://github.com/tomacox74/js2il/issues/985), this triage document plus [NodeGapPopularityBacklog.md](./NodeGapPopularityBacklog.md) remain the clearest current ordering signals until issue metadata catches up.
