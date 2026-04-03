# Issue triage snapshot (refreshed 2026-03-26)

This file captures a point-in-time recommended ordering for all currently open GitHub issues.

Synced to:
- Repo: `master` @ `5513dfb3`
- Active review branches at latest update: `copilot/support-npm-package-entrypoints` via PR [#912](https://github.com/tomacox74/js2il/pull/912) and `copilot/debug-portable-pdb-metadata` via PR [#913](https://github.com/tomacox74/js2il/pull/913)
- GitHub: open issues / PR state as of 2026-03-26 after merging PR [#909](https://github.com/tomacox74/js2il/pull/909)
- Open issues: 22
- Open PRs: 2

## What changed since the previous snapshot

- PR [#901](https://github.com/tomacox74/js2il/pull/901) landed and closed [#870](https://github.com/tomacox74/js2il/issues/870); the prior Node networking follow-on queue also shrank with [#790](https://github.com/tomacox74/js2il/issues/790), [#873](https://github.com/tomacox74/js2il/issues/873), [#875](https://github.com/tomacox74/js2il/issues/875), [#876](https://github.com/tomacox74/js2il/issues/876), and [#877](https://github.com/tomacox74/js2il/issues/877) now closed.
- PR [#909](https://github.com/tomacox74/js2il/pull/909) landed and closed [#860](https://github.com/tomacox74/js2il/issues/860); [#859](https://github.com/tomacox74/js2il/issues/859) and [#857](https://github.com/tomacox74/js2il/issues/857) also closed, so the earlier ECMA quick-win slice advanced materially.
- The active review queue is now split across PR [#912](https://github.com/tomacox74/js2il/pull/912) for [#892](https://github.com/tomacox74/js2il/issues/892) and PR [#913](https://github.com/tomacox74/js2il/pull/913) for [#891](https://github.com/tomacox74/js2il/issues/891), rather than a single networking-focused in-review item.
- New issue [#914](https://github.com/tomacox74/js2il/issues/914) is now the clearest remaining Node/hosting compatibility follow-on after the March transport/process work landed.

## Ranking method

- Use [TriageScoreboard.md](./TriageScoreboard.md) as the current planning source of truth: Node/hosting compatibility is still the primary lane, ECMA-262 semantics the secondary lane, and issue/doc reliability work the parallel hygiene lane.
- Within that lane weighting, prefer issues that most improve developer experience and compatibility, unlock the broadest follow-on work, and can ship in test-backed slices.
- Performance remains behind feature/correctness work unless it directly unblocks compatibility.
- This is a recommendation-oriented queue, not a claim that every adjacent issue must ship strictly serially. The execution notes below call out the most important dependency tracks.

## Recommended next picks

- **Active review items:** [#892](https://github.com/tomacox74/js2il/issues/892) `Hosting/NuGet: support npm package entrypoints in Js2IL.SDK build task` via PR [#912](https://github.com/tomacox74/js2il/pull/912), and [#891](https://github.com/tomacox74/js2il/issues/891) `debugging: complete Portable PDB metadata and JS breakpoint binding` via PR [#913](https://github.com/tomacox74/js2il/pull/913)
- **Primary next item:** [#914](https://github.com/tomacox74/js2il/issues/914) `Support child_process.fork() in JsEngine hosting scenarios`
- **Best bounded ECMA parallel win:** [#861](https://github.com/tomacox74/js2il/issues/861) `ECMA-262: expose Map/Set/WeakMap/WeakSet constructor values and prototype surfaces`
- **Best tooling / self-hosting follow-on:** [#842](https://github.com/tomacox74/js2il/issues/842) `scripts/ECMA262: extractEcma262SectionHtml.js still does not work under js2il`

## Recommended order

### Tier 1 - Active review and immediate next slice

1. **[#892](https://github.com/tomacox74/js2il/issues/892)** `Hosting/NuGet: support npm package entrypoints in Js2IL.SDK build task` - PR [#912](https://github.com/tomacox74/js2il/pull/912) is already open, and landing it removes one of the biggest remaining SDK authoring friction points by giving the MSBuild path parity with the CLI's module-id workflow.
2. **[#891](https://github.com/tomacox74/js2il/issues/891)** `debugging: complete Portable PDB metadata and JS breakpoint binding` - PR [#913](https://github.com/tomacox74/js2il/pull/913) is already open; better JS breakpoint binding is high leverage for execution-test debugging and day-to-day developer trust.
3. **[#914](https://github.com/tomacox74/js2il/issues/914)** `Support child_process.fork() in JsEngine hosting scenarios` - This is now the clearest remaining Node/hosting compatibility gap after the standalone child-process baseline and related March Node work landed.
4. **[#861](https://github.com/tomacox74/js2il/issues/861)** `ECMA-262: expose Map/Set/WeakMap/WeakSet constructor values and prototype surfaces` - Best bounded ECMA win now that the earlier TDZ/class/module quick wins have moved off the board.
5. **[#864](https://github.com/tomacox74/js2il/issues/864)** `ECMA-262: expose String.prototype, string iterators, and missing string APIs` - String public surfaces and iterators remain a very visible correctness gap in ordinary code.
6. **[#862](https://github.com/tomacox74/js2il/issues/862)** `ECMA-262: complete Map/Set iterable construction and prototype methods` - Natural follow-on once the public constructor/prototype surfaces in [#861](https://github.com/tomacox74/js2il/issues/861) are in place.
7. **[#863](https://github.com/tomacox74/js2il/issues/863)** `ECMA-262: implement advanced Proxy traps and Proxy.revocable` - Important metaprogramming follow-on, but less broadly blocking than the string/collection items above.
8. **[#865](https://github.com/tomacox74/js2il/issues/865)** `ECMA-262: implement iterator helpers and public Iterator/AsyncIterator surfaces` - Valuable modern surface area, though it hits fewer workloads than the current string/collection gaps.
9. **[#866](https://github.com/tomacox74/js2il/issues/866)** `ECMA-262: implement arguments exotic objects and mapped parameter aliasing` - Important spec completeness work, but narrower day-to-day breakage than the items above.

### Tier 2 - Tooling and investigation follow-ons

10. **[#842](https://github.com/tomacox74/js2il/issues/842)** `scripts/ECMA262: extractEcma262SectionHtml.js still does not work under js2il` - This is no longer just a vague investigation item; it is now a concrete compile blocker (`URL`) plus a concrete async/runtime failure in local-file mode.
11. **[#841](https://github.com/tomacox74/js2il/issues/841)** `Investigate using existing .NET HTTP primitives for future Node HTTP work` - Still useful architectural input, but it should inform future networking iterations rather than displace the remaining shipping feature work above.

### Tier 3 - Deferred performance queue

12. **[#451](https://github.com/tomacox74/js2il/issues/451)** `perf(il): expand typed temps/locals to reduce casts/boxing` - First general-purpose performance enabler once feature/correctness priorities relax.
13. **[#737](https://github.com/tomacox74/js2il/issues/737)** `perf: callsite-based typed parameter specialization for non-exported functions` - Builds on the same typed-fast-path direction as [#451](https://github.com/tomacox74/js2il/issues/451).
14. **[#738](https://github.com/tomacox74/js2il/issues/738)** `perf(prime): close PrimeJavaScript gap with spec-safe hot-path optimizations` - Worth doing, but still behind the current feature backlog.
15. **[#740](https://github.com/tomacox74/js2il/issues/740)** `perf(prime): keep sieve loop math in typed locals with fallback` - Child slice under [#738](https://github.com/tomacox74/js2il/issues/738).
16. **[#742](https://github.com/tomacox74/js2il/issues/742)** `perf(prime): trim timing/config coercion overhead in main path` - Another scoped child under [#738](https://github.com/tomacox74/js2il/issues/738).
17. **[#743](https://github.com/tomacox74/js2il/issues/743)** `perf(prime): add Prime perf acceptance gate and reporting` - Best once active Prime optimization work resumes.
18. **[#746](https://github.com/tomacox74/js2il/issues/746)** `perf: make dromaeo-object-regexp faster than Jint prepared` - Valuable benchmark target, but still lower than compatibility work.
19. **[#747](https://github.com/tomacox74/js2il/issues/747)** `perf(regexp): cache Regex instances by source+flags` - Child optimization under [#746](https://github.com/tomacox74/js2il/issues/746).
20. **[#748](https://github.com/tomacox74/js2il/issues/748)** `perf(dispatch): add RegExp fast paths in Object.CallMember1/2` - Another child optimization under [#746](https://github.com/tomacox74/js2il/issues/746).
21. **[#768](https://github.com/tomacox74/js2il/issues/768)** `Perf: devirtualize calls to const/arrow function bindings (dromaeo-object-regexp-modern)` - Scenario-specific perf follow-on rather than a broad ecosystem unblocker.
22. **[#837](https://github.com/tomacox74/js2il/issues/837)** `perf(runtime): investigate DLR-backed CallMember fast path` - Still a research/investigation item rather than a concrete implementation slice.

## Execution notes

- **Hosting / package-consumption track:** [#892](https://github.com/tomacox74/js2il/issues/892) (active review via PR [#912](https://github.com/tomacox74/js2il/pull/912)) -> [#914](https://github.com/tomacox74/js2il/issues/914), with [#891](https://github.com/tomacox74/js2il/issues/891) (active review via PR [#913](https://github.com/tomacox74/js2il/pull/913)) improving the adjacent debugging workflow.
- **ECMA quick-win track:** [#861](https://github.com/tomacox74/js2il/issues/861) -> [#864](https://github.com/tomacox74/js2il/issues/864) -> [#862](https://github.com/tomacox74/js2il/issues/862) -> [#863](https://github.com/tomacox74/js2il/issues/863) -> [#865](https://github.com/tomacox74/js2il/issues/865) -> [#866](https://github.com/tomacox74/js2il/issues/866).
- **Self-hosting / docs tooling:** [#842](https://github.com/tomacox74/js2il/issues/842) is the concrete "js2il should run its own ECMA extraction script" blocker; [#841](https://github.com/tomacox74/js2il/issues/841) remains background architecture input rather than a direct delivery item.
- **Performance track:** [#451](https://github.com/tomacox74/js2il/issues/451) -> [#737](https://github.com/tomacox74/js2il/issues/737) -> [#738](https://github.com/tomacox74/js2il/issues/738) with [#740](https://github.com/tomacox74/js2il/issues/740), [#742](https://github.com/tomacox74/js2il/issues/742), and [#743](https://github.com/tomacox74/js2il/issues/743) as child slices; [#746](https://github.com/tomacox74/js2il/issues/746) -> [#747](https://github.com/tomacox74/js2il/issues/747) / [#748](https://github.com/tomacox74/js2il/issues/748), with [#768](https://github.com/tomacox74/js2il/issues/768) and [#837](https://github.com/tomacox74/js2il/issues/837) as narrower or research-heavy follow-ons.

## Metadata gaps

- Several open issues still lack consistent `priority:*` and `lane:*` labeling, so ordering still relies more on docs/test impact than on GitHub metadata.
- With PRs [#912](https://github.com/tomacox74/js2il/pull/912) and [#913](https://github.com/tomacox74/js2il/pull/913) both open, the "active item" notion should now be treated as two parallel review slices rather than a single branch.
- After the March closeout of the TLS/HTTPS, crypto, file-stream, timers/promises, zlib, child-process, class-elements, module, and TDZ slices, the remaining open backlog is much more weighted toward hosting/tooling and ECMA surface completion than toward core Node transport features.
