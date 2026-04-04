# Issue triage snapshot (refreshed 2026-04-04)

This file captures a point-in-time recommended ordering for all currently open GitHub issues.

Synced to:
- Repo: `master` @ `143929f1`
- Latest release on `master`: `v0.9.6` via PR [#937](https://github.com/tomacox74/js2il/pull/937)
- Active review branches at latest update: none
- GitHub: open issues / PR state as of 2026-04-04 after merging PR [#941](https://github.com/tomacox74/js2il/pull/941)
- Open issues: 22
- Open PRs: 0

## What changed since the previous snapshot

- `master` moved past the original 2026-04-03 snapshot in two important ways: commit `a4e275f4` landed the core rewritten-module sequence-point mapping work behind [#919](https://github.com/tomacox74/js2il/issues/919), and PR [#941](https://github.com/tomacox74/js2il/pull/941) upgraded the GitHub Actions workflows to the newer supported Node runtime.
- Because the rewrite-aware debug mapping work is already on `master`, [#919](https://github.com/tomacox74/js2il/issues/919) is no longer the primary code-implementation item in the backlog; it is now a narrower debugger-validation / documentation closeout item.
- The active review queue is still empty on `master`, so there is no merge-first work left ahead of the issue backlog.
- The open issue count is still 22, with the backlog shape unchanged at a high level: the `test262` umbrella and child set [#927](https://github.com/tomacox74/js2il/issues/927)-[#934](https://github.com/tomacox74/js2il/issues/934), the deferred performance lane, and the remaining bounded follow-ons [#841](https://github.com/tomacox74/js2il/issues/841), [#842](https://github.com/tomacox74/js2il/issues/842), [#919](https://github.com/tomacox74/js2il/issues/919), and [#935](https://github.com/tomacox74/js2il/issues/935).
- The remaining backlog is now shaped much differently than it was in late March:
  - 8 `test262` adoption issues ([#927](https://github.com/tomacox74/js2il/issues/927)-[#934](https://github.com/tomacox74/js2il/issues/934))
  - 10 deferred performance issues ([#451](https://github.com/tomacox74/js2il/issues/451), [#737](https://github.com/tomacox74/js2il/issues/737), [#738](https://github.com/tomacox74/js2il/issues/738), [#742](https://github.com/tomacox74/js2il/issues/742), [#743](https://github.com/tomacox74/js2il/issues/743), [#746](https://github.com/tomacox74/js2il/issues/746), [#747](https://github.com/tomacox74/js2il/issues/747), [#748](https://github.com/tomacox74/js2il/issues/748), [#768](https://github.com/tomacox74/js2il/issues/768), [#837](https://github.com/tomacox74/js2il/issues/837))
  - 4 non-performance follow-ons ([#841](https://github.com/tomacox74/js2il/issues/841), [#842](https://github.com/tomacox74/js2il/issues/842), [#919](https://github.com/tomacox74/js2il/issues/919), [#935](https://github.com/tomacox74/js2il/issues/935))

## Ranking method

- Use [TriageScoreboard.md](./TriageScoreboard.md) as the strategic source of truth, but adapt it to the current backlog reality: the Node/hosting quick-win queue that dominated the prior snapshot is now mostly off the board.
- Prefer bounded issues that improve developer trust or runtime fidelity first, then build the new `test262` conformance program in dependency order, and keep performance behind correctness/tooling unless it directly unblocks those tracks.
- Treat [#927](https://github.com/tomacox74/js2il/issues/927) as the umbrella tracker for the `test262` program, and rank the child issues by execution order rather than by issue number alone.

## Recommended next picks

- **Primary next item:** [#935](https://github.com/tomacox74/js2il/issues/935) `runtime: model built-in constructor values as real function objects`
- **Best tooling / self-hosting follow-on:** [#842](https://github.com/tomacox74/js2il/issues/842) `scripts/ECMA262: extractEcma262SectionHtml.js still does not work under js2il`
- **Debugger closeout item:** [#919](https://github.com/tomacox74/js2il/issues/919) `debugging: finish JS breakpoint binding beyond PR 913 metadata work` - now narrowed to representative validation, hidden-sequence-point polish, and explicit debugger-limitations documentation after the core rewrite mapping landed on `master`
- **Strategic next program:** [#927](https://github.com/tomacox74/js2il/issues/927) `test262: create a phased conformance program for js2il`, starting with [#928](https://github.com/tomacox74/js2il/issues/928) -> [#929](https://github.com/tomacox74/js2il/issues/929) -> [#930](https://github.com/tomacox74/js2il/issues/930) -> [#931](https://github.com/tomacox74/js2il/issues/931) -> [#932](https://github.com/tomacox74/js2il/issues/932) -> [#933](https://github.com/tomacox74/js2il/issues/933), while keeping [#934](https://github.com/tomacox74/js2il/issues/934) explicitly deferred until the MVP exists.

## Recommended order

### Tier 1 - Immediate bounded correctness and tooling wins

1. **[#935](https://github.com/tomacox74/js2il/issues/935)** `runtime: model built-in constructor values as real function objects` - This is now the clearest small runtime-fidelity follow-up after [#921](https://github.com/tomacox74/js2il/pull/921): it closes the remaining reflective `Map`/`Set`/`WeakMap`/`WeakSet` constructor-object gap without reopening the larger keyed-collection surface work.
2. **[#842](https://github.com/tomacox74/js2il/issues/842)** `scripts/ECMA262: extractEcma262SectionHtml.js still does not work under js2il` - This remains the best self-hosting/tooling issue because it has a checked-in repro, a concrete compile blocker, and a separate runtime failure that currently prevents js2il from running one of its own ECMA tooling scripts.
3. **[#919](https://github.com/tomacox74/js2il/issues/919)** `debugging: finish JS breakpoint binding beyond PR 913 metadata work` - The main rewritten-module source-mapping work is already on `master`, so this is now a narrower validation/documentation closeout rather than the next code-implementation slice.

### Tier 2 - `test262` program bootstrapping

4. **[#927](https://github.com/tomacox74/js2il/issues/927)** `test262: create a phased conformance program for js2il` - Keep this as the umbrella issue that defines the scope, sequencing, and reporting model for the child issues below.
5. **[#928](https://github.com/tomacox74/js2il/issues/928)** `test262: decide upstream intake and sync model` - This is the first concrete decision point because acquisition, pinning, and licensing expectations constrain every downstream runner and CI decision.
6. **[#929](https://github.com/tomacox74/js2il/issues/929)** `test262: implement metadata/frontmatter parser` - Once intake is decided, metadata parsing is the next hard dependency; the runner cannot classify includes, flags, features, or negative tests without it.
7. **[#930](https://github.com/tomacox74/js2il/issues/930)** `test262: create MVP runner for plain synchronous script tests` - This is the first runnable delivery slice and should stay intentionally narrow so failures remain actionable.
8. **[#931](https://github.com/tomacox74/js2il/issues/931)** `test262: classify negative tests, exclusions, and baselines` - This can partially evolve alongside the MVP runner, but it should land before broadening the slice so output stays reproducible and triageable.
9. **[#932](https://github.com/tomacox74/js2il/issues/932)** `test262: add CI workflow and machine-readable reporting` - CI and summary output should follow a working local slice rather than arrive before the runner semantics are stable.
10. **[#933](https://github.com/tomacox74/js2il/issues/933)** `test262: connect conformance results to ECMA-262 docs and backlog` - This becomes high value once there is real output to map back into the docs and issue system.
11. **[#934](https://github.com/tomacox74/js2il/issues/934)** `test262: expand beyond the MVP to modules, async, and harness-heavy suites` - Important, but explicitly later: this should remain deferred until the narrow MVP runner and reporting loop are already working.

### Tier 3 - Research / architecture follow-on

12. **[#841](https://github.com/tomacox74/js2il/issues/841)** `Investigate using existing .NET HTTP primitives for future Node HTTP work` - Still useful architectural input, but it is a research issue rather than the next shipping slice now that the more immediate hosting backlog has landed.

### Tier 4 - Deferred performance queue

13. **[#451](https://github.com/tomacox74/js2il/issues/451)** `perf(il): expand typed temps/locals to reduce casts/boxing` - Best broad performance enabler once the current correctness/tooling priorities relax.
14. **[#737](https://github.com/tomacox74/js2il/issues/737)** `perf: callsite-based typed parameter specialization for non-exported functions` - Builds on the same typed-fast-path direction as [#451](https://github.com/tomacox74/js2il/issues/451).
15. **[#738](https://github.com/tomacox74/js2il/issues/738)** `perf(prime): close PrimeJavaScript gap with spec-safe hot-path optimizations` - Still the umbrella Prime performance issue, but now clearly a secondary lane behind the active correctness and conformance work.
16. **[#742](https://github.com/tomacox74/js2il/issues/742)** `perf(prime): trim timing/config coercion overhead in main path` - Scoped child under [#738](https://github.com/tomacox74/js2il/issues/738).
17. **[#743](https://github.com/tomacox74/js2il/issues/743)** `perf(prime): add Prime perf acceptance gate and reporting` - Best once the next round of Prime tuning resumes.
18. **[#746](https://github.com/tomacox74/js2il/issues/746)** `perf: make dromaeo-object-regexp faster than Jint prepared` - Valuable benchmark target, but still behind compatibility and tooling work.
19. **[#747](https://github.com/tomacox74/js2il/issues/747)** `perf(regexp): cache Regex instances by source+flags` - Child optimization under [#746](https://github.com/tomacox74/js2il/issues/746).
20. **[#748](https://github.com/tomacox74/js2il/issues/748)** `perf(dispatch): add RegExp fast paths in Object.CallMember1/2` - Another child optimization under [#746](https://github.com/tomacox74/js2il/issues/746).
21. **[#768](https://github.com/tomacox74/js2il/issues/768)** `Perf: devirtualize calls to const/arrow function bindings (dromaeo-object-regexp-modern)` - Still scenario-specific rather than a broad ecosystem unblocker.
22. **[#837](https://github.com/tomacox74/js2il/issues/837)** `perf(runtime): investigate DLR-backed CallMember fast path` - Remains a research-heavy idea rather than a near-term implementation slice.

## Execution notes

- **No active review queue remains:** there are currently no open PRs on `master`, so the next work can start directly instead of finishing review branches first.
- **Immediate top-of-stack track:** [#935](https://github.com/tomacox74/js2il/issues/935) and [#842](https://github.com/tomacox74/js2il/issues/842) are now the clearest bounded next shipping slices, while [#919](https://github.com/tomacox74/js2il/issues/919) has been narrowed to debugger validation/docs closeout work.
- **`test262` track:** use [#927](https://github.com/tomacox74/js2il/issues/927) as the parent, then deliver [#928](https://github.com/tomacox74/js2il/issues/928) -> [#929](https://github.com/tomacox74/js2il/issues/929) -> [#930](https://github.com/tomacox74/js2il/issues/930) -> [#931](https://github.com/tomacox74/js2il/issues/931) -> [#932](https://github.com/tomacox74/js2il/issues/932) -> [#933](https://github.com/tomacox74/js2il/issues/933), leaving [#934](https://github.com/tomacox74/js2il/issues/934) as the deliberate post-MVP expansion bucket.
- **Performance track:** [#451](https://github.com/tomacox74/js2il/issues/451) -> [#737](https://github.com/tomacox74/js2il/issues/737) remains the best general optimization path; [#738](https://github.com/tomacox74/js2il/issues/738) stays the Prime umbrella with [#742](https://github.com/tomacox74/js2il/issues/742) / [#743](https://github.com/tomacox74/js2il/issues/743) as child slices, while [#746](https://github.com/tomacox74/js2il/issues/746) -> [#747](https://github.com/tomacox74/js2il/issues/747) / [#748](https://github.com/tomacox74/js2il/issues/748) and [#768](https://github.com/tomacox74/js2il/issues/768) / [#837](https://github.com/tomacox74/js2il/issues/837) remain lower-priority benchmark work.

## Metadata gaps

- Open-issue labeling still lags the actual queue: `0/22` open issues currently carry a `priority:*` label, `0/22` carry a `lane:*` label, and `17/22` have no labels at all.
- The new `test262` track is well-structured by issue bodies and parent/child links, but that structure is not yet reflected in GitHub labels.
- With no open PRs, this triage document is currently a more reliable ordering signal than any active review-queue metadata.
