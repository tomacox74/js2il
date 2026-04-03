# Issue triage snapshot (refreshed 2026-04-03)

This file captures a point-in-time recommended ordering for all currently open GitHub issues.

Synced to:
- Repo: `master` @ `065fd088`
- Latest release on `master`: `v0.9.6` via PR [#937](https://github.com/tomacox74/js2il/pull/937)
- Active review branches at latest update: none
- GitHub: open issues / PR state as of 2026-04-03 after merging PR [#938](https://github.com/tomacox74/js2il/pull/938)
- Open issues: 22
- Open PRs: 0

## What changed since the previous snapshot

- The late-March review queue is fully cleared: PRs [#912](https://github.com/tomacox74/js2il/pull/912), [#913](https://github.com/tomacox74/js2il/pull/913), [#920](https://github.com/tomacox74/js2il/pull/920), [#921](https://github.com/tomacox74/js2il/pull/921), [#922](https://github.com/tomacox74/js2il/pull/922), [#923](https://github.com/tomacox74/js2il/pull/923), [#924](https://github.com/tomacox74/js2il/pull/924), [#925](https://github.com/tomacox74/js2il/pull/925), and [#926](https://github.com/tomacox74/js2il/pull/926) all landed on `master`, closing issues [#892](https://github.com/tomacox74/js2il/issues/892), [#891](https://github.com/tomacox74/js2il/issues/891), [#914](https://github.com/tomacox74/js2il/issues/914), [#861](https://github.com/tomacox74/js2il/issues/861), [#864](https://github.com/tomacox74/js2il/issues/864), [#862](https://github.com/tomacox74/js2il/issues/862), [#863](https://github.com/tomacox74/js2il/issues/863), [#865](https://github.com/tomacox74/js2il/issues/865), and [#866](https://github.com/tomacox74/js2il/issues/866).
- Release and CI state also moved forward: PR [#917](https://github.com/tomacox74/js2il/pull/917) cut `v0.9.5`, PR [#918](https://github.com/tomacox74/js2il/pull/918) added published-package BenchmarkDotNet mode, PR [#936](https://github.com/tomacox74/js2il/pull/936) landed the current Prime hot-path tuning, PR [#937](https://github.com/tomacox74/js2il/pull/937) cut `v0.9.6`, and PR [#938](https://github.com/tomacox74/js2il/pull/938) fixed the benchmark artifact-path workflow bug on `master`.
- The active review queue went from two open PRs to zero open PRs, so there is no merge-first work left ahead of the issue backlog.
- The open issue count stayed flat at 22 only because the earlier closeout was offset by new follow-up issues [#919](https://github.com/tomacox74/js2il/issues/919), the `test262` umbrella and child set [#927](https://github.com/tomacox74/js2il/issues/927)-[#934](https://github.com/tomacox74/js2il/issues/934), and runtime follow-up [#935](https://github.com/tomacox74/js2il/issues/935).
- The remaining backlog is now shaped much differently than it was in late March:
  - 8 `test262` adoption issues ([#927](https://github.com/tomacox74/js2il/issues/927)-[#934](https://github.com/tomacox74/js2il/issues/934))
  - 10 deferred performance issues ([#451](https://github.com/tomacox74/js2il/issues/451), [#737](https://github.com/tomacox74/js2il/issues/737), [#738](https://github.com/tomacox74/js2il/issues/738), [#742](https://github.com/tomacox74/js2il/issues/742), [#743](https://github.com/tomacox74/js2il/issues/743), [#746](https://github.com/tomacox74/js2il/issues/746), [#747](https://github.com/tomacox74/js2il/issues/747), [#748](https://github.com/tomacox74/js2il/issues/748), [#768](https://github.com/tomacox74/js2il/issues/768), [#837](https://github.com/tomacox74/js2il/issues/837))
  - 4 non-performance follow-ons ([#841](https://github.com/tomacox74/js2il/issues/841), [#842](https://github.com/tomacox74/js2il/issues/842), [#919](https://github.com/tomacox74/js2il/issues/919), [#935](https://github.com/tomacox74/js2il/issues/935))

## Ranking method

- Use [TriageScoreboard.md](./TriageScoreboard.md) as the strategic source of truth, but adapt it to the current backlog reality: the Node/hosting quick-win queue that dominated the prior snapshot is now mostly off the board.
- Prefer bounded issues that improve developer trust or runtime fidelity first, then build the new `test262` conformance program in dependency order, and keep performance behind correctness/tooling unless it directly unblocks those tracks.
- Treat [#927](https://github.com/tomacox74/js2il/issues/927) as the umbrella tracker for the `test262` program, and rank the child issues by execution order rather than by issue number alone.

## Recommended next picks

- **Primary next item:** [#919](https://github.com/tomacox74/js2il/issues/919) `debugging: finish JS breakpoint binding beyond PR 913 metadata work`
- **Best bounded runtime-correctness win:** [#935](https://github.com/tomacox74/js2il/issues/935) `runtime: model built-in constructor values as real function objects`
- **Best tooling / self-hosting follow-on:** [#842](https://github.com/tomacox74/js2il/issues/842) `scripts/ECMA262: extractEcma262SectionHtml.js still does not work under js2il`
- **Strategic next program:** [#927](https://github.com/tomacox74/js2il/issues/927) `test262: create a phased conformance program for js2il`, starting with [#928](https://github.com/tomacox74/js2il/issues/928) -> [#929](https://github.com/tomacox74/js2il/issues/929) -> [#930](https://github.com/tomacox74/js2il/issues/930) -> [#931](https://github.com/tomacox74/js2il/issues/931) -> [#932](https://github.com/tomacox74/js2il/issues/932) -> [#933](https://github.com/tomacox74/js2il/issues/933), while keeping [#934](https://github.com/tomacox74/js2il/issues/934) explicitly deferred until the MVP exists.

## Recommended order

### Tier 1 - Immediate bounded correctness and tooling wins

1. **[#919](https://github.com/tomacox74/js2il/issues/919)** `debugging: finish JS breakpoint binding beyond PR 913 metadata work` - With the PDB metadata baseline already landed, this is now the highest-leverage developer-experience follow-on: getting original JavaScript breakpoints to bind and hit reliably improves day-to-day debugging across the whole repo.
2. **[#935](https://github.com/tomacox74/js2il/issues/935)** `runtime: model built-in constructor values as real function objects` - This is the clearest small runtime-fidelity follow-up after [#921](https://github.com/tomacox74/js2il/pull/921): it closes the remaining reflective `Map`/`Set`/`WeakMap`/`WeakSet` constructor-object gap without reopening the larger keyed-collection surface work.
3. **[#842](https://github.com/tomacox74/js2il/issues/842)** `scripts/ECMA262: extractEcma262SectionHtml.js still does not work under js2il` - This remains the best self-hosting/tooling issue because it has a checked-in repro, a concrete compile blocker, and a separate runtime failure that currently prevents js2il from running one of its own ECMA tooling scripts.

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

- **No active review queue remains:** there are currently no open PRs, so the next work can start directly from `master` instead of finishing review branches first.
- **Immediate top-of-stack track:** [#919](https://github.com/tomacox74/js2il/issues/919), [#935](https://github.com/tomacox74/js2il/issues/935), and [#842](https://github.com/tomacox74/js2il/issues/842) are the clearest bounded next slices and can be worked independently.
- **`test262` track:** use [#927](https://github.com/tomacox74/js2il/issues/927) as the parent, then deliver [#928](https://github.com/tomacox74/js2il/issues/928) -> [#929](https://github.com/tomacox74/js2il/issues/929) -> [#930](https://github.com/tomacox74/js2il/issues/930) -> [#931](https://github.com/tomacox74/js2il/issues/931) -> [#932](https://github.com/tomacox74/js2il/issues/932) -> [#933](https://github.com/tomacox74/js2il/issues/933), leaving [#934](https://github.com/tomacox74/js2il/issues/934) as the deliberate post-MVP expansion bucket.
- **Performance track:** [#451](https://github.com/tomacox74/js2il/issues/451) -> [#737](https://github.com/tomacox74/js2il/issues/737) remains the best general optimization path; [#738](https://github.com/tomacox74/js2il/issues/738) stays the Prime umbrella with [#742](https://github.com/tomacox74/js2il/issues/742) / [#743](https://github.com/tomacox74/js2il/issues/743) as child slices, while [#746](https://github.com/tomacox74/js2il/issues/746) -> [#747](https://github.com/tomacox74/js2il/issues/747) / [#748](https://github.com/tomacox74/js2il/issues/748) and [#768](https://github.com/tomacox74/js2il/issues/768) / [#837](https://github.com/tomacox74/js2il/issues/837) remain lower-priority benchmark work.

## Metadata gaps

- Open-issue labeling still lags the actual queue: `0/22` open issues currently carry a `priority:*` label, `0/22` carry a `lane:*` label, and `17/22` have no labels at all.
- The new `test262` track is well-structured by issue bodies and parent/child links, but that structure is not yet reflected in GitHub labels.
- With no open PRs, this triage document is currently a more reliable ordering signal than any active review-queue metadata.
