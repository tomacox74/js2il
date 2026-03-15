# Issue triage snapshot (2026-03-15)

This file captures a point-in-time recommended ordering for all currently open GitHub issues.

Synced to:
- Repo: `master` @ `3a0ef44d`
- Active branch at latest update: `issue-871-http-parity` via PR [#897](https://github.com/tomacox74/js2il/pull/897)
- GitHub: open issues / PR state as of 2026-03-15 after opening PR [#897](https://github.com/tomacox74/js2il/pull/897)
- Open issues: 31
- Open PRs: 2

## What changed since the previous snapshot

- PR [#890](https://github.com/tomacox74/js2il/pull/890) merged and closed [#872](https://github.com/tomacox74/js2il/issues/872), so stream lifecycle parity is no longer the active review item.
- PR [#894](https://github.com/tomacox74/js2il/pull/894) merged and closed [#874](https://github.com/tomacox74/js2il/issues/874), landing the binary/keepalive-oriented `node:net` follow-on that HTTP now builds on.
- PR [#895](https://github.com/tomacox74/js2il/pull/895) merged and closed [#772](https://github.com/tomacox74/js2il/issues/772), so the pragmatic ESM interop slice has moved out of the active infrastructure queue.
- PR [#897](https://github.com/tomacox74/js2il/pull/897) is now open to implement [#871](https://github.com/tomacox74/js2il/issues/871); treat that issue as the active in-review item, with [#870](https://github.com/tomacox74/js2il/issues/870) now the next primary Node follow-on and [#859](https://github.com/tomacox74/js2il/issues/859) still the best bounded ECMA parallel slice.

## Ranking method

- Use [TriageScoreboard.md](./TriageScoreboard.md) as the current planning source of truth: Node compatibility is still the primary feature lane and ECMA-262 semantics the secondary lane.
- Within that lane weighting, prefer issues that most improve developer experience and compatibility, unlock the broadest follow-on work, and can ship in test-backed slices.
- Performance remains behind feature/correctness work unless it directly unblocks compatibility.
- This is a recommendation-oriented queue, not a claim that every adjacent issue must ship strictly serially. The execution notes below call out the most important dependency tracks.

## Recommended next picks

- **Active review item:** [#871](https://github.com/tomacox74/js2il/issues/871) `node: expand HTTP parity beyond the loopback baseline` via PR [#897](https://github.com/tomacox74/js2il/pull/897)
- **Primary next item:** [#870](https://github.com/tomacox74/js2il/issues/870) `node: implement TLS and HTTPS support baseline`
- **Best bounded parallel win:** [#859](https://github.com/tomacox74/js2il/issues/859) `ECMA-262: support computed class elements, private methods, and static blocks`
- **Best platform/API track:** [#790](https://github.com/tomacox74/js2il/issues/790) `node: expand crypto and webcrypto practical coverage`

## Recommended order

### Tier 1 - Current execution slice

1. **[#871](https://github.com/tomacox74/js2il/issues/871)** `node: expand HTTP parity beyond the loopback baseline` - PR [#897](https://github.com/tomacox74/js2il/pull/897) is open; if it lands as proposed, chunked/streaming HTTP plus a keep-alive `Agent` baseline should be closure-ready.
2. **[#870](https://github.com/tomacox74/js2il/issues/870)** `node: implement TLS and HTTPS support baseline` - Secure networking is still entirely absent and is now the most important remaining Node networking gap after the landed `net` + `http` work.
3. **[#790](https://github.com/tomacox74/js2il/issues/790)** `node: expand crypto and webcrypto practical coverage` - Real auth, signing, and secure protocol code still needs more than hashing/random helpers.
4. **[#873](https://github.com/tomacox74/js2il/issues/873)** `node: add FileHandle and file stream APIs` - Important for tooling, package managers, and bundlers once the stream/socket substrate is solid.
5. **[#859](https://github.com/tomacox74/js2il/issues/859)** `ECMA-262: support computed class elements, private methods, and static blocks` - Still the best bounded ECMA parallel slice while the primary lane stays Node-first.
6. **[#875](https://github.com/tomacox74/js2il/issues/875)** `node: implement timers/promises baseline` - A practical modern async layer once the larger networking cliffs are reduced.
7. **[#876](https://github.com/tomacox74/js2il/issues/876)** `node: implement zlib compression baseline` - Important HTTP/package-flow follow-on once the core transport layers are credible.
8. **[#877](https://github.com/tomacox74/js2il/issues/877)** `node: expand child_process IPC and process-control parity` - Useful for toolchains, but still behind networking/filesystem/platform compatibility.
9. **[#857](https://github.com/tomacox74/js2il/issues/857)** `ECMA-262: implement full module record/linking/evaluation semantics` - The deeper module umbrella remains open after the pragmatic `#772` closure.
10. **[#860](https://github.com/tomacox74/js2il/issues/860)** `ECMA-262: enforce temporal dead zone semantics for lexical bindings` - Spec-correct TDZ behavior still offers high correctness value once the higher-leverage items above move.

### Tier 2 - Next wave after the current slice

11. **[#861](https://github.com/tomacox74/js2il/issues/861)** `ECMA-262: expose Map/Set/WeakMap/WeakSet constructor values and prototype surfaces` - JS-visible collection constructors/prototypes unblock reflective and prototype-based code.
12. **[#862](https://github.com/tomacox74/js2il/issues/862)** `ECMA-262: complete Map/Set iterable construction and prototype methods` - Common collection APIs remain missing even when the runtime has partial backing types.
13. **[#864](https://github.com/tomacox74/js2il/issues/864)** `ECMA-262: expose String.prototype, string iterators, and missing string APIs` - High-frequency string APIs and iterators are still visibly incomplete.
14. **[#863](https://github.com/tomacox74/js2il/issues/863)** `ECMA-262: implement advanced Proxy traps and Proxy.revocable` - Important metaprogramming follow-on, but less commonly blocking than string/collection gaps.
15. **[#875](https://github.com/tomacox74/js2il/issues/875)** `node: implement timers/promises baseline` - Valuable modern async layer once the larger compatibility cliffs above are reduced.
16. **[#876](https://github.com/tomacox74/js2il/issues/876)** `node: implement zlib compression baseline` - Practical HTTP/package-flow support, but not ahead of the current loader/network work.
17. **[#877](https://github.com/tomacox74/js2il/issues/877)** `node: expand child_process IPC and process-control parity` - Useful for toolchains, but after core module/runtime/network compatibility moves.
18. **[#865](https://github.com/tomacox74/js2il/issues/865)** `ECMA-262: implement iterator helpers and public Iterator/AsyncIterator surfaces` - Useful modern surface area, but not as broadly blocking as the items above.
19. **[#866](https://github.com/tomacox74/js2il/issues/866)** `ECMA-262: implement arguments exotic objects and mapped parameter aliasing` - Important spec work, though it causes less day-to-day breakage than the surrounding queue.
20. **[#841](https://github.com/tomacox74/js2il/issues/841)** `Investigate using existing .NET HTTP primitives for future Node HTTP work` - Good architectural input for the HTTP/TLS track, but not a user-facing deliverable by itself.

### Tier 3 - Investigation follow-ons

21. **[#842](https://github.com/tomacox74/js2il/issues/842)** `Investigate why extractEcma262SectionHtml.js does not compile under js2il` - Likely best re-scoped after the module/network surface improves; current repro is a tooling follow-up.

### Tier 4 - Deferred performance queue

22. **[#451](https://github.com/tomacox74/js2il/issues/451)** `perf(il): expand typed temps/locals to reduce casts/boxing` - First general-purpose performance enabler once feature/correctness priorities relax.
23. **[#737](https://github.com/tomacox74/js2il/issues/737)** `perf: callsite-based typed parameter specialization for non-exported functions` - Builds on the same typed-fast-path direction as [#451](https://github.com/tomacox74/js2il/issues/451).
24. **[#738](https://github.com/tomacox74/js2il/issues/738)** `perf(prime): close PrimeJavaScript gap with spec-safe hot-path optimizations` - Worth doing, but still behind the current feature backlog.
25. **[#740](https://github.com/tomacox74/js2il/issues/740)** `perf(prime): keep sieve loop math in typed locals with fallback` - Child slice under [#738](https://github.com/tomacox74/js2il/issues/738).
26. **[#742](https://github.com/tomacox74/js2il/issues/742)** `perf(prime): trim timing/config coercion overhead in main path` - Another scoped child under [#738](https://github.com/tomacox74/js2il/issues/738).
27. **[#743](https://github.com/tomacox74/js2il/issues/743)** `perf(prime): add Prime perf acceptance gate and reporting` - Best once active Prime optimization work resumes.
28. **[#746](https://github.com/tomacox74/js2il/issues/746)** `perf: make dromaeo-object-regexp faster than Jint prepared` - Valuable benchmark target, but still lower than compatibility work.
29. **[#747](https://github.com/tomacox74/js2il/issues/747)** `perf(regexp): cache Regex instances by source+flags` - Child optimization under [#746](https://github.com/tomacox74/js2il/issues/746).
30. **[#748](https://github.com/tomacox74/js2il/issues/748)** `perf(dispatch): add RegExp fast paths in Object.CallMember1/2` - Another child optimization under [#746](https://github.com/tomacox74/js2il/issues/746).
31. **[#768](https://github.com/tomacox74/js2il/issues/768)** `Perf: devirtualize calls to const/arrow function bindings (dromaeo-object-regexp-modern)` - Scenario-specific perf follow-on rather than a broad ecosystem unblocker.
32. **[#837](https://github.com/tomacox74/js2il/issues/837)** `perf(runtime): investigate DLR-backed CallMember fast path` - Still a research/investigation item rather than a concrete implementation slice.

## Execution notes

- **Module / interop track:** [#857](https://github.com/tomacox74/js2il/issues/857) is now the remaining deeper module umbrella after [#772](https://github.com/tomacox74/js2il/issues/772) closed through PR [#895](https://github.com/tomacox74/js2il/pull/895).
- **Node I/O track:** [#871](https://github.com/tomacox74/js2il/issues/871) (active review via PR [#897](https://github.com/tomacox74/js2il/pull/897)) -> [#870](https://github.com/tomacox74/js2il/issues/870), with [#873](https://github.com/tomacox74/js2il/issues/873) and [#790](https://github.com/tomacox74/js2il/issues/790) as the next highest-value platform follow-ons.
- **ECMA quick-win track:** [#859](https://github.com/tomacox74/js2il/issues/859) -> [#860](https://github.com/tomacox74/js2il/issues/860) -> [#861](https://github.com/tomacox74/js2il/issues/861) -> [#862](https://github.com/tomacox74/js2il/issues/862) -> [#864](https://github.com/tomacox74/js2il/issues/864) -> [#863](https://github.com/tomacox74/js2il/issues/863) -> [#865](https://github.com/tomacox74/js2il/issues/865) -> [#866](https://github.com/tomacox74/js2il/issues/866).
- **Investigation items:** [#841](https://github.com/tomacox74/js2il/issues/841), [#842](https://github.com/tomacox74/js2il/issues/842), and [#837](https://github.com/tomacox74/js2il/issues/837) should inform adjacent work, but they should not replace the main feature queue.

## Metadata gaps

- Several open issues still lack consistent `priority:*` and `lane:*` labeling, so ordering still relies more on the docs/test demand signals than on GitHub metadata.
- With PR [#897](https://github.com/tomacox74/js2il/pull/897) now open for [#871](https://github.com/tomacox74/js2il/issues/871), this ranked list should treat HTTP parity as the active in-review item rather than still-unclaimed work.
