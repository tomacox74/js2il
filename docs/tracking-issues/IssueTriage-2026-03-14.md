# Issue triage snapshot (2026-03-14)

This file captures a point-in-time recommended ordering for all currently open GitHub issues.

Synced to:
- Repo: `master` @ `980ace5a`
- Active branch at latest update: `issue-870-https-tls-baseline` via PR [#901](https://github.com/tomacox74/js2il/pull/901)
- GitHub: open issues / PR state as of 2026-03-15 after opening PR [#901](https://github.com/tomacox74/js2il/pull/901)
- Open issues: 30
- Open PRs: 1

## What changed since the previous snapshot

- PR [#897](https://github.com/tomacox74/js2il/pull/897) merged and closed [#871](https://github.com/tomacox74/js2il/issues/871), so the expanded streamed/chunked HTTP baseline is now on `master`.
- PR [#901](https://github.com/tomacox74/js2il/pull/901) is now open to implement [#870](https://github.com/tomacox74/js2il/issues/870); treat secure networking as the active in-review item, with [#790](https://github.com/tomacox74/js2il/issues/790) now the next primary Node follow-on, [#859](https://github.com/tomacox74/js2il/issues/859) still the best bounded ECMA parallel slice, and [#873](https://github.com/tomacox74/js2il/issues/873) the next I/O-heavy platform track.

## Ranking method

- Use [TriageScoreboard.md](./TriageScoreboard.md) as the current planning source of truth: Node compatibility is still the primary feature lane and ECMA-262 semantics the secondary lane.
- Within that lane weighting, prefer issues that most improve developer experience and compatibility, unlock the broadest follow-on work, and can ship in test-backed slices.
- Performance remains behind feature/correctness work unless it directly unblocks compatibility.
- This is a recommendation-oriented queue, not a claim that every adjacent issue must ship strictly serially. The execution notes below call out the most important dependency tracks.

## Recommended next picks

- **Active review item:** [#870](https://github.com/tomacox74/js2il/issues/870) `node: implement TLS and HTTPS support baseline` via PR [#901](https://github.com/tomacox74/js2il/pull/901)
- **Primary next item:** [#790](https://github.com/tomacox74/js2il/issues/790) `node: expand crypto and webcrypto practical coverage`
- **Best bounded parallel win:** [#859](https://github.com/tomacox74/js2il/issues/859) `ECMA-262: support computed class elements, private methods, and static blocks`
- **Best platform/API track:** [#873](https://github.com/tomacox74/js2il/issues/873) `node: add FileHandle and file stream APIs`

## Recommended order

### Tier 1 - Current execution slice

1. **[#870](https://github.com/tomacox74/js2il/issues/870)** `node: implement TLS and HTTPS support baseline` - PR [#901](https://github.com/tomacox74/js2il/pull/901) is open; if it lands as proposed, secure transport will move from throw-only stubs to a practical PEM-backed TLS/HTTPS baseline with explicit advanced-feature diagnostics.
2. **[#790](https://github.com/tomacox74/js2il/issues/790)** `node: expand crypto and webcrypto practical coverage` - Real auth, signing, and secure protocol code still needs more than hashing/random helpers.
3. **[#873](https://github.com/tomacox74/js2il/issues/873)** `node: add FileHandle and file stream APIs` - Important for tooling, package managers, and bundlers once the stream/socket substrate is solid.
4. **[#859](https://github.com/tomacox74/js2il/issues/859)** `ECMA-262: support computed class elements, private methods, and static blocks` - Still the best bounded ECMA parallel slice while the primary lane stays Node-first.
5. **[#875](https://github.com/tomacox74/js2il/issues/875)** `node: implement timers/promises baseline` - A practical modern async layer once the larger networking cliffs are reduced.
6. **[#876](https://github.com/tomacox74/js2il/issues/876)** `node: implement zlib compression baseline` - Important HTTP/package-flow follow-on once the core transport layers are credible.
7. **[#877](https://github.com/tomacox74/js2il/issues/877)** `node: expand child_process IPC and process-control parity` - Useful for toolchains, but still behind networking/filesystem/platform compatibility.
8. **[#857](https://github.com/tomacox74/js2il/issues/857)** `ECMA-262: implement full module record/linking/evaluation semantics` - The deeper module umbrella remains open after the pragmatic `#772` closure.
9. **[#860](https://github.com/tomacox74/js2il/issues/860)** `ECMA-262: enforce temporal dead zone semantics for lexical bindings` - Spec-correct TDZ behavior still offers high correctness value once the higher-leverage items above move.
10. **[#861](https://github.com/tomacox74/js2il/issues/861)** `ECMA-262: expose Map/Set/WeakMap/WeakSet constructor values and prototype surfaces` - JS-visible collection constructors/prototypes unblock reflective and prototype-based code once the current higher-leverage items move.

### Tier 2 - Next wave after the current slice

11. **[#862](https://github.com/tomacox74/js2il/issues/862)** `ECMA-262: complete Map/Set iterable construction and prototype methods` - Common collection APIs remain missing even when the runtime has partial backing types.
12. **[#864](https://github.com/tomacox74/js2il/issues/864)** `ECMA-262: expose String.prototype, string iterators, and missing string APIs` - High-frequency string APIs and iterators are still visibly incomplete.
13. **[#863](https://github.com/tomacox74/js2il/issues/863)** `ECMA-262: implement advanced Proxy traps and Proxy.revocable` - Important metaprogramming follow-on, but less commonly blocking than string/collection gaps.
14. **[#865](https://github.com/tomacox74/js2il/issues/865)** `ECMA-262: implement iterator helpers and public Iterator/AsyncIterator surfaces` - Useful modern surface area, but not as broadly blocking as the items above.
15. **[#866](https://github.com/tomacox74/js2il/issues/866)** `ECMA-262: implement arguments exotic objects and mapped parameter aliasing` - Important spec work, though it causes less day-to-day breakage than the surrounding queue.
16. **[#841](https://github.com/tomacox74/js2il/issues/841)** `Investigate using existing .NET HTTP primitives for future Node HTTP work` - Good architectural input for the HTTP/TLS track, but not a user-facing deliverable by itself.

### Tier 3 - Investigation follow-ons

17. **[#842](https://github.com/tomacox74/js2il/issues/842)** `Investigate why extractEcma262SectionHtml.js does not compile under js2il` - With secure networking now in review via [#870](https://github.com/tomacox74/js2il/issues/870), the remaining blockers are increasingly the `URL`/runtime/compiler follow-ons tracked in the issue itself.

### Tier 4 - Deferred performance queue

18. **[#451](https://github.com/tomacox74/js2il/issues/451)** `perf(il): expand typed temps/locals to reduce casts/boxing` - First general-purpose performance enabler once feature/correctness priorities relax.
19. **[#737](https://github.com/tomacox74/js2il/issues/737)** `perf: callsite-based typed parameter specialization for non-exported functions` - Builds on the same typed-fast-path direction as [#451](https://github.com/tomacox74/js2il/issues/451).
20. **[#738](https://github.com/tomacox74/js2il/issues/738)** `perf(prime): close PrimeJavaScript gap with spec-safe hot-path optimizations` - Worth doing, but still behind the current feature backlog.
21. **[#740](https://github.com/tomacox74/js2il/issues/740)** `perf(prime): keep sieve loop math in typed locals with fallback` - Child slice under [#738](https://github.com/tomacox74/js2il/issues/738).
22. **[#742](https://github.com/tomacox74/js2il/issues/742)** `perf(prime): trim timing/config coercion overhead in main path` - Another scoped child under [#738](https://github.com/tomacox74/js2il/issues/738).
23. **[#743](https://github.com/tomacox74/js2il/issues/743)** `perf(prime): add Prime perf acceptance gate and reporting` - Best once active Prime optimization work resumes.
24. **[#746](https://github.com/tomacox74/js2il/issues/746)** `perf: make dromaeo-object-regexp faster than Jint prepared` - Valuable benchmark target, but still lower than compatibility work.
25. **[#747](https://github.com/tomacox74/js2il/issues/747)** `perf(regexp): cache Regex instances by source+flags` - Child optimization under [#746](https://github.com/tomacox74/js2il/issues/746).
26. **[#748](https://github.com/tomacox74/js2il/issues/748)** `perf(dispatch): add RegExp fast paths in Object.CallMember1/2` - Another child optimization under [#746](https://github.com/tomacox74/js2il/issues/746).
27. **[#768](https://github.com/tomacox74/js2il/issues/768)** `Perf: devirtualize calls to const/arrow function bindings (dromaeo-object-regexp-modern)` - Scenario-specific perf follow-on rather than a broad ecosystem unblocker.
28. **[#837](https://github.com/tomacox74/js2il/issues/837)** `perf(runtime): investigate DLR-backed CallMember fast path` - Still a research/investigation item rather than a concrete implementation slice.

## Execution notes

- **Module / interop track:** [#857](https://github.com/tomacox74/js2il/issues/857) is now the remaining deeper module umbrella after [#772](https://github.com/tomacox74/js2il/issues/772) closed through PR [#895](https://github.com/tomacox74/js2il/pull/895).
- **Node secure-networking track:** [#870](https://github.com/tomacox74/js2il/issues/870) (active review via PR [#901](https://github.com/tomacox74/js2il/pull/901)) -> [#790](https://github.com/tomacox74/js2il/issues/790), with [#873](https://github.com/tomacox74/js2il/issues/873) as the next I/O-heavy follow-on once secure transport is in review.
- **ECMA quick-win track:** [#859](https://github.com/tomacox74/js2il/issues/859) -> [#860](https://github.com/tomacox74/js2il/issues/860) -> [#861](https://github.com/tomacox74/js2il/issues/861) -> [#862](https://github.com/tomacox74/js2il/issues/862) -> [#864](https://github.com/tomacox74/js2il/issues/864) -> [#863](https://github.com/tomacox74/js2il/issues/863) -> [#865](https://github.com/tomacox74/js2il/issues/865) -> [#866](https://github.com/tomacox74/js2il/issues/866).
- **Investigation items:** [#841](https://github.com/tomacox74/js2il/issues/841), [#842](https://github.com/tomacox74/js2il/issues/842), and [#837](https://github.com/tomacox74/js2il/issues/837) should inform adjacent work, but they should not replace the main feature queue.

## Metadata gaps

- Several open issues still lack consistent `priority:*` and `lane:*` labeling, so ordering still relies more on the docs/test demand signals than on GitHub metadata.
- With PR [#901](https://github.com/tomacox74/js2il/pull/901) now open for [#870](https://github.com/tomacox74/js2il/issues/870), this ranked list should treat secure networking as the active in-review item rather than still-unclaimed work.
