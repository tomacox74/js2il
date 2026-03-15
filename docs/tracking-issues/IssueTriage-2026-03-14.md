# Issue triage snapshot (2026-03-14)

This file captures a point-in-time recommended ordering for all currently open GitHub issues.

Synced to:
- Repo: `master` @ `f189b807`
- Active branch at latest update: `issue-872-stream-lifecycle-parity` @ `5667f157`
- GitHub: open issues / PR state as of 2026-03-14 after opening PR [#890](https://github.com/tomacox74/js2il/pull/890)
- Open issues: 32
- Open PRs: 1

## What changed since the previous snapshot

- PR [#889](https://github.com/tomacox74/js2il/pull/889) merged on 2026-03-14 and closed [#858](https://github.com/tomacox74/js2il/issues/858), so the former active review item is complete.
- PR [#890](https://github.com/tomacox74/js2il/pull/890) is now open to implement [#872](https://github.com/tomacox74/js2il/issues/872); treat that issue as the active in-review item rather than the next unclaimed pick.
- With [#858](https://github.com/tomacox74/js2il/issues/858) closed, the current open-issue mix is 9 Node.js issues, 10 ECMA-262 issues, 11 performance issues, and 2 investigation issues.
- Once [#872](https://github.com/tomacox74/js2il/issues/872) lands, the next highest-value Node I/O follow-on is [#874](https://github.com/tomacox74/js2il/issues/874) while [#859](https://github.com/tomacox74/js2il/issues/859) remains the best bounded ECMA parallel slice.

## Ranking method

- Use [TriageScoreboard.md](./TriageScoreboard.md) as the current planning source of truth: Node compatibility is still the primary feature lane and ECMA-262 semantics the secondary lane.
- Within that lane weighting, prefer issues that most improve developer experience and compatibility, unlock the broadest follow-on work, and can ship in test-backed slices.
- Performance remains behind feature/correctness work unless it directly unblocks compatibility.
- This is a recommendation-oriented queue, not a claim that every adjacent issue must ship strictly serially. The execution notes below call out the most important dependency tracks.

## Recommended next picks

- **Active review item:** [#872](https://github.com/tomacox74/js2il/issues/872) `node: expand stream lifecycle and helper parity` via PR [#890](https://github.com/tomacox74/js2il/pull/890)
- **Primary next item:** [#874](https://github.com/tomacox74/js2il/issues/874) `node: expand net socket parity and binary data handling`
- **Best bounded parallel win:** [#859](https://github.com/tomacox74/js2il/issues/859) `ECMA-262: support computed class elements, private methods, and static blocks`
- **Best infrastructure track:** [#772](https://github.com/tomacox74/js2il/issues/772) `ECMA-262: ES Modules live bindings & module records (beyond current lowering)`

## Recommended order

### Tier 1 - Current execution slice

1. **[#872](https://github.com/tomacox74/js2il/issues/872)** `node: expand stream lifecycle and helper parity` - PR [#890](https://github.com/tomacox74/js2il/pull/890) is open; if it lands as proposed this issue should be closure-ready.
2. **[#874](https://github.com/tomacox74/js2il/issues/874)** `node: expand net socket parity and binary data handling` - The new stream slice sets up the next practical socket/binary follow-on for `net`, `http`, `tls`, and file streams.
3. **[#772](https://github.com/tomacox74/js2il/issues/772)** `ECMA-262: ES Modules live bindings & module records (beyond current lowering)` - Pragmatic live-binding and cycle correctness remains the biggest non-Node infrastructure follow-on.
4. **[#871](https://github.com/tomacox74/js2il/issues/871)** `node: expand HTTP parity beyond the loopback baseline` - Buffered, loopback-only HTTP is still too narrow for real clients, servers, and package traffic.
5. **[#870](https://github.com/tomacox74/js2il/issues/870)** `node: implement TLS and HTTPS support baseline` - Secure networking is still entirely absent and should follow socket/binary hardening.
6. **[#857](https://github.com/tomacox74/js2il/issues/857)** `ECMA-262: implement full module record/linking/evaluation semantics` - Important deeper module follow-on after the practical loader/live-binding slices above.
7. **[#859](https://github.com/tomacox74/js2il/issues/859)** `ECMA-262: support computed class elements, private methods, and static blocks` - Modern class forms remain validator-blocked and are common in current JS.
8. **[#790](https://github.com/tomacox74/js2il/issues/790)** `node: expand crypto and webcrypto practical coverage` - Extends JS2IL from hashing/random-only support into real auth and signing scenarios.
9. **[#873](https://github.com/tomacox74/js2il/issues/873)** `node: add FileHandle and file stream APIs` - Important for tooling, package managers, and bundlers once the stream/socket substrate is stronger.
10. **[#860](https://github.com/tomacox74/js2il/issues/860)** `ECMA-262: enforce temporal dead zone semantics for lexical bindings` - Spec-correct TDZ behavior improves correctness and developer-facing error quality.

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

- **Module / interop track:** [#772](https://github.com/tomacox74/js2il/issues/772) -> [#857](https://github.com/tomacox74/js2il/issues/857). With [#869](https://github.com/tomacox74/js2il/issues/869) already closed by PR [#888](https://github.com/tomacox74/js2il/pull/888), the remaining practical module follow-on is live bindings/cycle correctness before the deeper module-record umbrella.
- **Node I/O track:** [#872](https://github.com/tomacox74/js2il/issues/872) (active review via PR [#890](https://github.com/tomacox74/js2il/pull/890)) -> [#874](https://github.com/tomacox74/js2il/issues/874) -> [#871](https://github.com/tomacox74/js2il/issues/871) -> [#870](https://github.com/tomacox74/js2il/issues/870), with [#873](https://github.com/tomacox74/js2il/issues/873) able to ride once stream/socket primitives are solid.
- **ECMA quick-win track:** [#859](https://github.com/tomacox74/js2il/issues/859) -> [#860](https://github.com/tomacox74/js2il/issues/860) -> [#861](https://github.com/tomacox74/js2il/issues/861) -> [#862](https://github.com/tomacox74/js2il/issues/862) -> [#864](https://github.com/tomacox74/js2il/issues/864) -> [#863](https://github.com/tomacox74/js2il/issues/863) -> [#865](https://github.com/tomacox74/js2il/issues/865) -> [#866](https://github.com/tomacox74/js2il/issues/866).
- **Investigation items:** [#841](https://github.com/tomacox74/js2il/issues/841), [#842](https://github.com/tomacox74/js2il/issues/842), and [#837](https://github.com/tomacox74/js2il/issues/837) should inform adjacent work, but they should not replace the main feature queue.

## Metadata gaps

- 32 of 32 open issues are missing `priority:*` labels.
- 32 of 32 open issues are missing `lane:*` labels.
- With PR [#890](https://github.com/tomacox74/js2il/pull/890) now open for [#872](https://github.com/tomacox74/js2il/issues/872), this ranked list should treat that issue as the active in-review item rather than still-unclaimed work.
